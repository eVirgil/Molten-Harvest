using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DroneAI : NetworkBehaviour {
    Vector3 TargetPosition;
    Vector3 StartCirculingPosition;
    const float CircleRadius = 1.5f;

    [SerializeField]
    float Speed = 3.0f;
    float turnSpeed = 60.0f;

    public bool isDroneInUse = false;
    [SyncVar]
    bool isDroneLaunched = false;
    bool isStep1Done = false;       // step 1 is reaching circling position
    bool isStep2Done = false;       // step 2 is circuling around the target position
    bool isStep3Done = false;       // step 3 is returning back to ship

    float TotalCirclingAngle = 0.0f;
    [SerializeField]
    float DistanceDroneCanTarget = 3f;

    [SerializeField]
    GameObject DroneBullet;

    float ShootTimerElapsed = 0.0f;
    float ShootCooldown = 1.0f / 1.0f;
    bool canShoot = false;


    public bool isDamaged = false;

    Animator spriteAnimatior;
    // Use this for initialization
    void Start () {
        turnSpeed = 360.0f/((2.0f * Mathf.PI * CircleRadius) / Speed);
        spriteAnimatior = transform.GetChild(0).GetComponent<Animator>();
        Repair();
    }
	
	// Update is called once per frame
	void Update () {
        //transform.GetChild(0).gameObject.SetActive(isDroneLaunched);
        if (isDroneInUse && isDroneLaunched) {
            if(!isStep1Done) {
                
                Vector3 direction = TargetPosition - transform.position;
                direction.Normalize();

                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

                // fix for sometimes drones reaching the crosshair and not moving at all
                Vector3 pos = transform.position + direction * Time.deltaTime * Speed;
                if(Vector3.Distance(pos, TargetPosition) < 1.0f) {
                    pos = StartCirculingPosition;
                }

                if( Vector3.Distance(pos, StartCirculingPosition) > 0.1f) {
                    transform.position = pos;
                }
                else {
                    isStep1Done = true;
                    transform.Rotate(0, 0, 90.0f);
                    TotalCirclingAngle = 0.0f;
                }
            }

            if(isStep1Done && !isStep2Done) {

                // fix for sometimes drones reaching the crosshair and not moving at all
                if (Vector3.Distance(transform.position, TargetPosition) < 1.0f) {
                    transform.position = StartCirculingPosition;
                    transform.Rotate(0, 0, 90.0f);
                    TotalCirclingAngle = 0.0f;
                }

                float movedelta = Speed * Time.deltaTime;
                float turndelta = turnSpeed * Time.deltaTime;
                TotalCirclingAngle += turndelta;
                
                transform.Rotate(0, 0, -turndelta);
                transform.Translate(0, movedelta, 0);

                if(!isDamaged) {
                    if (!canShoot) {
                        ShootTimerElapsed += Time.deltaTime;
                        if (ShootTimerElapsed >= ShootCooldown) {
                            canShoot = true;
                            Debug.Log("Shoot");
                            TryToShootTarget();
                        }
                    }
                }
                

                if( TotalCirclingAngle > 360.0f) {
                    isStep2Done = true;
                }
            }

            if( isStep2Done && !isStep3Done) {
                Vector3 direction = GameNetworkManager.singleton.shipMovementController.transform.position - transform.position;
                direction.Normalize();

                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

                Vector3 pos = transform.position + direction * Time.deltaTime * Speed;
                if (Vector3.Distance(pos, GameNetworkManager.singleton.shipMovementController.transform.position) > 0.1f) {
                    transform.position = pos;
                }
                else {
                    isStep3Done = true;
                    isDroneLaunched = false;
                    isDroneInUse = false;
                    //transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
	}

    void TryToShootTarget() {
        GameObject nearestEnemy = null;

        GameObject []enemyArr = GameObject.FindGameObjectsWithTag("Enemy");
        float leastDistance = 0.0f;

        if ( enemyArr.Length > 0) {
            nearestEnemy = enemyArr[0];
            leastDistance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            for (int i = 0; i < enemyArr.Length; i++) {
                float distance = Vector3.Distance(transform.position, enemyArr[i].transform.position );
                if( distance < leastDistance) {
                    nearestEnemy = enemyArr[i];
                    leastDistance = distance;
                }
            }
        }
        
        if( nearestEnemy != null) {
            if (leastDistance < DistanceDroneCanTarget) {
                AIShipMover_Missile bullet = Instantiate(DroneBullet).GetComponent<AIShipMover_Missile>();
                if(bullet != null) {
                    Vector3 tmpPos = transform.position;
                    tmpPos.z = nearestEnemy.transform.position.z;
                    bullet.transform.position = tmpPos;
                    bullet.ship = nearestEnemy;
                }

                canShoot = false;
            }
        }
    }

    public void SetTargetPosition(Vector3 pPosition) {
        TargetPosition = pPosition;

        Vector3 shipPos = GameNetworkManager.singleton.shipMovementController.transform.position;
        Vector3 direction = shipPos - TargetPosition;
        direction.Normalize();

        StartCirculingPosition = TargetPosition + direction * CircleRadius;
    }

    [ClientRpc]
    public void RpcLaunchDrone(float pDelay) {
        isDroneInUse = true;
        StartCoroutine(DelayAndLaunch(pDelay));
    }

    IEnumerator DelayAndLaunch(float pDelay) {
        yield return new WaitForSeconds(pDelay);
        //transform.GetChild(0).gameObject.SetActive(true);
        isDroneLaunched = true;
        canShoot = false;

        isStep1Done = false;
        isStep2Done = false;
        isStep3Done = false;
    }

    public void Damage() {
        isDamaged = true;
        spriteAnimatior.enabled = true;
    }

    public void Repair() {
        isDamaged = false;
        spriteAnimatior.enabled = false;
    }
}

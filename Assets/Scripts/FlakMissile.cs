using UnityEngine;
using System.Collections;

public class FlakMissile : MonoBehaviour {
    public GameObject FollowObject;
    [SerializeField]
    GameObject Exploding;
    [SerializeField]
    float MissileSpeed = 3.0f;

    [SerializeField]
    float BlastRadius = 1.5f;

    bool DidCheckForEnemies = false;
    bool CanCheckForEnemies = false;
    float TotalCollisionPeriod = 1.5f;
    float collisionPeriodElapsed = 0.0f;
    Vector3 direction;
	// Use this for initialization
	void Start () {
        direction = FollowObject.transform.position - transform.position;
        
        direction.Normalize();
        
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);


        
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += direction * MissileSpeed * Time.deltaTime;

        if(FollowObject != null && Vector3.Distance(transform.position, FollowObject.transform.position) < 0.1f) {
            
            Destroy( FollowObject );
            Destroy( gameObject );
        }

        
    }

    void OnDestroy() {
        GameObject explosion = Instantiate(Exploding);
        explosion.transform.position = transform.position;
        explosion.transform.rotation = transform.rotation;

        CanCheckForEnemies = true;
        collisionPeriodElapsed = 0.0f;

        CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();

        if (shake != null) {
            shake.DoShake();
        }

        if (CanCheckForEnemies) {
            GameObject[] objArr = GameObject.FindGameObjectsWithTag("Enemy");
            //collisionPeriodElapsed += Time.deltaTime;
            //if (collisionPeriodElapsed < TotalCollisionPeriod) {
            for (int i = 0; i < objArr.Length; i++) {
                if (Vector3.Distance(objArr[i].transform.position, transform.position) < BlastRadius) {
                    EnemyCollisionManager collisionManager = objArr[i].GetComponent<EnemyCollisionManager>();
                    if (collisionManager != null) {
                        collisionManager.DestroyEnemy();
                    }
                }
            }
            //}

            objArr = GameObject.FindGameObjectsWithTag("Drone");
            for (int i = 0; i < objArr.Length; i++) {
                if (Vector3.Distance(objArr[i].transform.position, transform.position) < BlastRadius) {
                    DroneAI droneAI = objArr[i].GetComponent<DroneAI>();
                    if( droneAI != null) {
                        droneAI.Damage();
                    }
                }
            }
        }

    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ShipMovementController : NetworkBehaviour
{

    public GameObject ship;
    public float ROTATION_SPEED;

    private Vector3 mousePosition;
    private Vector3 touchPosition;
    private Vector3 shipPosition;

    private GameObject laserPrefab;
    public GameObject laser;

    // cannons
    [SerializeField]
    GameObject CannonCrosshair;
    [SerializeField]
    float CannonRotationSpeed = 0.0010f;

    // left cannon 
    [SerializeField]
    GameObject LeftCannon;
    [SerializeField]
    float LeftCannonCooldown = 1.0f;
    float lc_TimeElapsed = 0.0f;
    Vector3 LeftCannonTarget;
    bool IsLeftCannonReady = true;
    GameObject LeftCannonCrosshair = null;
    float lc_TimeElapsedRotating = 0.0f;
    float lc_TotalTimeRotating = 0.0f;
    Quaternion lc_StartRotation;
    Quaternion lc_EndRotation;

    // right cannon 
    [SerializeField]
    GameObject RightCannon;
    [SerializeField]
    float RightCannonCooldown = 1.0f;
    float rc_TimeElapsed = 0.0f;
    Vector3 RightCannonTarget;
    bool IsRightCannonReady = true;
    GameObject RightCannonCrosshair = null;
    float rc_TimeElapsedRotating = 0.0f;
    float rc_TotalTimeRotating = 0.0f;
    Quaternion rc_StartRotation;
    Quaternion rc_EndRotation;

    [SerializeField]
    FlakMissile flakMissile;
    // end of cannon variables


    // drones
    [SerializeField]
    GameObject DronePrefab;
    bool areDronesReady = true;
    GameObject DroneCrosshair;
    [SerializeField]
    float TimeTakenToRepairOneDrone = 5.0f;
    float TimeElapsedRepairing = 0.0f;
    DroneAI[] DronesArray;
    Animator droneRepairAnimator;
    // end of drone variables

    public bool laserFired = false;
    [SyncVar]
    public bool laserReady = true;

    private float laserStartTime;
    public float laserElapsedTime;
    private const float LASER_OFFSET = 15.0f;
    public float laserDuration = 2.0f;
    public float laserRechargeCooldown = 4.0f;

    // for client purpose.
    // collect all touch input and send it to server
    List<Vector3> inputTouchesArr;

    // for server purpose
    // collect all inputs from client and save them along with the 
    // player position who generated that input
    Dictionary<IEnums.PlayerPosition, Vector3[]> inputDictionary;

    float inputSendRate = 60;
    float timeElapsedSinceLastInput = 0.0f;

    // Use this for initialization
    void Start()
    {
        laser.SetActive(false);
        inputTouchesArr = new List<Vector3>();

        InitInputDictionary();

        InitDrones();

        droneRepairAnimator = transform.FindChild("Ship").GetComponent<Animator>();
        droneRepairAnimator.enabled = false;
    }

    void InitDrones() {
        DronesArray = new DroneAI[10];
        for ( int i=0; i < DronesArray.Length; i++ ) {
            DronesArray[i] = GameNetworkManager.singleton.SpawnNetworkAwareObject(DronePrefab, Vector3.zero, Quaternion.Euler(0,0,0)).GetComponent<DroneAI>();
            DronesArray[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void InitInputDictionary() {
        inputDictionary = new Dictionary<IEnums.PlayerPosition, Vector3[]>();

        inputDictionary.Add(IEnums.PlayerPosition.CoaxialCannon, new Vector3[0]);
        inputDictionary.Add(IEnums.PlayerPosition.Drones, new Vector3[0]);
        inputDictionary.Add(IEnums.PlayerPosition.Missiles, new Vector3[0]);
        inputDictionary.Add(IEnums.PlayerPosition.Shield, new Vector3[0]);
    }

    // Update is called once per frame
    void Update()
    {
        SendInputToServer();
        // only move if its server all clients will receive the movement and rotation
        // data through network transform
        
        if (isServer) {
            //transform.Rotate( Vector3.forward* Time.deltaTime * 30 );
            updateShipRotation( inputDictionary[IEnums.PlayerPosition.CoaxialCannon] );
            updateWeaponStatus( inputDictionary[IEnums.PlayerPosition.CoaxialCannon] );

            processCannonInput(inputDictionary[IEnums.PlayerPosition.Missiles]);
            updateCannonRotation();

            processDroneInput(inputDictionary[IEnums.PlayerPosition.Drones]);
            updateDroneStatus();
        }

        updateLaserStatus();
    }


    void SendInputToServer() {
        timeElapsedSinceLastInput += Time.deltaTime;

        if( timeElapsedSinceLastInput >= 1.0f/inputSendRate) {
            timeElapsedSinceLastInput = 0.0f;
            inputTouchesArr.Clear();

            for( int  i = 0; i < Input.touchCount; i++) {
                inputTouchesArr.Add(Input.GetTouch(0).position);
            }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (Input.GetMouseButton(0)) {
                inputTouchesArr.Add(Input.mousePosition);
            }
            else if(Input.GetMouseButton(1)) {
                inputTouchesArr.Add( Vector3.zero );
            }
            if (Input.GetMouseButton(1)) {
                inputTouchesArr.Add(Input.mousePosition);
            }
#endif
            GameNetworkManager.singleton.SendInput( inputTouchesArr.ToArray() );
        }
    }

    // this syncs all inputs from all other clients into single place in server
    public void SyncInputFromClients( IEnums.PlayerPosition pPosition, Vector3[] pTouchInput ) {
        if (inputDictionary == null) {
            InitInputDictionary();
        }
        
        inputDictionary[pPosition] = pTouchInput;
    }

    void FixedUpdate()
    {

    }

    private void updateDroneStatus() {
        int numDronesInBase = 0;
        bool areDronesDamaged = false;
        for (int i = 0; i < DronesArray.Length; i++) {
            if( !DronesArray[i].isDroneInUse) {
                numDronesInBase++;
            }

            if (!areDronesDamaged && DronesArray[i].isDamaged) {
                areDronesDamaged = true;
            }
        }

        
        areDronesReady = numDronesInBase == DronesArray.Length;
        if( areDronesReady && DroneCrosshair != null ) {
            Debug.Log("delete");
            Destroy( DroneCrosshair );
            DroneCrosshair = null;
        }

        // check if drones are damaged
        droneRepairAnimator.enabled = areDronesDamaged;
        if ( areDronesDamaged) {
            TimeElapsedRepairing += Time.deltaTime;

            if(TimeElapsedRepairing >= TimeTakenToRepairOneDrone) {
                TimeElapsedRepairing = 0.0f;

                for( int i = 0; i < DronesArray.Length; i++) {
                    if( DronesArray[i].isDamaged) {
                        DronesArray[i].Repair();
                        break;
                    }
                }
            }
        }
    }

    private void processDroneInput(Vector3[] pTouchInput) {
        if( pTouchInput.Length > 0) {
            if( areDronesReady) {
                areDronesReady = false;
                DroneCrosshair = GameObject.Instantiate(CannonCrosshair);
                Vector3 pos = Camera.main.ScreenToWorldPoint(pTouchInput[0]);
                pos.z = transform.position.z;
                DroneCrosshair.transform.position = pos;
                float timeDelay = 0.0f;

                for( int i=0;i<DronesArray.Length;i++) {
                    if (!DronesArray[i].isDamaged) {
                        timeDelay += (1 / 4.0f);

                        DronesArray[i].SetTargetPosition(pos);
                        DronesArray[i].RpcLaunchDrone(timeDelay);
                    }
                    
                }
            }
        }
    }

    private void updateCannonRotation() {
        if( !IsLeftCannonReady) {
            // rotate the cannon, its no longer in player control
            if(LeftCannonCrosshair != null) {
                // if crosshair is not null then that means the missile is not fired yet
                lc_TimeElapsedRotating += Time.deltaTime;
                Quaternion tmp = Quaternion.Lerp(lc_StartRotation, lc_EndRotation, lc_TimeElapsedRotating / lc_TotalTimeRotating);

                LeftCannon.transform.localRotation = tmp;
                
                if ( lc_TimeElapsedRotating >= lc_TotalTimeRotating ) {
                    
                    Vector3 relativePos = LeftCannonCrosshair.transform.position - LeftCannon.transform.position;
                    float distance = Vector3.Distance(Vector3.zero, relativePos);
                    float angle = Mathf.Asin(relativePos.x / distance) * Mathf.Rad2Deg;

                    Vector3 pos = LeftCannon.transform.position + LeftCannon.transform.up * 0.5f;
                    pos.z += 0.3f;
                    
                    GameObject obj = GameNetworkManager.singleton.SpawnNetworkAwareObject(flakMissile.gameObject, pos, Quaternion.Euler(0, 0, -angle));
                    FlakMissile missile = obj.GetComponent<FlakMissile>();
                    missile.FollowObject = LeftCannonCrosshair;

                    LeftCannonCrosshair = null;
                    lc_TimeElapsed = 0.0f;
                }
            }
            else {
                lc_TimeElapsed += Time.deltaTime;

                if(lc_TimeElapsed >= LeftCannonCooldown) {
                    IsLeftCannonReady = true;
                }
            }
        }

        if( !IsRightCannonReady) {
            // rotate the cannon, its no longer in player control
            if (RightCannonCrosshair != null) {
                // if crosshair is not null then that means the missile is not fired yet
                
                rc_TimeElapsedRotating += Time.deltaTime;
                Quaternion tmp = Quaternion.Lerp(rc_StartRotation, rc_EndRotation, rc_TimeElapsedRotating / rc_TotalTimeRotating);

                RightCannon.transform.localRotation = tmp;

                if (rc_TimeElapsedRotating >= rc_TotalTimeRotating) {
                    Vector3 relativePos = RightCannonCrosshair.transform.position - RightCannon.transform.position;
                    float distance = Vector3.Distance(Vector3.zero, relativePos);
                    float angle = Mathf.Asin(relativePos.x / distance) * Mathf.Rad2Deg;

                    Vector3 pos = RightCannon.transform.position + RightCannon.transform.up * 0.5f;
                    pos.z += 0.3f;

                    GameObject obj = GameNetworkManager.singleton.SpawnNetworkAwareObject(flakMissile.gameObject, pos, Quaternion.Euler(0, 0, 180.0f + angle));
                    FlakMissile missile = obj.GetComponent<FlakMissile>();
                    missile.FollowObject = RightCannonCrosshair;

                    RightCannonCrosshair = null;
                    rc_TimeElapsed = 0.0f;
                }
            }
            else {
                rc_TimeElapsed += Time.deltaTime;

                if (rc_TimeElapsed >= RightCannonCooldown) {
                    IsRightCannonReady = true;
                }
            }
        }
    }

    private void processCannonInput(Vector3[] pTouchInput) {

        if( pTouchInput.Length > 0) {

            // check which side the input is
            // if p1 and p2 are points on line and (x,y) is point to determine position
            // side = sign((p2.x-p1.x) * (y-p1.y) - (p2.y-p1.y)*(x-p1.x))
            // 0 if on line, +1 or -1 depending on side

            Vector2 point1 = Vector2.zero;
            Vector2 point2 = new Vector2( Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));

            Vector2 screenMid = new Vector2(Screen.width/2.0f, Screen.height/2.0f);
            Vector2 point = new Vector2(pTouchInput[0].x, pTouchInput[0].y) - screenMid;
            

            float side = Mathf.Sign((point2.x - point1.x) * (point.y-point1.y) - (point2.y - point1.y) * (point.x - point1.x));
            if(side > 0 && IsLeftCannonReady) {
                Debug.Log("Left (" + point.x + ", " + point.y + ") (" + point2.x+", "+ point2.y+")");

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pTouchInput[0]);
                worldPos.z = LeftCannon.transform.position.z;
                
                LeftCannonTarget = worldPos;
                

                // rotate left cannon
                // get relative touch position to left cannon
                
                Vector3 relativePos = worldPos - LeftCannon.transform.position;
                float distance = Vector3.Distance(Vector3.zero, relativePos);
                //float angle = Mathf.Asin(relativePos.x / distance) * Mathf.Rad2Deg;

                float denominator = relativePos.x == 0.0f ? 0.0001f : relativePos.x;
                float slop1 = relativePos.y / denominator;
                denominator = transform.up.x == 0.0f ? 0.0001f : transform.up.x;
                float slop2 = transform.up.y/ denominator;

                float angle = Mathf.Atan2(slop2 - slop1, 1.0f + slop2 * slop1) * Mathf.Rad2Deg;
                
                if (angle > 90.0f) angle = angle - 180.0f;
                if (angle < -90 && angle > -180.0f ) angle = angle + 180.0f;
                
                // making distance equal to the y if the point is outside the range
                if ( angle < -30.0f) {
                    angle = -30.0f;
                    //distance = relativePos.y;
                }
                else if( angle > 30.0f) {
                    angle = 30.0f;
                    //distance = relativePos.y;
                }

                if (LeftCannonCrosshair == null) {
                    LeftCannonCrosshair = GameObject.Instantiate(CannonCrosshair);
                }

                lc_StartRotation = LeftCannon.transform.localRotation;
                lc_EndRotation = Quaternion.Euler(0, 0, -angle);

                Quaternion tmp = LeftCannon.transform.localRotation;
                LeftCannon.transform.localRotation = lc_EndRotation;
                LeftCannonCrosshair.transform.position = LeftCannon.transform.up * distance + LeftCannon.transform.position;
                LeftCannon.transform.localRotation = tmp;

                lc_TimeElapsedRotating = 0.0f;
                lc_TotalTimeRotating = Quaternion.Angle(lc_StartRotation, lc_EndRotation) / CannonRotationSpeed;
                
                IsLeftCannonReady = false;
            }
            else if(side < 0 && IsRightCannonReady) {
                Debug.Log("Right (" + point.x + ", " + point.y + ") (" + point2.x + ", " + point2.y + ")");

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pTouchInput[0]);
                worldPos.z = RightCannon.transform.position.z;
                Debug.Log(worldPos);
                RightCannonTarget = worldPos;

                Vector3 relativePos = worldPos - RightCannon.transform.position;
                float distance = Vector3.Distance(Vector3.zero, relativePos);
                //float angle = Mathf.Asin(relativePos.x / distance) * Mathf.Rad2Deg;

                float denominator = relativePos.x == 0.0f ? 0.0001f : relativePos.x;
                float slop1 = relativePos.y / denominator;
                denominator = transform.up.x == 0.0f ? 0.0001f : transform.up.x;
                float slop2 = transform.up.y / denominator;

                float angle = Mathf.Atan2(slop2 - slop1, 1.0f + slop2 * slop1) * Mathf.Rad2Deg;

                if (angle > 90.0f) angle = angle - 180.0f;
                if (angle < -90 && angle > -180.0f) angle = angle + 180.0f;

                if (angle < -30.0f) {
                    angle = -30.0f;
                }

                if (angle > 30.0f) {
                    angle = 30.0f;
                }

                if(RightCannonCrosshair == null) {
                    RightCannonCrosshair = GameObject.Instantiate(CannonCrosshair);
                }

                rc_StartRotation = RightCannon.transform.localRotation;
                rc_EndRotation = Quaternion.Euler(0, 0, -angle);
                
                Quaternion tmp = RightCannon.transform.localRotation;
                RightCannon.transform.localRotation = rc_EndRotation;
                RightCannonCrosshair.transform.position = -RightCannon.transform.up * distance + RightCannon.transform.position;
                RightCannon.transform.localRotation = tmp;

                rc_TimeElapsedRotating = 0.0f;
                rc_TotalTimeRotating = Quaternion.Angle(rc_StartRotation, rc_EndRotation) / CannonRotationSpeed;

                IsRightCannonReady = false;
            }

            
        }
        else {

        }
    }

    private void updateShipRotation(Vector3[] pTouchInput)
    {
        if (pTouchInput.Length > 0 && pTouchInput[0] != Vector3.zero)
        {
            touchPosition = pTouchInput[0];
        }
        
        shipPosition = Camera.main.WorldToScreenPoint(transform.position);

        Vector3 vectorToTarget = touchPosition - shipPosition;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * ROTATION_SPEED);
    }

    private void updateWeaponStatus(Vector3[] pTouchInput)
    {
        Vector3 laserPosition = transform.position;
        if (pTouchInput.Length > 1 && laserReady)
        {
            FireLaserGun();
        }
        
    }


    private void FireLaserGun() {
        // asking server for approval to fire laser gun
        GameNetworkManager.singleton.FireLaserGun();
    }

    public void FireLaserGunApproved() {
        laserFired = true;
        laserReady = false;

        CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();

        if( shake != null) {
            shake.Shake();
        }
    }

    private void updateLaserStatus()
    {
        if (laserFired)
        {
            GetComponent<AudioSource>().Play();
            laserStartTime = Time.time;
            laser.SetActive(true);
            laserFired = false;
        }
        else
        {
            laser.transform.rotation = transform.rotation; // Lock laser to ship's orientation

            laserElapsedTime = Time.time - laserStartTime;
            
            if (laserElapsedTime >= laserDuration && !laserFired)
            {
                laser.SetActive(false);
                CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();

                if (shake != null) {
                    shake.StopShake();
                }
            }
            if (laserElapsedTime >= laserRechargeCooldown)
            {
                laserReady = true;
            }
        }
    }

}

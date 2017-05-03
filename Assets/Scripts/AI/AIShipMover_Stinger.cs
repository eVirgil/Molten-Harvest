using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Stinger : AIShipMover_Basic
{
    public float TurnRandomizationRate = 3;
    private float lastRandomTurn = 0;
    public GameObject ship;


    // Use this for initialization
    private void Start()
    {
        MoveSpeed = 6f;
        TurnSpeed = 60f;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float moveSpeed = MoveSpeed * Time.deltaTime;
        float turnSpeed = TurnSpeed * Time.deltaTime;

        if (transform.position.x <= -10.5)
        {
            transform.Rotate(0, 0, 90);
        }
        if (transform.position.x >= 10.5)
        {
            transform.Rotate(0, 0, -90);
        }
        if (transform.position.y >= 7.5)
        {
            transform.Rotate(0, 0, 180);
        }
        if (transform.position.y <= -7.5)
        {
            transform.Rotate(0, 0, -180);
        }
        if (Time.time > lastRandomTurn + TurnRandomizationRate)
        { 
            transform.Rotate(0, 0, Random.Range(-90, 90));
            lastRandomTurn = Time.time;
        }
        if (Mathf.Sqrt(Mathf.Pow((transform.position.x - ship.transform.position.x), 2) + Mathf.Pow((transform.position.y - ship.transform.position.y), 2)) < 1.7)
        {
            transform.Rotate(0,0 , transform.rotation.z + 180);
        }

        transform.Translate(0, -moveSpeed, 0);
        transform.Rotate(0, 0, -turnSpeed);
    }
}

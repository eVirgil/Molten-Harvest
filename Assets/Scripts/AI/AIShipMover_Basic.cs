using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Basic : NetworkBehaviour
{
    public float MoveSpeed = 4f;
    public float TurnSpeed = 60f;


    // Use this for initialization
    private void Start()
    {
        MoveSpeed = 4f;
        TurnSpeed = 60f;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        // allowing the move script to run on client also 
        // so that network transform send rate can be kept low and
        // still the movement looks smooth due
        Move();
    }

    private void Move()
    {
        float moveSpeed = MoveSpeed * Time.deltaTime;
        float turnSpeed = TurnSpeed * Time.deltaTime;

        transform.Rotate(0, 0, -turnSpeed);
        transform.Translate(0, -moveSpeed, 0);
    }
}

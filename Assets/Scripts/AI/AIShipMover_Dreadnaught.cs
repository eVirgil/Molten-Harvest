using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Dreadnaught : AIShipMover_Basic
{
    // Use this for initialization
    private void Start()
    {

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

        transform.Rotate(0, 0, -turnSpeed);
        transform.Translate(0, -moveSpeed, 0);
    }
}

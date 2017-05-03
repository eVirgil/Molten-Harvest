using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Swarm : AIShipMover_Basic
{
    public GameObject ship;

    // Use this for initialization
    private void Start()
    {
        MoveSpeed = 3f;
        transform.up = -ship.transform.position - transform.position;
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

        transform.Translate(0, moveSpeed, 0);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Missile : AIShipMover_Basic
{
    public GameObject ship;

    // Use this for initialization
    private void Start()
    {
        transform.up = ship.transform.position - transform.position;
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

        if(Vector3.Distance(transform.position, Vector3.zero) > 15.0f) {
            // destroy if missile is away from screen
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShipMover_Frigate : AIShipMover_Basic
{
    public int ShipRange = 4;
    private bool inRange = false;
    public GameObject ship;


    // Use this for initialization
    private void Start()
    {
        MoveSpeed = 2f;
        transform.up = -ship.transform.position - transform.position;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (!inRange)
        {
            Move();
            CheckRange();
        }
    }

    private void Move()
    {
        float moveSpeed = MoveSpeed * Time.deltaTime;

        transform.Translate(0, moveSpeed, 0);
    }
    private void CheckRange()
    {
        if (Mathf.Sqrt(Mathf.Pow((transform.position.x - ship.transform.position.x), 2) + Mathf.Pow((transform.position.y - ship.transform.position.y), 2)) < ShipRange)
        {
            inRange = true;
        }
    }
}

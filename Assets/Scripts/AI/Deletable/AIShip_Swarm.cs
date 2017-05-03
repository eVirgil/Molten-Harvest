using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShip_Swarm : AIShip
{
    // Use this for initialization
    void Start()
    {
        HitPoints = 1;
        ShieldPoints = 0;
        DestroyTime = 12.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (HitPoints <= 0)
            Destroy();
    }


    private void Destroy()
    {
        Destroy(gameObject, DestroyTime);
    }


}

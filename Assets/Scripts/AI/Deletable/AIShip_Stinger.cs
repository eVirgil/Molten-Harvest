using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShip_Stinger : AIShip
{
    // Use this for initialization
    void Start()
    {
        HitPoints = 1;
        ShieldPoints = 0;
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

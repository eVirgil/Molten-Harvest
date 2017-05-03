using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShip_Frigate : AIShip
{
    AudioSource shieldLossAudio;
    //public float ShieldBreakDelay = 0.25f;

    // Use this for initialization
    void Start()
    {
        shieldLossAudio = GetComponent<AudioSource>();
        HitPoints = 1;
        ShieldPoints = 1;
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

    private IEnumerator BreakShield()
    {
        yield return new WaitForSecondsRealtime(ShieldBreakDelay);
        ShieldPoints = 0;
        shieldLossAudio.Play();
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShip_Dreadnaught : AIShip
{
    public bool isShieldUp = true;
    public float ShieldBrokenTime;
    public float ShieldRechargeTime = 5;
    public float ShieldBreakDelay = 1;
    AudioSource shieldLossAudio;
    AudioSource shieldRegainAudio;

    // Use this for initialization
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        shieldLossAudio = audioSources[1];
        shieldRegainAudio = audioSources[2];

        HitPoints = 1;
        DestroyTime = 120;        
    }

    // Update is called once per frame
    void Update()
    {
        if (isShieldUp == false && (ShieldBrokenTime + ShieldRechargeTime) >= Time.time)
        {
            RestoreShield();
        }
    }

    void Awake()
    {

    }

    private IEnumerator BreakShield()
    {
        yield return new WaitForSecondsRealtime(ShieldBreakDelay);
        isShieldUp = false;
        ShieldBrokenTime = Time.time;
        shieldLossAudio.Play();
    }

    private void RestoreShield()
    {
        isShieldUp = true;
        shieldRegainAudio.Play();
    }

    private void Destroy()
    {
        Destroy(gameObject, DestroyTime);
    }
}

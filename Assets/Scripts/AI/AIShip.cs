using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AIShip : NetworkBehaviour {

    public int HitPoints = 1;
    public float DestroyTime = 0.94f; //Change on collision detection

    //Shield Data
    public int ShieldPoints = 0;
    public float ShieldBrokenTime;
    public float ShieldRechargeTime = 5f;
    public float ShieldBreakDelay = .75f; //1sec: Dreadnought; .25: Frigate
    public AudioClip ShieldRegainAudio;
    public AudioClip ShieldLossAudio;
    public Sprite ShieldDownSprite;
    public Sprite ShieldUpSprite;
    public AnimationClip ShieldLossAnimation;
    public AnimationClip ShieldRegainAnimation;

    public bool isShieldUp {
        get { return ShieldPoints > 0; }
    }



	// Use this for initialization
	void Start () {
        HitPoints = 1;
        DestroyTime = 1.4f;
        if (ShieldPoints == 1)
        {
            GetComponent<SpriteRenderer>().sprite = ShieldUpSprite;
        }
        if (ShieldPoints == 0)
        {
            ShieldBrokenTime = int.MaxValue;
        }     
	}
	
	// Update is called once per frame
	void Update () {
        //| If HP is 0, Destroy object
        if (HitPoints <= 0)
            Destroy();

        //| If Shield is broken Check before restoring
        if (ShieldPoints == 0 && (ShieldBrokenTime + ShieldRechargeTime) <= Time.time)
        {
            RestoreShield();
        }        
	}

    private void Destroy()
    {
        Destroy(gameObject, DestroyTime);
    }

    public void BreakShield()
    {
        GetComponent<Animator>().SetBool("isShieldUp", false);
        GetComponent<SpriteRenderer>().sprite = ShieldDownSprite;
        GetComponent<AudioSource>().clip = ShieldLossAudio;
        GetComponent<AudioSource>().Play();
        ShieldBrokenTime = Time.time;
        ShieldPoints = 0;
    }

    private void RestoreShield()
    {
        GetComponent<Animator>().SetBool("isShieldUp", true);
        GetComponent<SpriteRenderer>().sprite = ShieldUpSprite;
        ShieldPoints = 1;
        GetComponent<AudioSource>().clip = ShieldRegainAudio;
        GetComponent<AudioSource>().Play();
    }


}

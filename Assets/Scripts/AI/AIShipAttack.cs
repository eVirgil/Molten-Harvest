using UnityEngine;
using System.Collections;

public class AIShipAttack : MonoBehaviour
{
    public GameObject ship;
    public GameObject weapon;
    public float weaponCooldown;
    public int ShipRange;
    public int ShotCount;

    float weaponFireTime = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Sqrt(Mathf.Pow((transform.position.x - ship.transform.position.x), 2) + Mathf.Pow((transform.position.y - ship.transform.position.y), 2)) < ShipRange)
        {
            Fire();
        }

    }
    private void Fire()
    {
        if(weapon != null) {
            if (Time.time > weaponFireTime + weaponCooldown) {
                for (int i = 0; i < ShotCount; i++) {
                    Instantiate(weapon, transform.position, transform.rotation);
                }
                weaponFireTime = Time.time;
            }
        }
    }   
}

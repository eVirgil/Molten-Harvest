using UnityEngine;
using System.Collections;

public class ShipCollisionManager : MonoBehaviour {

    public EnergyManager energyManager;
    public float MissileDamage;
    public float SmallEnergyDamage;
    public float LargeEnergyDamage;
    public float KamikazeDamage;

	void OnTriggerEnter2D(Collider2D collider)
    {
        float damage = 0.0f;

        if (collider.tag == "Missile")
        {
            damage = MissileDamage;
        }
        else if (collider.tag == "Small Energy")
        {
            damage = SmallEnergyDamage;
        }
        else if (collider.tag == "Large Energy")
        {
            damage = LargeEnergyDamage;
        }
        else if (collider.tag == "Enemy")
        {
            damage = KamikazeDamage;
        }
        else
        {
        }

        energyManager.decreaseEnergy(damage);
    }
}

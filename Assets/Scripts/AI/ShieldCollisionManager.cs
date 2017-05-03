using UnityEngine;
using System.Collections;

public class ShieldCollisionManager : MonoBehaviour
{

    public Transform explosionPrefab;
    public AudioClip explosionSound;
    private AudioSource audioSource;
    private GameObject explosion;

    private bool DidHitSomething = false;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = explosionSound;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        // Prevent enemies from colliding with one another
        if (collider.gameObject.tag != "Enemy" && GetComponent<AIShip>().ShieldPoints == 0 || collider.gameObject.tag == "Player")
        {
            DestroyEnemy();
        }
        else if (collider.gameObject.tag != "Enemy" && GetComponent<AIShip>().ShieldPoints == 1)
        {
            GetComponent<AIShip>().BreakShield();
        }
    }


    public void DestroyEnemy()
    {
        Destroy(gameObject);

    }

    void OnDestroy()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    }
}

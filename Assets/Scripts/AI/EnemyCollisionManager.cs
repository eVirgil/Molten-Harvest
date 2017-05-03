using UnityEngine;
using System.Collections;

public class EnemyCollisionManager : MonoBehaviour
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
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        // Prevent enemies from colliding with one another
        if (collider.gameObject.tag != "Enemy")
        {
            DestroyEnemy();
        }

        if( gameObject.tag == "Enemy" && collider.gameObject.tag == "Friendly") {
            DestroyEnemy();
            Destroy(collider.gameObject);
        }

        if( gameObject.tag == "Drone" && collider.gameObject.tag == "Laser") {
            DroneAI droneAI = gameObject.GetComponent<DroneAI>();
            if( droneAI != null) {
                droneAI.Damage();
            }
        }
    }


    public void DestroyEnemy() {
        Destroy(gameObject);
        
    }

    void OnDestroy() {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    }
}

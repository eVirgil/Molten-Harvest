using UnityEngine;
using System.Collections;

public class ExplosionTimeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource source = GetComponent<AudioSource>();
        if (source != null) {
            GetComponent<AudioSource>().Play();
        }
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

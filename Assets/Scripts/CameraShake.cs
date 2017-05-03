using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

    public bool Shaking;
    private float ShakeDecay;
    private float ShakeIntensity;
    private float ShakeDelay;
    private bool isShakeDecay = true;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    void Start() {
        Shaking = false;
        ShakeDelay = 0.0f;
    }


    // Update is called once per frame
    void Update() {
        if (ShakeIntensity > 0) {
            ShakeDelay += Time.deltaTime;
            if (ShakeDelay > 0.008f) {
                ShakeDelay = 0.0f;
                transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
                transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                                OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                                OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                                OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

                if (isShakeDecay) {
                    ShakeIntensity -= (ShakeDecay * Time.deltaTime);
                }
            }
        }
        else if (Shaking) {
            Shaking = false;

            transform.position = OriginalPos;
            transform.rotation = OriginalRot;
        }
    }

    public void DoShake() {
        
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;

        ShakeIntensity = 0.01f;
        ShakeDecay = 0.01f;
        Shaking = true;
        isShakeDecay = true;
    }

    public void Shake() {
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;

        ShakeIntensity = 0.01f;
        ShakeDecay = 0.005f;
        Shaking = true;
        isShakeDecay = false;
    }

    public void StopShake() {
        ShakeIntensity = 0.0f;
    }
}

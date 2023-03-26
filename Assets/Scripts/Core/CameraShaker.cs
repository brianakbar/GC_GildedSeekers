namespace Creazen.Seeker.Core {
    using System.Collections;
    using UnityEngine;

    public class CameraShaker : MonoBehaviour {
        [SerializeField] float shakeDuration = 1f;
        [SerializeField] float shakeMagnitude = 0.5f;

        Vector3 initialPosition;

        void Start() {
            initialPosition = Camera.main.transform.position;
        }

        public void Play() {
            StartCoroutine(Shake());
        }

        IEnumerator Shake() {
            float duration = shakeDuration;
            while(duration > 0) {
                Camera.main.transform.position = initialPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
                yield return new WaitForEndOfFrame();
                duration -= Time.deltaTime;
            }
            Camera.main.transform.position = initialPosition;
        }
    }
}
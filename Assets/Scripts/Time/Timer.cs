namespace Creazen.Seeker.Time {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class Timer : MonoBehaviour {
        [SerializeField] int initialTime = 60;
        [SerializeField] UnityEvent onCountdownComplete;

        int currentTime;
        public int CurrentTime {
            get {
                return currentTime;
            }
        }

        public int InitialTime {
            get {
                return initialTime;
            }
        }

        void Awake() {
            currentTime = initialTime;
        }

        void Start() {
            StartCoroutine(ProcessCountdown());
        }

        IEnumerator ProcessCountdown() {
            while(true) {
                yield return new WaitForSecondsRealtime(1);
                currentTime--;
                if(currentTime <= 0) {
                    if(onCountdownComplete != null) onCountdownComplete.Invoke();
                    yield break;
                }
            }
        }
    }
}
namespace Creazen.Seeker.Time {
    using System.Collections;
    using Creazen.Seeker.Session;
    using UnityEngine;
    using UnityEngine.Events;

    public class Timer : MonoBehaviour, ISession {
        [SerializeField] int initialTime = 60;
        [SerializeField] UnityEvent onCountdownComplete;

        Coroutine processCountdown;

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
            processCountdown = StartCoroutine(ProcessCountdown());
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

        void ISession.Reset() {
            currentTime = initialTime;
            if(processCountdown != null) {
                StopCoroutine(processCountdown);
                processCountdown = StartCoroutine(ProcessCountdown());
            }
        }
    }
}
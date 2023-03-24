namespace Creazen.Seeker.UI.Time {
    using Creazen.Seeker.Session;
    using Creazen.Seeker.Time;
    using TMPro;
    using UnityEngine;

    public class TimerUI : MonoBehaviour, ISession {
        TextMeshProUGUI timerText;
        Timer timer;

        void Awake() {
            timerText = GetComponent<TextMeshProUGUI>();
            timer = FindObjectOfType<Timer>();
        }

        void Update() {
            if(timer.CurrentTime <= 0) {
                gameObject.SetActive(false);
            }
            else {
                timerText.text = timer.CurrentTime.ToString();
            }
        }

        void ISession.Reset() {
            gameObject.SetActive(true);
        }
    }
}
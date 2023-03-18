namespace Creazen.Seeker.UI.Time {
    using Creazen.Seeker.Time;
    using TMPro;
    using UnityEngine;

    public class TimerUI : MonoBehaviour {
        TextMeshProUGUI timerText;
        Timer timer;

        void Awake() {
            timerText = GetComponent<TextMeshProUGUI>();
            timer = FindObjectOfType<Timer>();
        }

        void Update() {
            timerText.text = timer.CurrentTime.ToString();
        }
    }
}
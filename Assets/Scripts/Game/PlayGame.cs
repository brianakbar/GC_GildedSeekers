namespace Creazen.Seeker.Game {
    using Creazen.Seeker.LevelManagement;
    using UnityEngine;

    public class PlayGame : MonoBehaviour {
        public void Play(bool asPlayer) {
            FindObjectOfType<GamePreference>().AsPlayer = asPlayer;
            FindObjectOfType<LevelManager>().GoToNextLevel();
        }
    }
}
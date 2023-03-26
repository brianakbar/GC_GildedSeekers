namespace Creazen.Seeker.Game {
    using Creazen.Seeker.AI;
    using Creazen.Seeker.Control;
    using Unity.MLAgents;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GamePreference : MonoBehaviour {
        bool asPlayer = true;
        public bool AsPlayer { get { return asPlayer; } set { asPlayer = value; } }

        const string playerTag = "Player";

        void Awake() {
            SceneManager.activeSceneChanged += SetPlayer;
        }

        void SetPlayer(Scene current, Scene next) {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if(player == null) return;

            UnlockTreasureAgent agent = player.GetComponent<UnlockTreasureAgent>();
            PlayerController playerController = player.GetComponent<PlayerController>();
            if(asPlayer) {
                Debug.Log(asPlayer);
                Destroy(player.GetComponent<DecisionRequester>());
                if(agent != null) Destroy(agent);
                if(playerController != null) playerController.enabled = true;
            }
            else {
                if(playerController != null) Destroy(playerController);
                //if(agent != null) agent.enabled = true;
            }
        }
    }
}
namespace Creazen.Seeker.LevelManagement {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Creazen.Seeker.Transition;

    public class LevelManager : MonoBehaviour {
        public void RestartLevel() {
            StartCoroutine(ProcessLoadLevel(SceneManager.GetActiveScene().buildIndex));
        }

        public void GoToNextLevel() {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //StartCoroutine(ProcessLoadLevel(SceneManager.GetActiveScene().buildIndex));
            int nextLevelBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (!IsSceneExist(nextLevelBuildIndex)) {
                StartCoroutine(ProcessLoadLevel(0));
                return;
            }

            StartCoroutine(ProcessLoadLevel(nextLevelBuildIndex));
        }

        IEnumerator ProcessLoadLevel(int buildIndex) {
            Transition transition = FindObjectOfType<Transition>();
            if(transition != null) yield return transition.StartTransition();
            yield return SceneManager.LoadSceneAsync(buildIndex);
            if(transition != null) yield return transition.EndTransition();
        }

        bool IsSceneExist(int buildIndex) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            if (string.IsNullOrWhiteSpace(scenePath)) return false;
            return true;
        }
    }
}
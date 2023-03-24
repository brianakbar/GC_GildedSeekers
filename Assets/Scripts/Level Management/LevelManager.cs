namespace Creazen.Seeker.LevelManagement {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelManager : MonoBehaviour {
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
            yield return SceneManager.LoadSceneAsync(buildIndex);
        }

        bool IsSceneExist(int buildIndex) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            if (string.IsNullOrWhiteSpace(scenePath)) return false;
            return true;
        }
    }
}
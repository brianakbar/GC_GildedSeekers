namespace Creazen.Seeker.LevelManagement {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelManager : MonoBehaviour {
        public void GoToNextLevel() {
            int nextLevelBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (!IsSceneExist(nextLevelBuildIndex)) return;

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
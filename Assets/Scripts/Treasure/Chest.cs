namespace Creazen.Seeker.Treasure {
    using System.Collections;
    using System.Linq;
    using Creazen.Seeker.LevelManagement;
    using UnityEngine;

    public class Chest : MonoBehaviour {
        [SerializeField] float waitTimeAfterUnlock = 1f;

        int keyRequired;

        Animator animator = null;

        void Awake() {
            animator = GetComponent<Animator>();

            keyRequired = FindObjectsOfType<Key>().Count();
        }

        public bool Unlock(int keyCount) {
            if (keyCount < keyRequired) return false;
            animator.SetTrigger("unlock");
            StartCoroutine(ProcessAfterUnlock());
            return true;
        }

        IEnumerator ProcessAfterUnlock() {
            yield return new WaitForSecondsRealtime(waitTimeAfterUnlock);
            FindObjectOfType<LevelManager>().GoToNextLevel();
        }
    }
}


namespace Creazen.Seeker.Treasure {
    using System;
    using System.Collections;
    using System.Linq;
    using Creazen.Seeker.LevelManagement;
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class Chest : MonoBehaviour, ISession {
        [SerializeField] int keyRequired = 1;
        [SerializeField] float waitTimeAfterUnlock = 1f;

        public event Action onUnlock;

        Coroutine processAfterUnlock;

        Animator animator = null;

        void Awake() {
            animator = GetComponent<Animator>();
        }

        public bool Unlock(int keyCount) {
            if (keyCount < keyRequired) return false;
            animator.SetTrigger("unlock");
            if (onUnlock != null) onUnlock();
            //processAfterUnlock = StartCoroutine(ProcessAfterUnlock());
            return true;
        }

        IEnumerator ProcessAfterUnlock() {
            yield return new WaitForSecondsRealtime(waitTimeAfterUnlock);
            FindObjectOfType<LevelManager>().GoToNextLevel();
        }

        void ISession.Reset() {
            if(processAfterUnlock != null) {
                StopCoroutine(processAfterUnlock);
                processAfterUnlock = null;
            }
            animator.Rebind();
            animator.Update(0);
        }
    }
}


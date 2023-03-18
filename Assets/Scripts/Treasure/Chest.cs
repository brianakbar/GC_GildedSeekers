namespace Creazen.Seeker.Treasure {
    using System.Linq;
    using UnityEngine;

    public class Chest : MonoBehaviour {
        int keyRequired;

        Animator animator = null;

        void Awake() {
            animator = GetComponent<Animator>();

            keyRequired = FindObjectsOfType<Key>().Count();
        }

        public bool Unlock(int keyCount) {
            if (keyCount < keyRequired) return false;
            animator.SetTrigger("unlock");
            return true;
        }
    }
}


namespace Creazen.Seeker.Treasure {
    using UnityEngine;

    public class Key : MonoBehaviour {
        Animator animator;

        void Awake() {
            animator = GetComponent<Animator>();
        }

        public void Catch() {
            animator.SetTrigger("getKey");
        }

        public void Destroy() {
            Destroy(gameObject);
        }
    }
}
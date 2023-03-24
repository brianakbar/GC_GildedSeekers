namespace Creazen.Seeker.Treasure {
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class Key : MonoBehaviour, ISession {
        Animator animator;

        void Awake() {
            animator = GetComponent<Animator>();
        }

        public void Catch() {
            animator.SetTrigger("getKey");
        }

        public void Destroy() {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }

        void ISession.Reset() {
            gameObject.SetActive(true);
            animator.Rebind();
            animator.Update(0);
        }
    }
}
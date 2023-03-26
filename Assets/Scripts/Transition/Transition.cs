namespace Creazen.Seeker.Transition {
    using System.Collections;
    using UnityEngine;

    public class Transition : MonoBehaviour {
        [SerializeField] float animationLength = 3f;
        [SerializeField] RuntimeAnimatorController startTransition;
        [SerializeField] RuntimeAnimatorController endTransition;

        bool isTransitionFinished;

        Animator animator;

        void Awake() {
            animator = GetComponent<Animator>();
        }

        public IEnumerator StartTransition() {
            isTransitionFinished = false;
            animator.speed = 1 / animationLength;
            animator.runtimeAnimatorController = startTransition;
            yield return new WaitUntil(() => isTransitionFinished);
        }

        public IEnumerator EndTransition() {
            isTransitionFinished = false;
            animator.speed = 1 / animationLength;
            animator.runtimeAnimatorController = endTransition;
            yield return new WaitUntil(() => isTransitionFinished);
        }

        public void Finished() {
            isTransitionFinished = true;
        }
    }
}
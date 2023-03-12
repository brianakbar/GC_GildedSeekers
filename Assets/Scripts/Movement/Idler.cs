namespace Creazen.EFE.Movement {
    using Creazen.EFE.Core;
    using UnityEngine;

    [RequireComponent(typeof(ActionScheduler))]
    public class Idler : MonoBehaviour, IAction {
        ActionScheduler scheduler = null;
        Animator animator = null;

        void Awake() {
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        void Start() {
            StartAction();
        }

        public void StartAction() {
            scheduler.StartAction(this);
        }

        void IAction.Cancel() {}
    }
}
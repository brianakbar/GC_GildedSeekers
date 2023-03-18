namespace Creazen.Seeker.Core {
    using UnityEngine;

    public class ActionScheduler : MonoBehaviour {
        IAction currentAction = null;

        public void StartAction(IAction action) {
            if(currentAction != null) currentAction.Cancel();

            currentAction = action;
        }
    }
}
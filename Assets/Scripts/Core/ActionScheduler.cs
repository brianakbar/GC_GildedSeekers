namespace Creazen.Seeker.Core {
    using System;
    using UnityEngine;

    public class ActionScheduler : MonoBehaviour {
        IAction currentAction = null;
        IAction defaultAction = null;

        public void SetDefault(IAction action) {
            defaultAction = action;
        }

        public bool StartAction(IAction action) {
            if(currentAction != null) {
                foreach(Type actionType in action.ExcludeType()) {
                    if(actionType == currentAction.GetType()) return false;
                }
                currentAction.Cancel();
            }

            currentAction = action;
            return true;
        }

        public void Finish() {
            if(defaultAction == null) return;

            currentAction = defaultAction;
        }
    }
}
namespace Creazen.Seeker.Movement {
    using System;
    using System.Collections.Generic;
    using Creazen.Seeker.Combat;
    using Creazen.Seeker.Core;
    using UnityEngine;

    [RequireComponent(typeof(ActionScheduler))]
    public class Idler : MonoBehaviour, IAction {
        ActionScheduler scheduler = null;
        Animator animator = null;
        Rigidbody2D body = null;

        void Awake() {
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            body = GetComponent<Rigidbody2D>();
        }

        void Start() {
            scheduler.SetDefault(this);
            StartAction();
        }

        public void StartAction() {
            if(!scheduler.StartAction(this)) return;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        void IAction.Cancel() {}

        IEnumerable<Type> IAction.ExcludeType() {
            yield return typeof(Fighter);
        }
    }
}
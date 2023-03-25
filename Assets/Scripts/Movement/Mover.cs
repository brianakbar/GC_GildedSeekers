namespace Creazen.Seeker.Movement {
    using System;
    using System.Collections.Generic;
    using Creazen.Seeker.Combat;
    using Creazen.Seeker.Core;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D), typeof(ActionScheduler))]
    public class Mover : MonoBehaviour, IAction {
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float[] jumpSpeeds = {5f};

        public event Action onLand;

        float floatZeroTolerance = 0.001f;
        bool isMoving = false;
        int numberOfJumpLeft;
        float moveDirection;

        const string terrainTag = "Terrain";

        Rigidbody2D body = null;
        ActionScheduler scheduler = null;
        Animator animator = null;

        void Awake() {
            body = GetComponent<Rigidbody2D>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();

            numberOfJumpLeft = jumpSpeeds.Length;
        }

        void Update() {
            ProcessMoving();

            if(animator == null) return;

            animator.SetBool("hasXVelocity",  !IsApproximatelyZero(body.velocity.x));
            animator.SetBool("hasYVelocity",  !IsApproximatelyZero(body.velocity.y));
            animator.SetFloat("yVelocity",  body.velocity.y);
        }

        public void StartAction(bool isRight) {
            if(!scheduler.StartAction(this)) return;
            moveDirection = isRight? 1 : -1;
            isMoving = true;
        }

        public bool Jump() {
            if(!scheduler.StartAction(this)) return false;

            if(numberOfJumpLeft > 0) {
                float jumpSpeed = jumpSpeeds[jumpSpeeds.Length - numberOfJumpLeft];
                body.velocity = new Vector2(body.velocity.x, jumpSpeed);
                numberOfJumpLeft--;
                return true;
            }
            return false;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(other.tag == terrainTag) {
                numberOfJumpLeft = jumpSpeeds.Length;
                if(onLand != null) onLand();
            }
        }

        void ProcessMoving() {
            if(isMoving) {
                transform.localScale = new Vector2(moveDirection, transform.localScale.y);
                body.velocity = new Vector2(moveSpeed  * moveDirection, body.velocity.y);
            }
        }

        bool IsApproximatelyZero(float value) {
            return (-floatZeroTolerance < value) && (value < floatZeroTolerance);
        }

        void IAction.Cancel() {
            isMoving = false;
        }

        IEnumerable<Type> IAction.ExcludeType() {
            yield return typeof(Fighter);
        }
    }
}
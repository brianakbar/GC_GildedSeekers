namespace Creazen.Seeker.Combat {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Creazen.Seeker.Attributes;
    using Creazen.Seeker.Core;
    using UnityEngine;

    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction {
        [SerializeField] float speed = 10f;
        [SerializeField] float anticipationWaitTime = 1f;
        [SerializeField] Vector2 knockoutSpeed = new Vector2();
        [SerializeField] float knockoutRestoreTime = 2f;
        [SerializeField] Collider2D hitBox = null;

        Coroutine attackProcess;

        Animator animator = null;
        ActionScheduler scheduler = null;
        Rigidbody2D body = null;

        void Awake() {
            animator = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
            body = GetComponent<Rigidbody2D>();
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(other.isTrigger) return;
            if(!other.IsTouching(hitBox)) return;

            float moveDirection = Mathf.Sign(transform.localScale.x);
            if(other.TryGetComponent(out Health health)) {
                Vector2 knockoutVelocity = new Vector2(knockoutSpeed.x * moveDirection, knockoutSpeed.y);
                health.TakeDamage(1, knockoutVelocity);
            }
            else {
                hitBox.enabled = false;
                Vector2 knockoutVelocity = new Vector2(knockoutSpeed.x * -1 * moveDirection, knockoutSpeed.y);
                body.velocity = knockoutVelocity;
                animator.SetTrigger("hitWall");
                StartCoroutine(ProcessKnockout());
            }
        }

        public void StartAction() {
            if(!Mathf.Approximately(body.velocity.y, 0)) return;

            if(!scheduler.StartAction(this)) return;
            attackProcess = StartCoroutine(ProcessAttack());
        }

        IEnumerator ProcessAttack() {
            body.velocity = new Vector2(0, body.velocity.y);
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("anticipate");
            yield return new WaitForSeconds(anticipationWaitTime);
            animator.SetTrigger("attack");
            float moveDirection = Mathf.Sign(transform.localScale.x);
            body.velocity = new Vector2(speed  * moveDirection, body.velocity.y);
            hitBox.enabled = true;
        }

        IEnumerator ProcessKnockout() {
            float moveDirection = Mathf.Sign(transform.localScale.x);
            while(true) {
                if(Mathf.Approximately(body.velocity.y, 0)) break;

                Vector2 verticalV = new Vector2(0, body.velocity.y);
                Vector2 horizontalV = new Vector2(body.velocity.x, 0);

                float heading = moveDirection * Vector3.Angle(verticalV + horizontalV, Vector3.up);
                transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, heading);

                yield return null;
            }
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            body.velocity = new Vector2(0, body.velocity.y);
            yield return new WaitForSeconds(knockoutRestoreTime);
            animator.SetBool("isAttacking", false);
            scheduler.Finish();
        }

        void IAction.Cancel() {
            StopCoroutine(attackProcess);
        }

        IEnumerable<Type> IAction.ExcludeType() {
            yield return typeof(Fighter);
        }
    }
}
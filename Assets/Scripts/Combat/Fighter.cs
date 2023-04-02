namespace Creazen.Seeker.Combat {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Creazen.Seeker.Attributes;
    using Creazen.Seeker.Core;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction {
        [SerializeField] float speed = 10f;
        [SerializeField] float anticipationWaitTime = 1f;
        [SerializeField] Vector2 knockoutSpeed = new Vector2();
        [SerializeField] float knockoutRestoreTime = 2f;
        [SerializeField] float attackRestoreTime = 0.1f;
        [SerializeField] Collider2D hitBox = null;
        [SerializeField] UnityEvent onHitWall;

        public event Action<GameObject> onHit;

        bool canAttack = true;
        float floatZeroTolerance = 0.001f;
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
                if(onHitWall != null) onHitWall.Invoke();
                StartCoroutine(ProcessKnockout());
            }
            if(onHit != null) onHit(other.gameObject);
        }

        public bool StartAction() {
            if(!canAttack) return false;
            if(!Mathf.Approximately(body.velocity.y, 0)) return false;

            if(!scheduler.StartAction(this)) return false;
            attackProcess = StartCoroutine(ProcessAttack());
            return true;
        }

        IEnumerator ProcessAttack() {
            canAttack = false;
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
                if(IsApproximatelyZero(body.velocity.y)) break;

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
            yield return new WaitForSeconds(attackRestoreTime);
            canAttack = true;
        }

        bool IsApproximatelyZero(float value) {
            return (-floatZeroTolerance < value) && (value < floatZeroTolerance);
        }

        void IAction.Cancel() {
            StopCoroutine(attackProcess);
        }

        IEnumerable<Type> IAction.ExcludeType() {
            yield return typeof(Fighter);
        }
    }
}
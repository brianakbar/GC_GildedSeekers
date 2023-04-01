namespace Creazen.Seeker.Attributes {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Creazen.Seeker.Core;
    using Creazen.Seeker.LevelManagement;
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class Health : MonoBehaviour, IAction, ISession {
        [SerializeField] float healthPoints = 1f;
        [SerializeField] Vector2 damageSpeed = new Vector2();
        [SerializeField] float waitTimeAfterDeath = 3f;

        public event Action onTakeDamage;
        public event Action onDie;

        Coroutine damageProcess;

        Rigidbody2D body = null;
        ActionScheduler scheduler = null;
        Animator animator = null;

        void Awake() {
            body = GetComponent<Rigidbody2D>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage, Vector2 knockoutVelocity) {
            if(!scheduler.StartAction(this)) return;

            if(onTakeDamage != null) onTakeDamage();
            damageProcess = StartCoroutine(ProcessDamage(damage, knockoutVelocity));
        }

        public bool IsDead() {
            return healthPoints <= 0;
        }

        IEnumerator ProcessDamage(float damage, Vector2 knockoutVelocity) {
            healthPoints -= damage;
            if(IsDead()) {
                foreach(Collider2D collider in GetComponentsInChildren<Collider2D>()) {
                    collider.enabled = false;
                }
            }

            float moveDirection = Mathf.Sign(knockoutVelocity.x) * -1;
            transform.localScale = new Vector2(moveDirection, transform.localScale.y);
            Vector2 damageVelocity = knockoutVelocity;
            if(!IsDead()) damageVelocity = new Vector2(damageSpeed.x * -1 * moveDirection, damageSpeed.y);

            body.velocity = damageVelocity;
            animator.SetBool("isTakingDamage", true);

            if(IsDead()) StartCoroutine(ProcessDeath());
            
            while(true) {
                if(Mathf.Approximately(body.velocity.y, 0)) break;

                if(IsDead()) {
                    Vector2 verticalV = new Vector2(0, body.velocity.y);
                    Vector2 horizontalV = new Vector2(body.velocity.x, 0);

                    float heading = moveDirection * Vector3.Angle(verticalV + horizontalV, Vector3.up);
                    transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, heading);
                }

                yield return null;
            }

            transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            body.velocity = new Vector2(0, body.velocity.y);

            animator.SetBool("isTakingDamage", false);
            scheduler.Finish();
        }

        IEnumerator ProcessDeath() {
            if(onDie != null) onDie();
            yield return new WaitForSeconds(waitTimeAfterDeath);

            FindObjectOfType<LevelManager>().RestartLevel();
        }

        IEnumerable<Type> IAction.ExcludeType() {
            return Enumerable.Empty<Type>();
        }

        void IAction.Cancel() {
            StopCoroutine(damageProcess);
        }

        void ISession.Reset() {
            foreach(Collider2D collider in GetComponentsInChildren<Collider2D>()) {
                collider.enabled = true;
            }
            if(damageProcess != null) {
                StopCoroutine(damageProcess);
                animator.SetBool("isTakingDamage", false);
                animator.Rebind();
                animator.Update(0);
            }
            body.velocity = new Vector2(0, body.velocity.y);
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }
}
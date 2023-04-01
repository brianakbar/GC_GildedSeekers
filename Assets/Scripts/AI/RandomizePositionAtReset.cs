namespace Creazen.Seeker.AI {
    using System.Collections;
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class RandomizePositionAtReset : MonoBehaviour, ISession {
        [SerializeField] Collider2D bodyCollider;
        [SerializeField] Vector2 resetArea;

        void Start() {
            StartCoroutine(ChangePosition());
        }

        void OnJump() {
            StartCoroutine(ChangePosition());
        }

        IEnumerator ChangePosition() {
            do {
                Vector2 randomPosition;
                float randomX = Random.Range(-resetArea.x/2, resetArea.x/2);
                float randomY = Random.Range(-resetArea.y/2, resetArea.y/2);
                randomPosition = new Vector2(randomX, randomY);
                transform.localPosition = randomPosition;
                yield return new WaitForSeconds(0.1f);
                //Debug.Log(bodyCollider.IsTouchingLayers(LayerMask.GetMask("Terrain")));
            } while(bodyCollider.IsTouchingLayers(LayerMask.GetMask("Terrain")));
        }

        void OnDrawGizmosSelected() {
            Gizmos.DrawWireCube(Vector3.zero, resetArea);
        }

        void ISession.Reset() {
            StartCoroutine(ChangePosition());
        }
    }
}
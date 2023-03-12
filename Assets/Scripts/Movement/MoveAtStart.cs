namespace Creazen.EFE.Movement {
    using UnityEngine;

    public class MoveAtStart : MonoBehaviour {
        [SerializeField] Vector2 velocity = new Vector2();

        Rigidbody2D body = null;

        void Awake() {
            body = GetComponent<Rigidbody2D>();
        }

        void Start() {
            if(body == null) return;

            body.velocity = velocity;
        }
    }
}
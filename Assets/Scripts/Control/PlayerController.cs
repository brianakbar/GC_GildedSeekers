namespace Creazen.Seeker.Control {
    using Creazen.Seeker.Combat;
    using Creazen.Seeker.Movement;
    using Creazen.Seeker.Treasure;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour {
        Idler idler = null;
        Mover mover = null;
        KeyHolder keyHolder = null;
        Fighter fighter = null;

        void Awake() {
            idler = GetComponent<Idler>();
            mover = GetComponent<Mover>();
            keyHolder = GetComponent<KeyHolder>();
            fighter = GetComponent<Fighter>();
        }

        void OnMove(InputValue value) {
            if(mover == null) return;

            float moveInput = value.Get<float>();
            if(moveInput == 0) {
                idler.StartAction();
            }
            else {
                mover.StartAction(moveInput == 1f? true : false);
            }
        }

        void OnJump() {
            mover.Jump();
        }

        void OnOpenChest() {
            if(keyHolder != null) keyHolder.UnlockChest();
        }

        void OnAttack() {
            if(fighter != null) fighter.StartAction();
        }
    }
}
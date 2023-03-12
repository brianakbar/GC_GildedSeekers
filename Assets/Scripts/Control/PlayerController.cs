namespace Creazen.EFE.Control {
    using Creazen.EFE.Movement;
    using Creazen.EFE.Treasure;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour {
        Idler idler = null;
        Mover mover = null;
        KeyHolder keyHolder = null;

        void Awake() {
            idler = GetComponent<Idler>();
            mover = GetComponent<Mover>();
            keyHolder = GetComponent<KeyHolder>();
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
            keyHolder.UnlockChest();
        }
    }
}
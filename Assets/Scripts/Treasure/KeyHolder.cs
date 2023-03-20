namespace Creazen.Seeker.Treasure {
    using System;
    using UnityEngine;

    public class KeyHolder : MonoBehaviour {
        [SerializeField] float chestOpenRadius = 5f;

        public event Action onCatch;

        int keyCount = 0;

        void OnTriggerEnter2D(Collider2D other) {
            if(other.TryGetComponent<Key>(out Key key)) {
                key.Catch();
                keyCount++;
                if(onCatch != null) onCatch();
            }
        }

        public int GetKeyCount() {
            return keyCount;
        }

        public bool UnlockChest() {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, chestOpenRadius);
            foreach(Collider2D collider in colliders) {
                if(collider.TryGetComponent<Chest>(out Chest chest)) {
                    if(chest.Unlock(keyCount)) return true;
                    return false;
                }
            }
            return false;
        }
    }
}
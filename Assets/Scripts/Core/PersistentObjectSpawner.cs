namespace Creazen.Seeker.Core {
    using UnityEngine;

    public class PersistentObjectSpawner : MonoBehaviour {
        [SerializeField] GameObject persistentObjectsPrefab;

        static bool hasSpawned = false;

        void Awake() {
            if(hasSpawned) return;

            SpawnPersistentObjects();
            hasSpawned = true;
        }

        public void SetHasSpawned(bool state) {
            hasSpawned = state;
        }

        void SpawnPersistentObjects() {
            GameObject persistentObjects = Instantiate(persistentObjectsPrefab);
            DontDestroyOnLoad(persistentObjects);
        }
    }
}

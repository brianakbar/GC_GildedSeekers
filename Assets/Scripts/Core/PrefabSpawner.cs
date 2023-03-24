namespace Creazen.Seeker.Core {
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class PrefabSpawner : MonoBehaviour, ISession {
        [SerializeField] GameObject prefabToSpawn;
        [SerializeField] Vector3 spawnPosition;
        [SerializeField] Transform parent;

        GameObject spawnedPrefab = null;

        public void Spawn() {
            spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            if(parent != null) {
                spawnedPrefab.transform.SetParent(parent);
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.DrawIcon(spawnPosition, "Idle 01.png");
        }

        void ISession.Reset() {
            Destroy(spawnedPrefab);
        }
    }
}
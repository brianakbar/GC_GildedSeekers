namespace Creazen.Seeker.Core {
    using Creazen.Seeker.Session;
    using UnityEngine;

    public class PrefabSpawner : MonoBehaviour, ISession {
        [SerializeField] GameObject prefabToSpawn;
        [SerializeField] Vector3 spawnPosition;
        [SerializeField] Transform parent;

        GameObject spawnedPrefab = null;

        public void Spawn() {
            if(prefabToSpawn == null) return; 
            
            if(spawnPosition != null) {
                spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition + parent.position, Quaternion.identity, parent);
            }
            else {
                spawnedPrefab = Instantiate(prefabToSpawn, parent);
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.DrawIcon(spawnPosition, "Idle 01.png");
        }

        void ISession.Reset() {
            if(spawnedPrefab) {
                Destroy(spawnedPrefab);
            }
        }
    }
}
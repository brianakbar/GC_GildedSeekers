namespace Creazen.Seeker.Core {
    using UnityEngine;

    public class PrefabSpawner : MonoBehaviour {
        [SerializeField] GameObject prefabToSpawn;
        [SerializeField] Vector3 spawnPosition;
        [SerializeField] Transform parent;
        
        public void Spawn() {
            GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            if(parent != null) {
                instance.transform.SetParent(parent);
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.DrawIcon(spawnPosition, "Idle 01.png");
        }
    }
}
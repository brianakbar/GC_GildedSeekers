namespace Creazen.Seeker.AI {
    using UnityEditor;
    using UnityEngine;

    public class TrainEnvironmentManager : MonoBehaviour {
        [SerializeField] GameObject trainEnvironmentPrefab;
        [SerializeField] Vector2 spacing = new Vector2();
        [SerializeField] Vector2 size = new Vector2(5, 2);

        void OnValidate() {
            EditorApplication.delayCall += RebuildEnvironment;
        }

        void RebuildEnvironment() {
            for (int i = transform.childCount; i > 0; --i) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            if(trainEnvironmentPrefab == null) return;

            for(int y = 0; y < size.y; y++) {
                for(int x = 0; x < size.x; x++) {
                    Instantiate(trainEnvironmentPrefab, new Vector2(x * spacing.x, y * spacing.y), 
                                Quaternion.identity, transform);
                }
            }
        }
    }
}
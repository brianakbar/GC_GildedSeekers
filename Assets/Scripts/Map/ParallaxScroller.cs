namespace Creazen.Seeker.Map {
    using UnityEngine;
    using UnityEngine.UI;

    public class ParallaxScroller : MonoBehaviour {
        [SerializeField] Vector2 moveSpeed;

        Material material;

        void Awake() {
            material = GetComponent<Image>().material;
        }

        void Update() {
            material.mainTextureOffset += moveSpeed * Time.deltaTime;
        }
    }
}
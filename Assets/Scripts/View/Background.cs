using UnityEngine;

namespace ElementsGame{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Background : MonoBehaviour
    {
        private SpriteRenderer _sRenderer;
        private Camera _camera;

        private void Awake()
        {
            _sRenderer = GetComponent<SpriteRenderer>();
            _camera = Camera.main;
        }

        private void Start()
        {
            AdjustBackground();
        }

        private void AdjustBackground()
        {
            Vector3 leftBottom = _camera.ScreenToWorldPoint(Vector3.zero);
            Vector3 rightTop = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            float screenWidth = rightTop.x - leftBottom.x;
            float screenHeight = rightTop.y - leftBottom.y;

            float spriteWidth = _sRenderer.sprite.bounds.size.x;
            float spriteHeight = _sRenderer.sprite.bounds.size.y;

            float widthCoef = screenWidth / spriteWidth;
            float heightCoef = screenHeight / spriteHeight;

            float coef = Mathf.Max(widthCoef, heightCoef);

            transform.localScale = Vector3.one * coef;
        }
    }
}


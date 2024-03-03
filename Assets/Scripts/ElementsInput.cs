using ElementsGame.Core;
using ElementsGame.View;
using UnityEngine;

namespace ElementsGame._Input{

    public class ElementsInput : MonoBehaviour
    {
        // for now I use an old input system
        [SerializeField] private ElementsView _gridView;

        private ElementsGrid _grid;

        private bool _isSelected;
        private bool _isMoved;

        private Camera _camera;

        private void Awake(){
            _camera = Camera.main;
        }

        public void Init(ElementsGrid grid){
            _grid = grid;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0)){
                Vector2 clickPoint = _camera.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(clickPoint, Vector2.zero);

                if(hit.collider){
                    Vector2Int gridPoint = _gridView.GetGridCoords(hit.point);
                    Debug.Log(gridPoint);
                }
            }
        }
    }
}

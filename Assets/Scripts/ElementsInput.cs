using ElementsGame.Core;
using ElementsGame.View;
using UnityEngine;

namespace ElementsGame._Input{
    // for now I use an old input system
    public class ElementsInput : MonoBehaviour
    {
        [SerializeField] private ElementsView _gridView;
        [SerializeField] private float _movementThreshold = 5f;

        private ElementsGrid _grid;

        private bool _isSelected;
        private bool _isMoved;

        private Camera _camera;

        private Vector2 _startPoint = Vector3.zero;
        private Vector2Int _gridPointPos;

        private void Awake(){
            _camera = Camera.main;
        }

        public void Init(ElementsGrid grid){
            _grid = grid;
        }

        private void Update()
        {
            Vector2 clickPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0)){
                RaycastHit2D hit = Physics2D.Raycast(clickPoint, Vector2.zero);

                if(hit.collider){
                    _gridPointPos = _gridView.GetGridCoords(hit.point);
                    _isSelected = true;
                    _startPoint = clickPoint;
                }
            }
        
            if(Input.GetMouseButton(0) && _isSelected){
                Vector2 directionOfMove = clickPoint - _startPoint;
                if (!_isMoved){
                    float distance = directionOfMove.magnitude;
                    _isMoved = distance > _gridView.CellSize * 0.5f;
                }
                else{
                    MoveType move = GetMoveTypeByVector(directionOfMove);
                    _grid.TryToMoveOnPos(_gridPointPos, move);
                    ResetState();
                }
            }

            if(Input.GetMouseButtonUp(0)){
                ResetState();
            }
        }

        private void ResetState(){
            _isSelected = false;
            _isMoved = false;
        }

        private MoveType GetMoveTypeByVector(Vector2 vector){
            float dotUp = Vector2.Dot(vector, Vector2.up);
            float dotDown = Vector2.Dot(vector, Vector2.down);
            float dotLeft = Vector2.Dot(vector, Vector2.left);
            float dotRight = Vector2.Dot(vector, Vector2.right);

            float largest = Mathf.Max(dotUp, dotDown, dotLeft, dotRight);

            MoveType move = MoveType.Right;

            if(largest == dotUp){
                move = MoveType.Up;
            }
            else if (largest == dotDown){
                move = MoveType.Down;
            }
            else if(largest == dotLeft){
                move = MoveType.Left;
            }

            return move;
        }
    }
}

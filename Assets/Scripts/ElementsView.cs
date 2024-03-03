using System.Collections.Generic;
using ElementsGame.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

namespace ElementsGame.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ElementsView : MonoBehaviour
    {
        // can be configed
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private ViewBlock _viewBlockPrefab;
        [Header("Debug"), SerializeField] private bool _isShowGrid = true;

        private ElementsGrid _grid;
        private Vector2[,] _posMatrix;

        private Dictionary<int, ViewBlock> _viewBlocks = new Dictionary<int, ViewBlock>();

        private BoxCollider2D _collider;

        private int _xSize, _ySize;

        private void Awake() {
            _collider = GetComponent<BoxCollider2D>();
        }

        public void Init(ElementsGrid grid)
        {
            _grid = grid;

            int[,] typeMatrix = _grid.GetTypeMatrix();
            _ySize = typeMatrix.GetLength(0);
            _xSize = typeMatrix.GetLength(1);

            InitPosMatrix();
            InitViewBlocks();
            SetBlocksPos();
            InitCollider();

            _grid.OnUpdated += UpdateGrid;
        }

        private void UpdateGrid()
        {
            int[,] ids = _grid.GetIdsMatrix();

            for (int y = 0; y < ids.GetLength(0); y++)
            {
                for (int x = 0; x < ids.GetLength(1); x++)
                {
                    if(ids[y, x] == 0){
                        continue;
                    }

                    int id = ids[y, x];
                    ViewBlock block = _viewBlocks[id];
                    Vector3 pos = _posMatrix[y, x];
                    block.SetPos(pos);
                }
            }
        }

        public Vector2Int GetGridCoords(Vector3 pos){
            Vector3 p = pos;
            p -= transform.position;
            int x = (int)(p.x / _cellSize + _xSize * 0.5f);
            int y = (int)(p.y / _cellSize + _ySize * 0.5f);

            Vector2Int result = new Vector2Int(x, y);

            return result;
        }

        private void InitCollider(){
            _collider.size = new Vector2(_xSize * _cellSize, _ySize * _cellSize);
        }

        private void SetBlocksPos()
        {
            int[,] idMatrix = _grid.GetIdsMatrix();

            for (int y = 0; y < idMatrix.GetLength(0); y++)
            {
                for (int x = 0; x < idMatrix.GetLength(1); x++)
                {
                    int id = idMatrix[y, x];
                    if (id == 0){
                        continue;
                    }

                    ViewBlock block = _viewBlocks[id];
                    Vector3 pos = _posMatrix[y ,x];
                    block.SetPos(pos);
                }
            }
        }

        private void InitViewBlocks(){
            _viewBlocks.Clear();

            int[,] typeMatrix = _grid.GetTypeMatrix();
            int[,] idMatrix = _grid.GetIdsMatrix();

            for (int y = 0; y < typeMatrix.GetLength(0); y++)
            {
                for (int x = 0; x < typeMatrix.GetLength(1); x++)
                {
                    if(typeMatrix[y, x] == 0)
                    {
                        continue;
                    }
                    int id = idMatrix[y ,x];
                    int type = typeMatrix[y, x];

                    ViewBlock viewBlock = Instantiate(_viewBlockPrefab, transform);                    
                    viewBlock.Init(type, new Vector2Int(x, y));
                    _viewBlocks.Add(id, viewBlock);
                }
            }
        }

        private void InitPosMatrix(){
            _posMatrix = new Vector2[_ySize, _xSize];
            for (int y = 0; y < _ySize; y++)
            {
                for (int x = 0; x < _xSize; x++)
                {
                    float xCoord = (x + 0.5f - _xSize * 0.5f) * _cellSize;
                    float yCoord = (y + 0.5f - _ySize * 0.5f) * _cellSize;
                    _posMatrix[y, x] = new Vector2(xCoord, yCoord);
                }
            }
        }

        private void OnDrawGizmos() {
            if (_isShowGrid)
            {
                Gizmos.color = Color.red;
                ShowGridPointsGizmos();
                Gizmos.color = Color.white;
                ShowGridGizmos();
            }
        }

        private void ShowGridGizmos(){
            if(_posMatrix == null)  return;

            for (int y = 0; y < _posMatrix.GetLength(0); y++)
            {
                for (int x = 0; x < _posMatrix.GetLength(1); x++)
                {
                    Vector3 rightTop = transform.position + (Vector3)(_posMatrix[x, y] + new Vector2(1, 1) * _cellSize * 0.5f);
                    Vector3 rightBottom = transform.position + (Vector3)(_posMatrix[x, y] + new Vector2(1, -1) * _cellSize * 0.5f);
                    Vector3 leftBottom = transform.position + (Vector3)(_posMatrix[x, y] + new Vector2(-1, -1) * _cellSize * 0.5f);
                    Vector3 leftTop = transform.position + (Vector3)(_posMatrix[x, y] + new Vector2(-1, 1) * _cellSize * 0.5f) ;
                    
                    Gizmos.DrawLine(rightTop, rightBottom);
                    Gizmos.DrawLine(rightBottom, leftBottom);
                    Gizmos.DrawLine(leftBottom, leftTop);
                    Gizmos.DrawLine(leftTop, rightTop);
                }
            }
        }

        private void ShowGridPointsGizmos(){
            if(_posMatrix == null)  return;

            for(int y = 0; y < _posMatrix.GetLength(0); y++){
                for(int x = 0; x < _posMatrix.GetLength(1); x++){
                    Gizmos.DrawSphere(transform.position + (Vector3)_posMatrix[x, y], 0.1f);
                }
            }
        }
    }
}

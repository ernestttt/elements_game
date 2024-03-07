using System.Collections.Generic;
using ElementsGame.Core;
using UnityEngine;
using System.Linq;
using ElementsGame.Data;

namespace ElementsGame.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ElementsView : MonoBehaviour
    {
        [SerializeField] private ViewBlock _viewBlockPrefab;
        [SerializeField] private Transform _groundPoint;
        [Header("Debug"), SerializeField] private bool _isShowGrid = true;

        private float _cellSize;

        private ElementsGrid _grid;
        private Vector2[,] _posMatrix;

        private Dictionary<int, ViewBlock> _viewBlocks = new Dictionary<int, ViewBlock>();

        private BoxCollider2D _collider;

        private int _xSize, _ySize; 

        private bool _isUpdated = false;

        private Camera _camera;

        public float CellSize => _cellSize;

        private void Awake() {
            _collider = GetComponent<BoxCollider2D>();
            _camera = Camera.main;
        }

        private void Update(){
            if(_viewBlocks.Values.Any(a => a.IsUpdated))    return;

            if (_isUpdated && !_grid.Normalize() && !_grid.UpdateMatch3())
            {
                _isUpdated = false;
            }
        }

        public void Init(ElementsGrid grid)
        {
            _grid = grid;
            _grid.OnUpdated += NormalizeGrid;
            _grid.OnMatched += MatchGrid;
            _grid.OnWin += OnWinHandler;
            _grid.OnLoose += OnLooseHandler;
            _grid.OnStarted += OnStartedHandler;
        }

        private void OnStartedHandler(){
            ResetSquares();
            int[,] typeMatrix = _grid.GetTypeMatrix();
            _ySize = typeMatrix.GetLength(0);
            _xSize = typeMatrix.GetLength(1);

            AdjustCellCize();
            AdjustPos();
            InitPosMatrix();
            InitViewBlocks();
            SetBlocksPos();
            InitCollider();
        }

        private void AdjustPos(){
            Vector2 lowerBorderPoint = transform.position + Vector3.down * _cellSize * _ySize * 0.5f;
            Vector2 diff = (Vector2)_groundPoint.position - lowerBorderPoint;
            transform.position += (Vector3)diff;
        }

        private void AdjustCellCize(){
            Vector3 leftBottom = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            Vector3 rightTop = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            float widthWorld = rightTop.x - leftBottom.x;
            float heightWorld = rightTop.y - leftBottom.y;

            widthWorld *= .9f;
            heightWorld *= .9f;

            float widthSize = widthWorld / _xSize;
            float heightSize = heightWorld / _ySize;

            _cellSize = Mathf.Min(widthSize, heightSize);
        }

        private void OnWinHandler(){
            
        }

        private void OnLooseHandler()
        {
            
        }

        private void ResetSquares(){
            foreach(var block in _viewBlocks)
            {
                Destroy(block.Value.gameObject);
            }
            _viewBlocks.Clear();
        }

        private void MatchGrid(int[] ids)
        {
            foreach (int id in ids){
                ViewBlock block = _viewBlocks[id];
                block.DestroyBlock();
            }
            _isUpdated = true;
        }

        private void NormalizeGrid()
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
                    block.SetTargetPos(pos, y * _xSize + x);
                }
            }

            _isUpdated = true;
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
                    block.SetPos(pos, y * _xSize + x);
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
                    viewBlock.Init(type, new Vector2Int(x, y), _cellSize);
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
                    _posMatrix[y, x] = (Vector2)transform.position + new Vector2(xCoord, yCoord);
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

            for (int y = 0; y < _posMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < _posMatrix.GetLength(0); x++)
                {
                    Vector3 rightTop = (Vector3)(_posMatrix[x, y] + new Vector2(1, 1) * _cellSize * 0.5f);
                    Vector3 rightBottom = (Vector3)(_posMatrix[x, y] + new Vector2(1, -1) * _cellSize * 0.5f);
                    Vector3 leftBottom = (Vector3)(_posMatrix[x, y] + new Vector2(-1, -1) * _cellSize * 0.5f);
                    Vector3 leftTop = (Vector3)(_posMatrix[x, y] + new Vector2(-1, 1) * _cellSize * 0.5f) ;
                    
                    Gizmos.DrawLine(rightTop, rightBottom);
                    Gizmos.DrawLine(rightBottom, leftBottom);
                    Gizmos.DrawLine(leftBottom, leftTop);
                    Gizmos.DrawLine(leftTop, rightTop);
                }
            }
        }

        private void ShowGridPointsGizmos(){
            if(_posMatrix == null)  return;

            for(int y = 0; y < _posMatrix.GetLength(1); y++){
                for(int x = 0; x < _posMatrix.GetLength(0); x++){
                    Gizmos.DrawSphere((Vector3)_posMatrix[x, y], 0.1f);
                }
            }
        }
    }
}

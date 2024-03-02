using System.Collections.Generic;
using ElementsGame.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

namespace ElementsGame.View
{
    public class ElementsView : MonoBehaviour
    {
        // can be configed
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private ViewBlock _viewBlockPrefab;
        [Header("Debug"), SerializeField] private bool _isShowGrid = true;

        private ElementsGrid _grid;
        private Vector2[,] _posMatrix;

        private Dictionary<int, ViewBlock> _viewBlocks = new Dictionary<int, ViewBlock>();

        public void Init(ElementsGrid grid)
        {
            _grid = grid;

            int[,] typeMatrix = _grid.GetTypeMatrix();
            int ySize = typeMatrix.GetLength(0);
            int xSize = typeMatrix.GetLength(1);

            InitPosMatrix(xSize, ySize);
            InitViewBlocks();
            SetBlocksPos();
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

        private void InitPosMatrix(int xSize, int ySize){
            _posMatrix = new Vector2[ySize, xSize];
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    float xCoord = (x + 0.5f - xSize * 0.5f) * _cellSize;
                    float yCoord = (y + 0.5f - ySize * 0.5f) * _cellSize;
                    _posMatrix[y, x] = new Vector2(xCoord, yCoord);
                }
            }
        }

        private void Update(){

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
                    Vector3 rightTop = _posMatrix[x, y] + new Vector2(1, 1) * _cellSize * 0.5f;
                    Vector3 rightBottom = _posMatrix[x, y] + new Vector2(1, -1) * _cellSize * 0.5f;
                    Vector3 leftBottom = _posMatrix[x, y] + new Vector2(-1, -1) * _cellSize * 0.5f;
                    Vector3 leftTop = _posMatrix[x, y] + new Vector2(-1, 1) * _cellSize * 0.5f;
                    
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
                    Gizmos.DrawSphere(_posMatrix[x, y], 0.1f);
                }
            }
        }
    }
}

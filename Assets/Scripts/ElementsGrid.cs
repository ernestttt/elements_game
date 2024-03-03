using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ElementsGame.Core
{
    
    public class ElementsGrid   
    {
        public event Action OnWin;
        public event Action OnUpdated;

        private Dictionary<int, Square> _squares = new Dictionary<int, Square>();
        private Dictionary<Vector2Int, Square> _squaresByPos = new Dictionary<Vector2Int, Square>();
        private List<int> _ids = new List<int>();

        private int[,] _typeMatrix;
        private int[,] _idsMatrix;

        private int _xSize, _ySize;

        public void Init(int[,] matrix)
        {
            _xSize = matrix.GetLength(1);
            _ySize = matrix.GetLength(0);

            _squares.Clear();
            _ids.Clear();

            // init all cubes
            for (int y = 0, id = 1; y < _ySize; y++)
            {
                for (int x = 0; x < _xSize; x++)
                {
                    if (matrix[y, x] == 0)
                    {
                        continue;
                    }

                    _squares.Add(id, 
                        new Square(id, matrix[y, x], new Vector2Int(x, y), new Vector2Int(_xSize, _ySize)));
                    id++;
                }
            }

            // init cubes' neighbours
            UpdateSquares();

            // set ids matrix dimension
            _idsMatrix = new int[_xSize, _ySize];
            // set type matrix demension
            _typeMatrix = new int[_xSize, _ySize];
        }

        private void UpdateNeighbours(){
            for (int i = 0; i < _ids.Count; i++)
            {
                Square cube = _squares[_ids[i]];
                cube.FindNeighbours(_squaresByPos);
            }
        }

        // should optimize this method
        private void UpdateSquares(){
            UpdateIds();
            UpdatePosDict();
            UpdateNeighbours();
        }

        private void UpdateIds()
        {
            _ids = _squares.Keys.ToList();
        }

        private void UpdatePosDict(){
            _squaresByPos.Clear();
            for(int i = 0; i < _ids.Count; i++){
                Square cube = _squares[_ids[i]];
                _squaresByPos.Add(cube.Pos, cube);
            }
        }

        public int GetIdForPos(Vector2Int pos)
        { 
            int resultId = -1;
            if(_squaresByPos.TryGetValue(pos, out Square square)){
                resultId = square.Id;
            }

            return resultId;
        }

        public bool TryToMoveOnPos(Vector2Int pos, MoveType move){
            int id = GetIdForPos(pos);
            return Move(id, move);
        }

        private bool Move(int cubeId, MoveType move)
        {   
            if(!_squares.TryGetValue(cubeId, out Square square)){
                return false;
            }
            bool isMoved = square.IsNormalized && square.Move(move);

            if(isMoved){
                UpdateSquares();
                // Normalize();
                OnUpdated?.Invoke();
            }

            return isMoved;
        }


        public bool Normalize()
        {
            IEnumerable<Square> normalizedSquares = _squares.Values.Where(a => !a.IsNormalized);
            bool isNormalize = normalizedSquares.Count() > 0;
            while(normalizedSquares.Count() > 0)
            {
                foreach (Square cube in normalizedSquares){
                    cube.Move(MoveType.Down);
                    UpdateSquares();
                }

                normalizedSquares = _squares.Values.Where(a => !a.IsNormalized);
            }

            if(isNormalize)
            {
                OnUpdated?.Invoke();
            }
            
            return isNormalize;
        }

        public int[,] GetIdsMatrix(){
            ResetMatrix(ref _idsMatrix);
            for(int i = 0; i < _ids.Count; i++){
                Square square = _squares[_ids[i]];
                Vector2Int coords = square.Pos;
                int id = square.Id;

                _idsMatrix[coords.y, coords.x] = id;
            }
            return _idsMatrix;
        }

        public int[,] GetTypeMatrix(){
            ResetMatrix(ref _typeMatrix);
            for (int i = 0; i < _ids.Count; i++)
            {
                Square square = _squares[_ids[i]];
                Vector2Int coords = square.Pos;
                int type = square.Type;

                _typeMatrix[coords.y, coords.x] = type;
            }
            return _typeMatrix;
        }

        private void ResetMatrix(ref int[,] matrix){
            for (int y = 0; y < _ySize; y++){
                for (int x = 0; x < _xSize; x++){
                    matrix[y,x] = 0;
                }
            }
        }
    }

    public enum MoveType
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }
}


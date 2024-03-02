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

        private Dictionary<int, Cube> _cubes = new Dictionary<int, Cube>();
        private Dictionary<Vector2Int, Cube> _cubesByPos = new Dictionary<Vector2Int, Cube>();
        private List<int> _ids = new List<int>();

        private int[,] _typeMatrix;
        private int[,] _idsMatrix;

        private int _xSize, _ySize;

        public void Init(int[,] matrix)
        {
            _xSize = matrix.GetLength(1);
            _ySize = matrix.GetLength(0);

            _cubes.Clear();
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

                    _cubes.Add(id, 
                        new Cube(id, matrix[y, x], new Vector2Int(x, y), new Vector2Int(_xSize, _ySize)));
                    _ids.Add(id);
                    id++;
                }
            }

            // init cubes' neighbours
            UpdateNeighbours();

            // set ids matrix dimension
            _idsMatrix = new int[_xSize, _ySize];
            // set type matrix demension
            _typeMatrix = new int[_xSize, _ySize];
        }

        // this method may be implemented through dictionary with pos keys
        private void UpdateNeighbours(){
            UpdatePosDict();
            for (int i = 0; i < _ids.Count; i++)
            {
                Cube cube = _cubes[_ids[i]];
                cube.FindNeighbours(_cubesByPos);
            }
        }

        private void UpdatePosDict(){
            _cubesByPos.Clear();
            for(int i = 0; i < _ids.Count; i++){
                Cube cube = _cubes[_ids[i]];
                _cubesByPos.Add(cube.Pos, cube);
            }
        }

        public int GetIdForPos(Vector2Int pos)
        { 
            int resultId = -1;
            if(_cubesByPos.TryGetValue(pos, out Cube cube)){
                resultId = cube.Id;
            }

            return resultId;
        }

        public bool TryToMoveOnPos(Vector2Int pos, MoveType move){
            int id = GetIdForPos(pos);
            return Move(id, move);
        }

        private bool Move(int cubeId, MoveType move)
        {   
            if(_cubes.TryGetValue(cubeId, out Cube cube)){
                return false;
            }

            bool isMoved = cube.Move(move);

            if(isMoved){
                Normalize();
                OnUpdated?.Invoke();
            }

            return isMoved;
        }

        private void Normalize()
        {
            UpdateNeighbours();
            IEnumerable<Cube> normalizedCubes = _cubes.Values.Where(a => !a.IsNormalized);

            if(normalizedCubes.Count() > 0)
            {
                foreach (Cube cube in normalizedCubes){
                    cube.Move(MoveType.Down);
                }
                Normalize();
            }
        }

        public int[,] GetIdsMatrix(){
            ResetMatrix(ref _idsMatrix);
            for(int i = 0; i < _ids.Count; i++){
                Cube cube = _cubes[_ids[i]];
                Vector2Int coords = cube.Pos;
                int id = cube.Id;

                _idsMatrix[coords.y, coords.x] = id;
            }
            return _idsMatrix;
        }

        public int[,] GetTypeMatrix(){
            ResetMatrix(ref _typeMatrix);
            for (int i = 0; i < _ids.Count; i++)
            {
                Cube cube = _cubes[_ids[i]];
                Vector2Int coords = cube.Pos;
                int type = cube.Type;

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


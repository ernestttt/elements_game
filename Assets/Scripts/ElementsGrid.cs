using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ElementsGame.Core{
    
    public class ElementsGrid   
    {
        public event Action OnWin;

        private List<Cube> _cubes = new List<Cube>();

        private int[,] _typeMatrix;
        private int[,] _idsMatrix;

        public void Init(int[,] matrix)
        {
            // init all cubes
            for (int y = 0, id = 1; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    if (matrix[y, x] == 0)
                    {
                        continue;
                    }

                    _cubes.Add(new Cube(id, matrix[y, x], new Vector2Int(x, y)));
                    
                    id++;
                }
            }

            // init cubes' neighbours
            for (int i = 0; i < _cubes.Count; i++){
                _cubes[i].FindNeighbours(_cubes);
            }

            // set ids matrix dimension
            _idsMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
            // set type matrix demension
            _typeMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
        }

        public bool Move(int cubeId, MoveType moveType)
        {
            return false;
        }

        private void Normalize()
        {

        }

        public int[,] GetIdsMatrix(){
            ResetMatrix(ref _idsMatrix);
            for(int i = 0; i < _cubes.Count; i++){
                Cube cube = _cubes[i];
                Vector2Int coords = cube.Pos;
                int id = cube.Id;

                _idsMatrix[coords.y, coords.x] = id;
            }
            return _idsMatrix;
        }

        public int[,] GetTypeMatrix(){
            ResetMatrix(ref _typeMatrix);
            for (int i = 0; i < _cubes.Count; i++)
            {
                Cube cube = _cubes[i];
                Vector2Int coords = cube.Pos;
                int type = cube.Type;

                _typeMatrix[coords.y, coords.x] = type;
            }
            return _typeMatrix;
        }

        private void ResetMatrix(ref int[,] matrix){
            for (int y = 0; y < matrix.GetLength(0); y++){
                for (int x = 0; x < matrix.GetLength(1); x++){
                    matrix[y,x] = 0;
                }
            }
        }

        private bool IsCanMove(int cubeId, MoveType move){
            throw new NotImplementedException();
        }
    }

    public class Cube
    {
        private int _id;
        private Vector2Int _pos;
        private int _type;

        // neighbour cubes
        private Cube _up;
        private Cube _right;
        private Cube _down;
        private Cube _left;

        public Vector2Int Pos => _pos;
        public int Id => _id;
        public int Type => _type;

        public Cube(int id, int type, Vector2Int pos)
        {
            _id = id;
            _type = type;
            _pos = pos;
        }

        public void FindNeighbours(IEnumerable<Cube> cubes){
            Vector2Int left = _pos + Vector2Int.left;
            Vector2Int right = _pos + Vector2Int.right;
            Vector2Int up = _pos + Vector2Int.up;
            Vector2Int down = _pos + Vector2Int.down;

            _left = cubes.FirstOrDefault(a => a._pos == left);
            _right = cubes.FirstOrDefault(a => a._pos == right);
            _up = cubes.FirstOrDefault(a => a._pos == up);
            _down = cubes.FirstOrDefault(a => a._pos == down);
        }
    }

    public enum MoveType{
        MoveUp = 0,
        MoveRight = 1,
        MoveDown = 2,
        MoveLeft = 3,
    }
}


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

        public void Init(int[,] matrix)
        {
            // init all cubes
            for (int y = 0, id = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++, id++)
                {
                    if (matrix[y, x] == 0)
                    {
                        continue;
                    }

                    _cubes.Add(new Cube(id, matrix[y, x], new Vector2Int(x, y)));
                }
            }

            // init cubes' neighbours
            for (int i = 0; i < _cubes.Count; i++){
                _cubes[i].FindNeighbours(_cubes);
            }
        }

        public bool Move(int cubeId, MoveType moveType)
        {
            return false;
        }

        private void Normalize()
        {

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

        public Cube(int id, int type, Vector2Int pos){
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


using System.Collections.Generic;
using UnityEngine;

namespace ElementsGame.Core
{
    public class Cube
    {
        private int _id;
        private Vector2Int _pos;
        private int _type;
        private Vector2Int _gridSize;

        // neighbour cubes
        private Cube _up;
        private Cube _right;
        private Cube _down;
        private Cube _left;

        public Vector2Int Pos => _pos;
        public int Id => _id;
        public int Type => _type;

        // if pos.y is equal to zero than it's the fist row
        public bool IsNormalized => _pos.y == 0 || (_down != null && _down.IsNormalized);

        public Cube(int id, int type, Vector2Int startPos, Vector2Int gridSize)
        {
            _id = id;
            _type = type;
            _pos = startPos;
            _gridSize = gridSize;
        }

        public void FindNeighbours(IDictionary<Vector2Int, Cube> cubes)
        {
            Vector2Int left = _pos + Vector2Int.left;
            Vector2Int right = _pos + Vector2Int.right;
            Vector2Int up = _pos + Vector2Int.up;
            Vector2Int down = _pos + Vector2Int.down;
            
            if(cubes.TryGetValue(left, out Cube cubeLeft))
            {
                _left = cubeLeft;
            }
            if (cubes.TryGetValue(right, out Cube cubeRight))
            {
                _right = cubeRight;
            }
            if (cubes.TryGetValue(up, out Cube cubeUp))
            {
                _up = cubeUp;
            }
            if (cubes.TryGetValue(down, out Cube cubeDown))
            {
                _down = cubeDown;
            }
        }

        private Vector2Int GetIncrementVector(MoveType move){
            Vector2Int result = Vector2Int.zero;

            switch (move)
            {
                case MoveType.Up:
                    result = Vector2Int.up;
                    break;
                case MoveType.Down:
                    result = Vector2Int.down;
                    break;
                case MoveType.Left:
                    result = Vector2Int.left;
                    break;
                case MoveType.Right:
                    result = Vector2Int.right;
                    break;
            }

            return result;
        }

        private bool IsCanMove(MoveType move){
            Vector2Int cubePos = _pos;
            Vector2Int moveVector = Vector2Int.zero;

            if(move == MoveType.Up && _up == null){
                return false;
            }

            moveVector = GetIncrementVector(move);

            Vector2Int nextCubePos = cubePos + moveVector;

            if (nextCubePos.x >= 0 && nextCubePos.x < _gridSize.x
                && nextCubePos.y >= 0 && nextCubePos.y < _gridSize.y)
            {
                return true;
            }

            return false;
        }

        private Cube FindNeighbour(MoveType move){
            Cube neighbour = null;

            switch (move)
            {
                case MoveType.Up:
                    neighbour = _up;
                    break;
                case MoveType.Down:
                    neighbour = _down;
                    break;
                case MoveType.Left:
                    neighbour = _left;
                    break;
                case MoveType.Right:
                    neighbour = _right;
                    break;
            }

            return neighbour;
        }

        // maybe this method can be deleted
        private void ExchangeWith(Cube neighbour, MoveType move){

            // neighbour neighbours
            Cube neighbourUp = neighbour._up;
            Cube neighbourDown = neighbour._down;
            Cube neighbourLeft = neighbour._left;
            Cube neighbourRight = neighbour._right;

            // our neighbours
            Cube ourUp = _up;
            Cube ourDown = _down;
            Cube ourLeft = _left;
            Cube ourRight = _right;

            // update neighbours, there may be the case when our neighbour may be the object himself
            neighbour._up = ourUp;
            neighbour._down = ourDown;
            neighbour._left = ourLeft;
            neighbour._right = ourRight;

            _up = neighbourUp;
            _down = neighbourDown;
            _left = neighbourLeft;
            _right = neighbourRight;

            // fix case when neighbour point on ourself
            switch (move)
            {
                case MoveType.Up:
                    neighbour._up = this;
                    _down = neighbour;
                    break;
                case MoveType.Down:
                    neighbour._down = this;
                    _up = neighbour;
                    break;
                case MoveType.Left:
                    neighbour._left = this;
                    _right = neighbour;
                    break;
                case MoveType.Right:
                    neighbour._right = this;
                    _left = neighbour;
                    break;
            }
        }

        public bool Move(MoveType move){
            if(!IsCanMove(move)) return false;

            Cube neighbour = FindNeighbour(move);

            if (neighbour != null)
            {
                //ExchangeWith(neighbour, move);
                neighbour._pos = _pos;
            }

            Vector2Int moveVector = GetIncrementVector(move);
            _pos += moveVector;

            return true;
        }
    }
}
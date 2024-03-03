using System.Collections.Generic;
using UnityEngine;

namespace ElementsGame.Core
{
    public class Square
    {
        private int _id;
        private Vector2Int _pos;
        private int _type;
        private Vector2Int _gridSize;

        // neighbour cubes
        private Square _up;
        private Square _right;
        private Square _down;
        private Square _left;

        public Vector2Int Pos => _pos;
        public int Id => _id;
        public int Type => _type;

        // if pos.y is equal to zero than it's the fist row
        public bool IsNormalized => _pos.y == 0 || (_down != null && _down.IsNormalized);

        public Square(int id, int type, Vector2Int startPos, Vector2Int gridSize)
        {
            _id = id;
            _type = type;
            _pos = startPos;
            _gridSize = gridSize;
        }

        public void FindNeighbours(IDictionary<Vector2Int, Square> squares)
        {
            ResetNeighbours();
            Vector2Int left = _pos + Vector2Int.left;
            Vector2Int right = _pos + Vector2Int.right;
            Vector2Int up = _pos + Vector2Int.up;
            Vector2Int down = _pos + Vector2Int.down;
            
            if(squares.TryGetValue(left, out Square cubeLeft))
            {
                _left = cubeLeft;
            }
            if (squares.TryGetValue(right, out Square cubeRight))
            {
                _right = cubeRight;
            }
            if (squares.TryGetValue(up, out Square cubeUp))
            {
                _up = cubeUp;
            }
            if (squares.TryGetValue(down, out Square cubeDown))
            {
                _down = cubeDown;
            }

            if(_down == this || _right == this || _left == this || _up == this){
                throw new System.Exception("Our neighbour point is this");
            }
        }

        private void ResetNeighbours(){
            _up = null;
            _right = null;
            _down = null;
            _left = null;
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

            if(move == MoveType.Up && _up == null){
                return false;
            }

            Vector2Int moveVector = GetIncrementVector(move);

            Vector2Int nextCubePos = cubePos + moveVector;

            if (nextCubePos.x >= 0 && nextCubePos.x < _gridSize.x
                && nextCubePos.y >= 0 && nextCubePos.y < _gridSize.y)
            {
                return true;
            }

            return false;
        }

        private Square FindNeighbour(MoveType move){
            Square neighbour = null;

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
        private void ExchangeWith(Square neighbour, MoveType move){

            // neighbour neighbours
            Square neighbourUp = neighbour._up;
            Square neighbourDown = neighbour._down;
            Square neighbourLeft = neighbour._left;
            Square neighbourRight = neighbour._right;

            // our neighbours
            Square ourUp = _up;
            Square ourDown = _down;
            Square ourLeft = _left;
            Square ourRight = _right;

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

            Square neighbour = FindNeighbour(move);

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
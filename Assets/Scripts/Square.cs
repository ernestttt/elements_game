using System.Collections.Generic;
using System.Linq;
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
        public bool IsNormalized => _pos.y == 0 || (_down != null && _down != this && _down.IsNormalized);

        // hide this property, make it variable
        public bool IsVisited { get; private set; }

        private bool _isDestroyed = false;

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

            Square neigbour = FindNeighbour(move);
            if(neigbour != null && neigbour._isDestroyed){
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

        public void ResetMatch(){
            IsVisited = false;
        }

        public List<Square> GetSquaresGroup()
        {
            List<Square> squares = new List<Square>();
            FillGroupRecursive(_type, ref squares);
            return squares;
        }

        public bool IsMatch3(List<Square> squares){

            List<Vector2Int> positions = squares.Select(a => a._pos).ToList();
            return IsMatchY(positions) || IsMatchX(positions);
        }

        private static bool IsMatchX(List<Vector2Int> positions)
        {
            bool isMatchX = false;
            int minX = positions.Select(a => a.x).Min();
            int maxX = positions.Select(a => a.x).Max();

            for (int x = minX; x <= maxX; x++)
            {
                int[] yCoords = positions.Where(a => a.x == x).Select(a => a.y).OrderBy(a => a).ToArray();
                if (yCoords.Length < 3)
                {
                    continue;
                }

                for (int i = 1, count = 0; i < yCoords.Length; i++)
                {
                    if (yCoords[i] == yCoords[i - 1] + 1)
                    {
                        count++;
                    }
                    else{
                        count = 0;
                    }


                    if (count == 2)
                    {
                        isMatchX = true;
                        break;
                    }
                }

                if (isMatchX)
                {
                    break;
                }

            }

            return isMatchX;
        }

        private static bool IsMatchY(List<Vector2Int> positions)
        {
            bool isMatchY = false;
            int minY = positions.Select(a => a.y).Min();
            int maxY = positions.Select(a => a.y).Max();

            for (int y = minY; y <= maxY; y++)
            {
                int[] xCoords = positions.Where(a => a.y == y).Select(a => a.x).OrderBy(a => a).ToArray();
                if (xCoords.Count() < 3)
                {
                    continue;
                }

                for (int i = 1, count = 0; i < xCoords.Length; i++)
                {
                    if (xCoords[i] == xCoords[i - 1] + 1)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if (count == 2)
                    {
                        isMatchY = true;
                        break;
                    }
                }

                if (isMatchY)
                {
                    break;
                }

            }

            return isMatchY;
        }

        public void MarkAsDestroyed()
        {
            _isDestroyed = true;
        }

        private void FillGroupRecursive(int type , ref List<Square> squares)
        {
            if(type != _type || IsVisited)   return;

            IsVisited = true;

            squares.Add(this);

            _down?.FillGroupRecursive(type, ref squares);
            _up?.FillGroupRecursive(type, ref squares);
            _left?.FillGroupRecursive(type, ref squares);
            _right?.FillGroupRecursive(type, ref squares);
        }

    }
}
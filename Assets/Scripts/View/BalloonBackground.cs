using System.Collections;
using System.Collections.Generic;
using ElementsGame.Core;
using UnityEngine;
using ElementsGame.View;
using System.Linq;

namespace ElementsGame.View
{
    public class BalloonBackground : MonoBehaviour
    {
        [SerializeField, Range(0, 0.5f)] private float _lowerScreenPoint = .3f;
        [SerializeField] private bool _showLines = false;
        [SerializeField] private Ballon _ballonPrefab;
        [SerializeField] private float _ballonTimePeriod = 3f;
        [SerializeField, Range(0, 3)] private int _numberOfBallons;

        private List<Ballon> _ballons = new List<Ballon>();

        private Camera _camera;

        private Vector2[] _cornerPoints;

        private float _nextBallonTime;

        private float _minDistance;

        private void Awake(){
            _camera = Camera.main;
        }

        private void Start()
        {
            Vector2 leftBottomPoint = _camera.ScreenToWorldPoint(new Vector2(0, Screen.height * _lowerScreenPoint));
            Vector2 rightTopPoint = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            float width = rightTopPoint.x - leftBottomPoint.x;
            float margin = width * 0.3f;

            Vector2 leftBottom = new Vector2(leftBottomPoint.x - margin, leftBottomPoint.y);
            Vector2 leftTop = new Vector2(leftBottomPoint.x - margin, rightTopPoint.y + margin);
            Vector2 rightTop = new Vector2(rightTopPoint.x + margin, rightTopPoint.y + margin);
            Vector2 rightBottom = new Vector2(rightTopPoint.x + margin, leftBottomPoint.y);

            _cornerPoints = new Vector2[] {leftBottom, leftTop, rightTop, rightBottom};


            _minDistance = width * 2;
            _nextBallonTime = Time.time + _ballonTimePeriod;
        }

        private void Update(){
            if(_nextBallonTime < Time.time){
                
                    Ballon ballon = null;
                if (_ballons.Count < _numberOfBallons)
                {
                    ballon = Instantiate(_ballonPrefab, Vector2.one * 1000, Quaternion.identity, transform);
                    _ballons.Add(ballon);
                }
                else if(_ballons.Any(a => !a.IsMoving)){
                    ballon = _ballons.First(a => !a.IsMoving);
                }

                if(ballon != null){
                    int firstNumber = Random.Range(0, 3);
                    int secondNumber = firstNumber;
                    while (firstNumber == secondNumber)
                    {
                        secondNumber = Random.Range(0, 3);
                    }
                    
                    Vector2 point1 = Vector2.zero;
                    Vector2 point2 = Vector2.zero;
                    
                    while((point1 - point2).magnitude < _minDistance)
                    {
                         point1 = GetRandomPointOnSegment(_cornerPoints[firstNumber], _cornerPoints[firstNumber + 1]);
                         point2 = GetRandomPointOnSegment(_cornerPoints[secondNumber], _cornerPoints[secondNumber + 1]);
                    }

                    ballon.SetBallonRoute(point1, point2);
                    _nextBallonTime = Time.time + _ballonTimePeriod * Random.Range(.8f, 1.5f);
                }
            }
        }

        private Vector2 GetRandomPointOnSegment(Vector2 pointA, Vector2 pointB){
            float t = Random.value;
            return pointA + t * (pointB - pointA);
        }

        private void OnDrawGizmos(){
            if(!_showLines || _cornerPoints == null) return;

            for (int i = 1; i < _cornerPoints.Length; i++){
                Debug.DrawLine(_cornerPoints[i - 1], _cornerPoints[i]);
            }
        }
    }
}


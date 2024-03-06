using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace ElementsGame.View{
    public class Ballon : MonoBehaviour
    {
        [SerializeField] private float _lowestSpeed = 2f;
        [SerializeField] private float _highestSpeed = 4f;
        [SerializeField] private float _lowScale = .4f;
        [SerializeField] private float _highScale = .8f;
        
        private Vector2 _start;
        private Vector2 _end;
        private float _currentSpeed;

        public bool IsMoving => (Vector2)transform.position != _end;

        private float _angle;
        private Vector2 _hillVector;
        private float _distance;

        public void SetPoints(Vector2 start, Vector2 end){
            _start = start;
            _end = end;
            _currentSpeed = Random.Range(_lowestSpeed, _highestSpeed);
            transform.position = _start;
            transform.localScale = Vector3.one * Random.Range(_lowScale, _highScale);
            _distance = (end - start).magnitude;
        }

        private void Update(){
            float currentDistance = ((Vector2)transform.position - _end).magnitude;
            float t = _currentSpeed * Time.deltaTime / currentDistance;
            Vector3 position = Vector2.Lerp(transform.position, _end, t);

            float progress = currentDistance / _distance;
            transform.position = position;
        }
    }
}


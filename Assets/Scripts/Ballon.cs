using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace ElementsGame.View{

    [RequireComponent(typeof(SpriteRenderer))]
    public class Ballon : MonoBehaviour
    {
        [SerializeField] private float _lowestSpeed = 2f;
        [SerializeField] private float _highestSpeed = 4f;
        [SerializeField] private float _lowScale = .4f;
        [SerializeField] private float _highScale = .8f;
        [SerializeField] private Vector2 _heightRange = new Vector2(-2, 2);
        [SerializeField] private Vector2 _numberOfPeriodsRange = new Vector2(0, 2);
        [SerializeField] private Sprite[] _ballonSprites;
        
        private Vector2 _start;
        private Vector2 _end;
        private Vector2 _currentPosOnLine;
        private float _currentSpeed;

        public bool IsMoving => (Vector2)transform.position != _end;

        private float _heightOfHill;
        private Vector2 _hillVector;
        private float _distance;
        private float _numberOfPeriods;

        private SpriteRenderer _sRenderer;

        private void Awake(){
            _sRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetPoints(Vector2 start, Vector2 end){
            _start = start;
            _end = end;
            _currentSpeed = Random.Range(_lowestSpeed, _highestSpeed);
            _currentPosOnLine = _start;
            transform.localScale = Vector3.one * Random.Range(_lowScale, _highScale);
            _distance = (end - start).magnitude;
            Vector2 direction = (end - start).normalized;
            _hillVector = new Vector2(-direction.y, direction.x);
            _heightOfHill = Random.Range(_heightRange.x, _heightRange.y);
            _numberOfPeriods = Random.Range(_numberOfPeriodsRange.x, _numberOfPeriodsRange.y);
            _sRenderer.sprite = _ballonSprites[Random.Range(0, _ballonSprites.Length)];
        }

        private void Update(){
            float currentDistance = (_currentPosOnLine - _end).magnitude;
            float t = _currentSpeed * Time.deltaTime / currentDistance;
            _currentPosOnLine = Vector2.Lerp(_currentPosOnLine, _end, t);

            float progress = currentDistance / _distance;
            float sin = Mathf.Sin(progress * Mathf.PI * _numberOfPeriods); 
            transform.position = _currentPosOnLine + _hillVector * sin * _heightOfHill;

            Debug.DrawLine(_start, _end);
            Debug.DrawLine(_currentPosOnLine, _currentPosOnLine + _hillVector * sin * _heightOfHill);
        }
    }
}


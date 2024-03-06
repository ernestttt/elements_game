using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElementsGame.UI{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _nextButton;

        public event Action OnRestart;
        public event Action OnNext;

        private void Start(){
            _restartButton.onClick.AddListener(() => OnRestart?.Invoke());
            _nextButton.onClick.AddListener(() => OnNext?.Invoke());
        }
    }
}


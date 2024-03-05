using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ElementsGame.Data{
    [CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private string _levelPath;
        [SerializeField] private int[] _levelIds;

        public string LevelPath => Application.dataPath + _levelPath;
        public int[] Ids => _levelIds;
    }
}


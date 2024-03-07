using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;  

namespace ElementsGame.Data{
    [CreateAssetMenu(fileName = "Game Config", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private string _levelPath;
        [SerializeField] private int[] _levelIds;
        [SerializeField] private BlockType[] _blockTypes;
        [SerializeField] private Sprite[] _ballonSprites;

        public string LevelPath => Application.dataPath + _levelPath;
        public int[] Ids => _levelIds;

        public SpriteRenderer GetBlockForType(int type){
            return _blockTypes.First(a => a.id == type).prefab;
        }

        public Sprite GetRandomBallonSprite(){
            return _ballonSprites[Random.Range(0, _ballonSprites.Length)];
        }
    }

    [Serializable]
    public struct BlockType{
        public int id;
        public SpriteRenderer prefab;
    }
}


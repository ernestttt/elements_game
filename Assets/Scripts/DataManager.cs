using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ElementsGame.Data;
using UnityEngine;

namespace ElementsGame.Data{

    public class DataManager
    {
        private Dictionary<int, LevelContainer> _levelContainters = new Dictionary<int, LevelContainer>();
        private GameConfig _gameConfig;
        private int[] _levelIds;

        private BinaryFormatter _bf = new BinaryFormatter();

        public DataManager(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            _levelIds = gameConfig.Ids;
            InitContainers();
        }

        private void InitContainers()
        {
            for(int i = 0; i < _levelIds.Length; i++){
                using (FileStream fs = File.Open(_gameConfig.LevelPath + $"/level_{_levelIds[i]}" , FileMode.Open)){
                    LevelContainer lc = (LevelContainer)_bf.Deserialize(fs);
                    _levelContainters.Add(lc.Id, lc);
                }
            }
        }
    }
}


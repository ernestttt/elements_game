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

        private int levelId = 0;

        private string _savedLevelKey = "saved";
        private string _levelIdStringKey = "levelId";

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

        public void SaveGame(int[,] matrix)
        {
            if(matrix == null)  return;
            PlayerPrefs.SetInt(_savedLevelKey, 1);
            PlayerPrefs.SetInt(_levelIdStringKey, levelId);
            using (FileStream file = File.Create($"{_gameConfig.LevelPath}/level_saved"))
            {
                _bf.Serialize(file, matrix);
            }
        }

        private bool TryToGetSavedLevel(out int[,] matrix){
            matrix = default;
            if (PlayerPrefs.GetInt(_savedLevelKey) == 0) return false;
            levelId = PlayerPrefs.GetInt(_levelIdStringKey);
            PlayerPrefs.SetInt(_savedLevelKey, 0);
            try
            {
                using (FileStream fs = File.Open($"{_gameConfig.LevelPath}/level_saved", FileMode.Open))
                {
                    matrix = (int[,])_bf.Deserialize(fs);
                }
            }
            catch{
                return false;
            }
            return true;
        }
            
        

        public int[,] GetLevel(){
            if(TryToGetSavedLevel(out int[,] matrix)){
                return matrix;
            }

            int id = _levelIds[levelId];
            levelId++;
            levelId %= _levelIds.Length;
            return _levelContainters[id].Matrix;
        } 
    }
}


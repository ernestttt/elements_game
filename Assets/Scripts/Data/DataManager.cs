using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

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

        private string SavelLevelPath => $"{Application.persistentDataPath}/level_saved";

        private string _levelPath;

        public async UniTask Init(GameConfig gameConfig)
        {
#if UNITY_EDITOR
            _levelPath = Application.dataPath + "/StreamingAssets";
#else
                _levelPath = Application.streamingAssetsPath;
#endif
            _gameConfig = gameConfig;
            _levelIds = gameConfig.Ids;
            await InitContainers();
        }

        private async UniTask InitContainers()
        {
            for(int i = 0; i < _levelIds.Length; i++){
                
                string path = _levelPath + $"/level_{_levelIds[i]}";
                LevelContainer lc = await LoadFile<LevelContainer>(path);
                if (lc !=null)
                {
                    _levelContainters.Add(lc.Id, lc);
                }
            }
        }

        private async UniTask<T> LoadFile<T>(string path){
            
            T result = default;
#if UNITY_ANDROID && !UNITY_EDITOR
            result = await LoadFileAndroid<T>(path);
#else
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                result = (T)_bf.Deserialize(fs);
            }
#endif
            return result;
        }

        private async UniTask<T> LoadFileAndroid<T>(string path){
            UnityWebRequest www = UnityWebRequest.Get(path);
            T result = default;
            await www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                using (MemoryStream ms = new MemoryStream(www.downloadHandler.data)){
                    result = (T)_bf.Deserialize(ms);
                }
            }

            return result;
        }

        public void SaveGame(int[,] matrix)
        {
            if(matrix == null)  return;
            PlayerPrefs.SetInt(_savedLevelKey, 1);
            PlayerPrefs.SetInt(_levelIdStringKey, levelId);
            using (FileStream file = File.Create(SavelLevelPath))
            {
                _bf.Serialize(file, matrix);
            }
        }

        private async UniTask<int[,]> TryToGetSavedLevel(){       
            if (PlayerPrefs.GetInt(_savedLevelKey) == 0) return null;

            levelId = PlayerPrefs.GetInt(_levelIdStringKey);

            PlayerPrefs.SetInt(_savedLevelKey, 0);
            int[,] matrix;
            using (FileStream fs = File.Open(SavelLevelPath, FileMode.Open))
            {
                matrix = (int[,])_bf.Deserialize(fs);
            }
            return matrix;
        }

        public async UniTask<int[,]> GetSameLevel(){
            int[,] matrix = await TryToGetSavedLevel();
            if(matrix != null){
                return matrix;
            }

            int id = _levelIds[levelId];

            return _levelContainters[id].Matrix;
        }

        public async UniTask<int[,]> GetNextLevel(){
            int[,] matrix = await TryToGetSavedLevel();
            if (matrix != null)
            {
                return matrix;
            }

            levelId++;
            levelId %= _levelIds.Length;
            int id = _levelIds[levelId];
            return _levelContainters[id].Matrix;
        } 
    }
}


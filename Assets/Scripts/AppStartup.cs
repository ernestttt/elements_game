using UnityEngine;
using ElementsGame.Core;
using ElementsGame.View;
using ElementsGame._Input;
using ElementsGame.Data;
using ElementsGame.UI;
using System.Collections;

namespace ElementsGame
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField] private ElementsView _elementsView;
        [SerializeField] private ElementsInput _input;
        [SerializeField] private GameConfig _config;
        [SerializeField] private UIManager _ui;

        private DataManager dataManager;
        private ElementsGrid grid;

        private void Awake(){
            Debug.developerConsoleVisible = true;
            Debug.developerConsoleEnabled = true;
            Application.targetFrameRate = 60;
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            Init();
        }

        private async void Init(){
            grid = new ElementsGrid();
            dataManager = new DataManager();
            await dataManager.Init(_config);

            _ui.OnNext += NextLevel;
            _ui.OnRestart += Restart;

            _elementsView.Init(grid);
            _input.Init(grid);

            grid.Init(await dataManager.GetSameLevel());
        }

        private async void Restart(){
            int[,] level = await dataManager.GetSameLevel();
            grid.Init(level);
        }

        private async void NextLevel(){
            int[,] level = await dataManager.GetNextLevel();
            grid.Init(level);
        }

        private void OnApplicationPause(bool pauseStatus) 
        {
            if(pauseStatus && dataManager != null){
                dataManager.SaveGame(grid.GetTypeMatrix());
            }
            
        }
    }
}

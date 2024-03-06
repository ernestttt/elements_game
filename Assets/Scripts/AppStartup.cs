using UnityEngine;
using ElementsGame.Core;
using ElementsGame.View;
using ElementsGame._Input;
using ElementsGame.Data;
using ElementsGame.UI;

namespace ElementsGame
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField] private ElementsView _elementsView;
        [SerializeField] private ElementsInput _input;
        [SerializeField] private GameConfig _config;
        [SerializeField] private UIManager _ui;

        DataManager dataManager;
        ElementsGrid grid;

        private void Start()
        {
            grid = new ElementsGrid();
            dataManager = new DataManager(_config);
            
            _ui.OnNext += NextLevel;
            _ui.OnRestart += Restart;

            _elementsView.Init(grid);
            _input.Init(grid);

            grid.OnWin += NextLevel;
            grid.OnLoose += NextLevel;

            grid.Init(dataManager.GetSameLevel());
        }

        private void Restart(){
            int[,] level = dataManager.GetSameLevel();
            grid.Init(level);
        }

        private void NextLevel(){
            int[,] level = dataManager.GetNextLevel();
            grid.Init(level);
        }

        private void OnApplicationPause(bool pauseStatus) 
        {
            if(pauseStatus){
                dataManager.SaveGame(grid.GetTypeMatrix());
            }
            
        }
    }
}

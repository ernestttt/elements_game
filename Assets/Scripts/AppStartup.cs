using UnityEngine;
using ElementsGame.Core;
using ElementsGame.View;
using ElementsGame._Input;
using ElementsGame.Data;

namespace ElementsGame
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField] private ElementsView _elementsView;
        [SerializeField] private ElementsInput _input;
        [SerializeField] private GameConfig _config;

        DataManager dataManager;
        ElementsGrid grid;

        private void Start()
        {
            grid = new ElementsGrid();
            dataManager = new DataManager(_config);

            _elementsView.Init(grid);
            _input.Init(grid);

            grid.OnWin += OnGameEnded;
            grid.OnLoose += OnGameEnded;

            grid.Init(dataManager.GetLevel());
        }

        private void OnGameEnded(){
            int[,] level = dataManager.GetLevel();
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

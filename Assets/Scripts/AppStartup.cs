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

        private int[,] _initMatrix = new int[,]{
        {0,1,2,1,1,0},
        {0,1,2,1,1,0},
        {0,2,1,2,2,0},
        {0,1,1,0,1,0},
        {0,1,2,0,0,0},
        {0,0,1,0,0,0},
    };

        private void Start()
        {
            ElementsGrid grid = new ElementsGrid();
            grid.Init(_initMatrix);
            _elementsView.Init(grid);
            _input.Init(grid);
            DataManager dataManager = new DataManager(_config);
        }
    }
}

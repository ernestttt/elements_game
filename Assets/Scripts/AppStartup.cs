using UnityEngine;
using ElementsGame.Core;
using ElementsGame.View;

namespace ElementsGame
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField] private ElementsView _elementsView;
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
        }
    }
}

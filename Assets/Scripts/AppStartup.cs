using UnityEngine;
using ElementsGame.Core;

public class AppStartup : MonoBehaviour {

    private int[,] _initMatrix = new int[,]{
        {0,1,2,1,1,0},
        {0,1,2,1,1,0},
        {0,2,1,2,2,0},
        {0,1,1,0,1,0},
        {0,1,2,0,0,0},
        {0,0,1,0,0,0},
    };

    private void Start() {
        ElementsGrid grid =new ElementsGrid();
        grid.Init(_initMatrix);
    }
}
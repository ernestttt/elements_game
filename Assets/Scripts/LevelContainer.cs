using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElementsGame.Data{

    [Serializable]
    public class LevelContainer
    {
        private int _id;
        private int[,] _matrix;

        [field : NonSerialized] public int Id => _id;
        [field : NonSerialized] public int[,] Matrix => _matrix;

        public LevelContainer(int id, int[,] matrix){
            _id = id;
            _matrix = matrix;
        }
    }
}


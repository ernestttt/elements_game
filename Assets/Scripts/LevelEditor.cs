using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using ElementsGame.Data;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LevelEditor : EditorWindow {
    
    private TextAsset _levelDescription;    
    private string _levelPath;
    private BinaryFormatter _formatter = new BinaryFormatter();

    [MenuItem("Custom/Open Editor")]
    private static void ShowWindow() {
        var window = GetWindow<LevelEditor>();
        window.titleContent = new GUIContent("Level Parser");
        window.Show();
    }

    private void OnGUI() {
        GUILayout.Label("Text description");
        _levelDescription = (TextAsset) EditorGUILayout.ObjectField(_levelDescription, typeof(TextAsset), true);

        if (GUILayout.Button("Create levels")){
            LevelContainer[] levelContainers = ParseLevels();
            SaveLevels(levelContainers);
        }

        _levelPath = Application.dataPath + GUILayout.TextField("/Levels");
    }

    private void SaveLevels(LevelContainer[] levels){
        foreach(var level in levels){
            using (FileStream file = File.Create($"{_levelPath}/level_{level.Id}")){
                _formatter.Serialize(file, level);
            }
        }
    }


    private LevelContainer[] ParseLevels()
    {
        List<LevelContainer> result = new List<LevelContainer>();
        string text = _levelDescription.text;
        
        List<string> levelsString = new List<string>();

        for (int startIndex = text.IndexOf('#'), endIndex = text.IndexOf('#', startIndex + 1); 
                startIndex != -1;
                startIndex = text.IndexOf('#', endIndex - 1),
                endIndex = text.IndexOf('#', startIndex + 1)
            )
        {
            if (endIndex == -1)
            {
                endIndex = text.Length;
            }
            string substring = text.Substring(startIndex, endIndex - startIndex);
            levelsString.Add(substring);
        }
        
        foreach(string levelString in levelsString){
            string[] lines = levelString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<int[]> levelMatrix = new List<int[]>();
            int id = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line[0] == '#')
                {
                    id = int.Parse(line.Substring(1));
                    continue;
                }

                string[] stringNumbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int[] numbers = new int[stringNumbers.Length];
                for (int j = 0; j < stringNumbers.Length; j++)
                {
                    numbers[j] = int.Parse(stringNumbers[j].Trim());
                }
                levelMatrix.Add(numbers);
            }

            levelMatrix.Reverse();
            result.Add(CreateLevel(id, levelMatrix.ToArray()));
        }
        
        return result.ToArray();
    }

    private LevelContainer CreateLevel(int id, int[][] numbers){
        int[,] martix = ConvertToMultidimensionalArray(numbers);
        return new LevelContainer(id, martix);
    }

    static int[,] ConvertToMultidimensionalArray(int[][] jaggedArray)
    {
        int rows = jaggedArray.Length;
        int cols = jaggedArray[0].Length;

        int[,] result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = jaggedArray[i][j];
            }
        }

        return result;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    public WFCAnalyzer textureAnalyzer;
    public int outputWidth = 20;
    public int outputHeight = 20;
    public float tileSize = 1.0f; // Size of each tile for positioning the prefabs

    private string[,] outputGrid;
    private Pattern[,] patterns;

    void Start()
    {
        if (textureAnalyzer.patterns.Length == 0)
        {
            Debug.LogError("No patterns found. Please analyze the texture first.");
            return;
        }

        patterns = textureAnalyzer.patterns;
        GenerateOutput();
    }

    void GenerateOutput()
    {
        outputGrid = new string[outputWidth, outputHeight];

        // Initialize output grid with all possible patterns
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                outputGrid[x, y] = "";
            }
        }

        // Simple WFC algorithm (can be optimized)
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                CollapseCell(x, y);
            }
        }

        // Instantiate the output pattern
        InstantiateOutputPattern();
    }

    void CollapseCell(int x, int y)
    {
        List<string> possiblePatterns = new List<string>(patterns.Keys);

        if (x > 0)
        {
            // Filter based on left neighbor
            string leftPattern = outputGrid[x - 1, y];
            if (leftPattern != "")
            {
                possiblePatterns = patterns[x,y].adjacencies[Direction.Right];
            }
        }

        if (y > 0)
        {
            // Filter based on top neighbor
            string topPattern = outputGrid[x, y - 1];
            if (topPattern != "")
            {
                possiblePatterns = patterns[x, y].adjacencies[Direction.Down];
            }
        }

        // Pick a random pattern from the possible patterns
        string selectedPattern = possiblePatterns[Random.Range(0, possiblePatterns.Count)];
        outputGrid[x, y] = selectedPattern;
    }

    void InstantiateOutputPattern()
    {
        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                Pattern pattern = patterns[x,y];
                for (int i = 0; i < pattern.objects.GetLength(0); i++)
                {
                    for (int j = 0; j < pattern.objects.GetLength(1); j++)
                    {
                        if (pattern.objects[i, j] != null)
                        {
                            Vector3 position = new Vector3(x * tileSize + i * tileSize / pattern.objects.GetLength(0),
                                                           0,
                                                           y * tileSize + j * tileSize / pattern.objects.GetLength(1));
                            Instantiate(pattern.objects[i, j], position, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}

public enum Direction { Right, Down }

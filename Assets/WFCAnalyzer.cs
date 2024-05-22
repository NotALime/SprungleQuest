using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCAnalyzer : MonoBehaviour
{
    public Texture2D sourceTexture;
    public InteriorTheme theme;
    public int patternSize = 3; // Size of the pattern (e.g., 3x3)

    [SerializeField]
    public Pattern[,] patterns;

    // Public method to trigger texture analysis
    public void AnalyzeTexture()
    {
        int width = sourceTexture.width;
        int height = sourceTexture.height;

        SetUpTheme();

        patterns = new Pattern[width, height];
        for (int y = 0; y < height - patternSize + 1; y++)
        {
            for (int x = 0; x < width - patternSize + 1; x++)
            {
                Color[] patternPixels = sourceTexture.GetPixels(x, y, patternSize, patternSize);
                GameObject[,] patternObjects = GetPatternObjects(patternPixels);

                patterns[x,y] = new Pattern(patternPixels, patternObjects);

                // Check adjacent patterns
                AddAdjacentPatterns(x, y);
            }
        }

        Debug.Log(patterns.Length.ToString());
        Debug.Log("Texture analysis complete.");
    }

    void AddAdjacentPatterns(int x, int y)
    {
        // Check right adjacency
        if (x < sourceTexture.width - patternSize)
        {
            string rightPatternKey = Pattern.GetKey(sourceTexture.GetPixels(x + 1, y, patternSize, patternSize));
            patterns[x,y].AddAdjacency(rightPatternKey, Direction.Right);
        }

        // Check down adjacency
        if (y < sourceTexture.height - patternSize)
        {
            string downPatternKey = Pattern.GetKey(sourceTexture.GetPixels(x, y + 1, patternSize, patternSize));
            patterns[x,y].AddAdjacency(downPatternKey, Direction.Down);
        }
    }

    GameObject[,] GetPatternObjects(Color[] patternPixels)
    {
        GameObject[,] patternObjects = new GameObject[patternSize, patternSize];
        for (int i = 0; i < patternSize; i++)
        {
            for (int j = 0; j < patternSize; j++)
            {
                patternObjects[i, j] = theme.GetPrefab(patternPixels[i * patternSize + j]);
            }
        }
        return patternObjects;
    }

    void SetUpTheme()
    {
        HashSet<Color> uniqueColors = new HashSet<Color>(sourceTexture.GetPixels());
        theme.mappings = new List<ColorPrefab>();
        foreach (Color color in uniqueColors)
        {
            ColorPrefab prefab = new ColorPrefab();
            prefab.color = color;
            theme.mappings.Add(prefab);
        }
    }
}

public class Pattern
{
    public Color[] pixels;
    public GameObject[,] objects;
    public Dictionary<Direction, List<string>> adjacencies = new Dictionary<Direction, List<string>>();

    public Pattern(Color[] pixels, GameObject[,] objects)
    {
        this.pixels = pixels;
        this.objects = objects;
        adjacencies[Direction.Right] = new List<string>();
        adjacencies[Direction.Down] = new List<string>();
    }

    public void AddAdjacency(string patternKey, Direction direction)
    {
        adjacencies[direction].Add(patternKey);
    }

    public static string GetKey(Color[] pixels)
    {
        string key = "";
        foreach (Color pixel in pixels)
        {
            key += pixel.ToString();
        }
        return key;
    }
}

[System.Serializable]
public class InteriorTheme
{
    public List<ColorPrefab> mappings;

    public GameObject GetPrefab(Color color)
    {
        foreach (ColorPrefab mapping in mappings)
        {
            if (mapping.color == color)
            {
                return mapping.prefab;
            }
        }
        return null;
    }
}

[System.Serializable]
public class ColorPrefab
{
        public Color color;
        public GameObject prefab;
}
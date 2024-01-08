using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private int worldWidth;
    [SerializeField] private int worldHeight;
    [SerializeField] private float noiseScale;

    [SerializeField] private int octaves;
    [SerializeField][Range(0, 1)] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private int seed;
    public bool autoUpdate;

    public void GenerateWorld()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(worldWidth, worldHeight, seed, noiseScale, octaves, persistance, lacunarity);

        WorldDisplay wd = FindObjectOfType<WorldDisplay>();
        wd.GenerateDisplayTexture(noiseMap);
    }

    private void OnValidate()
    {
        if (worldWidth < 1)
        {
            worldWidth = 1;
        }
        if (worldHeight < 1)
        {
            worldHeight = 1;
        }
        if (octaves < 0)
        {
            octaves = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
    }
}

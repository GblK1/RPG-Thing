using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int worldWidth, int worldHeight, int seed, float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[worldWidth, worldHeight];

        System.Random wrng = new System.Random(seed);
        Vector2[] octaveoffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = wrng.Next(-1000, 1000);
            float offsetY = wrng.Next(-1000, 1000);
            octaveoffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        if (worldWidth <= 0)
        {
            worldWidth = 1;
        }
        if (worldHeight <= 0)
        {
            worldHeight = 1;
        }

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - worldWidth / 2) / scale * frequency + octaveoffsets[i].x;
                    float sampleY = (y - worldHeight / 2) / scale * frequency + octaveoffsets[i].y;

                    float heightInThatPoint = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += heightInThatPoint * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        return noiseMap;
    }
}
    
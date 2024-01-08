using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer;

    public void GenerateDisplayTexture(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D worldTexture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        worldTexture.SetPixels(colorMap);
        worldTexture.Apply();

        textureRenderer.sharedMaterial.mainTexture = worldTexture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}

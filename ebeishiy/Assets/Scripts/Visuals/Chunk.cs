using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int chunkSizeX, chunkSizeY, shunkSizeZ;
    public int[] _chunk;
    [SerializeField] private GameObject sampleVoxel;

    private void Start()
    {
        _chunk = new int[chunkSizeX + chunkSizeY + shunkSizeZ];

        CreateThisChunk();
    }

    private void CreateThisChunk()
    {
        for (int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < shunkSizeZ; z++)
                {
                    GameObject newVoxel = new GameObject();
                    newVoxel.transform.parent = transform;


                }
            }
        }
    }
}

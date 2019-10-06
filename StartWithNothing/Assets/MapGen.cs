using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public int terrainSize = 256;
    public float perlinSteps = 1;
    public float perlinAmplifier = 1;

    int timer = 5;
    float currentTimer;

    Terrain terra;
    TerrainData terraData;

    void Start()
    {
        terra = GetComponent<Terrain>();
        terraData = terra.terrainData;

        terraData.size = new Vector3(terrainSize, 10, terrainSize);

        InitTerrain();
        PerlinMap();
        LowerSea();

        currentTimer = timer;
    }

    void PerlinMap()
    {
        float perlinx = Random.Range(0f, 1f);
        float perliny = Random.Range(0f, 1f);

        float[,] terraheight = terraData.GetHeights(0, 0, terraData.heightmapResolution, terraData.heightmapResolution);

        for (int x = 0; x < terraData.heightmapResolution; x++, perlinx += perlinSteps)
        {
            perliny = 0;
            for (int z = 0; z < terraData.heightmapResolution; z++, perliny += perlinSteps)
            {
               
                terraheight[x,z] = terraheight[x,z] + Mathf.PerlinNoise(perlinx, perliny) * perlinAmplifier;
            }
        }

        terraData.SetHeights(0, 0, terraheight);
    }

    void LowerSea()
    {
        Vector2 mapCenter = new Vector2(terraData.heightmapResolution/2f, terraData.heightmapResolution/2f);
        float[,] terraheight = terraData.GetHeights(0,0,terraData.heightmapResolution, terraData.heightmapResolution);

        for (int x = 0; x < terraData.heightmapResolution; x++)
        {
            for (int z = 0; z < terraData.heightmapResolution; z++)
            {
                float seaAmplifier;

                float distance = (mapCenter - new Vector2(x,z)).magnitude;
                

                if (distance > terraData.heightmapResolution / 4f)
                {
                    seaAmplifier = (distance - terraData.heightmapResolution / 4f) * 0.02f + Random.Range(0f,.05f);
                    terraheight[x, z] = terraheight[x,z] - seaAmplifier;
                }
            }
        }

        terraData.SetHeights(0, 0, terraheight);
    }

    void InitTerrain()
    {
        float[,] terraheight = new float[terraData.heightmapResolution, terraData.heightmapResolution];

        for (int x = 0; x < terraData.heightmapResolution; x++)
        {

            for (int z = 0; z < terraData.heightmapResolution; z++)
            {

                terraheight[x, z] = 0.5f;
            }
        }

        terraData.SetHeights(0, 0, terraheight);
    }

    // Update is called once per frame
    void Update()
    {
        if((currentTimer -= Time.deltaTime) <= 0)
        {
            currentTimer = timer;

            InitTerrain();
            PerlinMap();
            LowerSea();
        }
    }
}

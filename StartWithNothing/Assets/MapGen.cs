using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class MapGen : MonoBehaviour
{
    public int terrainSize = 256;
    public float perlinSteps = 1;
    public float perlinAmplifier = 1;

    int timer = 10;
    float currentTimer;

    Terrain terra;
    TerrainData terraData;

    Voronoi vor;
    Rectf clipRect;
    void Start()
    {
        terra = GetComponent<Terrain>();
        terraData = terra.terrainData;

        terraData.size = new Vector3(terrainSize, 10, terrainSize);




        GenerationProcedure();
        currentTimer = timer;
    }

    void GenerationProcedure()
    {
        GenerateVoronoi();
        VoronoiTerrain();
        SmoothEdges(5);
        PerlinMap();
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
               
                terraheight[x,z] = terraheight[x,z] + Mathf.PerlinNoise(perlinx, perliny) * .5f;
            }
        }

        terraData.SetHeights(0, 0, terraheight);
    }

    void GenerateVoronoi()
    {
        List<Vector2f> points = new List<Vector2f>();
        clipRect = new Rectf(terraData.heightmapResolution * 0.25f, terraData.heightmapResolution * 0.25f, terraData.heightmapResolution * 0.5f, terraData.heightmapResolution * 0.5f);
        for (int x = 0; x < 20; x++)
        {
            points.Add(new Vector2f(Random.Range(terraData.heightmapResolution * 0.25f, terraData.heightmapResolution * 0.75f), Random.Range(terraData.heightmapResolution * 0.25f, terraData.heightmapResolution * 0.75f)));
        }

        vor = new Voronoi(points, clipRect);
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

    void VoronoiTerrain()
    {
        float[,] terraheight = new float[terraData.heightmapResolution, terraData.heightmapResolution];

        for (int x = 0; x < terraData.heightmapResolution; x++)
        {

            for (int z = 0; z < terraData.heightmapResolution; z++)
            {
                if(PointIsInVoronoi(new Vector2f(x,z)))
                    terraheight[x, z] = 0.5f;
                else
                    terraheight[x, z] = 0f;
            }
        }

        terraData.SetHeights(0, 0, terraheight);
    }

    bool PointIsInVoronoi(Vector2f p)
    {

        Vector2f key = new Vector2f();
        float mindist = float.MaxValue;
        foreach(KeyValuePair<Vector2f, Site> s in vor.SitesIndexedByLocation)
        {
            float dist = (s.Key - p).magnitude; 
            if(dist < mindist)
            {
                mindist = dist;
                key = s.Key;
            }
        }

        float pdist = (key - p).magnitude;
        foreach (Vector2f vec in vor.SitesIndexedByLocation[key].Region(clipRect))
        {
            if ((key - vec).magnitude > pdist) return true;
        }
        return false;
        
    }

    void SmoothEdges(int smoothness)
    {
        float[,] heightmap = terraData.GetHeights(0, 0, terraData.heightmapResolution, terraData.heightmapResolution);
        float[,] nextheightmap = new float[terraData.heightmapResolution, terraData.heightmapResolution];
        for (int s = 0; s < smoothness; s++)
        {
            for (int x = 0; x < terraData.heightmapResolution; x++)
            {
                for (int y = 0; y < terraData.heightmapResolution; y++)
                {
                    float sum = 0;
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            if (i < 0 || i > terraData.heightmapResolution - 1 || j < 0 || j > terraData.heightmapResolution - 1) continue;
                            //Debug.Log(j);
                            sum += heightmap[i, j];
                        }
                    }
                    nextheightmap[x, y] = sum / 9f;
                }
            }
            heightmap = nextheightmap;
        }
        terraData.SetHeights(0, 0, heightmap);
    }

    // Update is called once per frame
    void Update()
    {
        if((currentTimer -= Time.deltaTime) <= 0)
        {
            currentTimer = timer;

            // InitTerrain();
            // PerlinMap();
            // LowerSea();
            GenerateVoronoi();
            VoronoiTerrain();
            SmoothEdges(5);
            PerlinMap();
        }
    }
}

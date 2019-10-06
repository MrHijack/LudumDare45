using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
public class MapGenerator : MonoBehaviour
{
    public int numPoints = 5;
    public int MapSize = 256;
    Voronoi vor;
    Terrain terra;
    TerrainData terraData;

    Mesh m = new Mesh();

    void Start()
    {

        terra = GetComponent<Terrain>();
        terraData = terra.terrainData;
        terraData.heightmapResolution = MapSize;
        

        List<Vector2f> points = new List<Vector2f>(numPoints);

        for (int i = 0; i < numPoints; i++)
        {
            points.Add(new Vector2f(Random.Range(0, 256), Random.Range(0, 256)));
        }

        vor = new Voronoi(points, new Rectf(0, 0, MapSize, MapSize));

        List<Vector2f> sitecords = vor.SiteCoords();
        List<List<Vector2f>> regions = vor.Regions();

        float[,] heightmap = new float[MapSize, MapSize];


        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                heightmap[x, y] = 0f;
            }
        }

        foreach (List<Vector2f> region in regions)
        {
            Vector2f prevPoint = new Vector2f(-1, -1);

            foreach (Vector2f point in region)
            {
                if(prevPoint.x == -1 && prevPoint.y == -1)
                {
                    prevPoint = point;
                    continue;
                }
                      

                Vector2f vec = point - prevPoint;
               // vec.Normalize();
                Debug.Log(vec);
                List<Vector2f> linePoints = new List<Vector2f>();
                for(float i = 0.01f; i < 1f; i+=0.01f)
                {
                    Vector2f p = point +(vec * i);
                    linePoints.Add(p);
                }
                foreach (Vector2f linepoint in linePoints)
                {
                 
                    int posx = Mathf.RoundToInt(linepoint.x);
                    int posy = Mathf.RoundToInt(linepoint.y);
                    if (posx >= MapSize || posx < 0 || posy >= MapSize || posy < 0) { continue; }
                    heightmap[(int)linepoint.x, (int)linepoint.y] = 0.01f;
                }

                prevPoint = point;
            }
        }

        foreach (Vector2f cord in sitecords)
        {
            heightmap[(int)cord.x, (int)cord.y] = 0.1f;
        }

        terraData.SetHeights(0, 0, heightmap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

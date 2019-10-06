using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    public float perlinAmplifier = 1f;
    public float perlinSteps = 1f;
    public int mapSize = 256;
    public float vertsPerUnit = 1;
    public int refresher = 5;
    float current;
    int rectcount;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    Mesh mesh;


    void Start()
    {
        mesh = new Mesh();
        rectcount = mapSize * (int)vertsPerUnit;
        current = refresher;


        GenerateFlatMesh();
        PerlinMesh();
        FinalizeMesh();
    }

    void GenerateFlatMesh()
    {
        for(float x = 0; x < mapSize; x+=1f /vertsPerUnit)
        {
            for(float z = 0; z < mapSize; z+= 1f / vertsPerUnit)
            {
                vertices.Add(new Vector3(x, 0, z));
            }
        }

        //Für jedes Viereck im Mesh
        for (int x = 0; x < rectcount - 1; x++)
        {
            for (int z = 0; z < rectcount - 1; z++)
            {
                int rectindex = x * rectcount + z;

                triangles.AddRange(new int[] { rectindex + 1, rectindex + rectcount, rectindex });
                triangles.AddRange(new int[] { rectindex + 1, rectindex + rectcount + 1 , rectindex + rectcount });

            }
        }

        
    }

    void PerlinMesh()
    {
        float perlinx = Random.Range(0f, 1f);
        float perliny = Random.Range(0f, 1f);
        
        for(int x = 0; x < rectcount; x++, perlinx+= perlinSteps)
        {
            perliny = 0;
            for (int z = 0; z < rectcount; z++, perliny+= perlinSteps)
            {
                int vertindex = x * rectcount + z;
                vertices[vertindex] = vertices[vertindex] + Vector3.up * Mathf.PerlinNoise(perlinx, perliny) * perlinAmplifier;
            }
        }
    }


    void FinalizeMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void FlattenMesh()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i] - new Vector3(0, vertices[i].y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if((current -= Time.deltaTime) <= 0)
        {
            current = refresher;
            FlattenMesh();
            PerlinMesh();
            FinalizeMesh();
        }
    }
}

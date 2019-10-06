using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class TerrainGenerator : MonoBehaviour
{

    public int MapSize = 256;
    public int numPoints = 10;
    float timer = 5f;
    float currentTime;

    Voronoi vor;
    Mesh mesh;

    List<Vector3> verticies;
    List<int> triangles;
    List<Color> colors;
    Vector2[] uvs;
    Rectf clipRect;
   // Texture2D tex;


    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
       // mesh = meshFilter.mesh;
       
        clipRect = new Rectf(0, 0, MapSize, MapSize);

    }

    void GenerateVoronoi(int numPoints)
    {
        List<Vector2f> points = new List<Vector2f>(numPoints);

        for (int i = 0; i < numPoints; i++)
        {
            points.Add(new Vector2f(Random.Range(0, MapSize), Random.Range(0, MapSize)));
        }

        vor = new Voronoi(points, new Rectf(0, 0, MapSize, MapSize));
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        verticies = new List<Vector3>();
        triangles = new List<int>();
      //  tex = new Texture2D(MapSize, MapSize);
       // colors = new List<Color>(1024);

        foreach (KeyValuePair<Vector2f, Site> site in vor.SitesIndexedByLocation)
        {
           // Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            Vector3 siteVec = new Vector3(site.Value.x, 0, site.Value.y);
            verticies.Add(siteVec);
           // tex.SetPixel(Mathf.RoundToInt(siteVec.x), Mathf.RoundToInt(siteVec.z), Color.red);
            int site_index = verticies.IndexOf(siteVec);
            List<Vector2f> verts = site.Value.Region(clipRect);
            int vert1_index, vert2_index;
            float multiplier = Mathf.Pow(10, 2);
            for (int i = 1; i < verts.Count; i++)
            {

                Vector3 vert1 = new Vector3(Mathf.Round(verts[i].x * multiplier) / multiplier, 0, Mathf.Round(verts[i].y * multiplier) / multiplier);
                Vector3 vert2 = new Vector3(Mathf.Round(verts[i - 1].x * multiplier) / multiplier, 0, Mathf.Round(verts[i - 1].y * multiplier) / multiplier);

              //  Debug.Log(verts[i]);
                if ((vert1_index = verticies.IndexOf(vert1)) == -1) { verticies.Add(vert1); vert1_index = verticies.IndexOf(vert1); }
                if ((vert2_index = verticies.IndexOf(vert2)) == -1) { verticies.Add(vert2); vert2_index = verticies.IndexOf(vert2); }

                triangles.AddRange(new int[] { site_index, vert1_index, vert2_index });
             //   colors[vert1_index] = c;
            //    colors[vert2_index] = c;
            }

            //manually add last triangle
            vert1_index = verticies.IndexOf(new Vector3(Mathf.Round(verts[0].x * multiplier) / multiplier, 0, Mathf.Round(verts[0].y * multiplier) / multiplier));
            vert2_index = verticies.IndexOf(new Vector3(Mathf.Round(verts[verts.Count - 1].x * multiplier) / multiplier, 0, Mathf.Round(verts[verts.Count - 1].y * multiplier) / multiplier));

            triangles.AddRange(new int[] { site_index, vert1_index, vert2_index });
        }
    }

    void RandomizeMesh()
    {
        foreach (KeyValuePair<Vector2f, Site> site in vor.SitesIndexedByLocation)
        {
            float randomheight = Random.Range(0f, 5f);
            Vector3 siteVec = new Vector3(site.Value.x, 0, site.Value.y);
            int site_index = verticies.IndexOf(siteVec);
            verticies[site_index] = siteVec + new Vector3(0, randomheight, 0);

            List<Vector2f> verts = site.Value.Region(clipRect);

            for (int i = 0; i < verts.Count; i++)
            {
                Vector3 vert = new Vector3(verts[i].x, 0, verts[i].y);

                int vert_index = verticies.IndexOf(vert);
                if (vert_index == -1) continue;
                verticies[vert_index] = vert + new Vector3(0, randomheight, 0);
            }
        }
    }

   void FinalizeMesh()
    {

        uvs = new Vector2[verticies.Count];
        for(int i= 0; i < verticies.Count; i++)
        {
            uvs[i] = new Vector2(verticies[i].x, verticies[i].y);
        }

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        //mesh.colors = colors.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshRenderer>().sharedMaterials[0].mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        if((currentTime -= Time.deltaTime) <=0)
        {
            currentTime = timer;
            GenerateVoronoi(numPoints);
            GenerateMesh();
            RandomizeMesh();
            FinalizeMesh();
        }
    }
}

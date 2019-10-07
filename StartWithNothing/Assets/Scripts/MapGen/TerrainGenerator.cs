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
       
        clipRect = new Rectf(0, 0, 0, 0);

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
      

        foreach (KeyValuePair<Vector2f, Site> site in vor.SitesIndexedByLocation)
        {
           
            Vector3 siteVec = new Vector3(site.Value.x, 0, site.Value.y);
            verticies.Add(siteVec);
        
            int site_index = verticies.IndexOf(siteVec);
            List<Vector2f> verts = site.Value.Region(clipRect);
            int vert1_index, vert2_index;
            
            for (int i = 1; i < verts.Count; i++)
            {

                Vector3 vert1 = RoundLocation(new Vector3(verts[i].x , 0, verts[i].y ));
                Vector3 vert2 = RoundLocation(new Vector3(verts[i - 1].x , 0, verts[i - 1].y));

              //  Debug.Log(verts[i]);
                if ((vert1_index = verticies.IndexOf(vert1)) == -1) { verticies.Add(vert1); vert1_index = verticies.IndexOf(vert1); }
                if ((vert2_index = verticies.IndexOf(vert2)) == -1) { verticies.Add(vert2); vert2_index = verticies.IndexOf(vert2); }

                triangles.AddRange(new int[] { site_index, vert1_index, vert2_index });
             //   colors[vert1_index] = c;
            //    colors[vert2_index] = c;
            }

            //manually add last triangle
            vert1_index = verticies.IndexOf(RoundLocation(new Vector3(verts[0].x, 0, verts[0].y)));
            vert2_index = verticies.IndexOf(RoundLocation(new Vector3(verts[verts.Count - 1].x, 0, verts[verts.Count - 1].y)));

            triangles.AddRange(new int[] { site_index, vert1_index, vert2_index });

        }
    }

    void RandomizeMesh()
    {
        foreach (KeyValuePair<Vector2f, Site> site in vor.SitesIndexedByLocation)
        {
            float randomheight = Random.Range(0f, 2f);
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

    void LowerSea()
    {
        List<Site> outerSea = new List<Site>();
        Dictionary<Vector2f,Site> sitelist = vor.SitesIndexedByLocation;
        for(int x = 0; x <= MapSize; x++)
        {
            for(int y = 0; y<= MapSize; y ++)
            {
                if (x > 0 && x < MapSize && y > 0 && y < MapSize) continue;
                Vector2f borderlocation = new Vector2f(x, y);
                Site closestSite = null;
                float closestDistance = float.MaxValue;
                foreach(KeyValuePair<Vector2f, Site> site in sitelist)
                {
                   float distance = (borderlocation - site.Key).magnitude;
                   if(distance <= closestDistance)
                    {
                        closestSite = site.Value;
                        closestDistance = (borderlocation - site.Key).magnitude;
                    }
                }

                if(!outerSea.Contains(closestSite))
                {
                    outerSea.Add(closestSite);
                }
            }
        }
        foreach (Site s in outerSea)
        {
            Vector3 sitelocation = new Vector3(s.x, 0, s.y);
            int vec_index = verticies.IndexOf(sitelocation);
            verticies[vec_index] = sitelocation + Vector3.down * 10;
            MoveSmooth(vec_index, Vector3.down * 10);

            foreach (Vector2f vec in s.Region(clipRect))
            {
                Vector3 vertLocation = RoundLocation(new Vector3(vec.x, 0, vec.y));
                if((vec_index = verticies.IndexOf(vertLocation)) == -1)continue;
                MoveSmooth(vec_index, Vector3.down *10);
            }
        }
    }

    float RoundLocation(float location)
    {
        float multiplier = Mathf.Pow(10, 2);
        return Mathf.Round(location * 100f) / 100f;

    }

    Vector3 RoundLocation(Vector3 v)
    {
        return new Vector3(RoundLocation(v.x), RoundLocation(v.y), RoundLocation(v.z));
    }

    void MoveSmooth(int vertindex, Vector3 direction)
    {
        int connectedSmoothness = 5;
        List<int>[] connectedVertices = new List<int>[connectedSmoothness];
        connectedVertices[0] = new List<int>(new int[] { vertindex });
        for (int x = 1; x < connectedSmoothness; x++)
        {
            connectedVertices[x] = GetConnectedVerteces(connectedVertices[x - 1]);
        }
        
        for (int x = 0; x < connectedSmoothness; x++)
        {
            foreach (int index in connectedVertices[x])
            {
                verticies[index] = verticies[index] + direction/x;
            }
        }

    }

    List<int> GetConnectedVerteces(List<int> vertindex)
    {
        List<int> connectedVertices = new List<int>();
        

        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (vertindex.Contains(triangles[i])|| vertindex.Contains(triangles[i+1]) || vertindex.Contains(triangles[i+2]))
                for (int j = i; j < i + 3; j++)
                {
                    if(!connectedVertices.Contains(triangles[j])) connectedVertices.Add(triangles[j]);
                }
        }

        return connectedVertices;

    }

    // Update is called once per frame
    void Update()
    {
        if((currentTime -= Time.deltaTime) <=0)
        {
            currentTime = timer;
            GenerateVoronoi(numPoints);
            GenerateMesh();
            FinalizeMesh();
            SubdivideMesh(4);
            LowerSea();
            FinalizeMesh();
            
           // mesh.RecalculateNormals();
            //FinalizeMesh();
        }
    }

    void SubdivideMesh(int subdivisions)
    {
        MeshHelper.Subdivide(mesh, subdivisions);
        triangles = new List<int>(mesh.triangles);
        verticies = new List<Vector3>(mesh.vertices);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterwave : MonoBehaviour
{
    Mesh m;

    // Start is called before the first frame update
    void Start()
    {
        m = GetComponent<MeshFilter>().mesh;

        Vector3[] verts = m.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = new Vector3(verts[i].x, Random.Range(0f, 1.5f), verts[i].z);
        }
        m.vertices = verts;
        m.RecalculateNormals();
        m.RecalculateTangents();
        m.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = m;
    }

    // Update is called once per frame
    void Update()
    {
/*
        Vector3[] verts = m.vertices;

            for (int i = 0; i < verts.Length; i++)
            {
               // verts[i] = new Vector3(verts[i].x, Mathf.Sin(verts[i].y * 360 + Time.realtimeSinceStartup), verts[i].z);
            }


        m.vertices = verts;
        m.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = m;
        */
    }
}

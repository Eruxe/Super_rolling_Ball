using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class meshProc : MonoBehaviour
{

    [SerializeField]
    GameObject prefab;

    [SerializeField]
    GameObject goal;

    GameObject Currentprefab;
    
    MeshFilter CurrentMeshFilter;

    delegate float TwoVariablesFunction(float x1, float x2);

    // Start is called before the first frame update
    void Start()
    {


        //Generation de seed
        //int seed = 45878;
        System.Random rnd = new System.Random();
        int seed = rnd.Next(10000, 99999);


        int nbPlat = (seed%7)+4;
        float sizex;
        float sizez;
        float sizey;
        float cursorx = transform.position.x;
        float cursory = transform.position.y;
        float cursorz = transform.position.z;



        for (int i = 0; i < nbPlat; i++)
        {
            //taille de la plateforme
            sizex = ((seed / (i+1)) % 5) + 2;
            sizez = ((seed / (i + 1)) % 4) + 2;

            //Deplacement vertical
            if ((seed+i) % 7 == 1 && i>0 && i<nbPlat-1)
            {
                sizey = (0.5f)*sizex;
            }
            else
            {
                sizey = 0;
            }

            //Centrage du spawn
            if (i == 0) cursorx -= sizex / 2;

            //Goal
            if (i == nbPlat - 1)
            {
                goal.transform.position = new Vector3(cursorx + sizex / 2, cursory, cursorz);
                sizez = ((seed / (i + 1)) % 2) + 4;
            }

            //Ajout d'une plateforme
            Currentprefab = Instantiate(prefab, new Vector3(cursorx, cursory, cursorz), Quaternion.identity) as GameObject;
            CurrentMeshFilter = Currentprefab.GetComponent(typeof(MeshFilter)) as MeshFilter;

            CurrentMeshFilter.sharedMesh = GenerateMutatedPlaneXZ(new Vector2(sizex, sizez),30,30,(x, z) => Mathf.Lerp(0, sizey, x));

            Currentprefab.AddComponent<MeshCollider>();

            
            //Deplacement du curseur
            cursorx += sizex;
            cursory += sizey;
        }

    }

    Mesh GenerateMutatedPlaneXZ(Vector2 size, int nSegmentsX, int nSegmentsZ, TwoVariablesFunction heightFunction)

    {
        Mesh mesh = new Mesh();
        mesh.name = "plane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 2 * 3]; //  
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];

        Vector2 halfSize = size * .5f;

        // remplissage des vertices, normals, uv
        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            int offset = (nSegmentsX + 1) * i;
            float kZ = (float)i / nSegmentsZ;
            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kX = (float)j / nSegmentsX;
                float x = Mathf.Lerp(0, size.x, kX);
                float z = Mathf.Lerp(-size.y/2, size.y/2, kZ);

                float y = heightFunction(kX, kZ);

                vertices[offset + j] = new Vector3(x, y, z);
                normals[offset + j] = Vector3.up;
                uvs[offset + j] = new Vector2(kX, kZ);
            }
        }

        // remplissage de triangles
        int index = 0;
        for (int i = 0; i < nSegmentsZ; i++)
        {
            int offset = (nSegmentsX + 1) * i;
            for (int j = 0; j < nSegmentsX; j++)
            {
                triangles[index++] = offset + j;
                triangles[index++] = offset + j + nSegmentsX + 1;
                triangles[index++] = offset + j + nSegmentsX + 1 + 1;

                triangles[index++] = offset + j;
                triangles[index++] = offset + j + nSegmentsX + 1 + 1;
                triangles[index++] = offset + j + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

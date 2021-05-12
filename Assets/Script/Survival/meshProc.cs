using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = System.Random;

public class meshProc : MonoBehaviour
{

    [SerializeField]
    GameObject prefab;

    [SerializeField]
    GameObject goal;

    GameObject Currentprefab;
    
    MeshFilter CurrentMeshFilter;

    public class MeshAndPos
    {
        public Mesh mesh;
        public Vector3 pos;
        public float width;
        

        public MeshAndPos(Mesh mesh, Vector3 pos,float width)
        {
            this.mesh = mesh;
            this.pos = pos;
            this.width = width;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Generation de seed
        //int seed = 45878;
        if (GameManager.seed == 0) GameManager.seed = new Random().Next(1, 999999999);
        int seed = GameManager.seed;
        Random Generator = new Random(seed);

        int nbPlat = Generator.Next(4,10);
        float sizex;
        float sizez;
        float sizey;
        float cursorx = transform.position.x;
        float cursory = transform.position.y;
        float cursorz = transform.position.z;
        float cursorRotation = 0;
        float targetRotation = 0;
        sizez = Generator.Next(3, 6);
        float nextSizeZ = sizez;
        int chaosFrequencyY = Generator.Next(20, 80);
        int chaosFrequencyWidth = Generator.Next(20, 50);
        int chaosFrequencyZ = Generator.Next(10, 50);
        int chaosFrequencyRotation = Generator.Next(5, 20);

        for (int i = 0; i < nbPlat; i++)
        {

            //taille de la plateforme
            sizex = Generator.Next(6, 9);


            Mutations.TwoVariablesFunction heighfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            Mutations.TwoVariablesFunction widthfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            Mutations.TwoVariablesFunction zfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            //Mutations
            if (i<nbPlat-1 && i>0)
            {
                heighfunct = Mutations.MutationSelectorY(Generator, chaosFrequencyY, ref sizex);
                int zModifier = Generator.Next(2);
                if (zModifier == 0) zfunct = Mutations.MutationSelectorZ(Generator, chaosFrequencyZ, ref sizex);
                else if (zModifier == 1) widthfunct = Mutations.MutationSelectorWidth(Generator, chaosFrequencyWidth, sizez);
                targetRotation = Mutations.SelectorRotation(Generator, chaosFrequencyRotation, cursorRotation);
            }

            //Centrage du spawn
            if (i == 0)
            {
                cursorx -= sizex / 2;
                cursorz -= sizez / 2;
            }

            //Plateforme du goal
            if (i == nbPlat - 1)
            {
                nextSizeZ = Generator.Next(4, 6);
                widthfunct = (pas, z) => Mathf.Lerp(0, nextSizeZ - sizez, pas);
            }

            //Ajout d'une plateforme
            Currentprefab = Instantiate(prefab, new Vector3(cursorx, cursory, cursorz), Quaternion.identity) as GameObject;
            CurrentMeshFilter = Currentprefab.GetComponent(typeof(MeshFilter)) as MeshFilter;

            MeshAndPos meshandpos = GenerateMutatedPlaneXZ(new Vector2(sizex, sizez), 30, 30, heighfunct, widthfunct, zfunct, new Vector2(cursorx,cursorz),cursorRotation, targetRotation);

            CurrentMeshFilter.sharedMesh = meshandpos.mesh;


            //rotation of the mesh from previous platform
            Vector3 vCursor = new Vector3(cursorx + meshandpos.pos.x, cursory + meshandpos.pos.y, cursorz + meshandpos.pos.z);
            Vector3 meshpivot;
            if (targetRotation - cursorRotation < 0)
            {
                float triangleHeight = sizez * Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(cursorRotation));
                float triangleWidth = sizez * Mathf.Cos(Mathf.Deg2Rad * Mathf.Abs(cursorRotation));
                meshpivot = new Vector3(cursorx, cursory, cursorz + sizez);
                if (cursorRotation > 0)
                {
                    CurrentMeshFilter.transform.position = new Vector3(cursorx + triangleHeight, cursory, cursorz + triangleWidth - sizez);
                    meshpivot = new Vector3(cursorx + triangleHeight, cursory, cursorz + triangleWidth);
                    vCursor.x += triangleHeight;
                    vCursor.z = vCursor.z + triangleWidth - sizez;
                }
                else if(cursorRotation < 0)
                {
                    CurrentMeshFilter.transform.position = new Vector3(cursorx - triangleHeight, cursory, cursorz + triangleWidth - sizez);
                    meshpivot = new Vector3(cursorx - triangleHeight, cursory, cursorz + triangleWidth);
                    vCursor.x -= triangleHeight;
                    vCursor.z = vCursor.z + triangleWidth - sizez;
                }
            }
            else
            {
                meshpivot = new Vector3(cursorx, cursory, cursorz);
            }

            CurrentMeshFilter.transform.RotateAround(meshpivot, new Vector3(0f, 1f, 0f), targetRotation);
            Vector3 newCursor = RotatePointAroundPivot(vCursor, meshpivot, new Vector3(0f, targetRotation, 0f));

            Currentprefab.AddComponent<MeshCollider>();

            //Placement du goal
            if (i == nbPlat - 1)
            {
                Vector3 center = CurrentMeshFilter.GetComponent<Collider>().bounds.center;
                center.y += 0.5f;
                goal.transform.position = center;
                goal.transform.Rotate(0, targetRotation, 0, Space.Self);
            }

            //Deplacement du curseur
            sizez = meshandpos.width;
            cursorx = newCursor.x;
            cursory = newCursor.y;
            cursorz = newCursor.z;
            cursorRotation = targetRotation;  
        }

    }

    MeshAndPos GenerateMutatedPlaneXZ(Vector2 size, int nSegmentsX, int nSegmentsZ, Mutations.TwoVariablesFunction heightFunction, Mutations.TwoVariablesFunction widthFunction, Mutations.TwoVariablesFunction zFunction, Vector2 absolutepos, float cursorRotation, float targetRotation)

    {
        Mesh mesh = new Mesh();
        mesh.name = "plane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1) * 2];
        int[] triangles = new int[(4*3) + ((nSegmentsX * nSegmentsZ) + nSegmentsX) * 2 * 3 * 2];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];

        int vDownOff = (nSegmentsX + 1) * (nSegmentsZ + 1);
        int tDownOff = nSegmentsX * nSegmentsZ * 2 * 3 - 1;

        float x = 0;
        float y = 0;
        float z = 0;
        float width = 0;

        // remplissage des vertices, normals, uv
        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            int offset = (nSegmentsX + 1) * i;
            float kZ = (float)i / nSegmentsZ;

            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kX = (float)j / nSegmentsX;
                width = size.y + widthFunction(kX * 2, kZ);
                float difference = widthFunction(kX * 2, kZ) / 2;

                x = Mathf.Lerp(0, size.x, kX);
                z = Mathf.Lerp(0- difference, size.y+difference, kZ) + zFunction(kX, kZ)/2;
                y = heightFunction(kX, kZ);

                if(j==0 && targetRotation - cursorRotation != 0)
                {
                    Vector3 pivot = new Vector3(0, 0, 0);
                    if (targetRotation - cursorRotation < 0)
                    {
                       pivot = new Vector3(0, 0, size.y);
                    }
                    float angleToPivot = targetRotation - cursorRotation;
                    Vector3 pointToPivot = new Vector3(x, y, z);
                    pointToPivot = RotatePointAroundPivot(pointToPivot, pivot, new Vector3(0,-angleToPivot,0));
                    x = pointToPivot.x;
                    z = pointToPivot.z;
                    y = pointToPivot.y;
                }

                if (j == 1 && Mathf.Abs(targetRotation - cursorRotation) > 90)
                {
                    Vector3 pivot = new Vector3(0, 0, 0);
                    if (targetRotation - cursorRotation < 0)
                    {
                        pivot = new Vector3(0, 0, size.y);
                    }
                    float angleToPivot = (targetRotation - cursorRotation)/2;
                    Vector3 pointToPivot = new Vector3(x, y, z);
                    pointToPivot = RotatePointAroundPivot(pointToPivot, pivot, new Vector3(0, -angleToPivot, 0));
                    x = pointToPivot.x;
                    z = pointToPivot.z;
                    y = pointToPivot.y;
                }

                //UP
                vertices[offset + j] = new Vector3(x, y, z);
                normals[offset + j] = Vector3.up;
                uvs[offset + j] = new Vector2(absolutepos.x + x, absolutepos.y + z);

                //DOWN
                vertices[(offset + j) + vDownOff] = new Vector3(x, y - 1, z);
                normals[(offset + j) + vDownOff] = Vector3.down;
                //uvs[(offset + j) + vDownOff] = new Vector2(kX, kZ);
            }
        }

        // remplissage de triangles
        int index = 0;
        int index2 = tDownOff + 1;
        for (int i = 0; i < nSegmentsZ; i++)
        {
            int offset = (nSegmentsX + 1) * i;
            for (int j = 0; j < nSegmentsX; j++)
            {
                //UP
                triangles[index++] = offset + j;
                triangles[index++] = offset + j + nSegmentsX + 1;
                triangles[index++] = offset + j + nSegmentsX + 1 + 1;

                triangles[index++] = offset + j;
                triangles[index++] = offset + j + nSegmentsX + 1 + 1;
                triangles[index++] = offset + j + 1;

                //DOWN
                triangles[index2++] = offset + j + vDownOff;
                triangles[index2++] = offset + j + nSegmentsX + 1 + 1 + vDownOff;
                triangles[index2++] = offset + j + nSegmentsX + 1 + vDownOff;

                triangles[index2++] = offset + j + vDownOff;
                triangles[index2++] = offset + j + 1 + vDownOff;
                triangles[index2++] = offset + j + nSegmentsX + 1 + 1 + vDownOff; 
            }
        }

        int index3 = index2;
        int index4 = index3 + (nSegmentsX * 2 * 3);
        // remplissage des triangles sur les cotes
        for (int i = 0; i < nSegmentsX; i++) {
            //RIGHT
            triangles[index3++] = i;
            triangles[index3++] = i + 1;
            triangles[index3++] = i + vDownOff +1;

            triangles[index3++] = i;
            triangles[index3++] = i + vDownOff + 1;
            triangles[index3++] = i + vDownOff;

            //LEFT
            triangles[index4++] = i + nSegmentsZ + (nSegmentsZ*nSegmentsX);
            triangles[index4++] = i + vDownOff + nSegmentsZ + (nSegmentsZ * nSegmentsX);
            triangles[index4++] = i + nSegmentsZ + 1 + (nSegmentsZ * nSegmentsX);

            triangles[index4++] = i + nSegmentsZ + 1 + (nSegmentsZ * nSegmentsX);
            triangles[index4++] = i + vDownOff + nSegmentsZ + (nSegmentsZ * nSegmentsX);
            triangles[index4++] = i + vDownOff + nSegmentsZ + 1 + (nSegmentsZ * nSegmentsX);
        }

        //remplissage triangles Back
        triangles[index4++] = 0;
        triangles[index4++] = vDownOff;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ*nSegmentsX) + vDownOff;
        triangles[index4++] = 0;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ * nSegmentsX) + vDownOff;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ * nSegmentsX);

        //remplissage triangles Front
        triangles[index4++] = 0 + nSegmentsX;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ * nSegmentsX) + vDownOff + nSegmentsX;
        triangles[index4++] = vDownOff + nSegmentsX;
        triangles[index4++] = 0 + nSegmentsX;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ * nSegmentsX) + nSegmentsX;
        triangles[index4++] = nSegmentsZ + (nSegmentsZ * nSegmentsX) + vDownOff + nSegmentsX;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        MeshAndPos result = new MeshAndPos(mesh, new Vector3(x,y,z-width),width);

        return result;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 v = point - pivot; //the relative vector from P2 to P1.
        v = Quaternion.Euler(angles.x,angles.y,angles.z) * v; //rotatate
        v = pivot + v; //bring back to world space
        return v;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

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

    delegate float TwoVariablesFunction(float x1, float x2);

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
        Random rnd = new Random();
        int seed = rnd.Next(0, 999999999);
        Random Generator = new Random(seed);


        int nbPlat = Generator.Next(4,10);
        float sizex;
        float sizez;
        float sizey;
        float cursorx = transform.position.x;
        float cursory = transform.position.y;
        float cursorz = transform.position.z;
        sizez = Generator.Next(3, 6);
        float nextSizeZ = sizez;
        int chaosFrequencyY = Generator.Next(20, 90);
        int chaosFrequencyWidth = Generator.Next(20, 60);
        int chaosFrequencyZ = Generator.Next(10, 50);

        for (int i = 0; i < nbPlat; i++)
        {

            //taille de la plateforme
            sizex = Generator.Next(6, 9);


            TwoVariablesFunction heighfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            TwoVariablesFunction widthfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            TwoVariablesFunction zfunct = (pas, z) => Mathf.Lerp(0, 0, pas);
            //Mutations
            if (i<nbPlat-1 && i>0)
            {
                heighfunct = MutationSelectorY(Generator, chaosFrequencyY, ref sizex);
                int zModifier = Generator.Next(3);
                if (zModifier <= 1) zfunct = MutationSelectorZ(Generator, chaosFrequencyZ, ref sizex);
                else widthfunct = MutationSelectorWidth(Generator, chaosFrequencyWidth, sizez);
            }

            //Centrage du spawn
            if (i == 0) cursorx -= sizex / 2;

            //Goal et widthMutation
            
            if (i == nbPlat - 1)
            {
                goal.transform.position = new Vector3(cursorx + sizex / 2, cursory, cursorz);
                nextSizeZ = Generator.Next(4, 6);
                widthfunct = (pas, z) => Mathf.Lerp(0, nextSizeZ - sizez, pas);
            }

            //Ajout d'une plateforme
            Currentprefab = Instantiate(prefab, new Vector3(cursorx, cursory, cursorz), Quaternion.identity) as GameObject;
            CurrentMeshFilter = Currentprefab.GetComponent(typeof(MeshFilter)) as MeshFilter;

            MeshAndPos meshandpos = GenerateMutatedPlaneXZ(new Vector2(sizex, sizez), 30, 30, heighfunct, widthfunct, zfunct);

            CurrentMeshFilter.sharedMesh = meshandpos.mesh;

            Currentprefab.AddComponent<MeshCollider>();


            //Deplacement du curseur
            sizez = meshandpos.width;
            cursorx += meshandpos.pos.x;
            cursory += meshandpos.pos.y;
            cursorz += meshandpos.pos.z- sizez/2;
            
        }

    }

    TwoVariablesFunction MutationSelectorY(Random Generator,int chaosFrequencyY,ref float sizex)
    {
        //VARIABLES POUR LES MUTATIONS
        float copyx = sizex;
        float intensity;
        float frequency = (float)Math.PI * 2;
        int negative;

        if (Generator.Next(100) > chaosFrequencyY)
        {
            int select = Generator.Next(6);
            negative = Generator.Next(2);
            //SELECTEUR MANUEL CI DESSOUS
            //select = 5;

            switch (select) {
                case 0:
                    if (negative > 0) copyx = -sizex;
                    return (pas, z) => Mathf.Lerp(0, copyx * 0.5f, pas);
                case 2:
                    sizex += Generator.Next(4,9);
                    intensity = (float)(Generator.Next(10, 20))/10;
                    return (theta, z) => (intensity * Mathf.Cos(theta * frequency)-intensity);
                case 3:
                    sizex += Generator.Next(6, 11);
                    intensity = (float)(Generator.Next(10, 20)) / 10;
                    return (theta, z) => (intensity * Mathf.Cos(theta * frequency + (float)Math.PI) +  intensity);
                case 4:
                    intensity = (float)(Generator.Next(6, 9)) / 10;
                    frequency = 8;
                    return (pas, z) => Mathf.Lerp(0, copyx * 0.3f + (intensity * Mathf.Sin(pas * frequency)), pas);
                case 5:
                    sizex += Generator.Next(3,7);
                    copyx = sizex;
                    intensity = (float)(Generator.Next(7, 17)) / 10;
                    return (pas, z) => Mathf.Lerp(0, -copyx * 0.5f + (intensity * Mathf.Sin(pas * frequency)), pas);
            }
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

    TwoVariablesFunction MutationSelectorWidth(Random Generator,int chaosFrequencyZ,float sizez)
    {
        if (Generator.Next(100) > chaosFrequencyZ)
        {
           int nextSizeZ = Generator.Next(2, 7);
           return (pas, z) => Mathf.Lerp(0, nextSizeZ - sizez, pas);
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

    TwoVariablesFunction MutationSelectorZ(Random Generator, int chaosFrequencyZ, ref float sizex)
    {
        //VAR POUR MUTATION
        float copyx = sizex;
        float angle;
        float intensity;
        float frequency;
        int negative;

        if (Generator.Next(100) > chaosFrequencyZ)
        {
            int select = Generator.Next(3);
            negative = Generator.Next(2);
            //SELECTEUR MANUEL CI DESSOUS
            //select = 0;

            switch (select){
                case 0:
                    angle = (float)(Generator.Next(3, 12)) / 10;
                    if (negative > 0) copyx = -sizex;
                    return (pas, z) => Mathf.Lerp(0, copyx * angle, pas);
                case 1:
                    sizex += Generator.Next(3, 10);
                    copyx = sizex;
                    intensity = (float)(Generator.Next(20, 35)) / 10;
                    frequency = (float)(Generator.Next(5, 9));
                    return (pas, z) => Mathf.Lerp(0, intensity * Mathf.Sin(pas * frequency), pas);
                case 2:
                    angle = (float)(Generator.Next(3, 8)) / 10;
                    sizex += Generator.Next(3, 8);
                    copyx = sizex;
                    if (negative > 0) copyx = -sizex;
                    intensity = (float)(Generator.Next(20, 35)) / 10;
                    frequency = (float)(Generator.Next(5, 9));
                    return (pas, z) => Mathf.Lerp(0, copyx * angle + (intensity * Mathf.Sin(pas * frequency)), pas);
            }
        }
        return (pas, z) => Mathf.Lerp(0, 0, pas);
    }

    MeshAndPos GenerateMutatedPlaneXZ(Vector2 size, int nSegmentsX, int nSegmentsZ, TwoVariablesFunction heightFunction, TwoVariablesFunction widthFunction, TwoVariablesFunction zFunction)

    {
        Mesh mesh = new Mesh();
        mesh.name = "plane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 2 * 3]; //  
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];

        Vector2 halfSize = size * .5f;

        float x=0;
        float y=0;
        float z=0;
        float width = 0;

        // remplissage des vertices, normals, uv
        for (int i = 0; i < nSegmentsZ + 1; i++)
        {
            int offset = (nSegmentsX + 1) * i;
            float kZ = (float)i / nSegmentsZ;
            for (int j = 0; j < nSegmentsX + 1; j++)
            {
                float kX = (float)j / nSegmentsX;
                width = size.y+widthFunction(kX*2, kZ);

                x = Mathf.Lerp(0, size.x, kX);
                z = Mathf.Lerp(-width/2, width / 2, kZ)+ zFunction(kX,kZ);
                y = heightFunction(kX, kZ);

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

        MeshAndPos result = new MeshAndPos(mesh, new Vector3(x,y,z),width);

        return result;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

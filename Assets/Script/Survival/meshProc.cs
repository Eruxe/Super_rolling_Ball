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
        public Vector3 middle;


        public MeshAndPos(Mesh mesh, Vector3 pos,float width,Vector3 middle)
        {
            this.mesh = mesh;
            this.pos = pos;
            this.width = width;
            this.middle = middle;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Generation de seed
        if (GameManager.seed == 0) GameManager.seed = new Random().Next(1, 999999999);
        int seed = GameManager.seed;
        Random Generator = new Random(seed);

        int nbPlat = Generator.Next(5,12);
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
        Vector3 raycursor = new Vector3(0,0,0);
        int antiFreeze = 0;
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
                raycursor = new Vector3(cursorx, cursory, cursorz+sizez);
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
            meshandpos.middle = new Vector3(cursorx, cursory, cursorz) + meshandpos.middle;
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
                    meshandpos.middle.x += triangleHeight;
                    meshandpos.middle.z = meshandpos.middle.z + triangleWidth - sizez;
                }
                else if(cursorRotation < 0)
                {
                    CurrentMeshFilter.transform.position = new Vector3(cursorx - triangleHeight, cursory, cursorz + triangleWidth - sizez);
                    meshpivot = new Vector3(cursorx - triangleHeight, cursory, cursorz + triangleWidth);
                    vCursor.x -= triangleHeight;
                    vCursor.z = vCursor.z + triangleWidth - sizez;
                    meshandpos.middle.x -= triangleHeight;
                    meshandpos.middle.z = meshandpos.middle.z + triangleWidth - sizez;
                }
                raycursor = CurrentMeshFilter.transform.position;
            }
            else
            {
                meshpivot = new Vector3(cursorx, cursory, cursorz);
            }

            CurrentMeshFilter.transform.RotateAround(meshpivot, new Vector3(0f, 1f, 0f), targetRotation);

            if (targetRotation - cursorRotation >=0) raycursor = RotatePointAroundPivot(raycursor, meshpivot, new Vector3(0f, targetRotation - cursorRotation, 0f));
            else raycursor = RotatePointAroundPivot(raycursor, meshpivot, new Vector3(0f, targetRotation , 0f));

            Vector3 newrayCursor = RotatePointAroundPivot(new Vector3(vCursor.x, vCursor.y, vCursor.z + meshandpos.width), meshpivot, new Vector3(0f, targetRotation, 0f));
            Vector3 newCursor = RotatePointAroundPivot(vCursor, meshpivot, new Vector3(0f, targetRotation, 0f));
            meshandpos.middle = RotatePointAroundPivot(meshandpos.middle, meshpivot, new Vector3(0f, targetRotation, 0f));

            //TEST DE COLLISIONS DES PLATEFORME (MAILLAGE)
            bool isPlatformCollide = false;
            Vector3 offSet = new Vector3(0, -0.5f, 0);
            Vector3 ceilingoff = new Vector3(0, 3, 0);
            Vector3 flooroff = new Vector3(0, -4, 0);

            Vector3 leftmiddle = Vector3.Lerp(new Vector3(raycursor.x,meshandpos.middle.y, raycursor.z), new Vector3(newrayCursor.x, meshandpos.middle.y, newrayCursor.z), 0.5f);
            Vector3 rightmiddle = Vector3.Lerp(new Vector3(meshpivot.x, meshandpos.middle.y, meshpivot.z), new Vector3(newCursor.x, meshandpos.middle.y, newCursor.z), 0.5f);
            Vector3 Zdiff = meshandpos.middle - Vector3.Lerp(new Vector3(meshpivot.x, meshandpos.middle.y, meshpivot.z), new Vector3(newrayCursor.x, meshandpos.middle.y, newrayCursor.z),0.5f);

            if (targetRotation - cursorRotation < 0)
            {
                leftmiddle = Vector3.Lerp(new Vector3(meshpivot.x, meshandpos.middle.y, meshpivot.z), new Vector3(newrayCursor.x, meshandpos.middle.y, newrayCursor.z), 0.5f);
                rightmiddle = Vector3.Lerp(new Vector3(raycursor.x, meshandpos.middle.y, raycursor.z), new Vector3(newCursor.x, meshandpos.middle.y, newCursor.z), 0.5f);
                Zdiff = meshandpos.middle - Vector3.Lerp(new Vector3(meshpivot.x, meshandpos.middle.y, meshpivot.z), new Vector3(newCursor.x, meshandpos.middle.y, newCursor.z), 0.5f);
            }

            raycursor = Vector3.MoveTowards(raycursor , meshandpos.middle , 0.3f);
            meshpivot = Vector3.MoveTowards(meshpivot , meshandpos.middle , 0.3f);

            isPlatformCollide = CollisionMaillage(meshandpos, offSet, meshpivot, raycursor, newCursor, newrayCursor, ceilingoff, flooroff, rightmiddle, leftmiddle, Zdiff, targetRotation, cursorRotation);
            DebugMaillage(meshandpos, offSet, meshpivot, raycursor, newCursor, newrayCursor, ceilingoff, flooroff, rightmiddle, leftmiddle, Zdiff, targetRotation, cursorRotation, isPlatformCollide);

            //Rebouclage si collision
            if (isPlatformCollide && antiFreeze<30)
            {
                Destroy(Currentprefab);
                if (i == nbPlat - 1) i--;
                i--;
                antiFreeze++;
                continue;
            }
            antiFreeze = 0;

            //Ajout du collider
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
            raycursor = newrayCursor;

            if(cursory-10 < GameManager.fallTresh)
            {
                GameManager.fallTresh = cursory - 10;
            }
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

        Vector3 middle = new Vector3(0,0,0);

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

                if(i==nSegmentsX/2 && j == nSegmentsZ / 2)
                {
                    middle = new Vector3(x, y, z);
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

        MeshAndPos result = new MeshAndPos(mesh, new Vector3(x,y,z-width),width,middle);

        return result;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 v = point - pivot; //the relative vector from P2 to P1.
        v = Quaternion.Euler(angles.x,angles.y,angles.z) * v; //rotatate
        v = pivot + v; //bring back to world space
        return v;
    }

    public void DebugMaillage(MeshAndPos meshandpos, Vector3 offSet, Vector3 meshpivot, Vector3 raycursor, Vector3 newCursor, Vector3 newrayCursor, Vector3 ceilingoff, Vector3 flooroff, Vector3 rightmiddle, Vector3 leftmiddle, Vector3 Zdiff, float targetRotation, float cursorRotation, bool isPlatformCollide)
    {
        Color c= Color.white;
        if (isPlatformCollide) c = Color.red;

        //GRILLE
        Debug.DrawLine(meshandpos.middle + offSet, meshpivot + offSet, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet, raycursor + offSet, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet, newCursor + offSet, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet, newrayCursor + offSet, c, 200);

        Debug.DrawLine(meshandpos.middle + offSet + ceilingoff, meshpivot + offSet + ceilingoff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + ceilingoff, raycursor + offSet + ceilingoff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + ceilingoff, newCursor + offSet + ceilingoff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + ceilingoff, newrayCursor + offSet + ceilingoff, c, 200);

        Debug.DrawLine(meshandpos.middle + offSet + flooroff, meshpivot + offSet + flooroff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + flooroff, raycursor + offSet + flooroff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + flooroff, newCursor + offSet + flooroff, c, 200);
        Debug.DrawLine(meshandpos.middle + offSet + flooroff, newrayCursor + offSet + flooroff, c, 200);

        //LIGNES SUR LE COTE
        if (targetRotation - cursorRotation < 0)
        {
            Debug.DrawLine(rightmiddle + offSet + Zdiff, raycursor + offSet, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff, meshpivot + offSet, c, 200);

            Debug.DrawLine(rightmiddle + offSet + Zdiff + ceilingoff, raycursor + offSet + ceilingoff, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff + ceilingoff, meshpivot + offSet + ceilingoff, c, 200);

            Debug.DrawLine(rightmiddle + offSet + Zdiff + flooroff, raycursor + offSet + flooroff, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff + flooroff, meshpivot + offSet + flooroff, c, 200);
        }
        else
        {
            Debug.DrawLine(rightmiddle + offSet + Zdiff, meshpivot + offSet, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff, raycursor + offSet, c, 200);

            Debug.DrawLine(rightmiddle + offSet + Zdiff + ceilingoff, meshpivot + offSet + ceilingoff, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff + ceilingoff, raycursor + offSet + ceilingoff, c, 200);

            Debug.DrawLine(rightmiddle + offSet + Zdiff + flooroff, meshpivot + offSet + flooroff, c, 200);
            Debug.DrawLine(leftmiddle + offSet + Zdiff + flooroff, raycursor + offSet + flooroff, c, 200);
        }

        Debug.DrawLine(rightmiddle + offSet + Zdiff, newCursor + offSet, c, 200);
        Debug.DrawLine(leftmiddle + offSet + Zdiff, newrayCursor + offSet, c, 200);

        Debug.DrawLine(rightmiddle + offSet + Zdiff + ceilingoff, newCursor + offSet + ceilingoff, c, 200);
        Debug.DrawLine(leftmiddle + offSet + Zdiff + ceilingoff, newrayCursor + offSet + ceilingoff, c, 200);

        Debug.DrawLine(rightmiddle + offSet + Zdiff + flooroff, newCursor + offSet + flooroff, c, 200);
        Debug.DrawLine(leftmiddle + offSet + Zdiff + flooroff, newrayCursor + offSet + flooroff, c, 200);

        //LIGNES VERTICALES
        Debug.DrawLine(meshandpos.middle + flooroff + offSet, meshandpos.middle + ceilingoff + offSet, c, 200);
        Debug.DrawLine(meshpivot + flooroff + offSet, meshpivot + ceilingoff + offSet, c, 200);
        Debug.DrawLine(raycursor + flooroff + offSet, raycursor + ceilingoff + offSet, c, 200);
        Debug.DrawLine(newCursor + flooroff + offSet, newCursor + ceilingoff + offSet, c, 200);
        Debug.DrawLine(newrayCursor + flooroff + offSet, newrayCursor + ceilingoff + offSet, c, 200);
        Debug.DrawLine(rightmiddle + offSet + Zdiff + flooroff, rightmiddle + offSet + Zdiff + ceilingoff, c, 200);
        Debug.DrawLine(leftmiddle + offSet + Zdiff + flooroff, leftmiddle + offSet + Zdiff + ceilingoff, c, 200);
    }

    public bool CollisionMaillage(MeshAndPos meshandpos, Vector3 offSet, Vector3 meshpivot, Vector3 raycursor, Vector3 newCursor, Vector3 newrayCursor, Vector3 ceilingoff, Vector3 flooroff, Vector3 rightmiddle, Vector3 leftmiddle, Vector3 Zdiff, float targetRotation, float cursorRotation)
    {
        bool result = false;

        //GRILLE
        result |= Physics.Linecast(meshandpos.middle + offSet, meshpivot + offSet);
        result |= Physics.Linecast(meshandpos.middle + offSet, raycursor + offSet);
        result |= Physics.Linecast(meshandpos.middle + offSet, newCursor + offSet);
        result |= Physics.Linecast(meshandpos.middle + offSet, newrayCursor + offSet);

        result |= Physics.Linecast(meshandpos.middle + offSet + ceilingoff, meshpivot + offSet + ceilingoff);
        result |= Physics.Linecast(meshandpos.middle + offSet + ceilingoff, raycursor + offSet + ceilingoff);
        result |= Physics.Linecast(meshandpos.middle + offSet + ceilingoff, newCursor + offSet + ceilingoff);
        result |= Physics.Linecast(meshandpos.middle + offSet + ceilingoff, newrayCursor + offSet + ceilingoff);

        result |= Physics.Linecast(meshandpos.middle + offSet + flooroff, meshpivot + offSet + flooroff);
        result |= Physics.Linecast(meshandpos.middle + offSet + flooroff, raycursor + offSet + flooroff);
        result |= Physics.Linecast(meshandpos.middle + offSet + flooroff, newCursor + offSet + flooroff);
        result |= Physics.Linecast(meshandpos.middle + offSet + flooroff, newrayCursor + offSet + flooroff);

        //LIGNES SUR LE COTE
        if (targetRotation - cursorRotation < 0)
        {
            result |= Physics.Linecast(rightmiddle + offSet + Zdiff, raycursor + offSet);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff, meshpivot + offSet);

            result |= Physics.Linecast(rightmiddle + offSet + Zdiff + ceilingoff, raycursor + offSet + ceilingoff);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff + ceilingoff, meshpivot + offSet + ceilingoff);

            result |= Physics.Linecast(rightmiddle + offSet + Zdiff + flooroff, raycursor + offSet + flooroff);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff + flooroff, meshpivot + offSet + flooroff);
        }
        else
        {
            result |= Physics.Linecast(rightmiddle + offSet + Zdiff, meshpivot + offSet);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff, raycursor + offSet);

            result |= Physics.Linecast(rightmiddle + offSet + Zdiff + ceilingoff, meshpivot + offSet + ceilingoff);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff + ceilingoff, raycursor + offSet + ceilingoff);

            result |= Physics.Linecast(rightmiddle + offSet + Zdiff + flooroff, meshpivot + offSet + flooroff);
            result |= Physics.Linecast(leftmiddle + offSet + Zdiff + flooroff, raycursor + offSet + flooroff);
        }

        result |= Physics.Linecast(rightmiddle + offSet + Zdiff, newCursor + offSet);
        result |= Physics.Linecast(leftmiddle + offSet + Zdiff, newrayCursor + offSet);

        result |= Physics.Linecast(rightmiddle + offSet + Zdiff + ceilingoff, newCursor + offSet + ceilingoff);
        result |= Physics.Linecast(leftmiddle + offSet + Zdiff + ceilingoff, newrayCursor + offSet + ceilingoff);

        result |= Physics.Linecast(rightmiddle + offSet + Zdiff + flooroff, newCursor + offSet + flooroff);
        result |= Physics.Linecast(leftmiddle + offSet + Zdiff + flooroff, newrayCursor + offSet + flooroff);

        //LIGNES VERTICALES
        result |= Physics.Linecast(meshandpos.middle + flooroff + offSet, meshandpos.middle + ceilingoff + offSet);
        result |= Physics.Linecast(meshpivot + flooroff + offSet, meshpivot + ceilingoff + offSet);
        result |= Physics.Linecast(raycursor + flooroff + offSet, raycursor + ceilingoff + offSet);
        result |= Physics.Linecast(newCursor + flooroff + offSet, newCursor + ceilingoff + offSet);
        result |= Physics.Linecast(newrayCursor + flooroff + offSet, newrayCursor + ceilingoff + offSet);
        result |= Physics.Linecast(rightmiddle + offSet + Zdiff + flooroff, rightmiddle + offSet + Zdiff + ceilingoff);
        result |= Physics.Linecast(leftmiddle + offSet + Zdiff + flooroff, leftmiddle + offSet + Zdiff + ceilingoff);

        return result;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

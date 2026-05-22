using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {
    
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    
    public bool createMesh;

    public Gradient gradient;
    public AnimationCurve heightCurve;

    public float octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;
    public float scale;
    public float heightMultiplier;
    public int seed;
    
    
    [Range(0,1)]
    public float lerp;

    public Transform waterTransform;

    public int size;
    private int xSize;
    private int zSize;
    
    float maxOffset = 1000.0f;
    private float xOffset;
    private float zOffset;

    void Awake() {
        seed = MainController.Instance.seed;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        
        CreateShape();
        UpdateMesh();
    }

    void Start() {
        
        NavMeshSurface navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();

    }
    
    // Fractal Brownian Motion
    float Noise(float x, float z, float minTerrainHeight, float maxTerrainHeight) {
        
        x = (x + xOffset) / xSize;
        z = (z + zOffset) / zSize;

        float amplitude = 1.0f;
        float frequency = 1.0f;
        float y = 0.0f;

        for (int i = 0; i < octaves; i++) {

            float sampleX = x / scale * frequency;
            float sampleZ = z / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1; // use built-in Perlin Noise Function
            y += perlinValue * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;

        }

        y = (float) Math.Pow(y, 2);     // square height value to flatten valleys
        
        // record min and max terrain height for normalisation
        if (y > maxTerrainHeight) {
            maxTerrainHeight = y;
        }
        else if (y < minTerrainHeight) {
            minTerrainHeight = y;
        }

        y = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, y);   // normalise terrain height to between 0 and 1

        y = heightCurve.Evaluate(y);    // apply custom height curve
        
        return y;
        
    }

    float Distance(float x, float z, float width, float height) {

        float nx = 2 * x / width - 1;
        float nz = 2 * z / height - 1;

        return (float)(1 - (1 - Math.Pow(nx, 2)) * (1 - Math.Pow(nz, 2)));

        // return Math.Min(1, ((math.pow(nx, 2) + math.pow(nz, 2)) / math.sqrt(2)));

    }
    
    // generate shape of the terrain to apply to mesh
    void CreateShape() {
        
        Random.InitState(seed);
        
        xOffset = Random.Range(0.0f, maxOffset);
        zOffset = Random.Range(0.0f, maxOffset);
        
        xSize = size;
        zSize = size;

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        float minTerrainHeight = float.MaxValue;
        float maxTerrainHeight = float.MinValue;

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {

                float d = Distance(x, z, xSize, zSize);
                float y = Noise(x, z, minTerrainHeight, maxTerrainHeight);
                
                y *= Mathf.Lerp(y, 1 - d, lerp);
                y *= heightMultiplier;
                
                
                
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight) {
                    maxTerrainHeight = y;
                }
                else if (y < minTerrainHeight) {
                    minTerrainHeight = y;
                }
                
                i++;

            }
        }
        
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
            
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];
        
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {

                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;

            }
        }

    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        meshCollider.sharedMesh = mesh;
        
        mesh.RecalculateNormals();
    }
    
    // update terrain whenever values are changed in the editor
    private void OnValidate() {
        if (mesh) {
            CreateShape();
            UpdateMesh();
        }

        if (createMesh) {
            createMesh = false;
        }
    }

    void Update()
    {

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectGeneratorController : MonoBehaviour {

    [Header("Settings")] 
    public GameObject objectPrefab;

    public bool treePlacer = false;

    public float spawnChance;
    public float checkDistance;
    public float checkHeight;
    public float checkRange;
    public float clustering;
    public float heightAboveWater = 5;
    public float groundEmbedDepth = 1;
    public LayerMask layerMask;
    public Vector2 positionPos;
    public Vector2 positionNeg;

    public float maxSize = 10;
    public float minSize;
    
    private int seed;

    public Transform waterTransform;

    private float xOffset;
    private float zOffset;
    
    void Start() {
        
        xOffset = Random.Range(0f, 1000f);
        zOffset = Random.Range(0f, 1000f);
        
        SpawnResources();
        
    }
    
    // re-spawn resources if values are changed in the editor
    private void OnValidate() {
        if (Application.isPlaying) {
            DeleteResources();
            SpawnResources();
        }
        
    }
    
    // noise function for modifying spawn rate
    float Noise(float x, float z) {

        float amplitude = 1.0f;
        float frequency = 1.0f;
        float cumulativeFrequency = 0.0f;
        float y = 0.0f;

        x /= positionPos.x * 2;
        z /= positionPos.y * 2;

        for (int i = 0; i < 5; i++) {
        
            float perlinValue = Mathf.PerlinNoise(x + xOffset, z + zOffset) * 2 - 1;
            y += perlinValue * amplitude;
            cumulativeFrequency += frequency;
        
        }
        
        return (float) math.pow(y, 2.0);

    }
    
    // spawns instances of selected prefab
    void SpawnResources() {
        
        seed = GameObject.Find("Terrain Mesh Generator").GetComponent<MeshGenerator>().seed;
        
        for(float x = positionNeg.x; x < positionPos.x; x += checkDistance) {
            for(float z = positionNeg.y; z < positionPos.y; z += checkDistance) {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, checkHeight, z), Vector3.down, out hit, checkRange, layerMask)) {
                    float spawn = Noise(x, z);
                    spawn = Mathf.Lerp(0, spawnChance, spawn + hit.point.y);
                    
                    if(spawn > Random.Range(0f, 101f) && spawn > clustering && hit.point.y > waterTransform.position.y + heightAboveWater) {
                        GameObject o = Instantiate(objectPrefab, new Vector3(hit.point.x, hit.point.y - groundEmbedDepth, hit.point.z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                        float objectSize = Random.Range(spawn, maxSize);
                        o.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
                        if (!treePlacer) {
                            o.transform.up = hit.normal;
                        }
                    }
                }
            }
        }
        
    }
    
    // deletes all palced objects
    void DeleteResources() {

        List<GameObject> l = new List<GameObject>();
        foreach (Transform child in transform) {
            l.Add(child.gameObject);
        }

        foreach (GameObject g in l) {
            Destroy(g);
        }
        
    }
    
}

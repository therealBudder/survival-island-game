using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearSpawner : MonoBehaviour {
    
    public Vector2 positionPos;
    public Vector2 positionNeg;
    
    public GameObject bear;
    public LayerMask layerMask;
    public Transform waterTransform;
    
    void Start()
    {
        SpawnBear();
    }
    
    // spawn bear in first valid location found, away from player
    void SpawnBear() {
        
        for(float x = positionNeg.x; x < positionPos.x; x += 20) {
            for(float z = positionNeg.y; z < positionPos.y; z += 20) {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, 30, z), Vector3.down, out hit, 50, layerMask)) {
                    
                    if(hit.point.y > waterTransform.position.y + 5) {
                        Instantiate(bear, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                        return;
                    }
                }
            }
        }
        
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    
    public Vector2 positionPos;
    public Vector2 positionNeg;
    
    public GameObject player;
    public LayerMask layerMask;
    public Transform waterTransform;
    
    void Start()
    {
        SpawnPlayer();
    }
    
    // spawn player in first valid location found
    void SpawnPlayer() {
        
        for(float x = positionPos.x; x > positionNeg.x; x -= 20) {
            for(float z = positionPos.y; z > positionNeg.y; z -= 20) {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(x, 30, z), Vector3.down, out hit, 50, layerMask)) {
                    
                    if(hit.point.y > waterTransform.position.y + 5) {
                        Instantiate(player, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                        return;
                    }
                }
            }
        }
        
    }
    
}

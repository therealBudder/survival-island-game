using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpItemController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (Input.GetKey(KeyCode.E)) {
                gameObject.SetActive(false);
                PlayerCharacterController player = other.GetComponent<PlayerCharacterController>();
                player.hunger += 5;
                player.mushroomsEaten += 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

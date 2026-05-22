using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponCollision : MonoBehaviour
{
    public int damage;
    public float hitCooldown;
    
    public EnemyMovementController thisEnemy;
    private PlayerCharacterController player;

    private void OnTriggerEnter(Collider other) {
        
        if (other.tag == "Player" && thisEnemy.isAttacking) {
            player = other.GetComponent<PlayerCharacterController>();
            if (player.health > 0 && player.canGetHit) {
                player.canGetHit = false;
                player.health -= damage;
                
                StartCoroutine(ResetHitCooldown());
            }
            
        }
        
    }
    
    IEnumerator ResetHitCooldown() {
        yield return new WaitForSeconds(hitCooldown);
        player.canGetHit = true;
    }
}

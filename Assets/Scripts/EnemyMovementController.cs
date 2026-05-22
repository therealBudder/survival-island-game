using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    public Transform target;
    public float attackDistance;
    public float range = 100;
    public float attackCooldown = 1;

    public bool isAttacking = false;
    public bool canAttack = true;
    
    private Vector3 startingPosition;
    private NavMeshAgent agent;
    private Animator animator;
    private float distance;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        startingPosition = agent.transform.position;
        target = GameObject.FindWithTag("Player").transform;
        print(startingPosition);
    }
    
    void Update() {
        
        // place enemy on nav mesh if not already
        if(!agent.isOnNavMesh) {

            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, NavMesh.AllAreas))
                transform.position = closestHit.position;
            agent.enabled = false;
            agent.enabled = true;
        }

        distance = Vector3.Distance(agent.transform.position, target.position);
        NavMeshPath navMeshPath = new NavMeshPath();
        
        if (distance < attackDistance && agent.CalculatePath(target.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete) {
            AttackState();
        }
        else if (distance <= range && agent.CalculatePath(target.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete) {
            // agent.SetPath(navMeshPath);
            RunState();
            agent.destination = target.position;
            
        }
        else if (Vector3.Distance(agent.transform.position, startingPosition) < 10) {
            SleepState();
        }
        else {
            RunState();
            agent.destination = startingPosition;
        }

    }

    void Attack() {
        
        isAttacking = true;
        canAttack = false;
        animator.SetTrigger("Attack1");
        StartCoroutine(ResetAttackCooldown());

    }

    void RunState() {
        
        agent.isStopped = false;
        animator.SetBool("Sleep", false);
        animator.SetBool("Run Forward", true);
        
    }

    void AttackState() {

        agent.isStopped = true;
        animator.SetBool("Sleep", false);
        animator.SetBool("Run Forward", false);
        if (canAttack) {Attack();}
        
    }

    void SleepState() {
        agent.isStopped = true;
        animator.SetBool("Run Forward", false);
        animator.SetBool("Sleep", true);
        
    }

    IEnumerator ResetAttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }
    
}

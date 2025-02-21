using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Transform target;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float activationRange = 10f;
    [SerializeField] private NavMeshAgent agent;

    [Header("Ataque")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private bool isAttacking = false;
    private bool isTargetDetected = false;
    private Health targetHealth;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        DetectPlayer();

        if (isTargetDetected && target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        isTargetDetected = distance <= activationRange;
    }

    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stoppingDistance)
        {
            agent.isStopped = false; // Asegurar que el agente puede moverse
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true; // Detener completamente al enemigo
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetHealth = other.GetComponent<Health>();

            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        while (isAttacking && targetHealth != null)
        {
            targetHealth.Damage(attackDamage);
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    // Dibuja los rangos de detección en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}

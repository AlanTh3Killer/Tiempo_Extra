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
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private NavMeshAgent agent;

    [Header("Combate")]
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private float attackInterval = 2.0f;
    [SerializeField] private float defenseDuration = 1.5f;
    private bool isTargetDetected = false;
    private bool isDefending = false;
    private bool isAttacking = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        combatSystem = GetComponent<CombatSystem>();
        agent.stoppingDistance = stoppingDistance;

        StartCoroutine(CombatLoop());
    }

    private void Update()
    {
        DetectPlayer();

        if (isTargetDetected && target != null && !isDefending)
        {
            FollowPlayer();
        }
    }

    private void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        isTargetDetected = distance <= activationRange;
    }

    private void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (!isAttacking && !isDefending)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    private IEnumerator CombatLoop()
    {
        while (true)
        {
            if (isTargetDetected)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange)
                {
                    isAttacking = true;
                    isDefending = false;
                    agent.isStopped = false;
                    Debug.Log($"{gameObject.name}: Atacando");
                    combatSystem.Attack(target.GetComponent<Health>());
                    yield return new WaitForSeconds(attackInterval);
                }

                isAttacking = false;
                isDefending = true;
                agent.isStopped = true;
                combatSystem.StartDefense();
                Debug.Log($"{gameObject.name}: Defendiendo");
                yield return new WaitForSeconds(defenseDuration);

                isDefending = false;
                agent.isStopped = false;
            }

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

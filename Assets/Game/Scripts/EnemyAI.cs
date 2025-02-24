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

    [Header("Combate")]
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private float defenseChance = 0.3f; // Probabilidad de defenderse
    [SerializeField] private float defenseCooldown = 5f;

    private bool isTargetDetected = false;
    private Health targetHealth;
    private bool canDefend = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        combatSystem = GetComponent<CombatSystem>();
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

        if (isTargetDetected)
        {
            Debug.Log($"{gameObject.name}: Jugador detectado, persiguiendo...");
        }
    }

    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            Debug.Log($"{gameObject.name}: Moviéndose hacia el jugador...");
        }
        else
        {
            agent.isStopped = true;
            Debug.Log($"{gameObject.name}: Ha llegado al jugador y se prepara para atacar o defenderse.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetHealth = other.GetComponent<Health>();

            if (canDefend && Random.value < defenseChance)
            {
                Debug.Log($"{gameObject.name}: Se está defendiendo!");
                StartCoroutine(Defend());
            }
            else if (targetHealth != null)
            {
                Debug.Log($"{gameObject.name}: Atacando al jugador!");
                combatSystem.Attack(targetHealth);
            }
        }
    }

    private IEnumerator Defend()
    {
        combatSystem.StartDefense();
        canDefend = false;
        Debug.Log($"{gameObject.name}: En cooldown de defensa...");
        yield return new WaitForSeconds(defenseCooldown);
        canDefend = true;
        Debug.Log($"{gameObject.name}: Puede volver a defenderse.");
    }
}

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

    [Header("Animaciones")]
    [SerializeField] private Animator animator; // 🔹 Ahora podemos asignarlo manualmente en el inspector

    private bool isTargetDetected = false;
    private bool isDefending = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private int lastAttackIndex = 0; // Para alternar entre animaciones de ataque
    private Health healthComponent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        combatSystem = GetComponent<CombatSystem>();
        healthComponent = GetComponent<Health>();

        // 🔹 Si no está asignado en el inspector, lo buscamos en los hijos
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        agent.stoppingDistance = stoppingDistance;
        StartCoroutine(CombatLoop());
    }

    private void Update()
    {
        if (isDead) return;

        DetectPlayer();

        if (isTargetDetected && target != null && !isDefending && !isAttacking)
        {
            FollowPlayer();
        }

        //  Actualizar animación de caminar
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    private void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        isTargetDetected = distance <= activationRange;
    }

    private void FollowPlayer()
    {
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

                if (distance <= attackRange && !isDefending)
                {
                    isAttacking = true;
                    isDefending = false;
                    agent.isStopped = true;

                    // 🔹 Alternar entre las animaciones de ataque
                    lastAttackIndex = (lastAttackIndex == 0) ? 1 : 0;
                    animator.SetInteger("attackIndex", lastAttackIndex);
                    animator.SetTrigger("isAttacking");

                    Debug.Log($"{gameObject.name}: Atacando");
                    combatSystem.Attack(target.GetComponent<Health>());

                    yield return new WaitForSeconds(attackInterval);
                    isAttacking = false;
                }

                if (!isAttacking)
                {
                    StartCoroutine(Defend());
                }
            }

            yield return null;
        }
    }

    private IEnumerator Defend()
    {
        isDefending = true;
        agent.isStopped = true;
        combatSystem.StartDefense();
        healthComponent.SetInvulnerable(true);

        // 🔹 Activar animación de defensa
        animator.SetBool("isDefending", true);

        Debug.Log($"{gameObject.name}: Defendiéndose");
        yield return new WaitForSeconds(defenseDuration);

        isDefending = false;
        agent.isStopped = false;
        healthComponent.SetInvulnerable(false);

        // 🔹 Desactivar animación de defensa
        animator.SetBool("isDefending", false);

        Debug.Log($"{gameObject.name}: Terminó la defensa");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // 🔹 Detener el NavMeshAgent completamente
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 🔹 Detener todas las corrutinas (para evitar que sigan ataques o defensas)
        StopAllCoroutines();

        // 🔹 Asegurar que no se ejecuten otras animaciones
        animator.SetBool("isWalking", false);
        animator.SetBool("isDefending", false);
        animator.SetInteger("attackIndex", 0);

        // 🔹 Asegurar que CombatSystem detiene el ataque y la defensa
        combatSystem.StopCombatActions();

        // 🔹 Activar animación de muerte
        animator.SetBool("isDead", true);

        Debug.Log($"{gameObject.name}: Ha muerto. Esperando animación...");

        // 🔹 Esperar a que termine la animación antes de destruir
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return null; // 🔹 Esperar un frame para que el Animator actualice su estado

        // 🔹 Esperar a que la animación realmente inicie
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            yield return null;
        }

        // 🔹 Obtener la duración de la animación de muerte
        float deathAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log($"{gameObject.name}: Animación de muerte dura {deathAnimLength} segundos.");

        yield return new WaitForSeconds(deathAnimLength);

        // 🔹 Destruir el enemigo después de la animación
        Destroy(gameObject);
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

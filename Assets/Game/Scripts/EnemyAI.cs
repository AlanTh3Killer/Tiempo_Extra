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
    [SerializeField] private Animator animator; // Ahora podemos asignarlo manualmente en el inspector

    // [Header("Sonidos")] <-- Agrega esto si quieres organizar el Inspector
    public AudioClip[] enemyAttackSounds; // Sonidos específicos para el enemigo

    private bool isTargetDetected = false;
    private bool isDefending = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private int lastAttackIndex = 0; // Para alternar entre animaciones de ataque
    private Health healthComponent;

    private void Start()
    {
        // Si quieres usar los mismos sonidos del jugador, puedes asignarlos automáticamente:
        if (SoundManager.instance != null && enemyAttackSounds.Length == 0)
        {
            enemyAttackSounds = SoundManager.instance.attackSounds;
        }

        agent = GetComponent<NavMeshAgent>();
        combatSystem = GetComponent<CombatSystem>();
        healthComponent = GetComponent<Health>();

        //  Si no está asignado en el inspector, lo buscamos en los hijos
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
            if (isTargetDetected && !FreezzeGame.IsDialogueActive)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange && !isDefending)
                {
                    // Reproducir sonido ANTES del ataque
                    if (enemyAttackSounds.Length > 0 && SoundManager.instance != null)
                    {
                        SoundManager.instance.PlayRandomSFX(enemyAttackSounds, 0.7f);
                        yield return new WaitForSeconds(0.1f); // Pequeño delay para sincronización
                    }

                    isAttacking = true;
                    isDefending = false;
                    agent.isStopped = true;

                    //  Alternar entre las animaciones de ataque
                    lastAttackIndex = (lastAttackIndex == 0) ? 1 : 0;
                    animator.SetInteger("attackIndex", lastAttackIndex);
                    // Antes de atacar:
                    animator.SetTrigger("isAttacking");
                    yield return new WaitForSeconds(0.3f); // Pre-hit delay

                    // Ejecutar ataque real
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

        // Activar animación de defensa
        animator.SetBool("isDefending", true);

        Debug.Log($"{gameObject.name}: Defendiéndose");
        yield return new WaitForSeconds(defenseDuration);

        isDefending = false;
        agent.isStopped = false;
        healthComponent.SetInvulnerable(false);

        //  Desactivar animación de defensa
        animator.SetBool("isDefending", false);

        Debug.Log($"{gameObject.name}: Terminó la defensa");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Detener componentes
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        StopAllCoroutines();

        // Animación de muerte
        animator.SetBool("isDead", true);
        animator.Play("Death", 0, 0f);

        // Obtener referencia al Health y notificar
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.NotifyLevelManager(); // Llamada CORRECTA al método en Health
        }
        else
        {
            Debug.LogError("Componente Health no encontrado en el enemigo");
        }

        // Destrucción después de la animación
        StartCoroutine(WaitForDeathAnimation());
    }

    [Header("Configuración de Muerte")]
    [Tooltip("Tiempo adicional después de la animación de muerte")]
    [SerializeField] private float deathAnimationExtraTime = 0.8f; // Ajusta este valor en el Inspector

    private IEnumerator WaitForDeathAnimation()
    {
        // Espera el frame actual para asegurar que la animación comenzó
        yield return null;

        // Obtiene el estado actual de la animación
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Calcula el tiempo restante de la animación
        float remainingAnimTime = stateInfo.length * (1f - stateInfo.normalizedTime);

        // Espera la animación + tiempo extra
        yield return new WaitForSeconds(remainingAnimTime + deathAnimationExtraTime);

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

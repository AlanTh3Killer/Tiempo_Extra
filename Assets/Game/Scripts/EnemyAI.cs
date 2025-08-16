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
    [SerializeField] private float rotationSpeed = 10f; 

    [Header("Combate")]
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private float attackInterval = 2.0f;
    [SerializeField] private float defenseDuration = 1.5f;

    [Header("Animaciones")]
    [SerializeField] private Animator animator;

    public AudioClip[] enemyAttackSounds;

    private bool isTargetDetected = false;
    private bool isDefending = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private int lastAttackIndex = 0;
    private Health healthComponent;

    private Collider enemyCollider;
    private Collider playerCollider;

    private void Start()
    {
        enemyCollider = GetComponent<Collider>();
        playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();

        //VersionVieja
        ////Evita empujones y colisiones físicas sin romper daño
        //Physics.IgnoreCollision(enemyCollider, playerCollider, true);

        if (SoundManager.instance != null && enemyAttackSounds.Length == 0)
        {
            enemyAttackSounds = SoundManager.instance.attackSounds;
        }

        agent = GetComponent<NavMeshAgent>();
        combatSystem = GetComponent<CombatSystem>();
        healthComponent = GetComponent<Health>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        agent.stoppingDistance = Mathf.Max(stoppingDistance, 1.5f);
        //Version Vieja
        //agent.stoppingDistance = stoppingDistance;
        StartCoroutine(CombatLoop());
    }

    private void Update()
    {
        if (isDead) return;

        DetectPlayer();

        if (isTargetDetected && target != null && !isDefending && !isAttacking)
        {
            //Antigua logica, solo esto
            //FollowPlayer();

            // Rotar hacia el jugador
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // evitar inclinación vertical
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }

            if (!isDefending && !isAttacking)
            {
                FollowPlayer();
            }
        }

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
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
            //Version Vieja
            //agent.SetDestination(target.position);

            // Dirección hacia el jugador
            Vector3 dirToPlayer = (target.position - transform.position).normalized;

            // Distancia mínima de seguridad (configurable por inspector)
            float safeDistance = Mathf.Max(attackRange * 0.8f, 1.0f);

            // Punto objetivo justo antes de entrar en rango de ataque
            Vector3 stopPosition = target.position - dirToPlayer * safeDistance;

            // Mover agente al punto calculado
            agent.SetDestination(stopPosition);
            //agent.SetDestination(targetPosition);
        }
    }

    private IEnumerator CombatLoop()
    {
        while (true)
        {
            if (isTargetDetected && !FreezzeGame.IsFrozen)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange && !isDefending)
                {
                    Vector3 attackDir = (target.position - transform.position).normalized;
                    attackDir.y = 0;
                    if (attackDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(attackDir);
                    }

                    //Physics.IgnoreCollision(enemyCollider, playerCollider, true);
                    isAttacking = true;

                    lastAttackIndex = (lastAttackIndex == 0) ? 1 : 0;
                    animator.SetInteger("attackIndex", lastAttackIndex);
                    animator.SetTrigger("isAttacking");

                    yield return new WaitForSeconds(attackInterval);
                    //Physics.IgnoreCollision(enemyCollider, playerCollider, false);
                    isAttacking = false;
                }

                // Dentro de CombatLoop()
                if (!isAttacking && !isDefending)
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

        animator.SetBool("isDefending", true);
        Debug.Log($"{gameObject.name}: Defendiéndose");

        yield return new WaitForSeconds(defenseDuration);

        combatSystem.EndDefense();                    
        healthComponent.SetInvulnerable(false);

        isDefending = false;
        agent.isStopped = false;

        animator.SetBool("isDefending", false);
        Debug.Log($"{gameObject.name}: Terminó la defensa");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        StopAllCoroutines();

        animator.SetBool("isDead", true);
        animator.Play("Death", 0, 0f);

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.NotifyLevelManager();
        }

        StartCoroutine(WaitForDeathAnimation());
    }

    // Método para el sonido de anticipación (windup)
    public void PlayAttackWindupSound()
    {
        if (!isDead && SoundManager.instance != null && enemyAttackSounds.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(enemyAttackSounds, 0.7f);
        }
    }

    // Método para el sonido de impacto
    public void PlayAttackImpactSound()
    {
        if (!isDead && SoundManager.instance != null && enemyAttackSounds.Length > 0)
        {
            SoundManager.instance.PlayRandomSFX(enemyAttackSounds, 1f); // Volumen más alto para el impacto
        }
    }

    public void ExecuteDamage()
    {
        if (target != null && !isDead)
        {
            // Verificación adicional de rango
            float currentDistance = Vector3.Distance(transform.position, target.position);
            if (currentDistance > attackRange * 1.2f) return;

            // 2. Comprobar si está en el campo de visión
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle > 45f) // 45 grados hacia cada lado
            {
                Debug.Log("Jugador fuera del ángulo de ataque");
                return;
            }

            // 3. Raycast para comprobar obstrucciones
            if (Physics.Raycast(transform.position + Vector3.up, dirToTarget, out RaycastHit hit, attackRange * 1.2f))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Ataque bloqueado por un obstáculo");
                    return;
                }
            }

            // 4) Llamar SIEMPRE a Attack(); Health decidirá sangre vs bloqueo
            var targetHealth = target.GetComponent<Health>();
            if (targetHealth != null && combatSystem.IsInAttackWindow())
            {
                Debug.Log($"[{name}] ExecuteDamage -> Attack() (Player invuln={targetHealth.IsInvulnerable()})");
                combatSystem.Attack(targetHealth);
            }

            //Posiblemente sirva conservar
            //// 4. Aplicar daño si todo está bien
            //var targetHealth = target.GetComponent<Health>();
            //if (targetHealth != null && !targetHealth.IsInvulnerable() && combatSystem.IsInAttackWindow())
            //{
            //    combatSystem.Attack(targetHealth);
            //    Debug.Log("¡Golpe conectado!");
            //}

            //Version vieja, NO IGNORAR
            //if (currentDistance <= attackRange * 1.2f) // 20% de margen adicional
            //{
            //    var targetHealth = target.GetComponent<Health>();
            //    if (targetHealth != null && !targetHealth.IsInvulnerable())
            //    {
            //        if (combatSystem.IsInAttackWindow())
            //        {
            //            combatSystem.Attack(targetHealth);
            //            Debug.Log("¡Golpe conectado dentro de ventana!");
            //        }
            //        else
            //        {
            //            Debug.Log("Estás fuera de la ventana de ataque.");
            //        }
            //    }
            //    //combatSystem.Attack(target.GetComponent<Health>());
            //    //Debug.Log("¡Golpe conectado!");
            //}

            //Ignorar
            //else
            //{
            //    Debug.Log("Golpe fallado: jugador fuera de rango");
            //}
        }
    }

    public bool IsTargetInAttackRange()
    {
        if (target == null) return false;

        float currentDistance = Vector3.Distance(transform.position, target.position);
        return currentDistance <= attackRange * 1.2f; // Con margen de seguridad
    }

    [Header("Configuración de Muerte")]
    [SerializeField] private float deathAnimationExtraTime = 0.8f;

    private IEnumerator WaitForDeathAnimation()
    {
        yield return null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float remainingAnimTime = stateInfo.length * (1f - stateInfo.normalizedTime);
        yield return new WaitForSeconds(remainingAnimTime + deathAnimationExtraTime);
        Destroy(gameObject);
    }

    // Versión alternativa que Unity detecta mejor
    public void AnimationEvent(string eventName)
    {
        if (eventName == "ExecuteDamage") ExecuteDamage();
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Rango de ataque normal
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Rango con margen (solo en editor)
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // Naranja semitransparente
        Gizmos.DrawWireSphere(transform.position, attackRange * 1.2f);

    }
}

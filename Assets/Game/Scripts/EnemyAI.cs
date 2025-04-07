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
    [SerializeField] private Animator animator;

    public AudioClip[] enemyAttackSounds;

    private bool isTargetDetected = false;
    private bool isDefending = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private int lastAttackIndex = 0;
    private Health healthComponent;

    private void Start()
    {
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
            agent.SetDestination(target.position);
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
                    if (enemyAttackSounds.Length > 0 && SoundManager.instance != null)
                    {
                        SoundManager.instance.PlayRandomSFX(enemyAttackSounds, 0.7f);
                        yield return new WaitForSeconds(0.1f);
                    }

                    isAttacking = true;
                    agent.isStopped = true;

                    lastAttackIndex = (lastAttackIndex == 0) ? 1 : 0;
                    animator.SetInteger("attackIndex", lastAttackIndex);
                    animator.SetTrigger("isAttacking");

                    yield return new WaitForSeconds(0.3f);
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

        animator.SetBool("isDefending", true);

        Debug.Log($"{gameObject.name}: Defendiéndose");
        yield return new WaitForSeconds(defenseDuration);

        isDefending = false;
        agent.isStopped = false;
        healthComponent.SetInvulnerable(false);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6.0f;
    public float gravity = 9.81f;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    [SerializeField] private Animator animator;

    [Header("Combate")]
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    public float comboTime = 1f;
    private int attackIndex = 0;
    private bool canAttack = true;
    private float lastAttackTime;
    private bool isAttacking = false;

    private Health healthComponent;

    [Header("Defensa")]
    private bool isDefending = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        healthComponent = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("El jugador no tiene un Animator asignado.");
        }

        PlayerInput.OnMove += Move;
        PlayerInput.OnAttack += Attack;
        PlayerInput.OnDefend += ToggleDefense;
    }

    void Update()
    {
        // --- Único cambio necesario ---
        if (FreezzeGame.IsFrozen || FreezzeGame.IsFrozen)
        {
            // Congelar completamente al personaje
            moveDirection = Vector3.zero;
            animator.SetBool("Player_isWalking", false);
            return;
        }

        // --- El resto del Update original se mantiene ---
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    void Move(Vector2 movement)
    {
        // 1. Verificar estados que bloquean movimiento
        if (isAttacking || isDefending || FreezzeGame.IsFrozen)
        {
            moveDirection = Vector3.zero;
            animator.SetBool("Player_isWalking", false); // Fuerza idle al pausar
            return;
        }

        // 2. Lógica original de movimiento (que ya funcionaba)
        moveDirection = new Vector3(movement.x, 0, movement.y);
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;

        if (!isMoving)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            transform.forward = moveDirection.normalized;
        }

        // 3. Aplicar gravedad solo si el juego no está pausado
        if (!FreezzeGame.IsFrozen && !controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 4. Mover solo si no está pausado
        if (!FreezzeGame.IsFrozen)
        {
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        // 5. Control de animación mejorado
        bool isActuallyMoving = !FreezzeGame.IsFrozen &&
                             moveDirection.sqrMagnitude > 0.05f;

        animator.SetBool("Player_isWalking", isActuallyMoving);

        Debug.Log($"Mov: {isMoving} | Real: {isActuallyMoving} | Vel: {controller.velocity.magnitude} | Pausa: {FreezzeGame.IsFrozen}");
    }

    void Attack()
    {
        // Añadir verificación de diálogos
        if (!canAttack || isDefending || FreezzeGame.IsFrozen || FreezzeGame.IsFrozen)
            return;

        // --- El resto del método Attack original se mantiene ---
        if (Time.time - lastAttackTime <= comboTime)
        {
            attackIndex = (attackIndex == 0) ? 1 : 0;
        }
        else
        {
            attackIndex = 0; // Si pasa mucho tiempo, reinicia el combo
        }

        animator.SetInteger("Player_AttackIndex", attackIndex);
        animator.SetTrigger("Player_Attack");
        // Reproducir sonido de ataque desde el SoundManager
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayRandomSFX(SoundManager.instance.attackSounds, 0.8f);
        }

        PerformAttack();
        StartCoroutine(AttackCooldown());

        lastAttackTime = Time.time;
        // Reemplázala con:
        StartCoroutine(ResetAttackState());

        //  Volver a Idle después del ataque
        //StartCoroutine(ReturnToIdleAfterAttack());
    }

    //  Esta corrutina esperará a que termine el ataque y regresará a Idle
    //IEnumerator ReturnToIdleAfterAttack()
    //{
    //    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Espera la duración exacta de la animación
    //    animator.SetTrigger("Player_Idle");
    //}

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f); // Duración aproximada del ataque
        isAttacking = false;
        animator.SetInteger("Player_AttackIndex", -1); // Reset opcional
    }

    void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1.5f);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                CombatSystem enemyCombat = enemy.GetComponent<CombatSystem>();

                if (enemyCombat != null && !enemyCombat.IsDefending())
                {
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Damage(attackDamage);
                    }
                }
            }
        }
    }

    void ToggleDefense(bool defending)
    {
        isDefending = defending;
        healthComponent.SetInvulnerable(isDefending);
        animator.SetBool("Player_isWalking", false);
        animator.SetBool("Player_isDefending", isDefending);

        if (isDefending)
        {
            animator.SetTrigger("Player_StartDefend");
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDestroy()
    {
        PlayerInput.OnMove -= Move;
        PlayerInput.OnAttack -= Attack;
        PlayerInput.OnDefend -= ToggleDefense;
    }
}

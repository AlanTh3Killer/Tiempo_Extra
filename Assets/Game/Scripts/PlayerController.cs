using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

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

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Partículas")]
    [SerializeField] private ParticleSystem[] particles;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        healthComponent = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("El jugador no tiene un Animator asignado.");
        }

        foreach (var ps in particles)
        {
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        PlayerInput.OnMove += Move;
        PlayerInput.OnAttack += Attack;
        PlayerInput.OnDefend += ToggleDefense;
        PlayerInput.OnDash += Dash;

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
        if (isDashing || isAttacking || isDefending || FreezzeGame.IsFrozen)
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
        //  Bloquear completamente si no puede atacar
        if (!canAttack || isDefending || FreezzeGame.IsFrozen)
            return;

        // Solo modificar combo si se va a lanzar el ataque
        if (Time.time - lastAttackTime <= comboTime)
        {
            attackIndex = (attackIndex == 0) ? 1 : 0;
        }
        else
        {
            attackIndex = 0; // Reinicia combo si pasó demasiado tiempo
        }

        animator.SetInteger("Player_AttackIndex", attackIndex);
        animator.SetTrigger("Player_Attack");

        canAttack = false;
        isAttacking = true;

        animator.SetBool("Player_isInAttackCooldown", true); // Opcional visual

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayRandomSFX(SoundManager.instance.attackSounds, 0.8f);
        }

        PerformAttack();
        StartCoroutine(AttackCooldown());
        StartCoroutine(ResetAttackState());

        lastAttackTime = Time.time;
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

                Health enemyHealth = enemy.GetComponent<Health>();

                if (enemyHealth != null)
                {
                    Debug.Log($"[{gameObject.name}] intentando dañar a {enemyHealth.gameObject.name}");

                    // Igual que en el enemigo, llamamos SIEMPRE
                    enemyHealth.Damage(attackDamage);
                }
                //Logica vieja
                //if (enemyCombat != null && !enemyCombat.IsDefending())
                //{
                //    Health enemyHealth = enemy.GetComponent<Health>();
                //    if (enemyHealth != null)
                //    {
                //        enemyHealth.Damage(attackDamage);
                //        Debug.Log($"[{gameObject.name}] intentando dañar a {enemyHealth.gameObject.name}");


                //    }
                //}
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
        PlayerInput.OnDash -= Dash;
    }

    public void Dash()
    {
        // Verificar si puede hacer dash desde condiciones de gameplay
        if (!canDash || isDashing || FreezzeGame.IsFrozen || isAttacking || isDefending)
            return;

        // Nueva lógica: solo permitir el dash si está en estado de caminar
        bool isWalking = animator.GetBool("Player_isWalking");
        if (!isWalking)
        {
            Debug.Log("No puedes esquivar si no te estás moviendo.");
            return;
        }

        animator.SetTrigger("Player_isDodging");

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        // Guardar dirección actual
        Vector3 dashDirection = moveDirection.normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward; // Por si está quieto
        }

        // Invulnerabilidad
        healthComponent.SetInvulnerable(true);

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            controller.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fin de invulnerabilidad
        healthComponent.SetInvulnerable(false);

        isDashing = false;

        // Cooldown para el siguiente dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void PlayParticleByIndex(int index)
    {
        if (index >= 0 && index < particles.Length && particles[index] != null)
        {
            particles[index].Play();
        }
        else
        {
            Debug.LogWarning($"[PlayerController] No se pudo reproducir la partícula en índice {index}");
        }
    }
}

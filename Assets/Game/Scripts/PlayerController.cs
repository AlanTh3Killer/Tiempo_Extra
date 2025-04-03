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

    void Move(Vector2 movement)
    {
        if (isAttacking || isDefending) return; // No moverse mientras ataca o se defiende

        moveDirection = new Vector3(movement.x, 0, movement.y);
        bool isMoving = moveDirection.sqrMagnitude > 0.01f; // Verifica si hay movimiento en la entrada

        if (!isMoving)
        {
            moveDirection = Vector3.zero; //  Forzar a 0 cuando no haya movimiento
        }
        else
        {
            transform.forward = moveDirection.normalized; // Rotar al moverse
        }

        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * speed * Time.deltaTime);

        //  Corrección: Usa la distancia recorrida en vez de velocity
        float movementThreshold = 0.05f; // Pequeño margen de error
        bool isActuallyMoving = moveDirection.sqrMagnitude > movementThreshold;

        animator.SetBool("Player_isWalking", isActuallyMoving);

        Debug.Log($"Moviendo: {isMoving} | Realmente Moviendo: {isActuallyMoving} | Velocidad: {controller.velocity.magnitude}");
    }

    void Attack()
    {
        if (!canAttack || isDefending) return;

        // Si el segundo ataque ocurre dentro del tiempo del combo, cambia el ataque
        if (Time.time - lastAttackTime <= comboTime)
        {
            attackIndex = (attackIndex == 0) ? 1 : 0;  // Alterna entre 0 y 1
        }
        else
        {
            attackIndex = 0; // Si pasa mucho tiempo, reinicia el combo
        }

        animator.SetInteger("Player_AttackIndex", attackIndex);
        animator.SetTrigger("Player_Attack");

        PerformAttack();
        StartCoroutine(AttackCooldown());

        lastAttackTime = Time.time;

        //  Volver a Idle después del ataque
        StartCoroutine(ReturnToIdleAfterAttack());
    }

    //  Esta corrutina esperará a que termine el ataque y regresará a Idle
    IEnumerator ReturnToIdleAfterAttack()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Espera la duración exacta de la animación
        animator.SetTrigger("Player_Idle");
    }

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f); // Ajusta esto al tiempo de la animación de ataque
        isAttacking = false;
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

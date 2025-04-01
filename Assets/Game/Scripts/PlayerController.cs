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

    [Header("Combate")]
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    public float attackRange = 1.5f; // Rango del ataque
    public float attackAngle = 60f; // Ángulo del ataque
    private bool canAttack = true;
    private Health healthComponent;

    [Header("Defensa")]
    public bool isDefending = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        healthComponent = GetComponent<Health>();

        PlayerInput.OnMove += Move;
        PlayerInput.OnAttack += Attack;
        PlayerInput.OnDefend += ToggleDefense;
    }

    void Move(Vector2 movement)
    {
        moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection.normalized;
        }

        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    void Attack()
    {
        if (!canAttack || isDefending) return;

        Debug.Log("¡Jugador está atacando!");
        PerformAttack();
        StartCoroutine(AttackCooldown());
    }

    void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                CombatSystem enemyCombat = enemy.GetComponent<CombatSystem>();

                if (enemyCombat != null && !enemyCombat.IsDefending())  //  NO ATACAR SI SE ESTÁ DEFENDIENDO
                {
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Damage(attackDamage);
                        Debug.Log("¡Golpeaste al enemigo!");
                    }
                }
                else
                {
                    Debug.Log("El enemigo bloqueó tu ataque.");
                }
            }
        }
    }

    void ToggleDefense(bool defending)
    {
        isDefending = defending;
        healthComponent.SetInvulnerable(isDefending);
        Debug.Log(isDefending ? "Defendiendo" : "Bajando defensa");
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDestroy()
    {
        PlayerInput.OnMove -= Move;
        PlayerInput.OnAttack -= Attack;
        PlayerInput.OnDefend -= ToggleDefense;
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el área de ataque en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibuja el cono del ataque
        Vector3 forward = transform.forward * attackRange;
        Vector3 leftLimit = Quaternion.Euler(0, -attackAngle / 2, 0) * forward;
        Vector3 rightLimit = Quaternion.Euler(0, attackAngle / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftLimit);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit);
    }
}

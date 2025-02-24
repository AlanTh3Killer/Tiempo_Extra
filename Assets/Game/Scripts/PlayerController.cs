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
        StartCoroutine(AttackCooldown());
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
}

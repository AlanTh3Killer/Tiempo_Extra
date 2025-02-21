using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = 9.81f; // Para que el CharacterController se mantenga en el suelo
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        PlayerInput.OnMove += Move;
        PlayerInput.OnAttack += Attack;
    }

    void Move(Vector2 movement)
    {
        moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection.normalized;
        }

        // Aplicar gravedad para mantenerlo en el suelo
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    void Attack()
    {
        Debug.Log("¡Jugador está atacando!");
    }

    private void OnDestroy()
    {
        PlayerInput.OnMove -= Move;
        PlayerInput.OnAttack -= Attack;
    }
}

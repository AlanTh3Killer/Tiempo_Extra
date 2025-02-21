using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = 9.81f; // Simulación de gravedad
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Obtener entrada de movimiento
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Crear vector de movimiento en X y Z
        moveDirection = new Vector3(moveX, 0, moveZ);

        // Si hay movimiento, rotar hacia la dirección
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection.normalized;
        }

        // Aplicar gravedad
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Aplicar movimiento
        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}

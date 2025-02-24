using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Combate")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float defenseDuration = 1f;

    private bool isAttacking = false;
    private bool isDefending = false;
    private Health healthComponent;

    private void Start()
    {
        healthComponent = GetComponent<Health>();
    }

    public void Attack(Health targetHealth)
    {
        if (!isAttacking && targetHealth != null)
        {
            Debug.Log($"{gameObject.name}: Iniciando ataque...");
            StartCoroutine(PerformAttack(targetHealth));
        }
    }

    private IEnumerator PerformAttack(Health targetHealth)
    {
        isAttacking = true;
        Debug.Log($"{gameObject.name}: Golpeó al objetivo e hizo {attackDamage} de daño!");
        targetHealth.Damage(attackDamage);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        Debug.Log($"{gameObject.name}: Puede atacar de nuevo.");
    }

    public void StartDefense()
    {
        if (!isDefending)
        {
            Debug.Log($"{gameObject.name}: Activando defensa!");
            StartCoroutine(PerformDefense());
        }
    }

    private IEnumerator PerformDefense()
    {
        isDefending = true;
        healthComponent.SetInvulnerable(true);
        Debug.Log($"{gameObject.name}: ¡Ahora es invulnerable!");
        yield return new WaitForSeconds(defenseDuration);
        healthComponent.SetInvulnerable(false);
        isDefending = false;
        Debug.Log($"{gameObject.name}: Terminó la defensa, ahora puede recibir daño.");
    }
}

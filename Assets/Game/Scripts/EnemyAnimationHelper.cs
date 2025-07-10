using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHelper : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private CombatSystem combatSystem;

    private void Awake()
    {
        // Busca automáticamente las referencias si no están asignadas
        if (enemyAI == null) enemyAI = GetComponentInParent<EnemyAI>();
        if (combatSystem == null) combatSystem = GetComponentInParent<CombatSystem>();
    }

    // Métodos para Animation Events (aparecerán en el dropdown de Unity)
    public void PlayAttackWindupSound() => enemyAI?.PlayAttackWindupSound();
    public void PlayAttackImpactSound() => enemyAI?.PlayAttackImpactSound();
    public void ExecuteDamage() => enemyAI?.ExecuteDamage();

    public void OpenAttackWindow() => combatSystem?.OpenAttackWindow();
    public void CloseAttackWindow() => combatSystem?.CloseAttackWindow();

    public void OpenDefenseWindow() => combatSystem?.OpenDefenseWindow();
    public void CloseDefenseWindow() => combatSystem?.CloseDefenseWindow();

}

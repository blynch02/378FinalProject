using UnityEngine;
using System; // Required for Action
using System.Collections; // Required for Coroutines

public class PlayerCharacter : MonoBehaviour, ICombatant
{
    public string characterName = "Hero";
    public int maxHealth = 100;
    private int _currentHealth;
    public int attackPower = 10; // Simple attack power

    public int CurrentHealth => _currentHealth;
    public bool IsDefeated => _currentHealth <= 0;

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    public string GetDisplayName()
    {
        return characterName;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void StartMyTurn(Action onTurnComplete)
    {
        Debug.LogFormat("{0}'s turn starts.", characterName);
        // In a real game, this is where you'd enable player input,
        // wait for them to select an action and target, then execute.
        // For now, we'll simulate a simple action.

        StartCoroutine(PerformAction(onTurnComplete));
    }

    private IEnumerator PerformAction(Action onTurnComplete)
    {
        // Simulate thinking/action time
        Debug.LogFormat("{0} is performing an action...", characterName);
        yield return new WaitForSeconds(1.0f); // Simulate animation/action duration

        // Example: Find an enemy and attack if one exists (very basic)
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>(); // Find all enemies
        EnemyAI targetEnemy = null;
        foreach(EnemyAI enemy in enemies)
        {
            if (enemy != null && !enemy.IsDefeated)
            {
                targetEnemy = enemy;
                break;
            }
        }

        if (targetEnemy != null)
        {
            Debug.LogFormat("{0} attacks {1} for {2} damage!", characterName, targetEnemy.GetDisplayName(), attackPower);
            targetEnemy.TakeDamage(attackPower);
        }
        else
        {
            Debug.LogFormat("{0} has no enemies to attack.", characterName);
        }

        Debug.LogFormat("{0}'s turn ends.", characterName);
        onTurnComplete?.Invoke(); // Signal that the turn is complete
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        Debug.LogFormat("{0} took {1} damage. Current Health: {2}", characterName, amount, _currentHealth);
        if (IsDefeated)
        {
            Debug.LogFormat("{0} has been defeated!", characterName);
            // Handle death (e.g., play animation, disable GameObject)
            // For now, just log it. We might want to disable the GameObject or change its appearance.
            // gameObject.SetActive(false); // Simple way to remove from combat
        }
    }
} 
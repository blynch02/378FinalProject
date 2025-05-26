using UnityEngine;
using System; // Required for Action
using System.Collections; // Required for Coroutines
using System.Collections.Generic; // Required for List
using System.Linq; // Required for Linq operations like Where

public class EnemyAI : MonoBehaviour, ICombatant
{
    public string enemyName = "Goblin";
    public int maxHealth = 50;
    private int _currentHealth;
    public int attackPower = 8;

    public int CurrentHealth => _currentHealth;
    public bool IsDefeated => _currentHealth <= 0;

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    public string GetDisplayName()
    {
        return enemyName;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void StartMyTurn(Action onTurnComplete)
    {
        Debug.LogFormat("--- {0}'s turn starts. ---", enemyName);
        StartCoroutine(PerformAction(onTurnComplete));
    }

    private IEnumerator PerformAction(Action onTurnComplete)
    {
        // Simulate AI "thinking" time
        yield return new WaitForSeconds(0.5f);

        // Basic AI: Find a random, non-defeated player character to attack
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
        List<PlayerCharacter> alivePlayers = players.Where(p => p != null && !p.IsDefeated).ToList();

        if (alivePlayers.Count > 0)
        {
            PlayerCharacter target = alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Count)];
            Debug.LogFormat("{0} attacks {1} for {2} damage!", enemyName, target.GetDisplayName(), attackPower);
            target.TakeDamage(attackPower);
        }
        else
        {
            Debug.LogFormat("{0} has no players to attack.", enemyName);
        }

        // Simulate action/animation duration
        yield return new WaitForSeconds(1.0f);

        Debug.LogFormat("--- {0}'s turn ends. ---", enemyName);
        onTurnComplete?.Invoke(); // Signal that the turn is complete
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        Debug.LogFormat("{0} took {1} damage. Current Health: {2}", enemyName, amount, _currentHealth);
        if (IsDefeated)
        {
            Debug.LogFormat("{0} has been defeated!", enemyName);
            // Handle death
            // gameObject.SetActive(false); // Simple way
        }
    }
} 
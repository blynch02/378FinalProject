using UnityEngine;
using System; // Required for Action

public interface ICombatant
{
    string GetDisplayName();
    GameObject GetGameObject(); // To get a reference to the combatant's GameObject
    bool IsDefeated { get; }
    void StartMyTurn(Action onTurnComplete); // The Action is a callback to signal turn completion
    void TakeDamage(int amount);
    int CurrentHealth { get; } // Useful for AI targeting or UI
}
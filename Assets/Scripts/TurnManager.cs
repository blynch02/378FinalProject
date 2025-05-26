using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // For ToList() and OrderBy (optional for now)
using UnityEngine.UI; // If you plan to update UI text for turns

public class TurnManager : MonoBehaviour
{
    public List<PlayerCharacter> playerCharacters; // Assign in Inspector
    public List<EnemyAI> enemyCharacters;        // Assign in Inspector

    private List<ICombatant> combatantsInTurnOrder;
    private int currentCombatantIndex = 0;
    private bool isCombatActive = false;
    private bool isTurnInProgress = false;

    public float delayBetweenTurns = 2.0f;

    // Optional: UI element to display whose turn it is
    public Text turnIndicatorText; // Assign in Inspector if you have one

    void Start()
    {
        InitializeCombat();
    }

    void InitializeCombat()
    {
        combatantsInTurnOrder = new List<ICombatant>();

        // Add players
        if (playerCharacters != null)
        {
            foreach (var player in playerCharacters)
            {
                if (player != null && player.gameObject.activeInHierarchy && !player.IsDefeated)
                {
                    combatantsInTurnOrder.Add(player);
                }
            }
        }

        // Add enemies
        if (enemyCharacters != null)
        {
            foreach (var enemy in enemyCharacters)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDefeated)
                {
                    combatantsInTurnOrder.Add(enemy);
                }
            }
        }
        
        // Simple turn order: players first, then enemies.
        // Later you might sort by a 'speed' stat or other criteria.
        // combatantsInTurnOrder = combatantsInTurnOrder.OrderByDescending(c => c.GetSpeedStat()).ToList(); // Example

        if (combatantsInTurnOrder.Count > 0)
        {
            isCombatActive = true;
            currentCombatantIndex = 0; // Start with the first combatant
            Debug.Log("Combat Initialized. Starting first turn.");
            StartCoroutine(BattleLoop());
        }
        else
        {
            Debug.LogWarning("No combatants found to initialize combat.");
            UpdateTurnIndicator("No combatants.");
        }
    }

    private IEnumerator BattleLoop()
    {
        while (isCombatActive)
        {
            if (combatantsInTurnOrder.Count == 0)
            {
                Debug.Log("No combatants left. Ending battle loop.");
                isCombatActive = false;
                break;
            }

            // Skip defeated combatants or remove them
            // For simplicity, we'll just check IsDefeated. A more robust system might remove them.
            ICombatant currentCombatant = combatantsInTurnOrder[currentCombatantIndex];
            
            if (currentCombatant == null || currentCombatant.IsDefeated || !currentCombatant.GetGameObject().activeInHierarchy)
            {
                Debug.LogFormat("Skipping defeated or inactive combatant: {0}", currentCombatant?.GetDisplayName() ?? "Unknown/Destroyed");
                ProceedToNextTurn();
                // Add a small delay to prevent tight loops if all remaining are defeated.
                if (CheckForAllDefeated(playerCharacters) || CheckForAllDefeated(enemyCharacters)) {
                    // Handled by victory/defeat checks below
                } else {
                     yield return null; // Wait a frame before re-evaluating the loop
                }
                continue; // Re-evaluate the loop with the new index
            }

            isTurnInProgress = true; // Mark that a turn is officially starting
            UpdateTurnIndicator("Turn: " + currentCombatant.GetDisplayName());
            Debug.LogFormat(">>> Starting turn for: {0}", currentCombatant.GetDisplayName());

            // Tell the combatant to take its turn and wait for it to complete
            // The combatant's StartMyTurn method MUST call the onTurnComplete callback.
            currentCombatant.StartMyTurn(() => {
                isTurnInProgress = false; // Turn action is now complete
                Debug.LogFormat("<<< {0} officially ended their turn action.", currentCombatant.GetDisplayName());
            });

            // Wait until the current combatant signals their turn is done
            yield return new WaitUntil(() => !isTurnInProgress || currentCombatant.IsDefeated);
            
            Debug.LogFormat("--- Post-action for {0}. Checking game state. ---", currentCombatant.GetDisplayName());

            // Check for victory/defeat conditions
            if (CheckForPlayerVictory())
            {
                Debug.Log("PLAYER VICTORY! All enemies defeated.");
                UpdateTurnIndicator("PLAYER VICTORY!");
                isCombatActive = false;
                // You could trigger a victory screen or sequence here
                yield break; // Exit the BattleLoop
            }

            if (CheckForEnemyVictory())
            {
                Debug.Log("ENEMY VICTORY! All players defeated.");
                UpdateTurnIndicator("ENEMY VICTORY!");
                isCombatActive = false;
                // You could trigger a game over screen or sequence here
                yield break; // Exit the BattleLoop
            }
            
            // If combat is still active, proceed to the next turn after a delay
            if(isCombatActive)
            {
                Debug.LogFormat("Waiting for {0} seconds before next turn...", delayBetweenTurns);
                yield return new WaitForSeconds(delayBetweenTurns);
                ProceedToNextTurn();
            }
        }
        Debug.Log("Battle Loop has ended.");
    }

    private void ProceedToNextTurn()
    {
        if (combatantsInTurnOrder.Count == 0) return;

        currentCombatantIndex = (currentCombatantIndex + 1) % combatantsInTurnOrder.Count;
        Debug.LogFormat("Proceeding to next turn. New index: {0}", currentCombatantIndex);
    }

    private bool CheckForPlayerVictory()
    {
        // Player wins if all enemies are defeated
        return enemyCharacters.All(e => e == null || e.IsDefeated || !e.gameObject.activeInHierarchy);
    }

    private bool CheckForEnemyVictory()
    {
        // Enemy wins if all players are defeated
        return playerCharacters.All(p => p == null || p.IsDefeated || !p.gameObject.activeInHierarchy);
    }

    private bool CheckForAllDefeated<T>(List<T> characters) where T : Component, ICombatant
    {
        if (characters == null || characters.Count == 0) return true; // No characters of this type means they are "all defeated" in a vacuum.
        return characters.All(c => c == null || c.IsDefeated || !c.gameObject.activeInHierarchy);
    }


    private void UpdateTurnIndicator(string message)
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = message;
        }
        // Debug.Log(message); // Also log to console for clarity
    }

    // Call this if you dynamically add/remove combatants mid-game,
    // though for this simple version it's not strictly necessary after initial setup.
    public void RefreshCombatantList()
    {
        // Remove defeated or inactive combatants
        combatantsInTurnOrder.RemoveAll(c => c == null || c.IsDefeated || !c.GetGameObject().activeInHierarchy);
        
        // Ensure currentCombatantIndex is valid
        if (combatantsInTurnOrder.Count > 0)
        {
            currentCombatantIndex = Mathf.Clamp(currentCombatantIndex, 0, combatantsInTurnOrder.Count - 1);
        }
        else 
        {
            currentCombatantIndex = 0;
            isCombatActive = false; // No one left
            Debug.Log("All combatants removed or defeated. Combat ended.");
            if (CheckForPlayerVictory()) UpdateTurnIndicator("PLAYER VICTORY!");
            else if (CheckForEnemyVictory()) UpdateTurnIndicator("ENEMY VICTORY!");
            else UpdateTurnIndicator("COMBAT ENDED - STALEMATE?");
        }
    }
} 
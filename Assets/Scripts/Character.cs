using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int health;
    public bool isdead = false;

    public Dictionary<string, int> effect_dur = new Dictionary<string, int>();

    [SerializeField] GameObject InputPanel;

    [SerializeField] private GameObject enemy1;

    [SerializeField] private GameObject battleSystem;

    private bool stunned = false;

    // New variables for animation
    private Animator animator;
    private SimpleSpriteBob spriteBobber;

    void Start()
    {
        //Initialize status effect dictionary
        InputPanel.SetActive(false);
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Burn_4", 0);

        // Initialize Animator and SimpleSpriteBob components
        animator = GetComponent<Animator>();
        spriteBobber = GetComponent<SimpleSpriteBob>();

        if (animator == null)
        {
            Debug.LogWarning(this.name + " is missing an Animator component.");
        }
        if (spriteBobber == null)
        {
            Debug.LogWarning(this.name + " is missing a SimpleSpriteBob component.");
        }
    }

    public void startTurn()
    {
        Debug.Log("It is " + this.name + "'s turn");
        handleStatusEffects();
        if (!stunned)
        {
            InputPanel.SetActive(true);
        }
        else
        {
            Debug.Log("You are stunned, skipping turn...");
            battleSystem.GetComponent<BattleSystem>().nextTurn();
        }
    }

    void handleStatusEffects()
    {
        if (effect_dur["Stun"] > 0)
        {
            stunned = true;
            effect_dur["Stun"] -= 1;
        }
        else
        {
            stunned = false;
        }

        if (effect_dur["Bleed_1"] > 0)
        {
            setHealth(health - 1);
            Debug.Log("Took 1pt of bleed damage! " + effect_dur["Bleed_1"] + " turns remaining.");
            effect_dur["Bleed_1"] -= 1;
        }

        if (effect_dur["Bleed_2"] > 0)
        {
            setHealth(health - 2);
            Debug.Log("Took 2pts of bleed damage! " + effect_dur["Bleed_2"] + " turns remaining.");
            effect_dur["Bleed_2"] -= 1;
        }

        if (effect_dur["Burn_4"] > 0)
        {
            setHealth(health - 4);
            Debug.Log("Took 4pts of burn damage! " + effect_dur["Burn_4"] + " turns remaining.");
            effect_dur["Burn_4"] -= 1;
        }
    }

    public void attack1()
    {
        Debug.Log(this.name + ": ATTACK 1 initiated.");
        InputPanel.SetActive(false); // Hide input panel immediately

        if (spriteBobber != null)
        {
            spriteBobber.SetBobbing(false); // Stop idle bob
        }
        else
        {
            Debug.LogWarning(this.name + " has no SimpleSpriteBob component to disable.");
        }

        if (animator != null)
        {
            animator.SetTrigger("DoAttack1"); // Trigger the animation state
        }
        else
        {
            Debug.LogError(this.name + " is missing Animator component for attack1!");
            // If no animator, we might need to call the completion logic directly or skip
            // For now, we'll assume the animation event won't fire, so let's log and potentially end turn.
            // This part depends on how critical the animation is.
            // As a fallback if animator is missing, we could call a simplified version or just end turn after a delay.
            // For this plan, we rely on the animation event.
            // Consider if you want an immediate fallback here if animator is null.
            // For now, if no animator, attack logic in OnAttack1AnimationComplete won't be called by event.
        }
        // Old logic is moved to OnAttack1AnimationComplete:
        // Enemy target = enemy1.GetComponent<Enemy>();
        // int damage = UnityEngine.Random.Range(2, 6);
        // target.setHealth(target.health - damage);
        // Debug.Log("Enemy Health: " + target.health);
        // battleSystem.GetComponent<BattleSystem>().nextTurn();
    }

    // This function will be called by an Animation Event at the end of Attack1 animation
    public void OnAttack1AnimationComplete()
    {
        Debug.Log(this.name + ": Attack1 Animation Complete (Event Fired).");

        // Re-enable bobbing
        if (spriteBobber != null)
        {
            spriteBobber.SetBobbing(true);
        }

        // Actual Attack Logic
        if (enemy1 != null && enemy1.GetComponent<Enemy>() != null) // Ensure enemy1 is assigned and has an Enemy component
        {
            Enemy target = enemy1.GetComponent<Enemy>();
            if (!target.isdead)
            {
                // Example damage, you might want specific damage values for Jester's Attack 1
                int damage = UnityEngine.Random.Range(2, 6); 
                Debug.Log(this.name + " attacks " + target.name + " with Attack 1.");
                target.setHealth(target.health - damage);
                Debug.Log(target.name + " health: " + target.health);
            }
            else
            {
                Debug.Log(this.name + " target " + target.name + " is already defeated.");
            }
        }
        else
        {
             Debug.LogWarning(this.name + ": enemy1 target not assigned or missing Enemy component for attack.");
        }

        // Progress the game
        if (battleSystem != null)
        {
            battleSystem.GetComponent<BattleSystem>().nextTurn();
        }
        else
        {
            Debug.LogError(this.name + " cannot find BattleSystem to end turn!");
        }
    }

    public void attack2()
    {
        Debug.Log("ATTACK 2 WENT THROUGH");
    }

    public void setHealth(int val)
    {
        this.health = val;
        if (this.health <= 0)
        {
            this.isdead = true;
            Destroy(this.gameObject);
            battleSystem.GetComponent<BattleSystem>().checkParty();
        }
    }
}

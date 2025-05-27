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

    void Start()
    {
        //Initialize status effect dictionary
        InputPanel.SetActive(false);
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Burn_4", 0);
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
        Debug.Log(this.name + ": ATTACK 1 WENT THROUGH");
        Enemy target = enemy1.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(2, 6);
        target.setHealth(target.health - damage);
        Debug.Log("Enemy Health: " + target.health);
        InputPanel.SetActive(false);
        battleSystem.GetComponent<BattleSystem>().nextTurn();
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

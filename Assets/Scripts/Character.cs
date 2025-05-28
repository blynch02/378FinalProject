using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
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

    [SerializeField] private GameObject healthBarPrefab;

    private HealthBar healthBar;

    [SerializeField] private Transform healthBarAnchor;

    public int maxHealth = 10;


    private bool stunned = false;

    private bool strength = false;

    private bool protection = false;

    private bool weakness = false;

    private bool terrified = false;

    void Start()
    {
        //Initialize status effect dictionary
        InputPanel.SetActive(false);
        health = maxHealth;
        GameObject hb = Instantiate(healthBarPrefab, healthBarAnchor ? healthBarAnchor.position : transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
        healthBar = hb.GetComponent<HealthBar>();
        Debug.Log($"Updating health bar: {health} / {maxHealth}");
        healthBar.SetHealth(health, maxHealth);
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Burn_4", 0);
        effect_dur.Add("Protection_50", 0);
        effect_dur.Add("Strength_20", 0);
        effect_dur.Add("Weakness_20", 0);
        effect_dur.Add("Terrified_30", 0);
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
        if (effect_dur.ContainsKey("Stun") && effect_dur["Stun"] > 0)
        {
            stunned = true;
            effect_dur["Stun"] -= 1;
        }
        else
        {
            stunned = false;
        }

        if (effect_dur.ContainsKey("Bleed_1") && effect_dur["Bleed_1"] > 0)
        {
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            setHealth(health - 1);
            Debug.Log("Took 1pt of bleed damage! " + effect_dur["Bleed_1"] + " turns remaining.");
            effect_dur["Bleed_1"] -= 1;
        }

        if (effect_dur.ContainsKey("Bleed_2") && effect_dur["Bleed_2"] > 0)
        {
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            setHealth(health - 2);
            Debug.Log("Took 2pts of bleed damage! " + effect_dur["Bleed_2"] + " turns remaining.");
            effect_dur["Bleed_2"] -= 1;
        }

        if (effect_dur.ContainsKey("Burn_4") && effect_dur["Burn_4"] > 0)
        {
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            setHealth(health - 4);
            Debug.Log("Took 4pts of burn damage! " + effect_dur["Burn_4"] + " turns remaining.");
            effect_dur["Burn_4"] -= 1;

        }
        if (effect_dur["Protection_50"] > 0)
        {
            protection = true;
            effect_dur["Protection_50"] -= 1;
        }
        else
        {
            protection = false;
        }

        if (effect_dur["Strength_20"] > 0)
        {
            strength = true;
            effect_dur["Strength_20"] -= 1;
        }
        else
        {
            strength = false;
        }

        if (effect_dur["Weakness_20"] > 0)
        {
            weakness = true;
            effect_dur["Weakness_20"] -= 1;
        }
        else
        {
            weakness = false;
        }

        if (effect_dur["Terrified_30"] > 0)
        {
            terrified = true;
            effect_dur["Terrified_30"] -= 1;
        }
        else
        {
            terrified = false;
        }
    }

    public void Reap_What_You_Sow()
    {
        Enemy target = enemy1.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(3, 5);
        int accuracyThreshold = 10;
        int bleedThreshold = 10;
        if (strength)
        {
            damage = (int)math.ceil(damage * 1.2);
        }
        if (weakness)
        {
            damage = (int)math.ceil(damage * .8);
        }
        if (terrified)
        {
            accuracyThreshold = accuracyThreshold + 30;
        }
        if (UnityEngine.Random.Range(0, 100) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Reap What You Sow WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= bleedThreshold)
            {
                Debug.Log("ENEMY BLEEDING");
                target.setStatusEffect("Bleed_2", 3);
            }
            else
            {
                Debug.Log("Missed Bleed Chance");
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        InputPanel.SetActive(false);
        battleSystem.GetComponent<BattleSystem>().nextTurn();
        
    }

    public void War_Song()
    {
        foreach (GameObject partymember in battleSystem.GetComponent<BattleSystem>().party)
        {
            Debug.Log("Buffed: " + partymember.name);
            partymember.GetComponent<Character>().setStatusEffect("Strength_20", 3);
        }
        InputPanel.SetActive(false);
        battleSystem.GetComponent<BattleSystem>().nextTurn();
    }

    public void Grim_Melody()
    {
        //Terrify 2 enemies
        int accuracyThreshold = 20;
        if (terrified)
        {
            accuracyThreshold = accuracyThreshold + 30;
        }
        if (UnityEngine.Random.Range(0, 100) >= accuracyThreshold)
        {
            setHealth(health - UnityEngine.Random.Range(1, 4));
        }
    }

    public void Ace_In_The_Sleeve()
    {
        Enemy target = enemy1.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(1, 15);
        int accuracyThreshold = 5;

        if (strength)
        {
            damage = (int)math.ceil(damage * 1.2);
        }
        if (weakness)
        {
            damage = (int)math.ceil(damage * .8);
        }
        if (terrified)
        {
            accuracyThreshold = accuracyThreshold + 30;
        }
        if (UnityEngine.Random.Range(0, 100) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Reap What You Sow WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        InputPanel.SetActive(false);
        battleSystem.GetComponent<BattleSystem>().nextTurn();
    }

    public void setHealth(int val)
    {
        health = Mathf.Clamp(val, 0, maxHealth);
        Debug.Log($"Updating health bar: {health} / {maxHealth}");

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (health <= 0 && !isdead)
        {
            isdead = true;
            Destroy(healthBar?.gameObject);
            Destroy(this.gameObject);
            battleSystem.GetComponent<BattleSystem>().checkParty();
        }
    }

    public void setStatusEffect(string effect, int dur)
    {
        this.effect_dur[effect] += dur;
    }

    public int getStatusEffect(string effect)
    {
        return this.effect_dur[effect];
    }
}

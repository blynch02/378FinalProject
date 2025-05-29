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
    [SerializeField] private GameObject currentTarget;
    [SerializeField] private GameObject targetButtonsGroup;



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
        targetButtonsGroup.SetActive(false);
        health = maxHealth;
        GameObject hb = Instantiate(healthBarPrefab, healthBarAnchor ? healthBarAnchor.position : transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
        healthBar = hb.GetComponent<HealthBar>();
        Debug.Log($"Updating health bar: {health} / {maxHealth}");
        healthBar.SetHealth(health, maxHealth);
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Bleed_4", 0);
        effect_dur.Add("Burn_4", 0);
        effect_dur.Add("Protection_50", 0);
        effect_dur.Add("Strength_20", 0);
        effect_dur.Add("Weakness_20", 0);
        effect_dur.Add("Terrified_30", 0);
    }

    public void startTurn()
    {
        if (isdead)
        {
            endTurn();
        }
        Debug.Log("It is " + this.name + "'s turn");
        handleStatusEffects();
        if (!stunned)
        {
            InputPanel.SetActive(true);

            if (targetButtonsGroup != null)
                targetButtonsGroup.SetActive(true);
        }
        else
        {
            Debug.Log("You are stunned, skipping turn...");
            endTurn();
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

        if (effect_dur.ContainsKey("Bleed_4") && effect_dur["Bleed_4"] > 0)
        {
            setHealth(health - 2);
            Debug.Log("Took 4pts of bleed damage! " + effect_dur["Bleed_4"] + " turns remaining.");
            effect_dur["Bleed_4"] -= 1;
        }

        if (effect_dur.ContainsKey("Burn_4") && effect_dur["Burn_4"] > 0)
        {
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            setHealth(health - 4);
            Debug.Log("Took 4pts of burn damage! " + effect_dur["Burn_4"] + " turns remaining.");
            effect_dur["Burn_4"] -= 1;

        }
        if (effect_dur.ContainsKey("Protection_50") && effect_dur["Protection_50"] > 0)
        {
            protection = true;
            effect_dur["Protection_50"] -= 1;
        }
        else
        {
            protection = false;
        }

        if (effect_dur.ContainsKey("Strength_20") && effect_dur["Strength_20"] > 0)
        {
            strength = true;
            effect_dur["Strength_20"] -= 1;
        }
        else
        {
            strength = false;
        }

        if (effect_dur.ContainsKey("Weakness_20") && effect_dur["Weakness_20"] > 0)
        {
            weakness = true;
            effect_dur["Weakness_20"] -= 1;
        }
        else
        {
            weakness = false;
        }

        if (effect_dur.ContainsKey("Terrified_30") && effect_dur["Terrified_30"] > 0)
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
        if (currentTarget == null) return;

        Enemy target = currentTarget.GetComponent<Enemy>();
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
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void War_Song()
    {
        foreach (GameObject partymember in battleSystem.GetComponent<BattleSystem>().party)
        {
            Debug.Log("Buffed: " + partymember.name);
            partymember.GetComponent<Character>().setStatusEffect("Strength_20", 3);
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
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
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Ace_In_The_Sleeve()
    {
        if (currentTarget == null) return;

        Enemy target = currentTarget.GetComponent<Enemy>();
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
            Debug.Log(this.name + ": ATTACK: Ace In The Sleeve WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Divine_Intervention()
    {
        Character target = battleSystem.GetComponent<BattleSystem>().party[0].GetComponent<Character>();
        int healVal = UnityEngine.Random.Range(4, 8);
        target.setHealth(target.health + healVal);
        Debug.Log("Healed " + target.name + "for " + healVal);
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Sanctuary_Of_Light()
    {
        foreach (GameObject partymember in battleSystem.GetComponent<BattleSystem>().party)
        {
            Character member = partymember.GetComponent<Character>();
            if (!member.isdead)
            {
                int healVal = UnityEngine.Random.Range(1, 4);
                Debug.Log("Healed: " + partymember.name + "For " + healVal);
                member.setHealth(member.health += healVal);
            }
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Sanctimonious_Smite()
    {
        if (currentTarget == null) return;
        int healVal = UnityEngine.Random.Range(1, 2);
        Enemy target = currentTarget.GetComponent<Enemy>();

        int damage = UnityEngine.Random.Range(5, 8);
        int accuracyThreshold = 10;
        int stunThreshold = 80;
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
            Debug.Log(this.name + ": ATTACK: Sanctimonious Smite WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            setHealth(health + healVal);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= stunThreshold)
            {
                target.setStatusEffect("Stun", 1);
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Holy_Rebuke()
    {
        if (currentTarget == null) return;
        Enemy target = currentTarget.GetComponent<Enemy>();

        int damage = UnityEngine.Random.Range(5, 8);
        int accuracyThreshold = 10;
        int stunThreshold = 30;
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
        if (target.getStatusEffect("Stun") > 0)
        {
            damage = (int)math.ceil(damage * 1.5);
        }

        if (UnityEngine.Random.Range(0, 100) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Holy Rebuke WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= stunThreshold)
            {
                target.setStatusEffect("Stun", 1);
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Dragons_Breath()
    {
        foreach (GameObject enemy in battleSystem.GetComponent<BattleSystem>().enemies)
        {
            int damage = UnityEngine.Random.Range(1, 5);
            Debug.Log("Healed: " + enemy.name + "For " + damage);
            Enemy member = enemy.GetComponent<Enemy>();
            member.setHealth(member.health -= damage);
            member.setStatusEffect("Burn_4", 2);
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Zephyrs_Call()
    {
        if (currentTarget == null) return;
        Enemy target = currentTarget.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(8, 15);
        int stunThreshold = 50;

        Debug.Log(this.name + ": ATTACK: Zephyrs Call WENT THROUGH");
        Debug.Log($"Updating health bar: {health} / {maxHealth}");
        target.setHealth(target.health - damage);
        Debug.Log("Enemy Health: " + target.health);
        if (UnityEngine.Random.Range(0, 100) >= stunThreshold)
        {
            setStatusEffect("Stun", 1);
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Aura_Of_Protection()
    {
        Character target = battleSystem.GetComponent<BattleSystem>().party[0].GetComponent<Character>();
        target.setStatusEffect("Protection_50", 2);
        Debug.Log("Protected " + target.name);
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Weakening_Curse()
    {
        if (currentTarget == null) return;
        Enemy target = currentTarget.GetComponent<Enemy>();
        target.setStatusEffect("Weakness_20", 1);
        Debug.Log("Cursed " + target.name);
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();

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
            Destroy(gameObject.GetComponent<SpriteRenderer>());
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

    public void SetTarget(GameObject target)
    {
        currentTarget = target;
        Debug.Log(this.name + " is now targeting " + target.name);
    }
    
    void endTurn()
    {
        currentTarget = null;
        BattleSystem bs = battleSystem.GetComponent<BattleSystem>();

        int originalIndex = bs.nextPlayer;
        int count = bs.party.Count;

        for (int i = 1; i <= count; i++)
        {
            int index = (originalIndex + i) % count;
            Character e = bs.party[index].GetComponent<Character>();
            if (!e.isdead)
            {
                bs.nextPlayer = index;
                break;
            }
        }

        bs.TriggerNextTurn();
    }
}



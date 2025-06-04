using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] public bool isdead = false;

    [SerializeField] private GameObject playerChar1;

    [SerializeField] private GameObject battleSystem;

    public Dictionary<string, int> effect_dur = new Dictionary<string, int>();

    private bool stunned = false;
    private bool protection = false;

    private bool strength = false;

    private bool weakness = false;

    private bool terrified = false;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarAnchor;
    [SerializeField] private GameObject[] targetButtons;

    [SerializeField] private Transform damagePopup;
    private List<Action> attacks;

    private HealthBar healthBar;
    public int maxHealth = 10;

    void Start()
    {
        attacks = new List<Action> { enemyAttack1, enemyAttack2, enemyAttack3 };
        health = maxHealth;

        GameObject hb = Instantiate(healthBarPrefab, transform);
        hb.transform.localPosition = healthBarAnchor.localPosition;
        hb.transform.localRotation = Quaternion.identity;

        healthBar = hb.GetComponent<HealthBar>();
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

    public void showStatusEffect(string message)
    {
        Vector3 randomOffset = new Vector3(
        UnityEngine.Random.Range(-100, 100),
        UnityEngine.Random.Range(90, 125),
        0);
        Transform damagePopupTransform = Instantiate(damagePopup, this.transform.position + randomOffset, Quaternion.identity);
        DamageNumbers dmgnums = damagePopupTransform.GetComponent<DamageNumbers>();
        dmgnums.setUpStatusEffect(message);
    }

    public void setHealth(int val)
    {
        Vector3 randomOffset = new Vector3(
        UnityEngine.Random.Range(-100, 100),
        UnityEngine.Random.Range(90, 125),
        0);
        Transform damagePopupTransform = Instantiate(damagePopup, this.transform.position + randomOffset, Quaternion.identity);
        DamageNumbers dmgnums = damagePopupTransform.GetComponent<DamageNumbers>();
        dmgnums.setUp(health - val);
        this.health = Mathf.Clamp(val, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (this.health <= 0 && !isdead)
        {
            isdead = true;

            Destroy(healthBar?.gameObject);
            destoyButtons();
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            battleSystem.GetComponent<BattleSystem>().checkEnemies();
        }
    }

    private void destoyButtons()
    {
        foreach (GameObject button in targetButtons)
        {
            Destroy(button);
        }
    }

    public void startTurn()
    {
        StartCoroutine(EnemyAttackRoutine());
    }

    private IEnumerator EnemyAttackRoutine()
    {

        if (isdead)
        {
            endTurn();
            yield break;
        }
        else
        {
            handleStatusEffects();
            if (isdead)
            {
                endTurn();
                yield break;
            }
            if (!stunned)
            {
                yield return new WaitForSeconds(2f);
                int attackNum = UnityEngine.Random.Range(0, 2);
                attacks[attackNum]();
            }
            else
            {
                endTurn();
                yield break;
            }
        }
    }

    void enemyAttack1()
    {
        BattleSystem bs = battleSystem.GetComponent<BattleSystem>();
        Character target = null;
        while (target == null || target.isdead)
        {
            target = bs.party[UnityEngine.Random.Range(0, 4)].GetComponent<Character>();
        }
        int damage = UnityEngine.Random.Range(4, 9);
        int accuracyThreshold = 10;
        int stunThreshold = 20;
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
        if (target.getStatusEffect("Protection_50") > 0)
        {
            damage = (int)math.ceil(damage * .5);
        }
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Lumbering Strike WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= stunThreshold)
            {
                Debug.Log(target.name + "Stunned");
                target.setStatusEffect("Stun", 1);
            }
            else
            {
                Debug.Log("Missed Stun Chance");
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        endTurn();
    }

    void enemyAttack2()
    {
        BattleSystem bs = battleSystem.GetComponent<BattleSystem>();
        Character target = null;
        while (target == null || target.isdead)
        {
            target = bs.party[UnityEngine.Random.Range(0, 4)].GetComponent<Character>();
        }
        int damage = UnityEngine.Random.Range(8, 14);
        int accuracyThreshold = 10;
        int stunThreshold = 20;
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
        if (target.getStatusEffect("Protection_50") > 0)
        {
            damage = (int)math.ceil(damage * .5);
        }
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Reckless Charge WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= stunThreshold)
            {
                Debug.Log("Stunned self");
                setStatusEffect("Stun", 1);
            }
            else
            {
                Debug.Log("Missed Stun Chance");
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        endTurn();
    }

    void enemyAttack3()
    {
        BattleSystem bs = battleSystem.GetComponent<BattleSystem>();
        Character target = null;
        while (target == null || target.isdead)
        {
            target = bs.party[UnityEngine.Random.Range(0, 4)].GetComponent<Character>();
        }
        int damage = UnityEngine.Random.Range(2, 5);
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
        if (target.getStatusEffect("Protection_50") > 0)
        {
            damage = (int)math.ceil(damage * .5);
        }
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Lacerate WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 100) >= bleedThreshold)
            {
                Debug.Log(target.name + "Bleeding");
                target.setStatusEffect("Bleed_4", 2);
            }
            else
            {
                Debug.Log("Missed Stun Chance");
            }
        }
        else
        {
            Debug.Log("MISSED ATTACK");
        }
        endTurn();
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

        if (effect_dur.ContainsKey("Bleed_1") && effect_dur["Bleed_1"] > 0)
        {
            setHealth(health - 1);
            effect_dur["Bleed_1"] -= 1;
            Debug.Log("Took 1pt of bleed damage! " + effect_dur["Bleed_1"] + " turns remaining.");
        }

        if (effect_dur.ContainsKey("Bleed_2") && effect_dur["Bleed_2"] > 0)
        {
            setHealth(health - 2);
            effect_dur["Bleed_2"] -= 1;
            Debug.Log("Took 2pts of bleed damage! " + effect_dur["Bleed_2"] + " turns remaining.");
        }

        if (effect_dur.ContainsKey("Bleed_4") && effect_dur["Bleed_4"] > 0)
        {
            setHealth(health - 2);
            effect_dur["Bleed_4"] -= 1;
            Debug.Log("Took 4pts of bleed damage! " + effect_dur["Bleed_4"] + " turns remaining.");
        }

        if (effect_dur.ContainsKey("Burn_4") && effect_dur["Burn_4"] > 0)
        {
            setHealth(health - 4);
            effect_dur["Burn_4"] -= 1;
            Debug.Log("Took 4pts of burn damage! " + effect_dur["Burn_4"] + " turns remaining.");

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

void endTurn()
{
    BattleSystem bs = battleSystem.GetComponent<BattleSystem>();

    int originalIndex = bs.nextEnemy;
    int count = bs.enemies.Count;

    // Advance to next alive enemy
    for (int i = 1; i <= count; i++)
    {
        int index = (originalIndex + i) % count;
        Enemy e = bs.enemies[index].GetComponent<Enemy>();
        if (!e.isdead)
        {
            bs.nextEnemy = index;
            break;
        }
    }

    bs.TriggerNextTurn();
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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] private List<UnityEngine.UI.Button> targetButtons;
    [SerializeField] private Transform bloodParticleTrans;
    [SerializeField] private ParticleSystem particles;
    private UnityEngine.UI.Button selectedButton;

    private HealthBar healthBar;

    [SerializeField] private Transform healthBarAnchor;

    [SerializeField] private Transform damagePopup;

    public int maxHealth = 10;


    private bool stunned = false;

    private bool strength = false;

    private bool protection = false;

    private bool weakness = false;

    private bool terrified = false;

    private AudioSource audioSource;

    [SerializeField] AudioClip[] audioClips; 
    [SerializeField] private GameObject currentAllyTarget;
    [SerializeField] private PartyTargetButtons partyTargetButtonsUI;


    // Variables for animation handling
    private Animator animator;
    private SimpleSpriteBob spriteBobber;

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
        audioSource = GetComponent<AudioSource>();
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Bleed_4", 0);
        effect_dur.Add("Burn_4", 0);
        effect_dur.Add("Protection_50", 0);
        effect_dur.Add("Strength_20", 0);
        effect_dur.Add("Weakness_20", 0);
        effect_dur.Add("Terrified_30", 0);
        effect_dur.Add("Poison_1", 0);

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
        if (isdead)
        {
            endTurn();
            return;
        }
        Debug.Log("It is " + this.name + "'s turn");
        handleStatusEffects();
        if (isdead)
        {
            endTurn();
            return;
        }
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
            return;
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
        
        if (effect_dur.ContainsKey("Bleed_1") && effect_dur["Poison_1"] > 0)
        {
            setHealth(health - 1);
            effect_dur["Poison_1"] -= 1;
            Debug.Log("Took 1pt of poison damage! " + effect_dur["Poison_1"] + " turns remaining.");
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
    public void Reap_What_You_Sow()
    {
        if (currentTarget == null) return;
        StartCoroutine(ExecuteAttackWithAnimation("Reaping", ExecuteReapWhatYouSowLogic));
    }

    // Generic coroutine for handling attack animations
    private System.Collections.IEnumerator ExecuteAttackWithAnimation(string animationTrigger, System.Action attackLogic)
    {
        Debug.Log(this.name + ": Starting attack with animation: " + animationTrigger);

        // Stop bobbing and start animation
        if (spriteBobber != null)
        {
            spriteBobber.SetBobbing(false);
        }

        if (animator != null)
        {
            animator.SetBool(animationTrigger, true);
            
            // Wait for animation to finish (you can adjust this timing or use animation length)
            yield return new WaitForSeconds(1.0f); // Adjust timing as needed
            
            animator.SetBool(animationTrigger, false);
        }

        // Execute the actual attack logic
        attackLogic?.Invoke();

        // Resume bobbing
        if (spriteBobber != null)
        {
            spriteBobber.SetBobbing(true);
        }
    }

    private void ExecuteReapWhatYouSowLogic()
    {
        if (currentTarget == null) return;

        Enemy target = currentTarget.GetComponent<Enemy>();

        int damage = UnityEngine.Random.Range(3, 6);
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

        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Reap What You Sow HIT " + target.name);
            target.setHealth(target.health - damage);
            Debug.Log(target.name + " Health: " + target.health);
            if (UnityEngine.Random.Range(0, 101) >= bleedThreshold)
            {
                Debug.Log(target.name + " is now BLEEDING");
                target.showStatusEffect("Bleeding!");
                target.setStatusEffect("Bleed_2", 3);
            }
            else
            {
                Debug.Log("Missed Bleed Chance on " + target.name);
            }
        }
        else
        {
            target.showStatusEffect("Missed");
            Debug.Log(this.name + ": ATTACK: Reap What You Sow MISSED " + target.name);
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
            Character member = partymember.GetComponent<Character>();
            member.showStatusEffect("Strength!");
            member.setStatusEffect("Strength_20", 3);
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
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            setHealth(health - UnityEngine.Random.Range(1, 5));
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
        int damage = UnityEngine.Random.Range(1, 16);
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
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Ace In The Sleeve WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
        }
        else
        {
            target.showStatusEffect("Missed");
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
        // Open the party selection UI
        partyTargetButtonsUI.Show(this, battleSystem.GetComponent<BattleSystem>().party, ExecuteDivineIntervention);
    }

    public void ExecuteDivineIntervention()
    {
        if (currentAllyTarget == null)
        {
            Debug.LogWarning("No ally target selected.");
            return;
        }

        Character target = currentAllyTarget.GetComponent<Character>();
        int healVal = UnityEngine.Random.Range(4, 9);
        target.setHealth(target.health + healVal);
        target.showStatusEffect("Healed!");
        Debug.Log("Healed " + target.name + " for " + healVal);

        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
            targetButtonsGroup.SetActive(false);
        partyTargetButtonsUI.Hide();

        endTurn();
    }

    public void SetAllyTarget(GameObject target)
    {
        currentAllyTarget = target;
        Debug.Log(name + " selected ally target: " + target.name);
    }

    public void Sanctuary_Of_Light()
    {
        foreach (GameObject partymember in battleSystem.GetComponent<BattleSystem>().party)
        {
            Character member = partymember.GetComponent<Character>();
            if (!member.isdead)
            {
                int healVal = UnityEngine.Random.Range(1, 5);
                Debug.Log("Healed: " + partymember.name + "For " + healVal);
                member.setHealth(member.health + healVal);
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
        
        // Option A: Execute immediately (current behavior)
        ExecuteSanctioniousSmiteLogic();
        
        // Option B: Execute with animation (just change the above line to this):
        // StartCoroutine(ExecuteAttackWithAnimation("Smiting", ExecuteSanctioniousSmiteLogic));
    }

    private void ExecuteSanctioniousSmiteLogic()
    {
        int healVal = UnityEngine.Random.Range(1, 3);
        Enemy target = currentTarget.GetComponent<Enemy>();

        int damage = UnityEngine.Random.Range(5, 9);
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
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Sanctimonious Smite WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            setHealth(health + healVal);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 101) >= stunThreshold)
            {
                target.showStatusEffect("Stunned!");
                target.setStatusEffect("Stun", 1);
            }
        }
        else
        {
            target.showStatusEffect("Missed");
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

        int damage = UnityEngine.Random.Range(5, 9);
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

        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            Debug.Log(this.name + ": ATTACK: Holy Rebuke WENT THROUGH");
            Debug.Log($"Updating health bar: {health} / {maxHealth}");
            target.setHealth(target.health - damage);
            Debug.Log("Enemy Health: " + target.health);
            if (UnityEngine.Random.Range(0, 101) >= stunThreshold)
            {
                target.showStatusEffect("Stunned!");
                target.setStatusEffect("Stun", 1);
            }
        }
        else
        {
            target.showStatusEffect("Missed!");
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
        int accuracyThreshold = 30;
        foreach (GameObject enemy in battleSystem.GetComponent<BattleSystem>().enemies)
        {
            if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
            {
                Enemy member = enemy.GetComponent<Enemy>();
                member.showStatusEffect("Burned!");
                member.setStatusEffect("Burn_4", 2);
            }
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
        int damage = UnityEngine.Random.Range(8, 16);
        int stunThreshold = 50;

        Debug.Log(this.name + ": ATTACK: Zephyrs Call WENT THROUGH");
        Debug.Log($"Updating health bar: {health} / {maxHealth}");
        target.setHealth(target.health - damage);
        Debug.Log("Enemy Health: " + target.health);
        if (UnityEngine.Random.Range(0, 101) >= stunThreshold)
        {
            showStatusEffect("Stunned");
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
        // Show party target selection panel and defer execution
        partyTargetButtonsUI.Show(this, battleSystem.GetComponent<BattleSystem>().party, ExecuteAuraOfProtection);
    }

    public void ExecuteAuraOfProtection()
    {
        if (currentAllyTarget == null)
        {
            Debug.LogWarning("No ally target selected.");
            return;
        }

        Character target = currentAllyTarget.GetComponent<Character>();
        target.setStatusEffect("Protection_50", 2);
        target.showStatusEffect("Protected!");
        Debug.Log("Protected " + target.name);

        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
            targetButtonsGroup.SetActive(false);
        partyTargetButtonsUI.Hide();

        endTurn();
    }

    public void Weakening_Curse()
    {
        if (currentTarget == null) return;
        Enemy target = currentTarget.GetComponent<Enemy>();
        target.showStatusEffect("Weakened!");
        target.setStatusEffect("Weakness_20", 1);
        Debug.Log("Cursed " + target.name);
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();

    }

    public void Bloodlet()
    {
        if (currentTarget == null) return;

        Enemy target = currentTarget.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(4, 7);
        int accuracyThreshold = 20;
        int bleedThreshold = 30;

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
        if (target.getStatusEffect("Bleed_1") > 0)
        {
            damage = damage * 2;
        }
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            target.setHealth(target.health - damage);
            if (UnityEngine.Random.Range(0, 101) >= bleedThreshold)
            {
                Debug.Log(target.name + " is now BLEEDING");
                target.showStatusEffect("Bleeding!");
                target.setStatusEffect("Bleed_1", 2);
            }
        }
        else
        {
            target.showStatusEffect("Missed!");
        }

        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Spill_Their_Blood()
    {
        if (currentTarget == null) return;

        Enemy target = currentTarget.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(1, 3);
        int accuracyThreshold = 10;
        int bleedThreshold = 0;

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
        if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
        {
            target.setHealth(target.health - damage);
            if (UnityEngine.Random.Range(0, 101) >= bleedThreshold)
            {
                Debug.Log(target.name + " is now BLEEDING");
                target.showStatusEffect("Bleeding!");
                target.setStatusEffect("Bleed_1", 2);
            }
        }
        else
        {
            target.showStatusEffect("Missed!");
        }

        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Shadows_Call()
    {
        int accuracyThreshold = 25;
        foreach (GameObject enemy in battleSystem.GetComponent<BattleSystem>().enemies)
        {
            Enemy member = enemy.GetComponent<Enemy>();
            if (UnityEngine.Random.Range(0, 101) >= accuracyThreshold)
            {
                member.showStatusEffect("Terrified!");
                member.setStatusEffect("Terrified_30", 1);
            }
            else
            {
                member.showStatusEffect("Missed!");
            }
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Herbal_Remedy()
    {
        int success_threshold = 30;
        if (UnityEngine.Random.Range(0, 101) >= success_threshold)
        {
            int healval = UnityEngine.Random.Range(3, 6);
            setHealth(health + healval);
            Cure();
            showStatusEffect("Cured!");
        }
        else
        {
            setStatusEffect("Poison_1", 4);
            showStatusEffect("Poisoned!");
        }
        InputPanel.SetActive(false);
        if (targetButtonsGroup != null)
        {
            targetButtonsGroup.SetActive(false);
        }
        endTurn();
    }

    public void Cure()
    {
        effect_dur["Bleed_1"] = 0;
        effect_dur["Bleed_2"] = 0;
        effect_dur["Bleed_4"] = 0;
        effect_dur["Burn_4"] = 0;
        effect_dur["Weakness_20"] = 0;
        effect_dur["Terrified_30"] = 0;
        return;
    }

    public void setHealth(int val)
    {
        if (val < this.health && particles != null)
        {
            Instantiate(particles, bloodParticleTrans.position, Quaternion.identity).Play();
            audioSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
            audioSource.PlayOneShot(audioClips[0]);
        }
        else if (val > this.health)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
            audioSource.PlayOneShot(audioClips[1]);  
        }
        Vector3 randomOffset = new Vector3(
        UnityEngine.Random.Range(-100, 100),
        UnityEngine.Random.Range(90, 125),
        0);
        Transform damagePopupTransform = Instantiate(damagePopup, this.transform.position + randomOffset, Quaternion.identity);
        DamageNumbers dmgnums = damagePopupTransform.GetComponent<DamageNumbers>();
        dmgnums.setUp(health - val);
        int currentHealth = health;
        health = Mathf.Clamp(val, 0, maxHealth);
        Debug.Log($"Updating health bar: {health} / {maxHealth}");

        if (healthBar != null)
        {
            healthBar.SetHealth(health, maxHealth);
        }
        if (health <= 0 && !isdead)
        {
            isdead = true;
            Destroy(healthBar?.gameObject);
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            battleSystem.GetComponent<BattleSystem>().checkParty();
        }
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

        foreach (var btn in targetButtons)
        {
            TMPro.TextMeshProUGUI txt = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (btn.name.Contains(target.name))
            {
                txt.text = "Selected";
                btn.image.color = Color.green;
                selectedButton = btn;
            }
            else
            {
                txt.text = btn.name;
                btn.image.color = Color.white;
            }
        }
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
        if (selectedButton != null)
        {
            var txt = selectedButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            txt.text = selectedButton.name;
            selectedButton.image.color = Color.white;
            selectedButton = null;
        }
        bs.TriggerNextTurn();
    }
}



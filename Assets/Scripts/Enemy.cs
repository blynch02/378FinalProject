using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] public bool isdead = false;

    [SerializeField] private GameObject playerChar1;

    [SerializeField] private GameObject battleSystem;

    public Dictionary<string, int> effect_dur = new Dictionary<string, int>();

    private bool stunned = false;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarAnchor;

    private HealthBar healthBar;
    public int maxHealth = 10;

    void Start()
{
    health = maxHealth;

    GameObject hb = Instantiate(healthBarPrefab, transform);
    hb.transform.localPosition = healthBarAnchor.localPosition;
    hb.transform.localRotation = Quaternion.identity;

    healthBar = hb.GetComponent<HealthBar>();
    healthBar.SetHealth(health, maxHealth);

    effect_dur.Add("Stun", 0);
    effect_dur.Add("Bleed_1", 0);
    effect_dur.Add("Bleed_2", 0);
    effect_dur.Add("Burn_4", 0);
}


    public void setHealth(int val)
    {
        this.health = Mathf.Clamp(val, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (this.health <= 0 && !isdead)
        {
            isdead = true;
            Destroy(healthBar?.gameObject);
            Destroy(this.gameObject);
            battleSystem.GetComponent<BattleSystem>().checkEnemies();
        }
    }



    public void startTurn()
    {
        StartCoroutine(EnemyAttackRoutine());
    }

    private IEnumerator EnemyAttackRoutine()
    {
        handleStatusEffects();

        if (!stunned)
        {
            yield return new WaitForSeconds(2f);
            Character target = playerChar1.GetComponent<Character>();
            int damage = Random.Range(2, 6);
            target.setHealth(target.health - damage);
            Debug.Log(target.name + " health: " + target.health);
        }
        else
        {
            Debug.Log("Stunnded, skipping turn");
        }
        endTurn();
    }


    void handleStatusEffects()
    {
        if (effect_dur["Stun"] != 0)
        {
            stunned = true;
        }
    }

    void endTurn()
    {
        battleSystem.GetComponent<BattleSystem>().nextTurn();
    }
}

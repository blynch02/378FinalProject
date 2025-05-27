using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int health;
    public bool isdead = false;

    public Dictionary<string, int> effect_dur = new Dictionary<string, int>();

    [SerializeField] private GameObject enemy1;

    private bool stunned = false;

    public bool isWaitingForAttack = true;

    void Start()
    {
        //Initialize status effect dictionary
        effect_dur.Add("Stun", 0);
        effect_dur.Add("Bleed_1", 0);
        effect_dur.Add("Bleed_2", 0);
        effect_dur.Add("Burn_4", 0);
    }

    void takeTurn()
    {
        
        handleStatusEffects();
        if (!stunned)
        {

        }
    }

    void handleStatusEffects()
    {
        if (effect_dur["Stun"] != 0)
        {
            stunned = true;
        }
    }

    public void attack1()
    {
        Enemy target = enemy1.GetComponent<Enemy>();
        int damage = UnityEngine.Random.Range(2, 6);
        target.setHealth(target.health - damage);
        Debug.Log("ATTACK 1 WENT THROUGH");
    }

    public void attack2()
    {
        Debug.Log("ATTACK 2 WENT THROUGH");
    }
}

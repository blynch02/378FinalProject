using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum BattleState
{
    START, WIN, LOSE,
    PLAYERTURN,
    ENEMYTURN
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerChar1;
    [SerializeField] private GameObject playerChar2;
    [SerializeField] private GameObject playerChar3;
    [SerializeField] private GameObject playerChar4;

    [SerializeField] private GameObject enemyChar1;
    [SerializeField] private GameObject enemyChar2;
    [SerializeField] private GameObject enemyChar3;
    [SerializeField] private GameObject enemyChar4;

    private List<GameObject> party;

    private List<GameObject> enemies;

    private int nextPlayer = 0;

    private int nextEnemy = 0;

    public BattleState state;
    private int round;
    void Start()
    {
        party = new List<GameObject> { playerChar1, playerChar2, playerChar3, playerChar4 };
        enemies = new List<GameObject> { enemyChar1, enemyChar2, enemyChar3, enemyChar4 };
        state = BattleState.START;
        setupBattle();
    }

    void setupBattle()
    {
        //Set GUI Values inside here 
        //Also instanciate enemy objects here

        //After battle is set up
        state = BattleState.PLAYERTURN;
        startBattle();
    }
    void startBattle()
    {
        nextTurn();
    }

    public void checkParty()
    {
        if (party.TrueForAll(character => character.GetComponent<Character>().isdead))
        {
            loseGame();
        }
    }

    public void checkEnemies()
    {
        if (enemies.TrueForAll(character => character.GetComponent<Enemy>().isdead))
        {
            winGame();
        }
    }

    void loseGame()
    {
        Debug.Log("Your party has been defeated, you lose!");
    }

    void winGame()
    {
        Debug.Log("You Win!");
    }

    public void nextTurn()
    {
        if (state == BattleState.PLAYERTURN)
        {
            int attempts = 0;
            while ((party[nextPlayer] == null || party[nextPlayer].GetComponent<Character>().isdead) && attempts < party.Count)
            {
                nextPlayer = (nextPlayer + 1) % party.Count;
                attempts++;
            }

            if (party[nextPlayer] != null && !party[nextPlayer].GetComponent<Character>().isdead)
            {
                Debug.Log("Next player to attack: " + party[nextPlayer].name);
                party[nextPlayer].GetComponent<Character>().startTurn();
                nextPlayer = (nextPlayer + 1) % party.Count;
                state = BattleState.ENEMYTURN;
            }
            else
            {
                Debug.Log("No valid players left.");
                checkParty();
            }
        }
        else if (state == BattleState.ENEMYTURN)
        {
            int attempts = 0;
            while ((enemies[nextEnemy] == null || enemies[nextEnemy].GetComponent<Enemy>().isdead) && attempts < enemies.Count)
            {
                nextEnemy = (nextEnemy + 1) % enemies.Count;
                attempts++;
            }

            if (enemies[nextEnemy] != null && !enemies[nextEnemy].GetComponent<Enemy>().isdead)
            {
                Debug.Log("Next enemy to attack: " + enemies[nextEnemy].name);
                enemies[nextEnemy].GetComponent<Enemy>().startTurn();
                nextEnemy = (nextEnemy + 1) % enemies.Count;
                state = BattleState.PLAYERTURN;
            }
            else
            {
                Debug.Log("No valid enemies left.");
                checkEnemies();
            }
        }
    }
}

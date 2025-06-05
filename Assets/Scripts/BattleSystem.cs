using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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

    public List<GameObject> party;

    public List<GameObject> enemies;

    public int nextPlayer = 0;

    public int nextEnemy = 0;

    public BattleState state;
    private int round;

    private int turnsTillNewRound;
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
        TriggerNextTurn();
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
            state = BattleState.WIN;
            winGame();
        }
    }

    void loseGame()
    {
        Debug.Log("Your party has been defeated, you lose!");
        state = BattleState.LOSE;
    }

    void winGame()
    {
        Debug.Log("You Win!");
    }

public void TriggerNextTurn()
{
    StartCoroutine(nextTurn());
}

private IEnumerator nextTurn()
{
        if (state == BattleState.LOSE)
        {
            yield return 0;
        }
        else if (state == BattleState.WIN)
        {
            yield return 0;
        }
        else
        {
            if (turnsTillNewRound % 8 == 0)
            {
                round += 1;
            }

            turnsTillNewRound += 1;

            if (state == BattleState.PLAYERTURN)
            {
                Character currentPlayer = party[nextPlayer].GetComponent<Character>();
                state = BattleState.ENEMYTURN;

                yield return new WaitForSeconds(0.1f); // Prevent immediate recursion
                Debug.Log("It is now" + currentPlayer.name + "'s turn");
                currentPlayer.startTurn();
            }
            else if (state == BattleState.ENEMYTURN)
            {
                Enemy currentEnemy = enemies[nextEnemy].GetComponent<Enemy>();
                state = BattleState.PLAYERTURN;

                yield return new WaitForSeconds(0.1f); // Prevent immediate recursion
                Debug.Log("It is now " + currentEnemy.name + "'s turn");
                currentEnemy.startTurn();
            }
        }
}

}

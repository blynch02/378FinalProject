using Unity.VisualScripting;
using UnityEngine;

public enum BattleState
{
    START, WIN, LOSE,
    PLAYERTURN1, PLAYERTURN2, PLAYERTURN3, PLAYERTURN4,
    ENEMYTURN1, ENEMYTURN2, ENEMYTURN3, ENEMYTURN4
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerChar1;
    [SerializeField] private GameObject playerChar2;
    [SerializeField] private GameObject playerChar3;
    [SerializeField] private GameObject playerChar4;

    [SerializeField] private GameObject enemy1;
    [SerializeField] private GameObject enemy2;
    [SerializeField] private GameObject enemy3;
    [SerializeField] private GameObject enemy4;
    public BattleState state;
    private int round;
    void Start()
    {
        state = BattleState.START;
        setupBattle();
    }

    void setupBattle()
    {
        //Set GUI Values inside here 
        //Also instanciate enemy objects here

        //After battle is set up
        state = BattleState.PLAYERTURN1;
        startBattle();
    }
    void startBattle()
    {
        round = 0;
        while (true)
        {
            // playerChar1.takeTurn();
            // enemy1.takeTurn();
            // playerChar2.takeTurn();
            // enemy2.takeTurn();
            // playerChar3.takeTurn();
            // enemy3.takeTurn();
            // playerChar4.takeTurn();
            // enemy4.takeTurn();
            round += 1;
        }
    }
}

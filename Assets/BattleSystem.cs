using UnityEngine;

public enum BattleState {START, PLAYERTURN, ENEMYTURN, WIN, LOSE}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerChar1;
    [SerializeField] private GameObject playerChar2;
    [SerializeField] private GameObject playerChar3;
    [SerializeField] private GameObject playerChar4;
    public BattleState state;
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
        playerturn();
    }
    
    void playerturn()
    { }
}

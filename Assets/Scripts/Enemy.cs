using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] public bool isDead;

    void Start()
    {

    }

    public void setHealth(int val)
    {
        this.health = val;
    }

    void Update()
    {
        if (this.health < 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}

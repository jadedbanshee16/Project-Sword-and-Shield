using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    private float health;

    public void takeDamage(float dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

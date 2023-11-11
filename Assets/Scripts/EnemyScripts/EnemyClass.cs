using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    private float health;

    public void takeDamage(float dmg, float pushBack, float stun, Vector3 direction)
    {
        //Do Pushback.


        //Do damage.
        health -= dmg;

        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

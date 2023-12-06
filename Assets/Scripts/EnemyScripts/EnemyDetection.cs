using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public EnemyClass _ai;

    private void OnTriggerEnter(Collider other)
    {
        //Check if triggered by player.
        if (other.CompareTag("Player"))
        {
            //Test if behind a wall.
            RaycastHit hit;

            //Check if there is a wall between player and enemy.
            if (Physics.Raycast(transform.position, other.transform.position - transform.position, out hit, 100))
            {
                if (hit.collider.tag == "Player")
                {
                    //Debug.Log("Player not behind object");
                    _ai.setPlayerFound(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if triggered by player.
        if (other.CompareTag("Player"))
        {
            //Ensure player is outside of the enemy range.
            if(Vector3.Distance(transform.position, other.transform.position) > _ai.getEnemyRange())
            {
                _ai.setPlayerFound(false);
            }
        }
    }
}

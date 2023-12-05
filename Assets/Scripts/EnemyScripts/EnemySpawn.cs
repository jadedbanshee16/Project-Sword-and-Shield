using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] enemies;

    private int island;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void setIsland(int i)
    {
        island = i;

        if (isFirst())
        {
            spawnEnemy();
        }
    }

    public int getIsland()
    {
        return island;
    }

    private bool isFirst()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Waypoint");

        int count = 0;

        for(int i = 0; i < objs.Length; i++)
        {
            if(objs[i].GetComponent<EnemySpawn>().getIsland() == getIsland())
            {
                count++;
            }
        }

        if(count > 1)
        {
            return false;
        } else
        {
            return true;
        }
    }

    private void spawnEnemy()
    {
        //When spawned, then spawn a single enemy.
        int rand = Random.Range(0, enemies.Length);

        //Ensure rand remains in line.
        if (rand >= enemies.Length)
        {
            rand = enemies.Length - 1;
        }

        //Spawn the type of enemy on this exact spot.
        Instantiate(enemies[rand], this.transform.position, Quaternion.identity, this.transform);

    }
}

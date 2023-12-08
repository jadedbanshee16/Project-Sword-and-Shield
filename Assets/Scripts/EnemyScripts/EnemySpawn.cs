using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] enemies;

    public int island;

    private void Start()
    {
        if (isFirst())
        {
            spawnEnemy();
        }
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
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

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

        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Spawn the type of enemy on this exact spot.
        GameObject enemyInstance = Instantiate(enemies[rand], pos, Quaternion.identity, this.transform);

        enemyInstance.GetComponent<EnemyClass>().setIsland(island);

    }
}

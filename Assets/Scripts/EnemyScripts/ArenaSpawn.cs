using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawn : MonoBehaviour
{
    [SerializeField]
    EnemySpawn[] arenaZones;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for(int i = 0; i < arenaZones.Length; i++)
            {
                arenaZones[i].spawnEnemy();
            }
        }
    }
}

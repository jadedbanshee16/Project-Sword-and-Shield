using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSword : GhostItem
{
    [Header("Sword Options")]
    [SerializeField]
    private GameObject _sword;

    [SerializeField]
    [Tooltip("How long the weapon will remain")]
    private float length;
    [SerializeField]
    [Tooltip("How fast the weapon is capable of moving")]
    private float speed;

    public override void use(Vector3 mPos, Vector3 pPos)
    {
        //Get direction of from player to mouse.
        Vector3 newPos = pPos + ((mPos - pPos).normalized * ghostRange);
        newPos = findClosestEnemy(newPos);

        newPos.y = 0.05f;

        //Get a random direction from the new position.
        Vector3 randomSpawn = newPos + (Random.insideUnitSphere * 0.5f);

        //Remove the 'y' of it.
        randomSpawn.y = newPos.y;

        Vector3 newDirection = (newPos - randomSpawn).normalized;

        //Spawn the object.
        GameObject swordInstance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>().GetPooledObject(PoolManager.objectType.sword);

        //Set position of the sword.
        swordInstance.SetActive(true);
        swordInstance.GetComponent<WeaponClass>().setWeapon(newDirection, randomSpawn, getDamage(), getpushBack(), getStun(), length, speed);
    }

    public override void stopUse()
    {
        base.stopUse();
    }

    private Vector3 findClosestEnemy(Vector3 pos)
    {
        //Find all enemies currently on the map and test distance between them.
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        int closestObject = -1;
        float closestDist = Mathf.Infinity;

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (Vector3.Distance(enemyObjects[i].transform.position, pos) < ghostRange &&
               Vector3.Distance(enemyObjects[i].transform.position, pos) < closestDist)
            {
                closestDist = Vector3.Distance(enemyObjects[i].transform.position, pos);
                closestObject = i;
            }
        }

        if (closestObject > -1)
        {
            pos = enemyObjects[closestObject].transform.position;
        }

        return pos;
    }
}

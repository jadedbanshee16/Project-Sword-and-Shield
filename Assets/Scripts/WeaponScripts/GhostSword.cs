using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSword : Weapon
{
    [Header("Sword Options")]
    [SerializeField]
    private GameObject _sword;

    [SerializeField]
    [Tooltip("How long the weapon will remain")]
    public float length;

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
        GameObject swordInstance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>().GetPooledObject(objectType.sword);

        //Set position of the sword.
        swordInstance.SetActive(true);
        swordInstance.transform.position = randomSpawn;
        swordInstance.transform.rotation = Quaternion.identity;
        swordInstance.GetComponent<SwordClass>().swipe(newDirection, speed, length, damage);
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

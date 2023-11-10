using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShield : Weapon
{
    [Header("Shield Options")]
    [SerializeField]
    private GameObject _shield;

    private GameObject shieldInstance;

    public override void use(Vector3 mPos, Vector3 pPos)
    {
        //Get direction of from player to mouse.
        Vector3 newPos = pPos + ((mPos - pPos).normalized * ghostRange);

        newPos.y = 0.15f;

        Vector3 newDirection = pPos + ((mPos - pPos).normalized * ghostRange * 2);

        newDirection.y = 0.15f;

        if (shieldInstance == null)
        {
            //Spawn the object.
            shieldInstance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>().GetPooledObject(objectType.shield);

            //Set position of the sword.
            shieldInstance.SetActive(true);
            shieldInstance.transform.position = newPos;
            shieldInstance.transform.LookAt(newDirection);
        }

        shieldInstance.GetComponent<ShieldClass>().useBlock(newDirection, newPos);
    }

    public override void stopUse()
    {
        setHoldModifier(originalHoldModifier);
        
        if(shieldInstance != null)
        {
            shieldInstance.GetComponent<ShieldClass>().stopBlock();
            shieldInstance = null;
        }
    }
}

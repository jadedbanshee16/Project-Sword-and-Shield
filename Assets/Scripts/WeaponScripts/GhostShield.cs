using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShield : GhostItem
{
    [Header("Shield Options")]
    [SerializeField]
    private GameObject _shield;

    private WeaponClass shieldInstance;

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
            shieldInstance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>().GetPooledObject(PoolManager.objectType.shield).GetComponent<WeaponClass>();

            shieldInstance.setWeapon(newDirection, newPos, getDamage(), getpushBack(), getStun(), getCost());

            //Set position of the sword.
            shieldInstance.gameObject.SetActive(true);
        } else
        {
            shieldInstance.changePositions(newDirection, newPos);
        }
    }

    public override void stopUse()
    {
        setHoldModifier(originalHoldModifier);
        
        if(shieldInstance != null)
        {
            shieldInstance.GetComponent<ShieldClass>().stopWeapon();
            shieldInstance = null;
        }
    }
}

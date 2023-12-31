using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpeningScript : MonoBehaviour
{
    public bool isOpen = false;

    public bool canBeOpened = false;

    public GameObject lightBulb;

    public enum doorType
    {
        key,
        terminal,
        terminal3
    }

    [SerializeField]
    private doorType type;
    public bool testLock()
    {
        if(type == doorType.key && !canBeOpened)
        {
            //Get the inventory.
            Inventory _inv = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory>();

            //If a key is the inventory, then lock can be opened
            if (_inv.getInventory(pickUps.resourceTypes.keys) > 0)
            {
                //Remove the resource as lock is opened;
                _inv.removeResource(pickUps.resourceTypes.keys, 1);

                updateLock(true);
            }
        }

        return canBeOpened;
    }

    //For interactables to update whether the lock has changed remotely.
    public void updateLock(bool newState)
    {
        canBeOpened = newState;

        //Change Light to indicate the correctness.
        if (!canBeOpened)
        {
            lightBulb.SetActive(false);
        }
        else
        {
            lightBulb.SetActive(true);
        }
    }

    public void setDoorType(doorType t)
    {
        type = t;
    }

    public doorType getDoorType()
    {
        return type;
    }
}

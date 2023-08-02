using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUps : Interactable
{
    public int amount;

    public resourceTypes resource;

    public enum resourceTypes
    {
        cogs,
        springs,
        keys
    }

    /*
     * All pickups have an interaction type based on type.
     */
    public override void Interact()
    {
        //When interacting,
        Debug.Log("Pickup interact");

        //Add to inventory.
        inv.addToResourceInventory(resource, amount);

        //Then drestroy this object.
        Destroy(this.gameObject);
    }

}

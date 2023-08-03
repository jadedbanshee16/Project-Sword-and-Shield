using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : Interactable
{
    public Animator anim_;

    private bool opened = false;

    public override void Interact()
    {
        Debug.Log("Door interaction");

        if (!opened)
        {
            //If a key is the inventory, then open door.
            if (inv.getInventory(pickUps.resourceTypes.keys) > 0)
            {
                //Remove the resource when done.
                inv.removeResource(pickUps.resourceTypes.keys, 1);

                anim_.SetBool("Key", true);

                opened = true;
            }
        }


    }
}

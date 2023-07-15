using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickUps : Interactable
{
    [Header("Weapon Stats")]
    public int lvl;
    public GameObject obj;

    /*
     * This will add a weapon to the inventory at level 1.
     */
    public override void Interact()
    {
        Debug.Log("Picked up a weapon: " + name + ", level " + lvl);

        //Put the object into the weapons list.
        inv.addToWeaponsInventory(obj);

        //Now, delete the current object.
        Destroy(this.gameObject);
    }

}

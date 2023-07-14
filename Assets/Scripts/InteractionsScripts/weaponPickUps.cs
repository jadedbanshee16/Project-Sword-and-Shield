using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickUps : Interactable
{
    public enum weaponType
    {
        onHand,
        offHand
    }

    public int lvl;
    public weaponType type;

    /*
     * This will add a weapon to the inventory at level 1.
     */
    public override void Interact()
    {
        Debug.Log("Picked up a " + type + " weapon: " + name + ", level " + lvl);

        //Now, delete the current object.
        Destroy(this.gameObject);
    }

}

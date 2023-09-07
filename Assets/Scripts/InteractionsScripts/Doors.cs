using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : Interactable
{
    public Animator anim_;

    private DoorOpeningScript doorScript;

    private bool opened = false;

    public override void Interact()
    {
        Debug.Log("Door interaction");

        //Get the doorscript.
        if(doorScript == null)
        {
            doorScript = GetComponent<DoorOpeningScript>();
        }

        if (!opened)
        {
            if (doorScript.testLock())
            {
                anim_.SetBool("Key", true);

                opened = true;
            }
        }
    }
}

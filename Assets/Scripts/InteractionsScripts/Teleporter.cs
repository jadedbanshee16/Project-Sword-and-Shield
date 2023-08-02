using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : Interactable
{
    public GameObject pair;

    public override void Interact()
    {
        //Get the pair and teleport.
        player_.transform.position = pair.transform.position;
    }
}

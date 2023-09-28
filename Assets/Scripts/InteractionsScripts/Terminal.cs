using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : Interactable
{
    /*
     * Template for UI Planes.
     * Off = 0;
     * Everything else = possible combinations. If just 1, then ON position.
     */
    public Material[] UIPlanes;
    public Renderer screen;
    public DoorOpeningScript door;
    public GameObject lightBulb;

    /*
     * Implement later
     */
    //private bool broken = false;
    private int currentPlane;

    private void Start()
    {
        currentPlane = 0;
        //Ensure the terminal starts in the 'off' position.
        switchUIPlane(currentPlane);

    }

    public override void Interact()
    {
        Debug.Log("Terminal interaction");

        //Change the current plane.
        currentPlane += 1;
        loopPlanes();
        switchUIPlane(currentPlane);

        //Quick binary switch.
        bool openable = false;
        if (currentPlane != 0)
        {
            openable = true;
        }

        Debug.Log(openable);

        //When set, go to door script and see if a match has occured.
        door.updateLock(openable);

        //Now, open the door.
        door.GetComponent<Interactable>().Interact();

        //Change Light to indicate the correctness.
        if (currentPlane == 0)
        {
            lightBulb.SetActive(false);
        }
        else
        {
            lightBulb.SetActive(true);
        }
    }

    //A function to loop the planes if the current plane is greater than the amount of possible lanes.
    private void loopPlanes()
    {
        if (currentPlane > UIPlanes.Length - 1)
        {
            currentPlane = 0;
        }
    }

    //Switch the planes to display only 1;
    private void switchUIPlane(int index)
    {
        screen.material = UIPlanes[index];
    }

    public void setDoor(GameObject obj)
    {
        door = obj.GetComponent<DoorOpeningScript>();
    }
}

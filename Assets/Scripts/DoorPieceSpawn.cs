using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceSpawn : MonoBehaviour
{
    public GameObject[] objs;

    private GameObject[] currentInstance;

    public DoorOpeningScript door;

    public void createPieces()
    {
        //Set the door to door type.
        door.setDoorType((DoorOpeningScript.doorType)Random.Range(0, 2));

        if (door.getDoorType() == DoorOpeningScript.doorType.key)
        {
            currentInstance = new GameObject[] { Instantiate(objs[1], transform) };
        }
        else if (door.getDoorType() == DoorOpeningScript.doorType.terminal)
        {
            currentInstance = new GameObject[] { Instantiate(objs[0], transform) };

        }
        else if (door.getDoorType() == DoorOpeningScript.doorType.terminal3)
        {
            currentInstance = new GameObject[] { Instantiate(objs[0], transform),
                                                 Instantiate(objs[0], transform),
                                                 Instantiate(objs[0], transform) };
        }

        for (int i = 0; i < currentInstance.Length; i++)
        {
            //Now link the door to the possible terminal.
            Terminal terminal = currentInstance[i].GetComponent<Terminal>();
            if (terminal != null)
            {
                terminal.setDoor(door.gameObject);
            }
        }
    }
}

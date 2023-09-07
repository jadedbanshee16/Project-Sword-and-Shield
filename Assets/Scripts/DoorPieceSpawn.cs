using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceSpawn : MonoBehaviour
{

    public GameObject[] objs;

    private GameObject currentInstance;

    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        //Choose one of the objects and spawn it.
        int rand = Random.Range(0, objs.Length);

        //create the new object.
        currentInstance = Instantiate(objs[rand], transform);

        //Now link the door to the possible terminal.
        Terminal terminal = currentInstance.GetComponent<Terminal>();
        if(terminal != null)
        {
            terminal.setDoor(door);
            door.GetComponent<DoorOpeningScript>().setDoorType(DoorOpeningScript.doorType.terminal);
        }
    }
}

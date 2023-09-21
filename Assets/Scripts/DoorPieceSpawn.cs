using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceSpawn : MonoBehaviour
{

    public enum doorType
    {
        key,
        terminal,
        ghostTerm,
        terminal3,
    }

    public GameObject[] objs;

    private GameObject[] currentInstance;

    public GameObject door;

    public doorType type;
    // Start is called before the first frame update
    void Start()
    {
        //Choose one of the objects and spawn it.
        int rand = Random.Range(0, 2);

        type = (doorType)rand;

        Debug.Log(type + " | " + (doorType)rand);

        if(type == doorType.key)
        {
            currentInstance = new GameObject[] { Instantiate(objs[1], transform) };
        } else if (type == doorType.terminal)
        {
            currentInstance = new GameObject[] { Instantiate(objs[0], transform) };

        } else if (type == doorType.terminal3)
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
                terminal.setDoor(door);
                door.GetComponent<DoorOpeningScript>().setDoorType(DoorOpeningScript.doorType.terminal);
            }
        }
    }
}

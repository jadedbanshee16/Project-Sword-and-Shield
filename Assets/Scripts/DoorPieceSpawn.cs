using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPieceSpawn : MonoBehaviour
{

    public GameObject[] objs;

    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        //Choose one of the objects and spawn it.
        int rand = Random.Range(0, objs.Length - 1);

        //create the new object.
        Instantiate(objs[rand], transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

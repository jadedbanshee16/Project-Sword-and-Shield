using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This keeps the gameobject addition of every object in the children.
 * 
 */
public class MapObjectPackage : MonoBehaviour
{
    public GameObject[] objects;

    public GameObject getObject(int ind)
    {
        return objects[ind];
    }
}

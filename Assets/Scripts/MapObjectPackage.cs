using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This keeps the gameobject addition of every object in the children.
 * 
 */
public class MapObjectPackage : MonoBehaviour
{
    public List<GameObject> objects;

    public int endFirstIsland;

    public int objectAmount()
    {
        return objects.Count;
    }

    public int getPairIndex()
    {
        return endFirstIsland;
    }

    //A function to get all objects in the package.
    public void collectObjects()
    {
        objects = new List<GameObject>();

        foreach(Transform child in transform)
        {
            objects.Add(child.gameObject);
        }
    }

    public GameObject getObject(int ind)
    {
        return objects[ind];
    }
}

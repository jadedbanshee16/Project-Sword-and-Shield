using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A class that works as an interactable for the ghost bareHand.
 * This keeps the object in question and the type of object that is working with.
 * Including Pickups.
 */
public class GhostInteractable : MonoBehaviour
{
    public GameObject obj;
    private GameObject objInstance;

    private void Start()
    {
        //Set up the new object.
        setObject(obj);
    }

    //Set the new gameObject.
    public GameObject getObject()
    {
        return obj;
    }

    public void setObject(GameObject b)
    {
        obj = b;
        Destroy(objInstance);
        objInstance = Instantiate(obj, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90), transform);
    }

    public void DestroyThis()
    {
        Destroy(transform.gameObject);
    }
}

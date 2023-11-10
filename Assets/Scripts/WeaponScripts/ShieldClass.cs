using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldClass : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 pos;
    private bool action = true;

    // Update is called once per frame
    void Update()
    {
        if (action)
        {
            transform.position = pos;
            transform.LookAt(direction);
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void useBlock(Vector3 d, Vector3 p)
    {
        direction = d;
        pos = p;
        action = true;
    }

    public void stopBlock()
    {
        action = false;
    }
}

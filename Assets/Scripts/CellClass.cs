using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellClass : MonoBehaviour
{
    //Fields
    public bool left = false;
    public bool right = false;
    public bool up = false;
    public bool down = false;

    //Sets
    public void setLeft(bool b)
    {
        left = b;
    }
    public void setRight(bool b)
    {
        right = b;
    }
    public void setUp(bool b)
    {
        up = b;
    }
    public void setDown(bool b)
    {
        down = b;
    }

    //Gets
    public bool getLeft()
    {
        return left;
    }
    public bool getRight()
    {
        return right;
    }
    public bool getUp()
    {
        return up;
    }
    public bool getDown()
    {
        return down;
    }
}

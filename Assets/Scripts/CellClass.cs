using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellClass : MonoBehaviour
{
    //Fields
    public GameObject[] zones;
    public GameObject[] walls;

    private bool allZonesUsed;

    //Make a single zone null, meaning it is no longer available to be used.
    public void removeZone(int index)
    {
        zones[index] = null;

        //Check to see if all zones are unavailable.
        allZonesUsed = isAllUsed();
    }

    //Remove a single wall.
    public void removeWalls(int index)
    {
        walls[index] = null;
    }

    //Remove all walls between a min and max number.
    //CONSTRAINTS IS 0 - 4.
    public void removeWalls(int min, int max)
    {
        for(int i = min - 1; i < max; i++)
        {
            walls[i] = null;
        }
    }

    //Add wall to the walls list and instantiate the wall.
    public void addWall(GameObject obj, int index, Vector3 pos)
    {
        walls[index - 1] = Instantiate(obj, pos, Quaternion.identity, transform);
    }

    //Return the transform on one of the zones by index.
    public Transform getZone(int index)
    {
        Transform trans = null;
        if(zones[index] != null)
        {
            trans = zones[index].transform;
        }

        return trans;
    }

    //Return the boolean displaying if all zones are unavailable.
    public bool getAllZonesUsed()
    {
        return allZonesUsed;
    }

    //Find out if all zones are being used.
    //Outputs a boolean flag to show it's all being used.
    public bool isAllUsed()
    {
        bool used = true;
        for (int i = 0; i < zones.Length; i++)
        {
            if (zones[i] != null)
            {
                used = false;
            }
        }

        return used;
    }

    public int findAvailable()
    {
        int used = -1;
        for (int i = 0; i < zones.Length; i++)
        {
            if (zones[i] != null)
            {
                used = i;
                return used;
            }
        }

        return used;
    }
}

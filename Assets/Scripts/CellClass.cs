using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellClass : MonoBehaviour
{
    //Fields
    public GameObject[] zones;
    public GameObject[] walls;

    private bool allZonesUsed;

    public void removeZone(int index, int[] spaces, GameObject obj)
    {
        for(int i = 0; i < spaces.Length; i++)
        {
            if(zones[index + spaces[i]] == null || (zones[index + spaces[i]] != null && zones[index + spaces[i]].CompareTag("Zone")))
            {
                Destroy(zones[index + spaces[i]]);
                zones[index + spaces[i]] = null;
            }

            //Set the square to not active as well.
            if (spaces[i] == 0)
            {
                zones[index] = obj;
            }
        }

        allZonesUsed = isAllUsed();
    }

    public void destroyZone(int index, int[] spaces)
    {
        for (int i = 0; i < spaces.Length; i++)
        {
            if (zones[index + spaces[i]] == null || zones[index + spaces[i]] != null)
            {
                Destroy(zones[index + spaces[i]]);
                zones[index + spaces[i]] = null;
            }

            //Set the square to not active as well.
            if (spaces[i] == 0)
            {
                zones[index] = null;
            }
        }

        allZonesUsed = isAllUsed();
    }

    //Remove a single wall.
    public void removeWalls(int index)
    {
        //Go through zones and find one that matches the wall.
        for(int i = 0; i < zones.Length; i++)
        {
            if(GameObject.ReferenceEquals(zones[i], walls[index]))
            {
                int[] sp = { 0 };
                destroyZone(i, sp);
                walls[index] = null;
            }
        }
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
    public void addWall(GameObject obj, int index, int angle, bool instantiated)
    {
        if (instantiated)
        {
            walls[index - 1] = obj;
            obj.transform.parent = transform;
            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.Euler(0, angle, 0);
        } else
        {
            walls[index - 1] = Instantiate(obj, transform.position, Quaternion.Euler(0, angle, 0), transform);
        }

        int ind = 0;
        //Now remove all zones of that wall.
        if(index - 1 == 0)
        {
            //Set up the spaces to use.
            ind = 0;
            int[] spaces = { 0, 1, 2, 3, 4 };
            removeZone(ind, spaces, walls[index - 1]);
        } else if (index - 1 == 1)
        {
            ind = 4;
            int[] spaces = { 0, 5, 10, 15, 20 };
            removeZone(ind, spaces, walls[index - 1]);
        } else if (index - 1 == 2)
        {
            ind = 24;
            int[] spaces = { 0, -1, -2, -3, -4 };
            removeZone(ind, spaces, walls[index - 1]);
        } else if (index - 1 == 3)
        {
            ind = 20;
            int[] spaces = { 0, -5, -10, -15, -20 };
            removeZone(ind, spaces, walls[index - 1]);
        }
    }

    public void addPath(GameObject obj)
    {
        List<int> pathPoints = new List<int>();
        int amount = 0;
        //First, decide which 'walls' need paths through them.
        for(int i = 0; i < walls.Length; i++)
        {
            if(walls[i] == null)
            {
                pathPoints.Add(i);
                amount++;
            }
        }

        //Go through and make paths starting from each end point and ending on the final point, which is the middle.
        for(int i = 0; i < pathPoints.Count; i++)
        {
            if(pathPoints[i] == 0)
            {
                zones[2] = Instantiate(obj, zones[2].transform.position, Quaternion.identity, transform);
                zones[7] = Instantiate(obj, zones[7].transform.position, Quaternion.identity, transform);
                zones[12] = Instantiate(obj, zones[12].transform.position, Quaternion.identity, transform);
            } else if (pathPoints[i] == 1)
            {
                zones[14] = Instantiate(obj, zones[14].transform.position, Quaternion.identity, transform);
                zones[13] = Instantiate(obj, zones[13].transform.position, Quaternion.identity, transform);
                zones[12] = Instantiate(obj, zones[12].transform.position, Quaternion.identity, transform);
            } else if(pathPoints[i] == 2)
            {
                zones[10] = Instantiate(obj, zones[10].transform.position, Quaternion.identity, transform);
                zones[11] = Instantiate(obj, zones[11].transform.position, Quaternion.identity, transform);
                zones[12] = Instantiate(obj, zones[12].transform.position, Quaternion.identity, transform);
            } else if (pathPoints[i] == 3)
            {
                zones[22] = Instantiate(obj, zones[22].transform.position, Quaternion.identity, transform);
                zones[17] = Instantiate(obj, zones[17].transform.position, Quaternion.identity, transform);
                zones[12] = Instantiate(obj, zones[12].transform.position, Quaternion.identity, transform);
            }
        }
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

    public int getZoneIndex()
    {
        GameObject z = null;

        int randZone = 0;

        while(z == null || z.CompareTag("Zone"))
        {
            randZone = UnityEngine.Random.Range(0, zones.Length - 1);

            z = zones[randZone];
        }

        return randZone;
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
            if (zones[i] != null && zones[i].CompareTag("Zone"))
            {
                used = false;
            }
        }

        return used;
    }

    public int findAvailable(int[] spaces)
    {
        //Hold all available indexes.
        List<int> availableZones = new List<int>();

        //Go through all possible zones.
        for (int i = 0; i < zones.Length; i++){
            bool available = true;
            //Go through all the valid zones based on how much space is being taken. Then try to spawn.
            for(int x = 0; x < spaces.Length; x++)
            {
                if((i + spaces[x]) < 0 || (i + spaces[x]) > zones.Length - 1 || zones[i + spaces[x]] == null || (zones[i + spaces[x]] != null && !zones[i + spaces[x]].CompareTag("Zone")))
                {
                    available = false;
                }
            }

            if (available)
            {
                availableZones.Add(i);
            }
        }

        //Randomly choose a zone.
        if(availableZones.Count > 0)
        {
            int rand = Random.Range(0, availableZones.Count);

            return availableZones[rand];
        } else
        {
            return -1;
        }
    }
}

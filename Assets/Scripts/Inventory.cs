using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Keep information on the ghost weapons.
    public GameObject[] ghostWeapons;

    public int[] resources;


    // Start is called before the first frame update
    void Start()
    {
        ghostWeapons = new GameObject[10];

        resources = new int[System.Enum.GetValues(typeof(pickUps.resourceTypes)).Length];
    }

    /*
     * A function that can be called and add an weapon to the weapons list.
     */
    public void addToWeaponsInventory(GameObject obj)
    {
        //Get the weapon script to work with.
        Weapon weap = obj.GetComponent<Weapon>();

        //If the weapon type is onHand OR offHand, add them to ghost list.
        if(weap.type == Weapon.weaponType.onHand)
        {
            for(int i = 0; i < ghostWeapons.Length; i++)
            {
                if(ghostWeapons[i] == null)
                {
                    ghostWeapons[i] = obj;
                    return;
                }
            }
        }
    }

    public void addToResourceInventory(pickUps.resourceTypes index, int amount)
    {
        if((int)index < resources.Length && index >= 0)
        {
            resources[(int)index] += amount;
        }
    }

    public int getInventory(pickUps.resourceTypes ind)
    {
        return resources[(int)ind];
    }

    public void removeResource(pickUps.resourceTypes ind, int amount)
    {
        resources[(int)ind] -= amount;
    }
}

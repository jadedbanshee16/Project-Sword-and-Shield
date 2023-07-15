using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Keep information on the ghost weapons.
    public GameObject[] ghostWeapons;


    // Start is called before the first frame update
    void Start()
    {
        ghostWeapons = new GameObject[10];
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
}

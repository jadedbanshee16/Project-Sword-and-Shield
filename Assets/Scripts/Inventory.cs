using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Keep information on the ghost weapons.
    public Weapon[] ghostWeapons;

    public int[] resources;

    public Weapon currentOnHand;
    public Weapon currentOffHand;

    public GameObject onHandInvent;
    public GameObject offHandInvent;

    public void setUpInventory()
    {
        onHandInvent = GameObject.FindGameObjectWithTag("OnHandUI");
        offHandInvent = GameObject.FindGameObjectWithTag("OffHandUI");

        //Set the first and second weapons.
        switchWeapons(0);
        switchWeapons(1);
        //offHandInvent = GameObject.Find("ActiveOffHand");

        resources = new int[System.Enum.GetValues(typeof(pickUps.resourceTypes)).Length];
    }

    /*
     * A function that can be called and add an weapon to the weapons list.
     */
    public void addToWeaponsInventory(GameObject obj)
    {
        //Get the weapon script to work with.
        Weapon weap = obj.GetComponent<Weapon>();

        for (int i = 0; i < ghostWeapons.Length; i++)
        {
            if (ghostWeapons[i] == null)
            {
                ghostWeapons[i] = obj.GetComponent<Weapon>();
                return;
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

    public void switchWeapons(int index)
    {
        if(ghostWeapons[index].getWeaponType() == Weapon.weaponType.onHand)
        {
            currentOnHand = ghostWeapons[index];
            onHandInvent.GetComponent<SetInventoryImage>().setImage(currentOnHand.getInventoryImage());
        } else if (ghostWeapons[index].getWeaponType() == Weapon.weaponType.offHand)
        {
            currentOffHand = ghostWeapons[index];
            offHandInvent.GetComponent<SetInventoryImage>().setImage(currentOffHand.getInventoryImage());
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

    public void useOnHand(Vector3 pPos, Vector3 mPos)
    {
        currentOnHand.GetComponent<Weapon>().use(mPos, pPos);
    }

    public void stopUseOnHand()
    {
        currentOnHand.GetComponent<Weapon>().stopUse();
    }

    public void useOffHand(Vector3 pPos, Vector3 mPos)
    {
        currentOffHand.GetComponent<Weapon>().use(mPos, pPos);
    }

    public void stopUseOffHand()
    {
        currentOffHand.GetComponent<Weapon>().stopUse();
    }

}

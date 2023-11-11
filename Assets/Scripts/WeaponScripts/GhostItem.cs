using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostItem : MonoBehaviour
{
    public enum weaponType
    {
        onHand,
        offHand,
        bellonHand
    }

    [Header("Weapon Info")]
    public weaponType type;
    public string idName;
    public Sprite inventoryImage;

    [Header("Weapon Stats")]
    [SerializeField]
    [Tooltip("How long before weapon can be activated again")]
    private float cooldown;
    [SerializeField]
    [Tooltip("How much manna it takes to activate weapon.")]
    private float cost;
    [SerializeField]
    [Tooltip("How far the object can / will be from the player")]
    public float ghostRange;
    [SerializeField]
    [Tooltip("How much damage the weapon does on contact with enemy")]
    private float damage;
    [SerializeField]
    [Tooltip("Stun modifier")]
    private float stun;
    [SerializeField]
    [Tooltip("Pushback modifier")]
    private float pushBack;
    [SerializeField]
    [Tooltip("How far the weapon can travel after being activated")]
    private float weaponRange;
    [SerializeField]
    [Tooltip("A modifier that can change aspects of the weapon.")]
    public float originalHoldModifier;

    private float holdModifier;

    //Functions
    public virtual void use(Vector3 mousePos, Vector3 player)
    {
        //Something when a generic item is used.
    }

    public virtual void stopUse()
    {
        //To stop using hold button weapons or revert modifiers from holding buttons.
        holdModifier = originalHoldModifier;
    }

    //References
    public string getIDName()
    {
        return idName;
    }

    public void setIDName(string s)
    {
        idName = s;
    }

    public weaponType getWeaponType()
    {
        return type;
    }

    public Sprite getInventoryImage()
    {
        return inventoryImage;
    }

    public float getHoldModifier()
    {
        return holdModifier;
    }

    public float getpushBack()
    {
        return pushBack;
    }

    public float getStun()
    {
        return stun;
    }

    public float getDamage()
    {
        return damage;
    }

    public void setHoldModifier(float num)
    {
        holdModifier = num;
    }
}

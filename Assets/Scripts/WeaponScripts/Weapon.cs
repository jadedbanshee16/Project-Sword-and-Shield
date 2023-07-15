using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum weaponType
    {
        onHand,
        offHand,
        bellonHand
    }

    //Fields
    public weaponType type;
    public string idName;

    //Functions
    public virtual void use()
    {
        //Something when a generic item is used.
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
}

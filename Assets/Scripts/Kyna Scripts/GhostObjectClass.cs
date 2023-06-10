using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObjectClass : MonoBehaviour
{
    //This keeps the type of object this particular object is.
    public enum Objecttype
    {
        idle,
        sword,
        shield
    }

    //An enum to decide if this object is either null, offhand, or onHand.
    public enum handStatus
    {
        nullStatus,
        onHandStatus,
        offHandStatus
    }

    public Objecttype type;
    public handStatus stat;

    /*
     * A class to move the user in a certain way.
     * This can be a teleport or moving slowly over time, depending on how the sub / main class deals with this.
     */
    public virtual Vector3 Move(float speed, Vector3 TargetPos, Vector3 currentPos)
    {
        //Base Move should move the playter via speed to the new position.
        Vector3 NDir = Vector3.Normalize(TargetPos - currentPos);
        currentPos += NDir * (speed * Time.deltaTime);
        return currentPos;
    }

    /*
     * A function to test the working of this particular class.
     */
    public virtual void checkWorking()
    {
        Debug.Log("Working in Object CLass");
    }
}

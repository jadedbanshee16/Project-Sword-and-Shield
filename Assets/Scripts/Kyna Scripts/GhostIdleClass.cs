using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostIdleClass : GhostObjectClass
{
    public override Vector3 Move(float speed, Vector3 TargetPos, Vector3 currentPos)
    {
        //Base Move should move the playter via speed to the new position.
        Vector3 NDir = Vector3.Normalize(TargetPos - currentPos);
        currentPos += NDir * (speed * Time.deltaTime);
        return currentPos;
    }

    public override void checkWorking()
    {
        Debug.Log("Working in Idle Class");
    }
}

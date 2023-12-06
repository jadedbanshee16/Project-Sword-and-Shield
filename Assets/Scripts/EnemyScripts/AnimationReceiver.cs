using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    private void OnAnimatorMove()
    {
        //Call the onAnimatorMove on the parent.
        this.transform.parent.GetComponent<EnemyClass>().OnAnimatorMove();
    }

    private void startAttack()
    {
        this.transform.parent.GetComponent<EnemyClass>().attack();
    }

    private void endAttack()
    {
        this.transform.parent.GetComponent<EnemyClass>().stopAttacking();
    }
}

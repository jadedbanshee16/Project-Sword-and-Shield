using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldClass : WeaponClass
{
    // Update is called once per frame
    public override void useWeapon()
    {
        if (getisAction())
        {
            transform.position = getPos();
            transform.LookAt(getDir());
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

    public override void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st)
    {
        base.setWeapon(d, p, dmg, psh, st);

        this.transform.LookAt(getDir());
    }

    public override void stopWeapon()
    {
        base.stopWeapon();
    }

    //If the object hits an enemy.
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Vector3 dmgMetric = getDamageMetrics();
            //ALWAYS put the collider object as child.
            other.transform.parent.GetComponent<EnemyClass>().takeDamage(dmgMetric.x, dmgMetric.y, dmgMetric.z, getDir());
        }
    }
}

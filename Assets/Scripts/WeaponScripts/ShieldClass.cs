using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldClass : WeaponClass
{
    // Update is called once per frame
    public override void useWeapon(Vector3 pos, Vector3 dir)
    {
        if (getisAction())
        {
            transform.position = pos;
            transform.LookAt(dir);
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

    public override void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float ct)
    {
        base.setWeapon(d, p, dmg, psh, st, ct);

        this.transform.LookAt(getDir());
    }

    //If the object hits an enemy.
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Vector3 dmgMetric = getDamageMetrics();
            //ALWAYS put the collider object as child.
            other.GetComponent<EnemyClass>().takeDamage(dmgMetric.x, dmgMetric.y, dmgMetric.z, (getDir() - transform.position).normalized, getCost());

            stopWeapon(true);

            _audioManager.playSound(AudioManager.audioType.weaponAudio, 2);
        }
    }
}

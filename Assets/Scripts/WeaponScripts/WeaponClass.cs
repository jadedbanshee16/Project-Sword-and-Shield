using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 pos;
    private bool action = true;

    private float damage;
    private float pushback;
    private float stun;

    private float cost;

    // Update is called once per frame
    void FixedUpdate()
    {
        useWeapon(pos, direction);
    }

    public virtual void useWeapon(Vector3 d, Vector3 p)
    {
        //Use this weapon is working.
    }

    public virtual void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float ct)
    {
        direction = d;
        pos = p;

        damage = dmg;
        pushback = psh;
        stun = st;

        cost = ct;

        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;

        action = true;
    }

    //An override for projectile based weapons.
    public virtual void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float ct, float time, float speed)
    {
        direction = d;
        pos = p;

        damage = dmg;
        pushback = psh;
        stun = st;

        cost = ct;

        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;

        action = true;
    }

    //An override for enemy based weapons.
    public virtual void setWeapon(float dmg)
    {
        damage = dmg;
    }

    public void changePositions(Vector3 dir, Vector3 ps)
    {
        direction = dir;
        pos = ps;
    }

    public virtual void stopWeapon()
    {
        action = false;
    }

    public Vector3 getDir()
    {
        return direction;
    }

    public Vector3 getPos()
    {
        return pos;
    }

    public float getCost()
    {
        return cost;
    }

    public bool getisAction()
    {
        return action;
    }

    public void setAction(bool b)
    {
        action = b;
    }

    public Vector3 getDamageMetrics()
    {
        return new Vector3(damage, pushback, stun);
    }

    public void setDamageMetrics(float d, float p, float s)
    {
        damage = d;
        pushback = p;
        stun = s;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Vector3 dmgMetric = getDamageMetrics();
            //ALWAYS put the collider object as child.
            other.GetComponent<EnemyClass>().takeDamage(dmgMetric.x, dmgMetric.y, dmgMetric.z, getDir(), cost);
        }
    }
}

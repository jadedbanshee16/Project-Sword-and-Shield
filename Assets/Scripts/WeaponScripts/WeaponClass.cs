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

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Using the update");
        useWeapon();
    }

    public virtual void useWeapon()
    {
        //Use this weapon is working.
    }

    public virtual void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st)
    {
        direction = d;
        pos = p;

        damage = dmg;
        pushback = psh;
        stun = st;

        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;

        action = true;
    }

    //An override for projectile based weapons.
    public virtual void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float time, float speed)
    {
        direction = d;
        pos = p;

        damage = dmg;
        pushback = psh;
        stun = st;

        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;

        action = true;
    }

    public virtual void stopWeapon()
    {
        action = false;
    }

    public Vector3 getDir()
    {
        return direction;
    }

    public void setDir(Vector3 d)
    {
        direction = d;
    }

    public Vector3 getPos()
    {
        return pos;
    }

    public void setPos(Vector3 p)
    {
        pos = p;
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
}

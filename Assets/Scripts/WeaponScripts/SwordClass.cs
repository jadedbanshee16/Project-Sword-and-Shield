using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordClass : WeaponClass
{
    private float time;
    private float timer;
    private float speed;

    // Update is called once per frame
    public override void useWeapon()
    {
        if (getisAction())
        {
            if(timer > 0)
            {
                //Move in given direction.
                this.transform.position += getDir() * Time.deltaTime * speed;

                //Change the rotation of the swinged object over time.
                transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(Vector3.left, Vector3.up), Quaternion.LookRotation(Vector3.right, Vector3.up), (timer / time));


                timer -= Time.deltaTime;
            } else
            {
                setAction(false);
                this.gameObject.SetActive(false);
            }
        }
    }

    public override void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float t, float sp)
    {
        setDir(d);
        setPos(p);

        setDamageMetrics(dmg, psh, st);

        time = t;
        timer = time;
        speed = sp;
        this.transform.localPosition = getPos();
        this.transform.rotation = Quaternion.identity;

        setAction(true);
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

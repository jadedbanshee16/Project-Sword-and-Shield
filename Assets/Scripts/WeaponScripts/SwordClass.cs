using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordClass : WeaponClass
{
    private float time;
    private float timer;
    private float speed;

    // Update is called once per frame
    public override void useWeapon(Vector3 dir, Vector3 pos)
    {
        if (getisAction())
        {
            if(timer > 0)
            {
                //Move in given direction.
                this.transform.position += pos * Time.deltaTime * speed;

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

    public override void setWeapon(Vector3 d, Vector3 p, float dmg, float psh, float st, float ct, float t, float sp)
    {
        base.setWeapon(d, p, dmg, psh, st, ct);

        time = t;
        timer = time;
        speed = sp;
        this.transform.localPosition = getPos();
        this.transform.rotation = Quaternion.identity;

        setAction(true);

        _audioManager.playSound(AudioManager.audioType.weaponAudio, 1);
    }

    //If the object hits an enemy.
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        _audioManager.playSound(AudioManager.audioType.weaponAudio, 0);
    }
}

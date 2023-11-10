using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordClass : MonoBehaviour
{

    private Vector3 direction;
    private bool action;
    private float time;
    private float timer;
    private float speed;
    private float damage;

    // Update is called once per frame
    void Update()
    {
        if (action)
        {
            if(timer > 0)
            {
                //Move in given direction.
                this.transform.position += direction * Time.deltaTime * speed;

                //Change the rotation of the swinged object over time.
                transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(Vector3.left, Vector3.up), Quaternion.LookRotation(Vector3.right, Vector3.up), (timer / time));


                timer -= Time.deltaTime;
            } else
            {
                action = false;
                this.gameObject.SetActive(false);
            }
        }
    }

    public void swipe(Vector3 d, float s, float t, float dmg)
    {
        direction = d;
        speed = s;
        time = t;
        timer = time;
        damage = dmg;

        action = true;
    }

    //If the object hits an enemy.
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //ALWAYS put the collider object as child.
            other.transform.parent.GetComponent<EnemyClass>().takeDamage(damage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordClass : WeaponClass
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 dmgMetric = getDamageMetrics();
            //ALWAYS put the collider object as child.
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>().deductHealth(getDamageMetrics().x);

            Vector3 dir = other.transform.position - this.transform.position;

            dir.y = 0;

            other.GetComponent<Rigidbody>().velocity = dir * getDamageMetrics().y;


            //this.gameObject.SetActive(false);
        }
    }
}

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
            this.gameObject.SetActive(false);
        }
    }
}

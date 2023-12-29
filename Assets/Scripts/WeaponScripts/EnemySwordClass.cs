using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordClass : WeaponClass
{
    public AudioSource _audio;
    private AudioManager _audioManager;

    private void Start()
    {
        //At the start, get audio source and audio manager.
        _audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        _audio = GetComponentInParent<AudioSource>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            Vector3 dmgMetric = getDamageMetrics();
            //ALWAYS put the collider object as child.
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>().deductHealth(getDamageMetrics().x);

            Vector3 dir = this.transform.position - other.transform.position;

            dir.y = 0;

            other.GetComponent<Rigidbody>().velocity = dir * getDamageMetrics().y * 1000;

            //Play the audio sound.
            _audioManager.playSound(_audio, AudioManager.audioType.enemyAudio, 2);
            //this.gameObject.SetActive(false);
        }
    }
}

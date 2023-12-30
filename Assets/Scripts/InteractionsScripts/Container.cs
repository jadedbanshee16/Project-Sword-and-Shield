using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    public Animator anim_;

    public float throwStrength;

    public int resourceContents;
    public int mandatoryContents;

    private bool opened = false;

    private PoolManager _manager;

    public void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>();
        _audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        _audio = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        if (!opened)
        {
            opened = true;
            //This chest will open when interacted with.
            anim_.SetBool("Opened", true);

            _audioManager.playSound(_audio, AudioManager.audioType.interactionAudio, 0);

            //Instaniate and spit out the objects on each side.
            for (int i = 0; i < mandatoryContents; i++)
            {
                Vector3 direction = randDirection();


                GameObject obj = _manager.GetPooledObject(PoolManager.objectType.key);

                obj.SetActive(true);
                obj.transform.position = direction;
                obj.transform.rotation = Quaternion.identity;
                obj.GetComponent<Rigidbody>().AddForce(direction.normalized * throwStrength);
            }

            int rand = Random.Range(0, resourceContents + 1);
            //Now instantiate the common resources, ranging from 0 to 3.
            for (int x = 0; x < rand; x++)
            {
                Vector3 direction = randDirection();

                GameObject obj = _manager.GetPooledObject(PoolManager.objectType.resource);

                obj.SetActive(true);
                obj.transform.position = direction;
                obj.transform.rotation = Quaternion.identity;
                obj.GetComponent<Rigidbody>().AddForce(direction.normalized * throwStrength);
            }
        }

    }

    private Vector3 randDirection()
    {
        Vector2 dir = Random.insideUnitCircle.normalized * 0.1f;

        return new Vector3(dir.x + transform.position.x, 0.1f, dir.y + transform.position.z);
    }
}

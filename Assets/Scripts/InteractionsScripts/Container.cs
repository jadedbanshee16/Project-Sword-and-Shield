using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    public Animator anim_;

    public float throwStrength;

    public GameObject[] resourceContents;
    public GameObject[] mandatoryContents;

    private bool opened = false;

    private PoolManager _manager;

    public void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>();
    }

    public override void Interact()
    {
        if (!opened)
        {
            opened = true;
            //This chest will open when interacted with.
            anim_.SetBool("Opened", true);

            //Instaniate and spit out the objects on each side.
            for (int i = 0; i < mandatoryContents.Length; i++)
            {
                Vector3 direction = randDirection();


                GameObject obj = _manager.GetPooledObject(PoolManager.objectType.key);

                obj.SetActive(true);
                obj.transform.position = direction;
                obj.transform.rotation = Quaternion.identity;
                obj.GetComponent<Rigidbody>().AddForce(direction.normalized * throwStrength);
            }

            //Now instantiate the common resources, ranging from 0 to 3.
            for(int x = 0; x < resourceContents.Length; x++)
            {
                int rand = Random.Range(0, 3);
                for(int v = 0; v < rand; v++)
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

    }

    private Vector3 randDirection()
    {
        Vector2 dir = Random.insideUnitCircle.normalized * 0.1f;

        return new Vector3(dir.x + transform.position.x, 0.1f, dir.y + transform.position.z);
    }
}

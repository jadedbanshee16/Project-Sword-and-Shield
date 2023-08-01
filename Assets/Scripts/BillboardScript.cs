using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{

    //Camera object.
    GameObject cam_;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(cam_ != null)
        {
            Vector3 pos = cam_.transform.position;
            pos.x = transform.position.x;
            pos.z = transform.position.z;
            transform.LookAt(pos);
        } else
        {
            findCamera();
        }

    }

    void findCamera()
    {
        cam_ = GameObject.FindGameObjectWithTag("MainCamera");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base movement for Kyna.
 */

public class KynaMovement : MonoBehaviour
{
    private Camera Maincam_;

    private Vector3 Ndir;
    private Vector3 NPos;

    public float positionOffset;

    public bool isIdle;
    private bool positionFound;

    public float maxSpeed;
    private float speed;

    //A timer for how long the character will go in a certain direction.
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        Maincam_ = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        speed = 0;

        timer = Random.Range(0, 3);

        positionFound = false;
        isIdle = true;

        //Set first position.
        Ndir = setNewPosition();
    }

    // Update is called once per frame
    void Update()
    {
        //If in idle mode, then complete the idle position.
        if (isIdle)
        {
            //Check if character is at position. If not, then move there.
            if(!positionFound)
            {
                Ndir = setNewPosition();
                transform.LookAt(NPos);
                if (speed < maxSpeed)
                {
                    speed += Time.deltaTime;
                }
            } else
            {
                if (speed > 0)
                {
                    speed -= Time.deltaTime;
                }

                Ndir = setNewPosition();
                timer = Random.Range(0, 3);
            }

            if (Vector3.Distance(transform.position, NPos) <= positionOffset)
            {
                positionFound = true;
            } else
            {
                positionFound = false;
            }

            transform.position += Ndir * (speed * Time.deltaTime);
        }
    }


    public Vector3 setNewPosition()
    {
        Vector3 dir = Vector3.zero;
        //Now get the position based on camera.
        Ray ray = Maincam_.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Now change Kyna's position to where the ray had hit. Set y to 0.
            Vector3 pos = hit.point;
            pos.y = 0;
            NPos = pos;
            dir = Vector3.Normalize(pos - transform.position);
        }

        return dir;
    }
}

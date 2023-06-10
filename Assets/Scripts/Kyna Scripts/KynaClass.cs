using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base movement for Kyna.
 */

public class KynaClass : MonoBehaviour
{
    private Camera Maincam_;
    public GameObject bareHand;
    private GhostObjectClass bareHandinstance;
    private GhostObjectClass offHandinstance;
    private GhostObjectClass onHandinstance;

    public float positionOffset;

    public float maxSpeed;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        Maincam_ = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        speed = 0;

        //Start the inventory and set the first as the bareHand obj.
        bareHandinstance = Instantiate(bareHand, transform.position, Quaternion.identity).GetComponent<GhostObjectClass>();

    }

    // Update is called once per frame
    void Update()
    {
        //Set the position to mouse.
        Vector3 NPos = setNewMousePosition();
        //Look at the position.
        transform.LookAt(NPos);
        //Find out if the transform is near the NPosition.
        bool positionFound = isNearPosition(NPos);

        //Check if character is at position. If not, then move there.
        if (!positionFound)
        {
            //Set the speed depending on heading to position or now.
            if (speed < maxSpeed)
            {
                speed += Time.deltaTime;
            }
        }
        else
        {
            //Set speed depending on head to position or not.
            if (speed > 0)
            {
                speed -= Time.deltaTime;
            }
        }

        //Set the position based on idle.
        transform.position = bareHandinstance.Move(speed, NPos, transform.position);
        //Set the bareHands idle to the mouse Position.
        bareHandinstance.gameObject.transform.position = NPos;
    }


    private Vector3 setNewMousePosition()
    {
        Vector3 dir = Vector3.zero;
        //Now get the position based on camera.
        Ray ray = Maincam_.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Now change Kyna's position to where the ray had hit. Set y to 0.
            Vector3 pos = hit.point;
            pos.y = 0;
            dir = pos;
        }

        return dir;
    }

    private bool isNearPosition(Vector3 targetPos)
    {
        //If transform is near the position.
        if (Vector3.Distance(transform.position, targetPos) <= positionOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

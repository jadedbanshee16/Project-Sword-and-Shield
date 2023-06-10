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

    //Movement variables.
    public float positionOffset;
    public float maxSpeed;
    private float speed;

    //Rotation variables.
    public float rotateSpeed = 5f;

    private Vector3 targetPosition;

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
        //Look at the position.
        lookAt(targetPosition);
        //Find out if the transform is near the NPosition.
        bool positionFound = isNearPosition(targetPosition);

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
        transform.position = bareHandinstance.Move(speed, targetPosition, transform.position);
        //Set the bareHands idle to the mouse Position.
        bareHandinstance.gameObject.transform.position = targetPosition;
    }

    public void setTargetPos(Vector3 pos)
    {
        targetPosition = pos;
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

    private void lookAt(Vector3 pos)
    {
        // Determine which direction to rotate towards
        Vector3 targetDir = pos - transform.position;

        // The step size is equal to speed times frame time.
        float step = rotateSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * This enemy is a GUARD type.
 * Attributes:
 * Uses waypoints
 * Fluid movements (no waiting around)
 * Attempts to get in range of player when seeing them.
 * When in range, complete an attack.
 * If lose sight of player, attempt to find them by going to last known position and looking around.
 * If not seeing player, return to waypoint path.
 */
public class GuardEnemy : EnemyClass
{
    public List<GameObject> checkPoints;

    private int currentCheckPoint;

    private Quaternion originalRot;

    [Header("Patrol Variables")]
    [SerializeField]
    private float patrolSpeed;

    private bool changePosition = false;
    private Vector3 newPosition;

    // Start is called before the first frame update
    void Start()
    {
        //Complete set up in main part.
        base.Start();

        //Debug.Log("Working?");

        GameObject[] points = GameObject.FindGameObjectsWithTag("Waypoint");

        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].GetComponent<EnemySpawn>().getIsland() == currentIsland)
            {
                checkPoints.Add(points[i]);
            }
        }

        if (checkPoints.Count > 0)
        {
            changedTargetLocation = checkPoints[0].transform.position;
            originalRot = transform.rotation;
        }
    }

    public override void move()
    {
        speed = patrolSpeed;

        //Debug.Log(_currentPath.corners.Length + " | " + currentCorner);
        //If near a waypoint...
        if (Vector3.Distance(transform.position, currentTargetLocation) < agentRange && checkPoints.Count > 1)
        {
            //Debug.Log("Within location");
            //_anim.SetBool("Turning", true);
            //Turn until position is changed.
            if (!changePosition)
            {
                //Increment to next checkpoint, ensuring that if it was the last checkpoint it will return to checkpoint 0.
                currentCheckPoint = (currentCheckPoint + 1) % checkPoints.Count;

                changePosition = true;
                waitTimer = Random.Range(5, 10);
            }

            //Get the direction of the next position.
            newPosition = (checkPoints[currentCheckPoint].transform.position - transform.position);

            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                _anim.SetBool("Idle", true);
            }
            else
            {
                _anim.SetBool("Idle", false);
                rotateTo(newPosition);

                //When facing the new point, start moving.
                if (Vector3.Angle(newPosition, transform.forward) <= angleTo)
                {
                    //animator.SetBool("Turning", false);
                    _anim.SetBool("Idle", false);

                    changedTargetLocation = checkPoints[currentCheckPoint].transform.position;
                    //Move the destination.
                    //_agent.SetDestination(currentTargetLocation);

                    //Ensure robot is always facing the person / position.
                    this.transform.LookAt(checkPoints[currentCheckPoint].transform.position);

                    changePosition = false;
                }
            }

        }
        else if (Vector3.Distance(transform.position, currentTargetLocation) < agentRange && checkPoints.Count <= 1)
        {
            //If there is only ONE checkpoint. Set to idle.
            /*
             * NOTE: Always have the bot start at the checkpoint if there is only 1 checkpoint.
             */
            _anim.SetBool("Idle", true);
            //m_animator.SetBool("Turning", false);
            transform.rotation = originalRot;
        }
    }
}

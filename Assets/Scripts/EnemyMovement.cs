using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    private Animator m_animator;

    [Header("Checkpoints")]
    //Keep an array of checkpoints.
    public Transform[] checkPoints;

    [Header("Rotation variables")]
    public float rotationSpeed;
    public float angleTo;

    [Header("NavMesh variables")]
    public float patrolSpeed = 3.5f;
    public float attackSpeed = 4.5f;


    int m_currentCheckPoint = 0;
    private bool changePosition = false;
    private bool searchActivate = false;


    float searchTime = 0;
    float searchTimer = 0;
    float turnTime = 0;
    float turnTimer = 0;

    private Vector3 newPosition;
    private Quaternion originalRot;
    // Start is called before the first frame update
    void Start()
    {
        if(checkPoints.Length > 0)
        {
            navMeshAgent.SetDestination(checkPoints[0].position);
            originalRot = transform.rotation;
        }

        m_animator = GetComponent<Animator>();
        m_animator.SetBool("Idle", false);
    }

    public void patrolMove()
    {
        navMeshAgent.speed = patrolSpeed;

        //If near a waypoint...
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && checkPoints.Length > 1)
        {

            m_animator.SetBool("Turning", true);
            if (!changePosition)
            {
                //Increment to next checkpoint, ensuring that if it was the last checkpoint it will return to checkpoint 0.
                m_currentCheckPoint = (m_currentCheckPoint + 1) % checkPoints.Length;

                changePosition = true;
            }


            //Get the direction of the next position.
            newPosition = (checkPoints[m_currentCheckPoint].position - transform.position);

            rotateTo(newPosition);

            //When facing the new point, start moving.
            if (Vector3.Angle(newPosition, transform.forward) <= angleTo)
            {
                m_animator.SetBool("Turning", false);
                m_animator.SetBool("Idle", false);
                //Move the destination.
                navMeshAgent.SetDestination(checkPoints[m_currentCheckPoint].position);

                //Ensure robot is always facing the person / position.
                this.transform.LookAt(checkPoints[m_currentCheckPoint].position);

                changePosition = false;
            }

        } else if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && checkPoints.Length == 1)
        {
            //If there is only ONE checkpoint. Set to idle.
            /*
             * NOTE: Always have the bot start at the checkpoint if there is only 1 checkpoint.
             */
            m_animator.SetBool("Idle", true);
            m_animator.SetBool("Turning", false);
            transform.rotation = originalRot;
        }
    }

    public bool attackMove(Vector3 target, bool onEnemy)
    {
        navMeshAgent.speed = attackSpeed;
        navMeshAgent.SetDestination(target);
        if (onEnemy)
        {
            //Ensure robot is always facing the person / position.
            this.transform.LookAt(target);
        }

        //Something to see if the attacker has met the enemy or a location they were in.
        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !onEnemy)
        {
            bool search = searchMove();
            navMeshAgent.speed = 0;
            //navMeshAgent.isStopped = true;
            searchActivate = false;

            return search;

        } else
        {
            m_animator.SetBool("Turning", false);
            m_animator.SetBool("Idle", false);
            searchActivate = true;
            //navMeshAgent.isStopped = false;
            return false;
        }

    }

    private bool searchMove()
    {
        if (searchActivate)
        {
            //Set a certain length to search.
            searchTime = Random.Range(5, 20);
            searchActivate = false;
        }
        m_animator.SetBool("Turning", true);

        rotateTo(newPosition);

        //When facing the new point, start moving.
        if (Vector3.Angle(newPosition, transform.forward) <= angleTo)
        {
            //Make a random length of time before it changes direction to look at.
            if(turnTimer < turnTime)
            {
                turnTimer += Time.deltaTime;
            } else
            {
                turnTime = Random.Range(3, 6);
                turnTimer = 0;
                newPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            }
            m_animator.SetBool("Turning", false);
            m_animator.SetBool("Idle", true);
        }

        //Increment time. when timer ends, return to patrol mode.
        if(searchTimer < searchTime)
        {
            searchTimer += Time.deltaTime;
            return false;
        } else
        {
            searchTimer = 0;
            navMeshAgent.SetDestination(checkPoints[m_currentCheckPoint].position);
            return true;
        }
    }

    private void rotateTo(Vector3 pos)
    {
        //Turn the creature around until it is facing that direction.
        pos.y = 0;

        Quaternion rot = Quaternion.LookRotation(pos);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }
}

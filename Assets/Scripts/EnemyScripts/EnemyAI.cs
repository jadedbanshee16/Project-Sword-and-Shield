using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private EnemyMovement m_patrolScript;

    bool m_playerInRange;

    bool attackMode;

    public Color neutralCol;
    public Color alertCol;

    public GameObject[] lights;

    private Vector3 m_TargetLoc;

    private bool onEnemy;

    // Start is called before the first frame update
    void Start()
    {
        m_patrolScript = GetComponentInParent<EnemyMovement>();
        changeMode(false);
    }

    // Update is called once per frame
    void Update()
    {
        //If not attackMode, move along patrol Path.
        if (!attackMode)
        {
            m_patrolScript.patrolMove();
        } else
        {
            if(m_patrolScript.attackMove(m_TargetLoc, onEnemy))
            {
                changeMode(false);
            }
        }


        //If within range, then calculate if there's a wall between them.
        if (m_playerInRange)
        {
            if (seePlayer())
            {
                changeMode(true);
                m_TargetLoc = player.transform.position;
                onEnemy = true;
            } else
            {
                onEnemy = false;
            }
        }
    }

    //Change the attackMode and lights depending on mode.
    private void changeMode(bool val)
    {
        //If true, then enter attack mode.
        if (val)
        {
            attackMode = true;
            for(int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<ChangeLightColour>().changeLightColour(alertCol);
            }
        //If false, then return to patrol mode.
        } else
        {
            attackMode = false;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<ChangeLightColour>().changeLightColour(neutralCol);
            }
        }
    }

    //A function to see if player is behind cover when spotted.
    private bool seePlayer()
    {
        //Get direction.
        Vector3 dir = player.position - transform.position + Vector3.up;

        //Create a ray between transform and player in that direction.
        Ray ray = new Ray(transform.position, dir);
        //Create a rayCast hit object to put information in.
        RaycastHit rayHit;

        //Now see if there is anything in the way. We create a raycastHit giving information on when it hits something.
        if (Physics.Raycast(ray, out rayHit))
        {
            //Now, if the collider of the rayHit belongs to player, then there's nothing in between.
            //You hit the player.
            if (rayHit.collider.transform == player)
            {
                return true;
            }

        }

        return false;
    }

    //Check if player is within point of view.
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            m_playerInRange = true;
        }
    }

    //Check if player leaves point of view.
    private void OnTriggerExit(Collider other)
    {
        if(other.transform == player)
        {
            m_playerInRange = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    private float health;

    private Rigidbody _rig;
    private PlayerClass _playerStats;

    private NavMeshAgent _agent;

    private int currentIsland;
    private Transform[] validPos;

    private Vector3 currentPos;

    private void Start()
    {
        _rig = transform.GetComponent<Rigidbody>();
        _playerStats = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>();
        _agent = transform.GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(currentPos, transform.position) > 0.1)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
            currentPos = hit.position;

            _agent.SetDestination(currentPos);
        }
    }

    public void takeDamage(float dmg, float pushBack, float stun, Vector3 direction, float cost)
    {
        //Do Pushback.

        _rig.velocity = direction * pushBack;

        //Do damage.
        health -= dmg;

        if(health <= 0)
        {
            Destroy(this.gameObject);
        }

        _playerStats.deductStamina(cost);
    }

    public void getValidPositions(Transform[] pos, int isl)
    {
        validPos = new Transform[pos.Length];

        for(int i = 0; i < validPos.Length; i++)
        {
            validPos[i] = pos[i];
        }

        currentIsland = isl;
    }
}

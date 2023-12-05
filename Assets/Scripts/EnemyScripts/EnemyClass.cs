using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    private float health;

    [Header("Enemy Detection Variables")]
    [SerializeField]
    private float enemyRange;
    [SerializeField]
    private float enemyAwareness;

    [Header("Attack Variables")]
    [SerializeField]
    private float weaponRange;
    [SerializeField]
    private float weaponCooldown;
    [SerializeField]
    private float damage;
    [SerializeField]
    private GameObject weapon;

    [Header("NavAgent Variables")]
    [SerializeField]
    private float agentRange;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float angularSpeed;

    private Rigidbody _rig;
    private PlayerClass _playerStats;
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _anim;

    private int currentIsland;
    private Transform[] validPos;

    private bool chasingPlayer;
    private bool hasAttacked;
    private bool isSearching;

    public float attackTimer;
    private float searchTimer;

    private void Start()
    {
        _rig = transform.GetComponent<Rigidbody>();
        _playerStats = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>();
        _agent = transform.GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _anim = GetComponentInChildren<Animator>();
        weapon.GetComponent<WeaponClass>().setWeapon(damage);
        weapon.SetActive(false);

        chasingPlayer = false;
        hasAttacked = false;
        isSearching = false;
        _agent.SetDestination(transform.position);
        _agent.speed = speed;
        _agent.angularSpeed = angularSpeed;

        _anim.SetBool("Idle", true);
    }

    private void FixedUpdate()
    {
        if (!chasingPlayer && !hasAttacked)
        {
            chasingPlayer = findPlayer();

            //Have the moving thing.
            move();
        } else if (chasingPlayer)
        {
            chase();
        }

        //Make enemy idle when near the position.
        if(Vector3.Distance(transform.position, _agent.destination) < agentRange)
        {
            _agent.isStopped = true;
            _anim.SetBool("Idle", true);
        } else
        {
            _agent.isStopped = false;
            _anim.SetBool("Idle", false);
        }

        //A cooldown to ensure the attack happens once for a short period of time.
        if (hasAttacked)
        {
            if(attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            } else
            {
                hasAttacked = false;
                weapon.SetActive(false);
                _anim.ResetTrigger("Attack");
            }
        }

        //A cooldown to ensure enemy returns to move if no longer chasing the enemy.
        if (isSearching)
        {
            if(searchTimer > 0)
            {
                searchTimer -= Time.deltaTime;
            } else
            {
                isSearching = false;
                chasingPlayer = false;
            }
        } else
        {
            isSearching = false;
        }
    }

    public void move()
    {
        _anim.SetBool("Idle", false);

        if (Vector3.Distance(_agent.destination, transform.position) < agentRange)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
            Vector3 currentPos = hit.position;

            _agent.SetDestination(currentPos);
        }
    }

    public void chase()
    {
        if (Vector3.Distance(transform.position, _player.position) > weaponRange)
        {
            Debug.Log("Chasing player: " + Vector3.Distance(transform.position, _player.position));
            _agent.SetDestination(_player.position);
        }
        else
        {
            if (!hasAttacked)
            {
                Debug.Log("Attacked");
                attack();
            }
        }
    }

    public void attack()
    {
        hasAttacked = true;
        attackTimer = weaponCooldown;
        weapon.SetActive(true);
        _anim.SetTrigger("Attack");
    }

    public bool findPlayer()
    {
        bool playerFound = false;

        //Check if player is near enemy.
        if (Vector3.Distance(_player.position, transform.position) < enemyRange)
        {
            Debug.Log("Player within range");
            //Check if player is infront.
            if(Vector3.Dot((_player.position - transform.position), transform.forward) > enemyAwareness)
            {
                Debug.Log("Player infront of enemy");
                RaycastHit hit;

                //Check if there is a wall between player and enemy.
                if(Physics.Raycast(transform.position, _player.position - transform.position, out hit, enemyRange))
                {
                    if (hit.collider.tag == "Player")
                    {
                        Debug.Log("Player not behind object");
                        playerFound = true;
                    }
                }
            }
        }

        return playerFound;
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

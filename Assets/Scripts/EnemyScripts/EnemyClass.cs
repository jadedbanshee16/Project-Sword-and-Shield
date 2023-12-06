using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected float health;

    [Header("Enemy Detection Variables")]
    [SerializeField]
    protected float enemyRange;
    [SerializeField]
    protected float detectionShock;

    [Header("Attack Variables")]
    [SerializeField]
    protected float weaponRange;
    [SerializeField]
    protected float weaponCooldown;
    [SerializeField]
    protected Vector3 damage;
    [SerializeField]
    protected GameObject weapon;

    [Header("NavAgent Variables")]
    [SerializeField]
    protected float agentRange;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float angularSpeed;
    [SerializeField]
    private float rotationSpeed;

    [Header("Other Variables")]
    [SerializeField]
    private GameObject[] loot;

    protected Rigidbody _rig;
    protected PlayerClass _playerStats;
    protected NavMeshAgent _agent;
    protected Transform _player;
    protected Animator _anim;

    public int currentIsland;
    protected Transform[] validPos;

    protected bool foundPlayer;
    protected bool chasingPlayer;
    protected bool hasAttacked;

    protected float attackTimer;
    protected float waitTimer;
    protected float shockTimer;

    protected void Start()
    {
        _rig = transform.GetComponent<Rigidbody>();
        _playerStats = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>();
        _agent = transform.GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _anim = GetComponentInChildren<Animator>();
        weapon.GetComponent<WeaponClass>().setWeapon(damage.x, damage.y, damage.z);
        weapon.SetActive(false);

        chasingPlayer = false;
        hasAttacked = false;
        _agent.SetDestination(transform.position);
        _agent.speed = speed;
        _agent.angularSpeed = angularSpeed;

        _anim.SetBool("Idle", true);
    }

    private void FixedUpdate()
    {
        if (foundPlayer)
        {
            //Stop the movement and watch the player.
            if(shockTimer > 0)
            {
                _anim.SetBool("Idle", true);
                _agent.SetDestination(this.transform.position);
                shockTimer -= Time.deltaTime;
            } else
            {
                chasingPlayer = true;
            }
        } else
        {
            move();
        }

        if (chasingPlayer)
        {
            chase();
        }

        //Make enemy idle when near the position.
        if (Vector3.Distance(transform.position, _agent.destination) < agentRange)
        {
            _agent.isStopped = true;
            //_anim.SetBool("Idle", true);

  
        } else
        {
            _agent.isStopped = false;
            _anim.SetBool("Idle", false);
        }

        /*//A cooldown to ensure the attack happens once for a short period of time.
        if (hasAttacked)
        {
            if(attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            } else
            {
                hasAttacked = false;
            }
        }*/
    }

    public virtual void move()
    {
        _anim.SetBool("Idle", false);

        _agent.speed = speed;

        if (Vector3.Distance(_agent.destination, transform.position) < agentRange)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 5;

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
            //Debug.Log("Chasing player: " + Vector3.Distance(transform.position, _player.position));
            _agent.SetDestination(_player.position);
            this.transform.LookAt(new Vector3(_player.position.x, 0, _player.position.z));
        }
        else
        {
            if (!hasAttacked)
            {
                //Debug.Log("Attacked");
                attack();
            }
        }
    }

    public void attack()
    {
        weapon.SetActive(true);
        _anim.applyRootMotion = true;
        _anim.SetTrigger("Attack");
    }

    public void stopAttacking()
    {
        hasAttacked = true;
        attackTimer = weaponCooldown;
        weapon.SetActive(false);
        _anim.applyRootMotion = false;
        _anim.ResetTrigger("Attack");
    }

    public void takeDamage(float dmg, float pushBack, float stun, Vector3 direction, float cost)
    {
        //Do Pushback.

        _rig.velocity = direction * pushBack;

        //Do damage.
        health -= dmg;

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }

        _playerStats.deductStamina(cost);

        //Set the chase.
        chasingPlayer = true;
    }

    public void getValidPositions(Transform[] pos, int isl)
    {
        validPos = new Transform[pos.Length];

        for (int i = 0; i < validPos.Length; i++)
        {
            validPos[i] = pos[i];
        }

        currentIsland = isl;
    }

    public void setPlayerFound(bool b)
    {
        foundPlayer = b;
    }

    public void setIsland(int i)
    {
        currentIsland = i;
    }

    public float getEnemyRange()
    {
        return enemyRange;
    }

    public void OnAnimatorMove()
    {
        this.transform.position += _anim.deltaPosition;
    }

    public void rotateTo(Vector3 pos)
    {
        //Turn the creature around until it is facing that direction.
        pos.y = 0;

        Quaternion rot = Quaternion.LookRotation(pos);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }

    /*public bool findPlayer()
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
    }*/
}

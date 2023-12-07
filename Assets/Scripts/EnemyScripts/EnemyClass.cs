using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{

    public enum enemyStates
    {
        patroling = 0,
        spottedPlayer = 1,
        chasingPlayer = 2,
        searchingPlayer = 3,
        attackedPlayer = 4
    };

    [SerializeField]
    protected float health;

    [Header("Enemy Detection Variables")]
    [SerializeField]
    protected Light detectLight;
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
    [SerializeField]
    protected float angleTo;
    [SerializeField]
    protected Vector3 rotatedPosition;

    [Header("Other Variables")]
    [SerializeField]
    private GameObject[] loot;

    [Header("State Colours")]
    [SerializeField]
    private Color normalCol;
    [SerializeField]
    private Color alarmCol;
    [SerializeField]
    private Color attackCol;

    protected Rigidbody _rig;
    protected PlayerClass _playerStats;
    protected NavMeshAgent _agent;
    protected Transform _player;
    protected Animator _anim;

    public int currentIsland;
    protected Vector3 currentTargetLocation;


    public enemyStates currentState;

    //protected bool foundPlayer;
    //protected bool chasingPlayer;
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

        hasAttacked = false;
        _agent.SetDestination(transform.position);
        _agent.speed = speed;
        _agent.angularSpeed = angularSpeed;

        _anim.SetBool("Idle", true);

        currentState = enemyStates.patroling;

        detectLight.color = normalCol;
    }

    private void FixedUpdate()
    {
        //If player was spotted but currently not chasing, then go through shock timer.
        if (currentState == enemyStates.spottedPlayer)
        {
            detectLight.color = alarmCol;
            //Stop the movement and watch the player.
            if(shockTimer > 0)
            {
                _anim.SetBool("Idle", true);
                _agent.SetDestination(this.transform.position);
                rotateTo(new Vector3(_player.position.x, 0, _player.position.z) - transform.position);
                shockTimer -= Time.deltaTime;
            } else
            {
                currentTargetLocation = _player.position;
                currentState = enemyStates.chasingPlayer;
            }
        //When not spotting player, then just complete move.
        } else if(currentState == enemyStates.patroling)
        {
            detectLight.color = normalCol;
            move();
        //When player spotted and time is up, start chasing player.
        } else if(currentState == enemyStates.chasingPlayer)
        {
            detectLight.color = attackCol;
            chase();
        //If enemy has attacked someone, then do this.
        } else if(currentState == enemyStates.attackedPlayer)
        {
            if(attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            } else
            {
                currentState = enemyStates.chasingPlayer;
                waitTimer = UnityEngine.Random.Range(5, 10);
            }
        //If enemy has lost sight or attacked player, then do this.
        } else if (currentState == enemyStates.searchingPlayer)
        {
            detectLight.color = alarmCol;
            search();
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

            currentTargetLocation = currentPos;

            _agent.SetDestination(currentTargetLocation);
        }
    }

    public void search()
    {
        rotateTo(rotatedPosition);

        //When facing the new point, start moving.
        if (Vector3.Angle(rotatedPosition, transform.forward) <= angleTo)
        {
            //animator.SetBool("Turning", false);
            //_anim.SetBool("Idle", false);

            if(waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                _anim.SetBool("Idle", true);
            } else
            {
                _anim.SetBool("Idle", false);
                //If can't find another (did not trigger back to chase) then return to normal.
                currentState = enemyStates.patroling;
            }
        }
    }

    public void chase()
    {
        if (Vector3.Distance(transform.position, currentTargetLocation) > weaponRange)
        {
            //Is the player able to be spotted?
            //Test if behind a wall.
            RaycastHit hit;

            //Check if there is a wall between player and enemy.
            if (Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, enemyRange, 0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Player")
                {
                    currentTargetLocation = _player.position;
                    //Debug.Log("Player not behind object");
                }
            }

            //Debug.Log(currentTargetLocation);
            _agent.SetDestination(currentTargetLocation);
        }
        else
        {
            //If withing range of the target location, but not the player, then enter search mode. If so, enter attack mode and attack.
            if(Vector3.Distance(transform.position, _player.position) > weaponRange)
            {
                if(Vector3.Distance(transform.position, _player.position) > enemyRange)
                {
                    currentState = enemyStates.searchingPlayer;
                    rotatedPosition = transform.position + UnityEngine.Random.insideUnitSphere * 5;
                    rotatedPosition.y = 0;
                    waitTimer = UnityEngine.Random.Range(5, 10);
                } else
                {
                    currentTargetLocation = _player.position;
                }
            } else
            {
                //Debug.Log(attackTimer);
                //Ensure the weapon cooldown isn't hit yet.
                if(attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                } else
                {
                    attack();
                }
            }
        }
    }

    public void attack()
    {
        currentState = enemyStates.attackedPlayer;
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

        currentState = enemyStates.chasingPlayer;
        currentTargetLocation = _player.position;
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
        currentState = enemyStates.chasingPlayer;
    }

    public void setPlayerFound()
    {
        if(currentState == enemyStates.patroling)
        {
            shockTimer = detectionShock;
            currentState = enemyStates.spottedPlayer;
        } else if (currentState == enemyStates.searchingPlayer)
        {
            currentState = enemyStates.chasingPlayer;
            currentTargetLocation = _player.position;
        } else if(currentState == enemyStates.chasingPlayer)
        {
            currentTargetLocation = _player.position;
        }
    }

    public void setIsland(int i)
    {
        currentIsland = i;
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

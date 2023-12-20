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
        attackedPlayer = 4,
        dying = 5
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

    [Header("Navigation Variables")]
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
    [SerializeField]
    protected float pathFindingDistance;
    protected List<Transform> objts;

    [Header("Other Variables")]
    [SerializeField]
    private int lootAmount;
    [SerializeField]
    private float throwStrength;

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
    protected PoolManager _manager;
    protected GameManager _gameManager;

    protected enemyStates currentState;
    public int currentIsland;

    protected NavMeshPath _currentPath;
    protected Vector3 currentTargetLocation;
    protected Vector3 changedTargetLocation;
    protected int currentCorner;

    protected float attackTimer;
    protected float waitTimer;
    protected float shockTimer;
    protected float changeDirectionTimer;

    protected void Start()
    {
        _rig = transform.GetComponent<Rigidbody>();
        _playerStats = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerClass>();
        _agent = transform.GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _anim = GetComponentInChildren<Animator>();
        _anim.applyRootMotion = false;
        weapon.GetComponent<WeaponClass>().setWeapon(damage.x, damage.y, damage.z);
        weapon.SetActive(false);
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PoolManager>();

        objts = new List<Transform>();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < o.Length; i++)
        {
            objts.Add(o[i].transform);
        }

        for(int i = 0; i < e.Length; i++)
        {
            objts.Add(e[i].transform);
        }



        //changedTargetLocation = transform.position;
        currentTargetLocation = Vector3.zero;
        _currentPath = new NavMeshPath();
        currentCorner = 0;
        //_agent.SetDestination(transform.position);
        //_agent.speed = speed;
        //_agent.angularSpeed = angularSpeed;

        _anim.SetBool("Idle", true);

        currentState = enemyStates.patroling;

        detectLight.color = normalCol;
    }

    private void FixedUpdate()
    {
        if(!_gameManager.getplayerStop() && !(currentState == enemyStates.dying))
        {
            //Debug.Log(changedTargetLocation + " | " + currentTargetLocation + " | " + transform.position);

            if (changedTargetLocation != currentTargetLocation)
            {
                //Debug.Log("Yo?");

                currentTargetLocation = changedTargetLocation;
                currentCorner = 1;
                //NavMesh.CalculatePath(transform.position, currentTargetLocation, NavMesh.AllAreas, _currentPath);

                /*for (int i = 0; i < _currentPath.corners.Length - 1; i++)
                {
                    //Debug.Log("Path " + i + ": " + _currentPath.corners[i]);
                    Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.yellow, Mathf.Infinity, false);
                }*/

                //Find the first current 
                _agent.SetDestination(currentTargetLocation);
            }

            //rotateTo(currentTargetLocation);

            Vector3 dir = Vector3.zero;

            //Debug.Log("Corner: " + currentCorner + " | " + _currentPath.corners.Length);

            //Using the given path and current position, move at 'speed' to that direction.
            if (currentCorner < _currentPath.corners.Length && _currentPath.corners.Length > 0)
            {
                //Debug.Log("Flag 1");
                //If not on current corner, then add another path to work it.
                if (Vector3.Distance(transform.position, _currentPath.corners[currentCorner]) > agentRange)
                {
                    if (currentState != enemyStates.dying)
                    {
                        //Debug.Log("Flag 2");
                        //Get direction.
                        dir = (_currentPath.corners[currentCorner] - transform.position).normalized;

                        //dir = bestDirToCorner(dir).normalized;

                        dir = (dir * Time.deltaTime * speed);
                        dir.y = 0;
                        rotateTo(dir);
                        transform.position += dir;

                        //_agent.SetDestination(currentTargetLocation);

                        rotateTo(currentTargetLocation);
                    }

                    //transform.LookAt(_currentPath.corners[currentCorner]);

                    /*Vector3 start = transform.position;
                    Vector3 debugLoc = _currentPath.corners[currentCorner] - transform.position;
                    Vector3 debugDir = dir.normalized * 0.2f;
                    //Draw2 the debug line.
                    Debug.DrawRay(start, debugLoc, Color.white, 0.0f, true);
                    Debug.DrawRay(start, debugDir, Color.red, 0.0f, true);*/
                }
                else
                {
                    //Debug.Log("Flag 2.1");
                    if (currentCorner + 1 < _currentPath.corners.Length)
                    {
                        currentCorner++;
                    }
                }
            }
            else
            {
                if (currentCorner > 0)
                {
                    currentCorner--;
                }
            }

            //If player was spotted but currently not chasing, then go through shock timer.
            if (currentState == enemyStates.spottedPlayer)
            {
                detectLight.color = alarmCol;
                //Stop the movement and watch the player.
                if (shockTimer > 0)
                {
                    _anim.SetBool("Idle", true);
                    changedTargetLocation = transform.position;
                    //_agent.SetDestination(this.transform.position);
                    rotateTo(new Vector3(_player.position.x, 0, _player.position.z) - transform.position);
                    shockTimer -= Time.deltaTime;
                }
                else
                {
                    changedTargetLocation = _player.position;
                    currentState = enemyStates.chasingPlayer;
                    _anim.SetBool("Idle", false);
                }
                //When not spotting player, then just complete move.
            }
            else if (currentState == enemyStates.patroling)
            {
                detectLight.color = normalCol;
                //_anim.SetBool("Idle", false);
                move();
                //When player spotted and time is up, start chasing player.
            }
            else if (currentState == enemyStates.chasingPlayer)
            {
                detectLight.color = attackCol;
                waitTimer = UnityEngine.Random.Range(5, 10);
                chase();
                //If enemy has attacked someone, then do this.
            }
            else if (currentState == enemyStates.attackedPlayer)
            {
                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                    changedTargetLocation = transform.position;
                    //_agent.SetDestination(transform.position);
                    rotateTo(_player.position - transform.position);
                }
                else
                {
                    currentState = enemyStates.chasingPlayer;
                    changedTargetLocation = _player.position;
                    _anim.SetBool("Idle", false);
                }
                //If enemy has lost sight or attacked player, then do this.
            }
            else if (currentState == enemyStates.searchingPlayer)
            {
                detectLight.color = alarmCol;
                search();
            }
            else if (currentState == enemyStates.dying)
            {
                changedTargetLocation = transform.position;
            }
        } else
        {
            _anim.SetBool("Idle", true);
        }


        //Make enemy idle when near the position.
        /*if (Vector3.Distance(transform.position, _agent.destination) < agentRange)
        {
            //_agent.isStopped = true;
            //_anim.SetBool("Idle", true);

  
        } else
        {
            //_agent.isStopped = false;
            //_anim.SetBool("Idle", false);
        }*/

        //Make something when there is just 1 point.
        /*if(_currentPath.corners.Length == 1)
        {
            //Just go to the point.
            Vector3 dir = (currentTargetLocation - transform.position).normalized;
            dir = transform.position + (dir * Time.deltaTime * speed);
            dir.y = 0;

            transform.position = dir;

            rotateTo(currentTargetLocation - transform.position);
        }*/
    }

    public virtual void move()
    {
        _anim.SetBool("Idle", false);

        //_agent.speed = speed;

        if (Vector3.Distance(currentTargetLocation, transform.position) < agentRange)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 5;

            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5, 1);
            Vector3 currentPos = hit.position;

            changedTargetLocation = currentPos;

            //_agent.SetDestination(currentTargetLocation);
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
                    changedTargetLocation = _player.position;
                    //Debug.Log("Player not behind object");
                }
            }

            //Debug.Log(currentTargetLocation);
            //_agent.SetDestination(currentTargetLocation);
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
                    changedTargetLocation = _player.position;
                }
            } else
            {
                //Debug.Log(attackTimer);
                //attack();
                _anim.SetTrigger("Attack");
            }
        }
    }

    public void attack()
    {
        currentState = enemyStates.attackedPlayer;
        attackTimer = weaponCooldown;
        weapon.SetActive(true);
        _anim.SetBool("Idle", true);
        transform.LookAt(_player);
        _anim.applyRootMotion = true;
    }

    public void stopAttacking()
    {
        //Debug.Log("Working...");
        weapon.SetActive(false);
        _anim.ResetTrigger("Attack");
        _anim.applyRootMotion = false;
    }

    public void die()
    {
        //Create an explosion.

        //Create the loot.
        //Now instantiate the common resources, ranging from 0 to 3.
        for (int x = 0; x < lootAmount; x++)
        {
            Vector3 direction = randDirection();

            GameObject obj = _manager.GetPooledObject(PoolManager.objectType.resource);

            obj.SetActive(true);
            obj.transform.position = direction;
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponent<Rigidbody>().AddForce(direction.normalized * throwStrength);
        }



        //Destroy object.
        Destroy(this.gameObject);
    }

    public void takeDamage(float dmg, float pushBack, float stun, Vector3 direction, float cost)
    {
        //Do Pushback.

        _rig.velocity = direction * pushBack;

        //Do damage.
        health -= dmg;

        if (health <= 0)
        {
            _rig.isKinematic = true;
            _anim.applyRootMotion = true;
            currentState = enemyStates.dying;
            _rig.velocity = Vector3.zero;
            _anim.SetTrigger("Die");
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
            changedTargetLocation = _player.position;
        } else if(currentState == enemyStates.chasingPlayer)
        {
            changedTargetLocation = _player.position;
        }
    }

    public void setIsland(int i)
    {
        currentIsland = i;
    }

    public int getIsland()
    {
        return currentIsland;
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

    public Vector3 bestDirToCorner(Vector3 targetDir)
    {
        Vector3 weightedAdditions = Vector3.zero;

        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);

        for (int i = 0; i < objts.Count; i++)
        {
            //Find objects within the pathfinding distance and in front of the enemy.
            if(objts[i] != null &&
               Vector3.Distance(objts[i].transform.position, transform.position) <= pathFindingDistance &&
               Vector3.Distance(_currentPath.corners[currentCorner], transform.position) > Vector3.Distance(objts[i].transform.position, transform.position) &&
               Vector3.Dot(transform.forward, objts[i].transform.position - transform.position) > 0)
            {
                Vector3 pos = new Vector3(objts[i].transform.position.x, 0, objts[i].transform.position.z);

                Vector3 direction = (pos - startPos).normalized;
                Vector3 up = startPos + Vector3.up;

                Vector3 perpDir = Vector3.Cross(direction.normalized, up.normalized);
                perpDir.y = 0;

                //Debug.DrawLine(startPos, pos, Color.green, 0f, true);
                //Debug.DrawLine(startPos, up, Color.blue, 0f, true);
                //Debug.DrawLine(startPos, startPos - perpDir.normalized, Color.grey, 0f, true);
                //Debug.DrawLine(startPos, startPos + perpDir.normalized, Color.grey, 0f, true);
                //Debug.DrawLine(startPos, startPos + transform.right, Color.blue, 0f, true);


                if(Vector3.Dot(pos, transform.right) < 0)
                {
                    //If within distance and in front of enemy, find the normal direction, and have it weighted based on distance to position, add it all together.
                    weightedAdditions += (perpDir).normalized * ((1 - (Vector3.Distance(pos, startPos) / pathFindingDistance)) / 2);
                } else
                {
                    //If within distance and in front of enemy, find the normal direction, and have it weighted based on distance to position, add it all together.
                    weightedAdditions += (startPos - perpDir).normalized * ((1 - (Vector3.Distance(pos, startPos) / pathFindingDistance)) / 2);
                }
            }
        }

        //Return the target direction, when the closer you get to needed position, the less the weighted additions are used.
        return targetDir + ((weightedAdditions) * Mathf.Min(1, Vector3.Distance(_currentPath.corners[currentCorner], transform.position) / pathFindingDistance));
    }

    private Vector3 randDirection()
    {
        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized * 0.1f;

        return new Vector3(dir.x + transform.position.x, 0.1f, dir.y + transform.position.z);
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

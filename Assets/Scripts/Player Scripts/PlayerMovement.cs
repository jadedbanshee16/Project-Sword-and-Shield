using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    Animator m_Animator;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_rig;
    GameManager _manager;
    AudioSource _audio;
    AudioManager _audioManager;
    
    public float turnSpeed = 20f;

    bool isWalking;
    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        m_rig = gameObject.GetComponent<Rigidbody>();
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audio = GetComponent<AudioSource>();
        _audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();

        dead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead && !_manager.getplayerStop())
        {
            //Get the inputs
            float xAxes = Input.GetAxis("Horizontal");
            float zAxes = Input.GetAxis("Vertical");

            //Set the vector to new positions based on inputs.
            m_Movement.Set(xAxes, 0f, zAxes);
            //As it the diagonals would be faster, normalize to just get direction.
            m_Movement.Normalize();

            //Test to see if you're moving in either X or Z axes. If so, return bool true.
            bool isMovingX = !Mathf.Approximately(xAxes, 0f);
            bool isMovingZ = !Mathf.Approximately(zAxes, 0f);

            //Now set isWalking.
            isWalking = isMovingX || isMovingZ;
            //Set walking boolean.
            m_Animator.SetBool("isWalking", isWalking);

            //Now create a rotation vector.
            //Rotate FROM transform forward TO target direction AT speed in radians WITH magnitude.
            Vector3 nForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);

            //Now change the rotation of the player to look towards the new desired location.
            m_Rotation = Quaternion.LookRotation(nForward);
        } else if (_manager.getplayerStop())
        {
            m_Animator.SetBool("isWalking", false);
        }
    }

    //A function called everytime the animation moves.
    //Ensures that root can work as well as change in position and rotation.
    private void OnAnimatorMove()
    {
        if (!dead && !_manager.getplayerStop())
        {
            //Make changes to the position based on movement and animation.
            m_rig.MovePosition(m_rig.position + m_Movement * m_Animator.deltaPosition.magnitude);
            //Now make the change to rotation, which is easier.
            m_rig.MoveRotation(m_Rotation);
        }
    }

    public void die()
    {
        dead = true;
        m_Animator.SetTrigger("Die");
        m_rig.isKinematic = true;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().setplayerStop_Dead(true, true);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().setFading(true, 0.2f);
    }

    public void restartWorld()
    {
        dead = false;
        m_Animator.ResetTrigger("Die");
        m_Animator.SetBool("isWalking", true);
        m_rig.isKinematic = false;
        _audioManager.playSound(_audio, AudioManager.audioType.gameAudio, 0);
        //Debug.Log("Made it");
        //Now restart the world.
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().restartGame(true);
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().setplayerStop(false);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().resetInventory();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().loadHUB();
    }

    public void stepOnce()
    {
        _audioManager.playSound(_audio, AudioManager.audioType.playerFootsteps);
    }
}

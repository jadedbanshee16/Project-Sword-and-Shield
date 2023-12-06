using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A script that allows the deactivation of the player controller and player stats, as well as activation.
 */
public class CancelController : MonoBehaviour
{
    private PlayerControl _controller;
    private PlayerClass _player;
    private Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<PlayerControl>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerClass>();
        _anim = GetComponentInChildren<Animator>();
    }

    public void turnOff()
    {
        _controller.enabled = false;
        _player.enabled = false;

        //Ensure animations will use the root change.
        _anim.applyRootMotion = true;
    }

    public void TurnOn()
    {
        _controller.enabled = true;
        _player.enabled = true;

        //Ensure animations will use the root change.
        _anim.applyRootMotion = false;
    }
}

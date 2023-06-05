using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerControl : MonoBehaviour
{
    private GameObject kyna_;
    private GameObject player_;
    private OptionsScript options;


    private GameObject offhand;
    private GameObject onHand;

    private Animator kynaAnim_;

    //Controls
    private KeyCode attack;


    // Start is called before the first frame update
    void Start()
    {
        kyna_ = GameObject.FindGameObjectWithTag("Kyna");
        kynaAnim_ = kyna_.GetComponentInChildren<Animator>();
        player_ = GameObject.FindGameObjectWithTag("Player");

        attack = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsScript>().attack;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(attack))
        {
            kynaAnim_.SetTrigger("Attack1");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerControl : MonoBehaviour
{
    private GameObject kyna_;
    private GameObject player_;
    private OptionsScript options;

    private Animator kynaAnim_;

    //Controls
    private KeyCode onHand;


    // Start is called before the first frame update
    void Start()
    {
        kyna_ = GameObject.FindGameObjectWithTag("Kyna");
        kynaAnim_ = kyna_.GetComponentInChildren<Animator>();
        player_ = GameObject.FindGameObjectWithTag("Player");
        onHand = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsScript>().onHand;
    }

    // Update is called once per frame
    void Update()
    {
        //When the onHand button is pressed, do the following.
        if (Input.GetKeyDown(onHand))
        {

        }
    }
}

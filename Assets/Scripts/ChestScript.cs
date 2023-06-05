using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{

    public Animator anim_;

    public bool interacting = false;

    public KeyCode interactionButton;

    // Start is called before the first frame update
    void Start()
    {
        anim_ = gameObject.GetComponentInChildren<Animator>();
        interactionButton = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsScript>().interact;
    }

    // Update is called once per frame
    void Update()
    {
        if (interacting && Input.GetKeyDown(interactionButton))
        {
            //Open chest.
            anim_.SetBool("Opened", true);
            Debug.Log("Opening...");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Unsure player is in. If player is in and keycode is pressed, 
        if (other.gameObject.CompareTag("Player"))
        {
            interacting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interacting = false;
    }




}

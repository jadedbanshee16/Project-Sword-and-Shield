using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : Interactable
{
    public GameObject pair;

    private GameObject playerObj;

    private bool interacting = false;

    private int partnerNum;

    private KeyCode interactionButton;

    private void Start()
    {
        interactionButton = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsScript>().interact;
    }

    private void Update()
    {
        if(interacting && Input.GetKeyDown(interactionButton))
        {
            teleport();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Unsure player is in. If player is in and keycode is pressed, 
        if(other.gameObject.CompareTag("Player"))
        {
            playerObj = other.gameObject;
            interacting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interacting = false;
    }

    public void setPartner(GameObject thePair, int x)
    {
        partnerNum = x;
        pair = thePair;
    }

    private void teleport()
    {
        //Get the pair and teleport.
        playerObj.transform.position = pair.transform.position;
    }
}

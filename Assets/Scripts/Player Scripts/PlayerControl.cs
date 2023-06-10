using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerControl : MonoBehaviour
{
    //Set all needed objects for scripts.
    private KynaClass kyna_;
    private GameObject player_;
    private Camera cam_;
    private OptionsScript options;
    private Animator kynaAnim_;

    //Controls
    private KeyCode onHand;

    //Interaction focus.
    public Interactable currentFocus;


    // Start is called before the first frame update
    void Start()
    {
        kyna_ = GameObject.FindGameObjectWithTag("Kyna").GetComponent<KynaClass>();
        kynaAnim_ = kyna_.GetComponentInChildren<Animator>();
        player_ = GameObject.FindGameObjectWithTag("Player");
        onHand = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsScript>().onHand;
        cam_ = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocused())
        {
            Vector3 newPos = currentFocus.interactionTransform.position;
            kyna_.setTargetPos(new Vector3(newPos.x, 0, newPos.z));
        } else
        {
            //Every update, reset mouse position to move Kyna ghost.
            kyna_.setTargetPos(MousePosition());
        }

        //When the onHand button is pressed, do the following.
        if (Input.GetKeyDown(onHand))
        {
            //When pressed, find out if an interactable is pressed.
            Interactable focus = MouseInteractable();

            //If interactable, complete interactable stuff.
            if(focus != null)
            {
                setFocus(focus);
            } else
            {
                deFocus();
            }
        }
    }

    /*
     * Set the current focus of the game.
     */
    private void setFocus(Interactable f)
    {
        currentFocus = f;

        if(currentFocus.getType() == Interactable.interactType.playerInteractable)
        {
            currentFocus.onFocused(player_.transform);
            //Ensure the player is close enough for an interaction to occur.
            //This is so it doesn't have to be focused unless player is near it.
            if (!currentFocus.isNearby())
            {
                deFocus();
            }
        } else if (currentFocus.getType() == Interactable.interactType.ghostInteractable)
        {
            currentFocus.onFocused(kyna_.transform);
        }
    }

    /*
     * Defocus a current object in the game.
     */
    private void deFocus()
    {
        if(isFocused())
        {
            currentFocus.onDeFocused();
        }
        currentFocus = null;
    }

    public bool isFocused()
    {
        if(currentFocus != null)
        {
            return true;
        }

        return false;
    }

    /*
     * Function to return cam position.
     */
    public Vector3 MousePosition()
    {
        Vector3 dir = Vector3.zero;
        //Now get the position based on camera.
        Ray ray = cam_.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Now change Kyna's position to where the ray had hit. Set y to 0.
            Vector3 pos = hit.point;
            pos.y = 0;
            dir = pos;
        }

        return dir;
    }

    /*
     * Function to return an object collision gameobject.
     */
    private Interactable MouseInteractable()
    {
        Interactable inter = null;
        //Now get the position based on camera.
        Ray ray = cam_.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            inter = hit.collider.GetComponent<Interactable>();
        }

        return inter;
    }
}

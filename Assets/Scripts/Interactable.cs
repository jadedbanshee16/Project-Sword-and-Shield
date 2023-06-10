using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum interactType
    {
        ghostInteractable,
        playerInteractable
    }

    public Transform interactionTransform;

    public float radius;
    private bool isFocus;
    private bool hasInteracted;

    Transform player_;
    public interactType interactableType;

    public virtual void Interact()
    {
        Debug.Log("INTERACT");
    }

    private void Update()
    {
        //CHeck is the transform of the player is close enough to interact.
        if (isFocus && !hasInteracted)
        {
            if (isNearby())
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void onFocused(Transform playerTransform)
    {
        isFocus = true;
        player_ = playerTransform;
        hasInteracted = false;
    }

    public void onDeFocused()
    {
        isFocus = false;
        player_ = null;
        hasInteracted = false;
    }

    public interactType getType()
    {
        return interactableType;
    }

    public bool isNearby()
    {
        float dist = Vector3.Distance(interactionTransform.position, player_.position);

        if(dist <= radius)
        {
            return true;
        }

        return false;
    }

    /*
     * Debug scripts
     */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}

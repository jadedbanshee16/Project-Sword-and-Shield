using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    private Animator anim_;

    private GameManager gameManager_;

    private bool isOpen = true;

    // Start is called before the first frame update
    void Start()
    {
        anim_ = gameObject.GetComponent<Animator>();
        gameManager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        anim_.SetBool("isOpened", isOpen);
    }

    public void setIsOpen(bool s)
    {
        isOpen = s;

        anim_.SetBool("isOpened", isOpen);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isOpen)
        {
            gameManager_.setNextLevel(true);
        }
    }
}

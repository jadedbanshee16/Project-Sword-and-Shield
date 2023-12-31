using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    private Animator anim_;
    private GameManager gameManager_;
    private AudioSource _audio;
    private AudioManager _audioManager;

    private bool isOpen = true;

    [SerializeField]
    private bool isExitDoor;

    // Start is called before the first frame update
    void Start()
    {
        anim_ = gameObject.GetComponent<Animator>();
        gameManager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audio = GetComponent<AudioSource>();
        _audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
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
            if (!isExitDoor)
            {
                gameManager_.setFading(true, 0.5F);
                gameManager_.setplayerStop_Dead(true, false);
                playDoorAudio();
            } else
            {
                //GameManager quit function.
                Debug.Log("Quit");
                gameManager_.quitGame();
            }

        }
    }

    private void playDoorAudio()
    {
        _audioManager.playSound(_audio, AudioManager.audioType.exitAudio);
    }
}

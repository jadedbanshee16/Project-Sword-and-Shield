using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Cinemachine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    //Level objects
    public LevelGeneration lvl;
    private LevelGeneration lvlInstance;

    private Inventory _inv;

    private float fadeSpeed = 1;
    private float clearSpeed = 1;
    private Image fadeimg;
    private bool isFaded;
    private bool fading;

    private float fadeTimer;


    //Level setting stuff.
    private float levelNum = 0;
    private float difficulty = 25f;
    private bool nextLevel = false;
    public bool playerStop;

    private int count;

    //An enum for zone types.
    private enum zoneType
    {
        spawn,
        exit,
        interactable,
        uninteractable
    }


    // Start is called before the first frame update
    void Start()
    {
        if(lvl != null)
        {
            beginGame();
        }

        //Other set up stuff.
        if(_inv == null)
        {
            _inv = GetComponent<Inventory>();
        }

        _inv.setUpInventory();
        count = 0;

        fadeimg = GetComponentInChildren<Image>();
        isFaded = true;
        fading = false;
        fadeTimer = 0;
        playerStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * This is just a test code
         */
        /*if (nextLevel || Input.GetKeyDown(KeyCode.Backspace))
        {
            restartGame();
            nextLevel = false;
        }

        if(count < 0)
        {
            Debug.Log(count);
            restartGame();
            nextLevel = false;
            count++;
        }*/

        //If fading, and is going to be faded, then continue fading out.
        if (fading && isFaded)
        {
            if(fadeTimer < 1)
            {
                fadeTimer += Time.deltaTime * fadeSpeed;

                Color n = fadeimg.color;
                n.a = fadeTimer;
                fadeimg.color = n;
            } else
            {
                fading = false;
                restartGame();
            }
        } else if (fading && !isFaded)
        {
            if (fadeTimer > 0)
            {
                fadeTimer -= Time.deltaTime * clearSpeed;

                Color n = fadeimg.color;
                n.a = fadeTimer;
                fadeimg.color = n;
            }
            else
            {
                fading = false;
                setplayerStop(false);
            }
        }
    }

    private void beginGame()
    {
        levelNum++;
        //Set up difficulty.
        //difficulty = setDifficulty(levelNum);
        //Debug.Log(difficulty + " : " + levelNum);

        //Generate the level:
        //Ground, walls, islands, etc.
        lvlInstance = Instantiate(lvl);
        lvlInstance.GenerateMap(difficulty);
        setFading(false, 1);
    }

    public void resetInventory()
    {
        _inv.setUpInventory();
    }

    public void restartGame() 
    {
        //Destroy and create the next level.
        Destroy(lvlInstance.gameObject);
        GetComponent<PoolManager>().resetPool();
        beginGame();
    }

    /*private float setDifficulty(float l)
    {
        return (float)(10f * (l / 100));
    }*/

    public void setNextLevel(bool b)
    {
        nextLevel = b;
        restartGame();
    }

    public void setplayerStop(bool b)
    {
        playerStop = b;
    }

    public bool getplayerStop()
    {
        return playerStop;
    }

    public void setFading(bool toFade, float speed)
    {
        fading = true;
        isFaded = toFade;

        if (isFaded)
        {
            fadeSpeed = speed;
            fadeTimer = 0;
        } else
        {
            clearSpeed = speed;
            fadeTimer = 1;
        }
    }
}

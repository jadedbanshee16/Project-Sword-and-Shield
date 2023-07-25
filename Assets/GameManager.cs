using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    //Level objects
    public LevelGeneration lvl;
    private LevelGeneration lvlInstance;


    //Level setting stuff.
    private float levelNum = 0;
    private float difficulty = 25f;
    private bool nextLevel = false;

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

        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * This is just a test code
         */
        if (nextLevel || Input.GetKeyDown(KeyCode.Backspace))
        {
            restartGame();
            nextLevel = false;
        }

        /*if(count < 0)
        {
            restartGame();
            nextLevel = false;
            count++;
        }*/
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
    }

    private void restartGame() 
    {
        //Destroy and create the next level.
        Destroy(lvlInstance.gameObject);
        beginGame();
    }

    /*private float setDifficulty(float l)
    {
        return (float)(10f * (l / 100));
    }*/

    public void setNextLevel(bool b)
    {
        nextLevel = b;
    }
}

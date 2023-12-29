using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Cinemachine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    //Level objects
    public LevelGeneration lvl;
    private LevelGeneration lvlInstance;
    private Transform spawnZone;
    [SerializeField]
    private GameObject _player;
    private GameObject playerInstance;
    private AudioManager _audio;

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
    private bool playerStop;
    private bool playerDead = false;

    private string currentlvl;

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
        currentlvl = SceneManager.GetActiveScene().name;

        if(GameObject.FindGameObjectWithTag("PlayerSpawn") != null)
        {
            spawnZone = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        }

        _audio = GetComponent<AudioManager>();

        //If the current level is a level NOT the menu OR the HUBWorld, then complete the lvlset.
        lvlSet();

        fadeimg = GetComponentInChildren<Image>();
        isFaded = true;
        fading = true;
        fadeTimer = 1;
        setplayerStop_Dead(true, true);
        setFading(false, 1f);
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
                fadeTimer = 1;
                if (!playerDead)
                {
                    restartGame();
                } else
                {
                    playerDead = false;
                }
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
                fadeTimer = 0;
                setplayerStop_Dead(false, false);
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
        spawnZone = lvlInstance.GenerateMap(difficulty);

        spawnPlayer(spawnZone.position);
    }

    public void resetInventory()
    {
        _inv.setUpInventory();
    }

    public void restartGame() 
    {
        if(currentlvl.Equals("HUBWorld"))
        {
            if(levelNum < 1)
            {
                //You're in HUD world. Go to the next world (Level 1).
                SceneManager.LoadScene("FactoryLevel");
            }
        } else if (currentlvl.Equals("FactoryLevel"))
        {
            //Destroy and create the next level.
            Destroy(lvlInstance.gameObject);
            GetComponent<PoolManager>().resetPool();
            beginGame();
        }

        setFading(false, 0.5f);
    }

    /*private float setDifficulty(float l)
    {
        return (float)(10f * (l / 100));
    }*/

    public void setplayerStop_Dead(bool b, bool die)
    {
        playerStop = b;
        playerDead = die;
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

    private void lvlSet()
    {
        //Other set up stuff.
        if (_inv == null)
        {
            _inv = GetComponent<Inventory>();
        }

        if (!currentlvl.Equals("HUBWorld"))
        {
            if (lvl != null)
            {
                beginGame();
            }
        } else
        {
            //Just spawn the player.
            if(spawnZone != null)
            {
                spawnPlayer(spawnZone.position);
            }

        }

        if (currentlvl.Equals("HUBWorld"))
        {
            _audio.playAmbient(0);
        } else if (currentlvl.Equals("FactoryLevel"))
        {
            _audio.playAmbient(1);
        }

        _inv.setUpInventory();

    }

    private void spawnPlayer(Vector3 pos)
    {
        //Generate the player.
        //Ensure that there isn't a player already loaded. If they are loaded, then set position to lvlSpawnInstance.
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            playerInstance = Instantiate(_player, pos, Quaternion.identity);
        }
        else
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
            playerInstance.transform.GetChild(1).transform.position = pos;
        }
    }

    public void loadHUB()
    {
        SceneManager.LoadScene("HUBWorld");
    }
}

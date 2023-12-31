using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    GameManager _manager;

    [SerializeField]
    SetInventoryImage cogs;

    [SerializeField]
    SetInventoryImage springs;

    [SerializeField]
    GameObject[] allCogs;
    [SerializeField]
    GameObject[] allSprings;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < allCogs.Length; i++)
        {
            allCogs[i].SetActive(false);
            allSprings[i].SetActive(false);
        }

        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //Now set the cog images to cog score.
        cogs.setTexMesh(_manager.getScore(0).ToString());
        springs.setTexMesh(_manager.getScore(1).ToString());

        //Now, set the cogs and springs to desired pile size.
        for(int i = 0; i < _manager.getScore(0) / 3; i++)
        {
            if(allCogs.Length > i)
            {
                allCogs[i].SetActive(true);
            }
        }

        for(int i = 0; i < _manager.getScore(1) / 3; i++)
        {
            if(allSprings.Length > i)
            {
                allSprings[i].SetActive(true);
            }
        }
    }
}

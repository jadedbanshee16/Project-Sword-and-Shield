using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPhaseScript : MonoBehaviour
{
    //Get the material color from Kyna.
    public SkinnedMeshRenderer rend;
    private Color col;

    private float alpha;
    public float alphaOffsetMax;
    public float alphaOffsetMin;
    private float alphaOffset;

    private bool alphaChange;

    //Timer stuff.
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        col = rend.material.color;
        alphaChange = true;
        alphaOffset = Random.Range(alphaOffsetMin, alphaOffsetMax);

        timer = Random.Range(3,10);
    }

    // Update is called once per frame
    void Update()
    {

        if (alphaChange)
        {
            if (alpha < 1)
            {
                col = changeAlpha(col, alphaOffset * Time.deltaTime);
            } else
            {
                alphaChange = !alphaChange;
                alphaOffset = Random.Range(alphaOffsetMin, alphaOffsetMax);
            }
        } else
        {
            if(alpha > 0)
            {
                col = changeAlpha(col, -(alphaOffset * Time.deltaTime));
            } else
            {
                if(timer <= 0)
                {
                    alphaChange = !alphaChange;
                    alphaOffset = Random.Range(alphaOffsetMin, alphaOffsetMax);
                    timer = Random.Range(3, 10);
                } else
                {
                    timer -= Time.deltaTime;
                }

            }
        }

        rend.material.color = col;
    }

    //Randomly blink in and out of existence.
    Color changeAlpha(Color c, float change)
    {
        alpha += change;
        c.a = alpha;

        return c;
    }
}

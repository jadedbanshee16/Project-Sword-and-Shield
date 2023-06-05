using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColour : MonoBehaviour
{
    private Light m_light;
    // Start is called before the first frame update
    void Start()
    {
        m_light = GetComponent<Light>();
    }

    public void changeLightColour(Color col)
    {
        m_light.color = col;
    }
}

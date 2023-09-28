using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskManager : MonoBehaviour
{
    public Image mat;

    private float alpha = 0;

    public Camera currentCam;
    public GameObject _player;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        Color col = mat.material.color;

        //The direction to the player.
        Vector3 direction = (_player.transform.position - currentCam.transform.position).normalized;

        if(Physics.Raycast(currentCam.transform.position, direction, out hit, 100f))
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawRay(currentCam.transform.position, hit.collider.transform.position, Color.yellow);
            if (hit.collider.tag == "Player")
            {
                if (alpha > 0)
                {
                    alpha -= Time.deltaTime;
                } else
                {
                    mat.gameObject.SetActive(false);
                }
            }
            else
            {
                if(alpha < 1)
                {
                    mat.gameObject.SetActive(true);
                    alpha += Time.deltaTime;
                }
            }
        }

        col = new Color(col.r, col.g, col.b, alpha);

        mat.material.color = col;
    }
}

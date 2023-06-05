using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KynaMovement : MonoBehaviour
{
    private Camera Maincam_;

    private Vector3 Nposition;


    // Start is called before the first frame update
    void Start()
    {
        Maincam_ = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        setNewPosition();

        //New

        transform.position = Nposition;
    }


    public void setNewPosition()
    {
        //Now get the position based on camera.
        Ray ray = Maincam_.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Now change Kyna's position to where the ray had hit. Set y to 0.
            Vector3 pos = hit.point;
            pos.y = 0;
            Nposition = pos;
        }
    }
}

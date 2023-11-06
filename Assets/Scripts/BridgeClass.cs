using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeClass : MonoBehaviour
{
    public List<GameObject> bridgePoints;

    [SerializeField]
    private GameObject bridgePrefab;

    private float bridgeSizeX;
    private float bridgeSizeZ;
    public float bridgeHeight;

    public GameObject lastPoint;
    private Vector3 firstPosition;

    private void Awake()
    {
        GameObject bridgeTemp = Instantiate(bridgePrefab);
        Collider bridgeCol = bridgeTemp.GetComponentInChildren<Collider>();

        //Set the cell size via bounds and centre.
        bridgeSizeX = ((bridgeCol.bounds.max.x - bridgeCol.bounds.center.x) * 2);
        bridgeSizeZ = ((bridgeCol.bounds.min.z - bridgeCol.bounds.center.z) * 2);

        Destroy(bridgeTemp);
    }

    //Create the bridges using the two points provided.
    public void createBridge()
    {
        //Set the bridgeX and bridgeZ.
        firstPosition =  new Vector3(this.transform.position.x, bridgeHeight, this.transform.position.z);
        Vector3 lastPosition = new Vector3(lastPoint.transform.position.x, bridgeHeight, lastPoint.transform.position.z);

        Vector3 currentPosition = firstPosition;

        bridgePoints.Add(Instantiate(bridgePrefab, currentPosition, Quaternion.identity, this.transform));

        //Debug.Log("current: " + currentPosition + " | last: " + lastPosition);

        Vector2 newDir = MazeDirection.focusedDir(new Vector2(lastPosition.x - currentPosition.x, currentPosition.z - lastPosition.z).normalized);

        int walkDist = 100;

        while(walkDist > 0 && lastPosition != currentPosition)
        {
            Vector2 lastDir = new Vector2(-newDir.x, -newDir.y);
            newDir = MazeDirection.focusedDir(new Vector2(lastPosition.x - currentPosition.x, lastPosition.z - currentPosition.z));

            currentPosition = newDirection(newDir, currentPosition);

            //Debug.Log(newDir + " | " + currentPosition);

            //Create a new object at each position, under this set.
            bridgePoints.Add(Instantiate(bridgePrefab, currentPosition, Quaternion.identity, this.transform));

            walkDist--;
        }
        //Now, walk from first position to last position, EXCEPT the first and last position.
        //Debug.Log(walkDist);
    }

    //Return new direction
    private Vector3 newDirection(Vector2 dir, Vector3 pos)
    {
        pos += new Vector3(dir.x * bridgeSizeX, 0, dir.y * bridgeSizeZ);

        return pos;
    }
}

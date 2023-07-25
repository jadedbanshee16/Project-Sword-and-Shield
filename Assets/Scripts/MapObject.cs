using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public float spawnProbability;

    public int[] takenSpaces;

    public float getSpawnProbability()
    {
        return spawnProbability;
    }
}

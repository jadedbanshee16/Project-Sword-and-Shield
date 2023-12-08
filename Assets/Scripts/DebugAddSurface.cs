using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DebugAddSurface : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}

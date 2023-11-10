using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum objectType
{
    sword,
    resource,
    key,
    shield
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager sharedInstance;
    private List<GameObject> swordObjects;
    private List<GameObject> resourceObjects;
    private List<GameObject> keyObjects;
    private List<GameObject> shieldObjects;

    [SerializeField]
    private GameObject[] projectilePrefabs;
    [SerializeField]
    private GameObject[] resourcePrefabs;
    [SerializeField]
    private GameObject[] specialPrefabs;
    [SerializeField]
    private int amountToPool;

    void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set all the sword objects.
        swordObjects = new List<GameObject>();
        resourceObjects = new List<GameObject>();
        keyObjects = new List<GameObject>();
        shieldObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            addToPool(objectType.sword);
            addToPool(objectType.resource);
            addToPool(objectType.key);
        }

        //For single use, can be changed.
        addToPool(objectType.shield);
    }

    public GameObject GetPooledObject(objectType t)
    {
        if(t == objectType.sword)
        {
            for (int i = 0; i < swordObjects.Count; i++)
            {
                if (!swordObjects[i].activeInHierarchy)
                {
                    return swordObjects[i];
                }
            }

            //If this far, out of points. Make new and retun object.
            addToPool(objectType.sword);
            return swordObjects[swordObjects.Count - 1];
        }

        if(t == objectType.resource)
        {
            List<GameObject> available = new List<GameObject>();

            for (int i = 0; i < resourceObjects.Count; i++)
            {
                if (!resourceObjects[i].activeInHierarchy)
                {
                    available.Add(resourceObjects[i]);
                }
            }

            if(available.Count > 0)
            {
                return available[Random.Range(0, available.Count - 1)];
            }

            //If this far, out of points. Make new and retun object.
            addToPool(objectType.resource);
            return resourceObjects[resourceObjects.Count - 1];
        }

        if (t == objectType.key)
        {
            for (int i = 0; i < keyObjects.Count; i++)
            {
                if (!keyObjects[i].activeInHierarchy)
                {
                    return keyObjects[i];
                }
            }

            //If this far, out of points. Make new and retun object.
            addToPool(objectType.key);
            return keyObjects[keyObjects.Count - 1];
        }


        if (t == objectType.shield)
        {
            for (int i = 0; i < shieldObjects.Count; i++)
            {
                if (!shieldObjects[i].activeInHierarchy)
                {
                    return shieldObjects[i];
                }
            }

            //If this far, out of points. Make new and retun object.
            addToPool(objectType.shield);
            return shieldObjects[shieldObjects.Count - 1];
        }

        return null;
    }

    private void addToPool(objectType t)
    {
        GameObject tmpObj;

        if(t == objectType.sword)
        {
            tmpObj = Instantiate(projectilePrefabs[0], this.transform);
            tmpObj.SetActive(false);
            swordObjects.Add(tmpObj);
        }

        if(t == objectType.resource)
        {
            if(resourceObjects.Count % 2 == 0)
            {
                tmpObj = Instantiate(resourcePrefabs[0], this.transform);
            } else
            {
                tmpObj = Instantiate(resourcePrefabs[1], this.transform);
            }
            
            tmpObj.SetActive(false);
            resourceObjects.Add(tmpObj);
        }

        if (t == objectType.key)
        {
            tmpObj = Instantiate(specialPrefabs[0], this.transform);
            tmpObj.SetActive(false);
            keyObjects.Add(tmpObj);
        }

        if(t == objectType.shield)
        {
            tmpObj = Instantiate(specialPrefabs[1], this.transform);
            tmpObj.SetActive(false);
            shieldObjects.Add(tmpObj);
        }
    }

    public void resetPool()
    {
        foreach(Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}

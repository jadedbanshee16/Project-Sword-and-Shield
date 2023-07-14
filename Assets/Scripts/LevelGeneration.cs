using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeDirection
{
    //Amount of directions.
    public const int dir = 4;

    //Set up for direction times using vector2s.
    private static Vector2[] vectors = {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    //Get a random value.
    public static int RandomVal()
    {
        return (int)UnityEngine.Random.Range(0, dir);
    }

    //Return the direction as a vector.
    public static Vector2 RandDir()
    {
        int c = RandomVal();
        return vectors[c];
    }
}

public class LevelGeneration : MonoBehaviour
{
    [Header("Grid Settings")]
    //LevelCells
    public int sizeX;
    public int sizeZ;
    [Header("Amount of Tiles")]
    public float maxWalkSize;
    private int walkSize;
    private int maxIslandCount;
    private float islandProbability;

    [Header("Basic Building Objects")]
    public GameObject cellPrefab;
    public GameObject hWall;
    public GameObject vWall;

    [Header("Player Objects")]
    public GameObject lvlSpawn;
    private GameObject lvlSpawnInstance;
    public GameObject lvlExit;
    private GameObject lvlExitInstance;
    public GameObject exitStairs;
    private GameObject exitStairsInstance;

    [Header("Player")]
    public GameObject Player;
    public GameObject playerInstance;

    [Header("Level objects")]
    public GameObject lvlInteractableZone;
    public GameObject[] lvlBridges;
    public GameObject lvlChests;

    private GameObject[,] cells;
    private GameObject[,] interactableCells;
    private List<Vector3> islands;

    //The bounding sizes of a cell.
    private float cellSizeX;
    private float cellSizeZ;

    //An enum for zone types.
    private enum zoneType
    {
        spawn,
        exit,
        interactable,
        uninteractable
    }

    private enum islandCellCountType
    {
        single,
        fullcount,
        interactableCount
    }

    //Create and destroy a prefab to get their bounds immediately.
    private void Awake()
    {
        GameObject cellTemp = Instantiate(cellPrefab);
        Collider cellCol = cellTemp.GetComponentInChildren<Collider>();

        //Set the cell size via bounds and centre.
        cellSizeX = ((cellCol.bounds.max.x - cellCol.bounds.center.x) * 2);
        cellSizeZ = ((cellCol.bounds.min.z - cellCol.bounds.center.z) * 2);

        Destroy(cellTemp);
    }

    /*
     * Generate a grid by populating the grid array, then generating a new cell for each grid position.
     */
    public void GenerateMap(float difficulty)
    {
        //Debug.Log("-----------------------");
        //Set the size and complexity of game based on difficulty.
        walkSize = (int)(maxWalkSize * difficulty);
        //islandProbability = (difficulty * 100);
        //maxIslandCount = (int)(difficulty * 10);

        islandProbability = 45;
        maxIslandCount = 7;
        walkSize = 45;

        if(walkSize > maxWalkSize)
        {
            walkSize = (int)maxWalkSize;
        }

        //Create a grid and populate cells.
        cells = new GameObject[sizeX, sizeZ];

        Vector2 randCor = RandomCoordinates(0, 0, sizeX, sizeZ);
        int i = 0;

        //Create a new island.
        int isCount = 0;
        bool newIsland = true;
        islands = new List<Vector3>();

        while (i < walkSize)
        {
            //Add a random function to start a new section.
            if(UnityEngine.Random.Range(0,100) < islandProbability && isCount < maxIslandCount)
            {
                randCor = RandomCoordinates(0, 0, sizeX, sizeZ);
                newIsland = true;
            }

            //Set a new direction. If valid, use it.
            Vector2 newDir = MazeDirection.RandDir();

            //While the coordinate is within the bounds of the grid, create cell at coordinate.
            if (ContainsCoordinates(randCor) && GetCell(randCor) == null)
            {
                //Find out if the cell has any neighbours within current island.
                if (!newIsland && findNeighbours(new Vector3(isCount, randCor.x, randCor.y)))
                {
                    CreateCell((int)randCor.x, (int)randCor.y);

                    islands.Add(new Vector3(isCount, randCor.x, randCor.y));
                    i++;
                }
                else
                {
                    //Create a cell as a new island.
                    CreateCell((int)randCor.x, (int)randCor.y);
                    isCount++;
                    newIsland = false;
                    islands.Add(new Vector3(isCount, randCor.x, randCor.y));
                    i++;
                }

            }


            //Change direction after it's been used.
            if (ContainsCoordinates(randCor + newDir))
            {
                //Choose new direction.
                randCor += newDir;
            }


        }

        //Go through each island and set it to an array.
        int c = islandCount();

        //Iterate to find small rooms and try to merge them.
        for(int p = 1; p <= c; p++)
        {
            int cellCount = countIslandCells(p, islandCellCountType.single);
            //See how many cells are withing the count.
            if (cellCount < 4)
            {
                for(int v = 0; v < islands.Count; v++)
                {
                    if(islands[v].x == p)
                    {
                        mergeToNeighbour(islands[v], c);
                    }
                }


                if(countIslandCells(p, islandCellCountType.single) < 3)
                {
                    //If there isn't any possible neighbours, just delete the cell.
                    //If there is a neighbour, merge them. Find the island cell and change the current neighbours.
                    for (int x = 0; x < islands.Count; x++)
                    {
                        if (islands[x].x == p)
                        {
                            removeIsland(x);
                            //Ensure a skip doesn't occur.
                            x--;
                        }
                    }
                }
            }
        }

        //Reorder the islands to ensure they are right in the island count.
        reorderIslands();

        //Redo the count.
        c = islandCount();

        //Reset edges
        setNoEdges();

        //Iterate through the amount of islands.
        for (int z = 1; z <= c; z++)
        {
            //Go through all the cells for that island.
            for(int b = 0; b < islands.Count; b++)
            {
                //Only go through the current island.
                if(islands[b].x == z)
                {
                    setNeighbours(islands[b]);
                }
            }
        }

        //Create the walls.
        CreateWall();

        //Now, create the the exit and entrance instances.
        lvlSpawnInstance = createZones(lvlSpawn, (int)zoneType.spawn);
        lvlExitInstance = createZones(lvlExit, (int)zoneType.exit);

        exitStairsInstance = createObject(exitStairs, lvlExitInstance.transform);

        //Ensure that there isn't a player already loaded. If they are loaded, then set position to lvlSpawnInstance.
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            playerInstance = Instantiate(Player, lvlSpawnInstance.transform.position, Quaternion.identity);
        } else
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
            playerInstance.transform.GetChild(1).transform.position = lvlSpawnInstance.transform.position;
            //Debug.Log("player: " + playerInstance.transform.position + " | lvlSpawn: " + lvlSpawnInstance.transform.position);
        }


        createZones(lvlInteractableZone, (int)zoneType.interactable);
        createBridges(lvlBridges[0], 0, 0.00f);
        createInteractable(lvlChests, 20f, 5);
    }

    /*
     * Script to create a cell.
     */
    private void CreateCell(int x, int z)
    {
        //Create a new cell and input the cell information into the grid array.
        GameObject newCell = Instantiate(cellPrefab, this.transform);
        cells[x, z] = newCell;
        //Now, set the cell location based on size of cell from bounds.
        newCell.transform.localPosition = new Vector3(x * cellSizeX, 0f, z * cellSizeZ);
    }

    /*
     * Create walls around the edges of a cell.
     */
    private void CreateWall()
    {
        //Go through all grid points and generate a wall at that point.
        //Iterate through all the grid points and find the open edges of each.
        for (int i = 0; i < sizeX; i++)
        {
            for (int u = 0; u < sizeZ; u++)
            {
                //If there is a cell in this zone, then check edges.
                if (cells[i, u] != null)
                {
                    //Get the object of that cell.
                    CellClass c = cells[i, u].GetComponent<CellClass>();

                    //Place a new object in the world at position.
                    if (!c.getLeft())
                    {
                        Instantiate(hWall, new Vector3(c.gameObject.transform.position.x - cellSizeX / 2f, 0, c.gameObject.transform.position.z), Quaternion.identity, c.gameObject.transform);
                    }

                    if (!c.getRight())
                    {
                        Instantiate(hWall, new Vector3(c.gameObject.transform.position.x + cellSizeX / 2f, 0, c.gameObject.transform.position.z), Quaternion.identity, c.gameObject.transform);
                    }

                    if (!c.getUp())
                    {
                        Instantiate(vWall, new Vector3(c.gameObject.transform.position.x, 0, c.gameObject.transform.position.z + cellSizeZ / 2), Quaternion.identity, c.gameObject.transform);
                    }

                    if (!c.getDown())
                    {
                        Instantiate(vWall, new Vector3(c.gameObject.transform.position.x, 0, c.gameObject.transform.position.z - cellSizeZ / 2), Quaternion.identity, c.gameObject.transform);
                    }
                }
            }
        }
    }

    private GameObject createObject(GameObject obj, Transform parent)
    {
        GameObject instance = Instantiate(obj, parent.position, Quaternion.identity, parent);

        return instance;
    }


    private void createBridges(GameObject obj, int type, float delay)
    {
        //Make different types based on distance.
        //Teleport (0) is without distance.
        if(type == 0)
        {
            if(islandCount() > 1)
            {
                //Create an array to give pair information.
                Vector2[] pairs = new Vector2[islandCount() - 1];

                int islandDivision;
                //Get first half of pairs. If even, just cut in half. If not, then add 1 to make even for division.
                if (islandCount() % 2 == 0)
                {
                    islandDivision = islandCount() / 2;

                } else
                {
                    islandDivision = (islandCount() + 1) / 2;
                }

                int pair = 0;

                //Get the amount of islands and if it's even, then half. If not, add one, halve, then remove the last.
                for(int i = 1; i < islandDivision; i++)
                {
                    pairs[pair] = new Vector2(i, i + 1);
                    pair++;
                }

                for(int i = islandDivision + 1; i <= islandCount(); i++)
                {
                    int rand = UnityEngine.Random.Range(1, islandDivision);
                    //If you go through all the counts and there is more in the count than island cells, then try again.
                    if (!countDuplicates(rand, countIslandCells(rand, islandCellCountType.interactableCount), pairs))
                    {
                        pairs[pair] = new Vector2(i, rand);
                    }
                    else
                    {
                        for (int y = 0; y < islandCount() - 1; y++)
                        {
                            if (!countDuplicates(y + 1, countIslandCells(y + 1, islandCellCountType.interactableCount), pairs))
                            {
                                pairs[pair] = new Vector2(i, y + 1);
                            }
                        }
                    }
                    pair++;
                }



                //islandCount() - 1) * 2
                /*List<int> islandNumbers = new List<int>();

                //Populate islandNumbers. Ensure each number has at least 1 in each, then random for the rest.
                for(int i = 0; i < (islandCount() - 1) * 2; i++)
                {
                    if(i < islandCount())
                    {
                        islandNumbers.Add(i + 1);
                    } else
                    {
                        int rand = UnityEngine.Random.Range(1, islandCount() + 1);

                        //If you go through all the counts and there is more in the count than island cells, then try again.
                        if(!countDuplicates(rand, countIslandCells(rand, islandCellCountType.interactableCount), islandNumbers)) {
                            islandNumbers.Add(rand);
                        } else
                        {
                            for(int y = 0; y < islandCount() - 1; y++)
                            {
                                if(!countDuplicates(y + 1, countIslandCells(y + 1, islandCellCountType.interactableCount), islandNumbers))
                                {
                                    islandNumbers.Add(y + 1);
                                }
                            }
                        }
                    }
                }

                for(int i = 0; i < islandNumbers.Count; i++)
                {
                    Debug.Log(islandNumbers[i] + " | " + countIslandCells(islandNumbers[i], islandCellCountType.interactableCount));
                }*/

                //Find the most duplicates and hold it largestAmount.
                /*int pair = 0;
                int first = 0;
                int last = islandNumbers.Count - 1;

                //Go through and link all the furthest points if capable of being linked.
                while(pair < pairs.Length)
                {
                    //If first and last are NOT the same, then make a link.
                    if(islandNumbers[first] != islandNumbers[last])
                    {
                        pairs[pair] = new Vector2(islandNumbers[first], islandNumbers[last]);
                        //Remove first and last.
                        islandNumbers.RemoveAt(first);
                        islandNumbers.RemoveAt(last - 1);
                        //reset last as things changed.
                        last = islandNumbers.Count - 1;
                        //Pair worked, so iterate.
                        pair++;
                    } else
                    {
                        last--;
                    }
                }*/

                //Print the pairs.
                /*for (int i = 0; i < pairs.Length; i++)
                {
                    if(pairs[i] != null)
                    {
                        Debug.Log("Pair " + i + ": " + pairs[i].x + " | " + pairs[i].y);
                    }
                }*/

                //First, count the duplicates of all the islands.
                /*int[] dupList = new int[islandCount()];

                int largestAmount = 0;

                for (int i = 0; i < dupList.Length; i++)
                {
                    dupList[i] = countDuplicates((i + 1), islandNumbers);

                    if(dupList[i] > largestAmount)
                    {
                        largestAmount = dupList[i];
                    }
                }*/

                //Start with the first, then take it to the one with the largest amount.
                /*while(islandNumbers.Count == 0)
                {
                    //Pick the first number. and placeholder 2.
                    int val1 = 0;
                    int val2 = val1;

                    int lAmount = 0;
                    int largestDup = 0;
                    //Find the largest duplicate.
                    for(int b = 0; b < dupList.Length; b++)
                    {
                        if(dupList[b] > lAmount && islandNumbers[val1] != b + 1)
                        {
                            largestDup = b + 1;
                            lAmount = dupList[b];
                        }
                    }

                    //Now, start iterating i until you run out of the amount, pairing as you go.
                    for(int i = 0; i < islandNumbers.Count; i++)
                    {
                        if(islandNumbers[i] == largestDup)
                        {
                            //Create a pair.
                        }
                    }

                }*/
                //Iterate through as many pairs for part 1.
                /*for (int b = 0; b < islandCount() - 1; b++)
                {
                    //This value will be the second number in the set.
                    int val1 = b + 1, val2 = val1;

                    int errorCount = 5;

                    //Ensure at least one island goes to the last island, as that doesn't get a chance to work.
                    if(val1 == 1)
                    {
                        val2 = islandCount();
                    }

                    int count = 0;

                    //Keep choosing a random number until a valid one is created.
                    while (val1 > 1 && (errorCount > 0 && val1 == val2))
                    {
                        //Debug.Log("Flag 1");
                        val2 = UnityEngine.Random.Range(1, islandCount() + 1);

                        //Debug.Log("Flag 2");
                        for (int x = 0; x < pairs.Length; x++)
                        {
                            //Ensure this isn't already used AND there is only 2 tiles.
                            if(pairs[x] != null && (pairs[x].x == val2 || pairs[x].y == val2))
                            {
                                count++;
                            }

                        }

                        //Debug.Log("Flag 3");
                        //Find out if this binding already exists.
                        for (int x = 0; x < pairs.Length; x++)
                        {
                            //Ensure this isn't already used AND there is only 2 tiles.
                            if (pairs[x] != null && pairs[x].y == val1 && pairs[x].x == val2)
                            {
                                //Failed. Restart.
                                val2 = val1;
                            }
                        }

                        //Debug.Log("Flag 4");
                        if (countIslandCells(val2, islandCellCountType.interactableCount) + 1 < count)
                        {
                            val2 = val1;
                        }

                        //Now iterate the error count.
                        errorCount--;
                    }

                    //Debug.Log("Flag 5");
                    pairs[b] = new Vector2(val1, val2);
                    //Debug.Log("Pair " + b + ": " + pairs[b].x + " | " + pairs[b].y);
                }*/




                bool valid = false;
                int counter = 0;
                for(int i = 1; i <= islandCount(); i++)
                {
                    for(int x = 0; x < pairs.Length; x++)
                    {
                        if(pairs[x].x == i || pairs[x].y == i)
                        {
                            counter++;
                            break;
                        }
                    }
                }

                if(counter == islandCount())
                {
                    valid = true;
                }

                counter = 0;
                for (int i = 1; i <= islandDivision; i++)
                {
                    for (int x = 0; x < pairs.Length; x++)
                    {
                        if (pairs[x].x == i || pairs[x].y == i)
                        {
                            counter++;
                        }
                    }

                    //Ensure to keep last run.
                    if(i != 1)
                    {
                        counter--;
                    }

                }

                if(counter == islandCount() - 1)
                {
                    valid = true;
                }

                if (!valid)
                {
                    Debug.Log(valid);
                }

                //Now, see how many duplicates of each their are. Match duplicates until at least one works off.

                //Print the pairs.
                /*for(int i = 0; i < pairs.Length; i++)
                {
                    if(pairs[i] != null)
                    {
                        Debug.Log("Pair " + i + ": " + pairs[i].x + " | " + pairs[i].y);
                    }
                }*/

                /*for (int x = 0; x < islands.Count; x++)
                {
                    Debug.Log(islands[x].x + " | " + (int)islands[x].y + " | " + (int)islands[x].z);
                }*/
                //Debug.Log("+++++++++++");
                //Decide how many teleporters are needed.
                for (int i = 0; i < pairs.Length; i++)
                {
                    List<Vector2> firstArr = new List<Vector2>();
                    List<Vector2> secondArr = new List<Vector2>();
                    //First, create two arrays pf interactable cell types that are linked to each island.
                    for (int x = 0; x < islands.Count; x++)
                    {
                        if(islands[x].x == (int)pairs[i].x)
                        {
                            if(interactableCells[(int)islands[x].y, (int)islands[x].z] != null)
                            {
                                firstArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                            }
                        }
                    }

                    for(int x = 0; x < islands.Count; x++)
                    {
                        if (islands[x].x == (int)pairs[i].y)
                        {
                            if (interactableCells[(int)islands[x].y, (int)islands[x].z] != null)
                            {
                                secondArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                            }
                        }
                    }

                    /*Debug.Log(firstArr.Count);
                    //Print the island pieces.
                    for(int b = 0; b < firstArr.Count; b++)
                    {
                        if(firstArr[b] != null)
                        {
                            Debug.Log(firstArr[b].x + " | " + firstArr[b].y);
                        }
                    }

                    Debug.Log(secondArr.Count);
                    //Print the island pieces.
                    for (int b = 0; b < secondArr.Count; b++)
                    {
                        if (secondArr[b] != null)
                        {
                            Debug.Log(secondArr[b].x + " | " + secondArr[b].y);
                        }
                    }*/

                    int randNum = UnityEngine.Random.Range(0, firstArr.Count);
                    int randNum2 = UnityEngine.Random.Range(0, secondArr.Count);

                    //Now create a teleporter within the island.
                    GameObject objt1 = Instantiate(obj, interactableCells[(int)firstArr[randNum].x, (int)firstArr[randNum].y].transform);
                    GameObject objt2 = Instantiate(obj, interactableCells[(int)secondArr[randNum2].x, (int)secondArr[randNum2].y].transform);

                    //Now, try and place a pair point on another island.
                    objt1.GetComponent<TeleporterScript>().setPartner(objt2, (int)pairs[i].y);
                    objt2.GetComponent<TeleporterScript>().setPartner(objt1, (int)pairs[i].x);

                    //Keep the zones, but remove it from the interactable cell list so it can't be used later.
                    interactableCells[(int)firstArr[randNum].x, (int)firstArr[randNum].y] = null;
                    interactableCells[(int)secondArr[randNum2].x, (int)secondArr[randNum2].y] = null;
                }
            }

        }
    }

    /*
     * Create some interactables, such as chests.
     */
    private void createInteractable(GameObject obj, float rand, int amount)
    {
        for(int i = 0; i < sizeX; i++)
        {
            for(int u = 0; u < sizeZ; u++)
            {
                if(interactableCells[i,u] != null)
                {
                    //Now if random placed chest needs to be placed, then place a chest.
                    if (UnityEngine.Random.Range(0, 100) < rand && amount > 0)
                    {
                        Instantiate(obj, interactableCells[i,u].transform);
                        interactableCells[i, u] = null;
                        amount--;
                    }
                }

            }
        }
    }

    /*
     * Create a spawn zone on one of the valid tiles.
     */
    private GameObject createZones(GameObject obj, int type)
    {
        GameObject objt = new GameObject();

        //Create the spawn.
        if (type == 1)
        {
            //Iterate through grid points.
            for (int i = 0; i < sizeX; i++)
            {
                for (int u = 0; u < sizeZ; u++)
                {
                    //If valid point, then spawn there.
                    if (cells[i, u] != null)
                    {
                        //Destroy the placeholder.
                        Destroy(objt);
                        objt = Instantiate(obj, GetCell(new Vector2(i, u)).transform.position, Quaternion.identity, GetCell(new Vector2(i, u)).transform);
                        return objt;
                    }
                }
            }
        } else if (type == 0)
        {
            //Iterate through grid points.
            for (int i = sizeX - 1; i >= 0; i--)
            {
                for (int u = sizeZ - 1; u >= 0; u--)
                {
                    //If valid point, then spawn there.
                    if (cells[i, u] != null)
                    {
                        //Destroy the placeholder.
                        Destroy(objt);
                        objt = Instantiate(obj, GetCell(new Vector2(i, u)).transform.position, Quaternion.identity, GetCell(new Vector2(i, u)).transform);
                        return objt;
                    }
                }
            }
        } else if (type == 2)
        {
            //Create a new interactable grid.
            interactableCells = new GameObject[sizeX, sizeZ];
            //Iterate through grid points.
            for (int i = 0; i < sizeX; i++)
            {
                for (int u = 0; u < sizeZ; u++)
                {
                    Destroy(objt);
                    //If valid point, then spawn there.
                    if (cells[i, u] != null && 
                        !GetCell(new Vector2(i,u)).transform.GetChild(GetCell(new Vector2(i, u)).transform.childCount - 1).CompareTag("Respawn") &&
                        !GetCell(new Vector2(i, u)).transform.GetChild(GetCell(new Vector2(i, u)).transform.childCount - 1).CompareTag("Finish"))
                    {
                        //Create random coordinates with the cell.
                        Collider cellCol = GetCell(new Vector2(i, u)).GetComponentInChildren<Collider>();
                        Vector2 pos = RandomCoordinates(cellCol.bounds.min.x - cellCol.bounds.center.x, 
                                                        cellCol.bounds.min.z - cellCol.bounds.center.z, 
                                                        cellCol.bounds.max.x - cellCol.bounds.center.x, 
                                                        cellCol.bounds.max.z - cellCol.bounds.center.z);

                        //Destroy the placeholder.
                        interactableCells[i,u] = Instantiate(obj, new Vector3(pos.x * 0.5f + GetCell(new Vector2(i, u)).transform.position.x, 0, 
                                                     pos.y * 0.5f + GetCell(new Vector2(i, u)).transform.position.z), Quaternion.identity, GetCell(new Vector2(i,u)).transform);
                    }
                }
            }
        }

        return objt;
    }

    private void setNeighbours(Vector3 cell)
    {
        //First, find a matching square.
        for(int x = 0; x < islands.Count; x++)
        {
            if(cell.x == islands[x].x)
            {
                CellClass c = cells[(int)cell.y, (int)cell.z].GetComponent<CellClass>();

                //Find left.
                if (cell.y - 1 == islands[x].y && cell.z == islands[x].z)
                {
                    c.setLeft(true);
                }

                //Find right.
                if (cell.y + 1 == islands[x].y && cell.z == islands[x].z)
                {
                    c.setRight(true);
                }

                //Find up
                if (cell.z + 1 == islands[x].z && cell.y == islands[x].y)
                {
                    c.setUp(true);
                }

                //Find down
                if (cell.z - 1 == islands[x].z && cell.y == islands[x].y)
                {
                    c.setDown(true);
                }
            }
        }
    }

    /*
     * A function to merge a cell to a neighbour.
     */
    private void mergeToNeighbour(Vector3 cell, int isCount)
    {
        //First, iterate through possible island counts.
        for(int i = 1; i <= isCount; i++)
        {
            if(findNeighbours(new Vector3(i, cell.y, cell.z)))
            {
                //If there is a neighbour, merge them. Find the island cell and change the current neighbours.
                for(int x = 0; x < islands.Count; x++)
                {
                    if(islands[x].y == cell.y && islands[x].z == cell.z)
                    {
                        islands[x] = new Vector3(i, cell.y, cell.z);
                        //After modification, return.
                        return;
                    }
                }
            }
        }
    }

    private void removeIsland(int index)
    {
        //Destroy the cell, then make the cell null, then remove from islands list.
        Destroy(GetCell(new Vector2((int)islands[index].y, (int)islands[index].z)));
        cells[(int)islands[index].y, (int)islands[index].z] = null;
        islands.RemoveAt(index);

        //Debug.Log("Deleted: " + index);
    }

    private bool findNeighbours(Vector3 cell)
    {
        //First, find a matching square.
        for (int x = 0; x < islands.Count; x++)
        {
            if (cell.x == islands[x].x)
            {

                //Find left.
                if (cell.y - 1 == islands[x].y && cell.z == islands[x].z)
                {
                    return true;
                }

                //Find right.
                if (cell.y + 1 == islands[x].y && cell.z == islands[x].z)
                {
                    return true;
                }

                //Find up
                if (cell.z + 1 == islands[x].z && cell.y == islands[x].y)
                {
                    return true;
                }

                //Find down
                if (cell.z - 1 == islands[x].z && cell.y == islands[x].y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void setNoEdges()
    {
        for (int y = 0; y < sizeX; y++)
        {
            for (int u = 0; u < sizeZ; u++)
            {
                if(cells[y,u] != null)
                {
                    CellClass c = cells[y, u].GetComponent<CellClass>();

                    c.setLeft(false);
                    c.setRight(false);
                    c.setUp(false);
                    c.setDown(false);
                }

            }
        }
    }

    /*
     * Return a cell from said coordinate.
     */
    public GameObject GetCell(Vector2 coordinates)
    {
        return cells[(int)coordinates.x, (int)coordinates.y];
    }

    /*
     * Generate random coordinates in a Vector2.
     */
    public Vector2 RandomCoordinates(float minX, float minY, float maxX, float maxY)
    {
        return new Vector2(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY));
    }

    public bool ContainsCoordinates(Vector2 coordinate)
    {
        //Ensure the coordinate is between 0 and X for x axis, and 0 and Z for the Z axis.
        return coordinate.x >= 0 && coordinate.x < sizeX && coordinate.y >= 0 && coordinate.y < sizeZ;
    }

    /*
     * ISLAND FUNCTIONS
     */
    private int islandCount()
    {
        int count = 0;
        int currentIsland = -1;
        for (int i = 0; i < islands.Count; i++)
        {
            if (islands[i].x != currentIsland)
            {
                count++;
                currentIsland = (int)islands[i].x;
            }
        }

        return count;
    }
    private int countIslandCells(int x, islandCellCountType t)
    {
        int count = 0;
        //Iterate through all islands.
        for (int i = 0; i < islands.Count; i++)
        {
            //If 't' is type 'fullcount', then count all cells UP to the end of the index island.
            if (t == islandCellCountType.fullcount)
            {
                if (islands[i].x <= x)
                {
                    count++;
                }
            //If 't' is type 'single' count only cells in the island.
            } else if (t == islandCellCountType.single)
            {
                if (islands[i].x == x)
                {
                    count++;
                }
            //If 't' is type 'interactableCount', then count all interactable cells in the island.
            } else if (t == islandCellCountType.interactableCount)
            {
                //Go through and match all cells in island and count it against interactable counts.
                for(int b = 0; b < sizeX; b++)
                {
                    for(int z = 0; z < sizeZ; z++)
                    {
                        //If x,z matches the coordinates of the island cell, then iterate count.
                        if(interactableCells[b,z] != null && islands[i].x == x && b == (int)islands[i].y && z == (int)islands[i].z)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    private void reorderIslands()
    {
        //First, order by number x.
        //Sort the list.
        islands = islands.OrderBy(v => v.x).ToList<Vector3>();

        float currentCount = 0;
        float numTemplate = -1;

        //Then change island number from 1 upwards.
        for(int i = 0; i < islands.Count; i++)
        {
            if(islands[i].x != currentCount)
            {
                if(islands[i].x != numTemplate)
                {
                    currentCount++;
                    numTemplate = islands[i].x;
                }

                islands[i] = new Vector3(currentCount, islands[i].y, islands[i].z);
            }
        }

    } 

    /*
 * A function to take a number, an amount and an array to find out if the amount of the duplicate number in the array is grater than the amount.
 * Returns true if so.
 */
    private bool countDuplicates(int num, int amount, int[] arr)
    {
        int count = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == num)
            {
                count++;
            }
        }

        if (count > amount)
        {
            return true;
        }

        return false;
    }

    private bool countDuplicates(int num, int amount, Vector2[] arr)
    {
        int[] fullCount = new int[arr.Length * 2];
        int arrCount = 0;
        for(int x = 0; x < fullCount.Length; x += 2)
        {
            fullCount[x] = (int)arr[arrCount].x;
            fullCount[x + 1] = (int)arr[arrCount].y;
            arrCount++;
        }

        int count = 0;
        for (int i = 0; i < fullCount.Length; i++)
        {
            if (fullCount[i] == num)
            {
                count++;
            }
        }

        if (count >= amount)
        {
            return true;
        }

        return false;
    }

    private bool countDuplicates(int num, int amount, List<int> arr)
    {
        int count = 0;
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] == num)
            {
                count++;
            }
        }

        if (count > amount)
        {
            return true;
        }

        return false;
    }

    private int countDuplicates(int num, List<int> arr)
    {
        int count = 0;
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] == num)
            {
                count++;
            }
        }

        return count;
    }
}

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
    public GameObject[] lvlAdditions;
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
        //setNoEdges();

        //Iterate through the amount of islands.
        for (int z = 1; z <= c; z++)
        {
            //Go through all the cells for that island.
            for(int b = 0; b < islands.Count; b++)
            {
                //Only go through the current island.
                if(islands[b].x == z)
                {
                    setWalls(islands[b]);
                }
            }
        }

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


        //createZones(lvlInteractableZone, (int)zoneType.interactable);
        createBridges(lvlBridges[0], 0, 0.00f);
        //createInteractables(lvlAdditions, 110f, 20);
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

    private void setWalls(Vector3 cell)
    {
        //Neighbour booleans.
        bool isUp = false;
        bool isRight = false;
        bool isDown = false;
        bool isLeft = false;

        //First, find a matching square.
        for (int x = 0; x < islands.Count; x++)
        {
            if (cell.x == islands[x].x)
            {
                //Find up (1)
                if (cell.z - 1 == islands[x].z && cell.y == islands[x].y)
                {
                    isUp = true;
                }

                //Find right. (2)
                if (cell.y + 1 == islands[x].y && cell.z == islands[x].z)
                {
                    isRight = true;
                }

                //Find down (3)
                if (cell.z + 1 == islands[x].z && cell.y == islands[x].y)
                {
                    isDown = true;
                }

                //Find left. (4)
                if (cell.y - 1 == islands[x].y && cell.z == islands[x].z)
                {
                    isLeft = true;
                }
            }
        }

        CellClass c = cells[(int)cell.y, (int)cell.z].GetComponent<CellClass>();

        //Add walls.
        if (!isUp)
        {
            c.addWall(vWall, 1, new Vector3(c.gameObject.transform.position.x, 0, c.gameObject.transform.position.z - cellSizeZ / 2f));
        }
        if (!isRight)
        {
            c.addWall(hWall, 2, new Vector3(c.gameObject.transform.position.x + cellSizeX / 2f, 0, c.gameObject.transform.position.z));
        }
        if (!isDown)
        {
            c.addWall(vWall, 3, new Vector3(c.gameObject.transform.position.x, 0, c.gameObject.transform.position.z + cellSizeZ / 2f));
        }
        if (!isLeft)
        {
            c.addWall(hWall, 4, new Vector3(c.gameObject.transform.position.x - cellSizeX / 2f, 0, c.gameObject.transform.position.z));
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
                    if (!countDuplicates(rand, countIslandCells(rand, islandCellCountType.single), pairs))
                    {
                        pairs[pair] = new Vector2(i, rand);
                    }
                    else
                    {
                        for (int y = 0; y < islandCount() - 1; y++)
                        {
                            if (!countDuplicates(y + 1, countIslandCells(y + 1, islandCellCountType.single), pairs))
                            {
                                pairs[pair] = new Vector2(i, y + 1);
                            }
                        }
                    }
                    pair++;
                }

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
                            //If the cell is not null AND has available slots, add to list.
                            if(GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)) != null && 
                              !GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)).GetComponent<CellClass>().getAllZonesUsed())
                            {
                                firstArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                            }
                        }
                    }

                    for(int x = 0; x < islands.Count; x++)
                    {
                        if (islands[x].x == (int)pairs[i].y)
                        {
                            if (GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)) != null &&
                              !GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)).GetComponent<CellClass>().getAllZonesUsed())
                            {
                                secondArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                            }
                        }
                    }

                    GameObject objt1;
                    GameObject objt2;

                    int randNum = UnityEngine.Random.Range(0, firstArr.Count);
                    int randZone = UnityEngine.Random.Range(0, 5);
                    //Create  the first teleporter at a random point on the map.
                    //Find out if the random zone is available.
                    if (GetCell(firstArr[randNum]).GetComponent<CellClass>().getZone(randZone) == null)
                    {
                        //If not, then find an available zone.
                        randZone = GetCell(firstArr[randNum]).GetComponent<CellClass>().findAvailable();
                    }

                    objt1 = Instantiate(obj, GetCell(firstArr[randNum]).GetComponent<CellClass>().getZone(randZone).position,
                        Quaternion.identity, GetCell(firstArr[randNum]).transform);
                    GetCell(firstArr[randNum]).GetComponent<CellClass>().removeZone(randZone);


                    randNum = UnityEngine.Random.Range(0, secondArr.Count);
                    randZone = UnityEngine.Random.Range(0, 5);
                    //Create a second teleporter at a random point on the map.
                    //Find out if the random zone is available.
                    if (GetCell(secondArr[randNum]).GetComponent<CellClass>().getZone(randZone) == null)
                    {
                        //If not, then find an available zone.
                        randZone = GetCell(secondArr[randNum]).GetComponent<CellClass>().findAvailable();
                    }

                    objt2 = Instantiate(obj, GetCell(secondArr[randNum]).GetComponent<CellClass>().getZone(randZone).position,
                        Quaternion.identity, GetCell(secondArr[randNum]).transform);
                    GetCell(secondArr[randNum]).GetComponent<CellClass>().removeZone(randZone);

                    //Now, try and place a pair point on another island.
                    objt1.GetComponent<TeleporterScript>().setPartner(objt2, (int)pairs[i].y);
                    objt2.GetComponent<TeleporterScript>().setPartner(objt1, (int)pairs[i].x);

                }
            }

        }
    }

    /* create Interactable [obsolete]
     * Create some interactables, such as chests.
     *
    private void createInteractables(GameObject[] objs, float rand, int amount)
    {
        for(int i = 0; i < sizeX; i++)
        {
            for(int u = 0; u < sizeZ; u++)
            {
                if(interactableCells[i,u] != null)
                {
                    //Now, go through all the zones in this cell.
                    for(int b = 0; b < 5; b++)
                    {
                        int num = UnityEngine.Random.Range(0, objs.Length + 1);
                        //Now if random placed chest needs to be placed, then place a chest.
                        if (num < objs.Length - 1 && UnityEngine.Random.Range(0, 100) < rand)
                        {
                            Instantiate(objs[num], interactableCells[i, u].transform.parent.GetChild(b + 1).transform.position, Quaternion.identity);
                        } else if (num < objs.Length && UnityEngine.Random.Range(0, 100) < rand && amount > 0)
                        {
                            Instantiate(objs[num], interactableCells[i, u].transform.parent.GetChild(b + 1).transform.position, Quaternion.identity);
                            amount--;
                        }
                    }

                    interactableCells[i, u] = null;

                }

            }
        }
    }*/

    /*
     * Create a spawn zone on one of the valid tiles.
     */
    private GameObject createZones(GameObject obj, int type)
    {
        GameObject objt = new GameObject();

        //Create the spawn.
        if (type == 0)
        {
            //Find the first island, and the gridpoint within it. Get a random number of the first island.
            int pos = UnityEngine.Random.Range(countIslandCells(1, islandCellCountType.fullcount) - countIslandCells(1, islandCellCountType.single), 
                                                countIslandCells(1, islandCellCountType.fullcount));

            //Find that island and create the zone on that island.
            if(cells[(int)islands[pos].y, (int)islands[pos].z] != null)
            {
                //Choose a random position to place the entrance.
                int randNum = UnityEngine.Random.Range(0, 5);
                Destroy(objt);
                //Create the respawn position in the middle of the 
                objt = Instantiate(obj, GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>().getZone(randNum).position, 
                                        Quaternion.identity, GetCell(new Vector2(islands[pos].y, islands[pos].z)).transform);

                //Now remove all interactions from this cell.
                for(int i = 0; i < 5; i++)
                {
                    GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>().removeZone(i);
                }

                return objt;
            }
        } else if (type == 1)
        {
            //Find the first island, and the gridpoint within it. Get a random number of the first island.
            int pos = UnityEngine.Random.Range(countIslandCells(islandCount() / 2, islandCellCountType.fullcount) - countIslandCells(islandCount() / 2, islandCellCountType.single), 
                countIslandCells(islandCount() / 2, islandCellCountType.fullcount));

            //Find that island and create the zone on that island.
            if (cells[(int)islands[pos].y, (int)islands[pos].z] != null)
            {
                //Choose a random position to place the entrance.
                int randNum = UnityEngine.Random.Range(0, 5);
                Destroy(objt);
                //Create the respawn position in the middle of the 
                objt = Instantiate(obj, GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>().getZone(randNum).position,
                                        Quaternion.identity, GetCell(new Vector2(islands[pos].y, islands[pos].z)).transform);

                //Remove JUST the zone that the exit is using.
                GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>().removeZone(randNum);

                return objt;
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

                        //Set the gameobject to the interactable cells.
                        interactableCells[i,u] = Instantiate(obj, new Vector3(pos.x * 0.5f + GetCell(new Vector2(i, u)).transform.position.x, 0, 
                                                     pos.y * 0.5f + GetCell(new Vector2(i, u)).transform.position.z), Quaternion.identity, GetCell(new Vector2(i,u)).transform);
                    }
                }
            }
        }

        return objt;
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

    /*
     * Set all the possible cells to have no walls.
     */
    private void setNoEdges()
    {
        for (int y = 0; y < sizeX; y++)
        {
            for (int u = 0; u < sizeZ; u++)
            {
                if(cells[y,u] != null)
                {
                    CellClass c = cells[y, u].GetComponent<CellClass>();

                    c.removeWalls(1, 4);
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

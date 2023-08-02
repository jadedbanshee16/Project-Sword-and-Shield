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
    public GameObject[] walls;

    [Header("Player Objects")]
    public GameObject[] lvlAspects;
    public GameObject[] lvlInstances;

    [Header("Player")]
    public GameObject Player;
    public GameObject playerInstance;

    [Header("Level objects")]
    public GameObject lvlInteractableZone;
    public GameObject[] lvlBridges;
    public GameObject[] lvlAdditions;
    public GameObject lvlChests;
    public GameObject path;

    private GameObject[,] cells;
    private GameObject[,] interactableCells;
    private List<Vector3> islands;
    private Vector2[] bridgePairs;

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
        //Make instances as large as aspects.
        lvlInstances = new GameObject[lvlAspects.Length];

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

        //createPaths(path);

        //Now, create the the exit and entrance instances.
        lvlInstances[0] = createPersistentZones(lvlAspects[0], (int)zoneType.spawn);
        lvlInstances[1] = createPersistentZones(lvlAspects[1], (int)zoneType.exit);

        //Ensure that there isn't a player already loaded. If they are loaded, then set position to lvlSpawnInstance.
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            playerInstance = Instantiate(Player, lvlInstances[0].transform.position, Quaternion.identity);
        } else
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;
            playerInstance.transform.GetChild(1).transform.position = lvlInstances[0].transform.position;
        }


        //Create pairs of islands in the pairs list for 'bridges'.
        createIslandPairs();

        //Generate puzzles in the world.
        //DEBUG: The only puzzle available is a teleporter. As more are added, have them generate different points.

        //Bridge objects. These are created objects that specifically deal with travelling across islands, and use island pairs.
        //Decide how many teleporters are needed.
        for (int b = 0; b < bridgePairs.Length; b++)
        {
            List<Vector2> firstArr = new List<Vector2>();
            List<Vector2> secondArr = new List<Vector2>();
            //First, create two arrays pf interactable cell types that are linked to each island.
            for (int x = 0; x < islands.Count; x++)
            {
                if (islands[x].x == (int)bridgePairs[b].x)
                {
                    //If the cell is not null AND has available slots, add to list.
                    if (GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)) != null &&
                      !GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)).GetComponent<CellClass>().getAllZonesUsed())
                    {
                        firstArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                    }
                }
            }

            for (int x = 0; x < islands.Count; x++)
            {
                if (islands[x].x == (int)bridgePairs[b].y)
                {
                    if (GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)) != null &&
                      !GetCell(new Vector2((int)islands[x].y, (int)islands[x].z)).GetComponent<CellClass>().getAllZonesUsed())
                    {
                        secondArr.Add(new Vector2((int)islands[x].y, (int)islands[x].z));
                    }
                }
            }

            //Find the closest cells per pair.
            Vector2 closestCells = findClosestCells(firstArr, secondArr);

            //If there is only 1 difference between the zones, remove a single wall.
            //If not, create teleporter.
            if(getDistanceBetweenCells(firstArr[(int)closestCells.x], secondArr[(int)closestCells.y]) == 1)
            {
                int rotation = 0;
                //Find the direction of the walls to be broken.
                //If vertical to each other and positive, the second sell is above the first.
                if(firstArr[(int)closestCells.x].x - secondArr[(int)closestCells.y].x == 0 && firstArr[(int)closestCells.x].y - secondArr[(int)closestCells.y].y > 0)
                {
                    GetCell(firstArr[(int)closestCells.x]).GetComponent<CellClass>().removeWalls(0);
                    GetCell(secondArr[(int)closestCells.y]).GetComponent<CellClass>().removeWalls(2);
                    rotation = 0;

                } else if (firstArr[(int)closestCells.x].x - secondArr[(int)closestCells.y].x < 0 && firstArr[(int)closestCells.x].y - secondArr[(int)closestCells.y].y == 0)
                {
                    GetCell(firstArr[(int)closestCells.x]).GetComponent<CellClass>().removeWalls(1);
                    GetCell(secondArr[(int)closestCells.y]).GetComponent<CellClass>().removeWalls(3);
                    rotation = 90;

                } else if (firstArr[(int)closestCells.x].x - secondArr[(int)closestCells.y].x == 0 && firstArr[(int)closestCells.x].y - secondArr[(int)closestCells.y].y < 0)
                {
                    GetCell(firstArr[(int)closestCells.x]).GetComponent<CellClass>().removeWalls(2);
                    GetCell(secondArr[(int)closestCells.y]).GetComponent<CellClass>().removeWalls(0);
                    rotation = 180;

                } else if (firstArr[(int)closestCells.x].x - secondArr[(int)closestCells.y].x > 0 && firstArr[(int)closestCells.x].y - secondArr[(int)closestCells.y].y == 0)
                {
                    GetCell(firstArr[(int)closestCells.x]).GetComponent<CellClass>().removeWalls(3);
                    GetCell(secondArr[(int)closestCells.y]).GetComponent<CellClass>().removeWalls(1);
                    rotation = 360;

                }

                //Now, add a package. So far, the teleporter package is all there is.
                MapObjectPackage newPackageObjects = Instantiate(lvlBridges[1], this.transform).GetComponent<MapObjectPackage>();

                int randNum = 0;

                for (int v = 0; v < newPackageObjects.objectAmount(); v++)
                {
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), GetCell(firstArr[(int)closestCells.x]).GetComponent<CellClass>(), true);
                }

            } else
            {
                //Now, add a package. So far, the teleporter package is all there is.
                MapObjectPackage newPackageObjects = Instantiate(lvlBridges[0], this.transform).GetComponent<MapObjectPackage>();

                int randNum = 0;

                for (int v = 0; v < newPackageObjects.objectAmount(); v++)
                {
                    //Find the cell position.
                    if (newPackageObjects.getPairIndex() < v)
                    {
                        randNum = UnityEngine.Random.Range(0, firstArr.Count);
                        createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), GetCell(firstArr[randNum]).GetComponent<CellClass>(), true);

                    }
                    else
                    {
                        randNum = UnityEngine.Random.Range(0, secondArr.Count);
                        createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), GetCell(secondArr[randNum]).GetComponent<CellClass>(), true);
                    }
                }
            }
        }


        createObjectZones(lvlAdditions, 3);
    }
    /*
     * Spawn an object in the world, if there is space for it.
     */
    private GameObject createObject(MapObject obj, CellClass cell, bool instantiated)
    {
        int index = cell.findAvailable(obj.takenSpaces);

        GameObject o = null;

        //Debug.Log(index);

        if (index > -1)
        {
            if (instantiated)
            {
                o = obj.gameObject;
                o.transform.position = cell.getZone(index).position;
                o.transform.rotation = Quaternion.identity;
                o.transform.SetParent(cell.gameObject.transform);
                cell.removeZone(index, obj.takenSpaces, o);
            } else
            {
                float prob = obj.getSpawnProbability();

                //If higher than percentage, then Instantiate.
                if (UnityEngine.Random.Range(0, 100) < prob)
                {
                    //Create a random rotation
                    o = Instantiate(obj.gameObject, cell.getZone(index).position, Quaternion.identity, cell.gameObject.transform);
                    cell.removeZone(index, obj.takenSpaces, o);
                }
            }
        }

        //Debug.Log("Created object: " + o);

        return o;
    }


    private void createIslandPairs()
    {
        bridgePairs = new Vector2[0];

        if (islandCount() > 1)
        {
            //Create an array to give pair information.
            bridgePairs = new Vector2[islandCount() - 1];

            int islandDivision;
            //Get first half of pairs. If even, just cut in half. If not, then add 1 to make even for division.
            if (islandCount() % 2 == 0)
            {
                islandDivision = islandCount() / 2;

            }
            else
            {
                islandDivision = (islandCount() + 1) / 2;
            }

            int pair = 0;

            //Get the amount of islands and if it's even, then half. If not, add one, halve, then remove the last.
            for (int i = 1; i < islandDivision; i++)
            {
                bridgePairs[pair] = new Vector2(i, i + 1);
                pair++;
            }

            for (int i = islandDivision + 1; i <= islandCount(); i++)
            {
                int rand = UnityEngine.Random.Range(1, islandDivision);
                //If you go through all the counts and there is more in the count than island cells, then try again.
                if (!countDuplicates(rand, countIslandCells(rand, islandCellCountType.single), bridgePairs))
                {
                    bridgePairs[pair] = new Vector2(i, rand);
                }
                else
                {
                    for (int y = 0; y < islandCount() - 1; y++)
                    {
                        if (!countDuplicates(y + 1, countIslandCells(y + 1, islandCellCountType.single), bridgePairs))
                        {
                            bridgePairs[pair] = new Vector2(i, y + 1);
                        }
                    }
                }
                pair++;
            }

            bool valid = false;
            int counter = 0;
            for (int i = 1; i <= islandCount(); i++)
            {
                for (int x = 0; x < bridgePairs.Length; x++)
                {
                    if (bridgePairs[x].x == i || bridgePairs[x].y == i)
                    {
                        counter++;
                        break;
                    }
                }
            }

            if (counter == islandCount())
            {
                valid = true;
            }

            counter = 0;
            for (int i = 1; i <= islandDivision; i++)
            {
                for (int x = 0; x < bridgePairs.Length; x++)
                {
                    if (bridgePairs[x].x == i || bridgePairs[x].y == i)
                    {
                        counter++;
                    }
                }

                //Ensure to keep last run.
                if (i != 1)
                {
                    counter--;
                }

            }

            if (counter == islandCount() - 1)
            {
                valid = true;
            }

            if (!valid)
            {
                Debug.Log(valid);
            }
        }
    }
    private void createObjectZones(GameObject[] objs, int cellAmount)
    {
        for(int i = 0; i < sizeX; i++)
        {
            for(int u = 0; u < sizeZ; u++)
            {
                if(GetCell(new Vector2(i,u)) != null)
                {
                    CellClass c = GetCell(new Vector2(i, u)).GetComponent<CellClass>();
                    //Ensure there are available places in the cell.
                    if (!c.isAllUsed())
                    {
                        int count = 5;

                        for(int b = 0; b < cellAmount; b++)
                        {

                            GameObject instance = createObject(objs[UnityEngine.Random.Range(0, objs.Length)].GetComponent<MapObject>(), c, false);

                            if(instance == null && count > 0)
                            {
                                b--;
                                count--;
                            }
                        }
                    }
                }
            }
        }
    }
    private GameObject createPersistentZones(GameObject obj, int type)
    {
        GameObject objt = null;

        //Create the spawn.
        if (type == 0)
        {
            //Find the first island, and the gridpoint within it. Get a random number of the first island.
            int pos = UnityEngine.Random.Range(countIslandCells(1, islandCellCountType.fullcount) - countIslandCells(1, islandCellCountType.single), 
                                                countIslandCells(1, islandCellCountType.fullcount));

            //Find that island and create the zone on that island.
            if(cells[(int)islands[pos].y, (int)islands[pos].z] != null)
            {
                objt = createObject(obj.GetComponent<MapObject>(), GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>(), false);
            }
        } else if (type == 1)
        {
            //Find the first island, and the gridpoint within it. Get a random number of the first island.
            int pos = UnityEngine.Random.Range(countIslandCells(islandCount() / 2, islandCellCountType.fullcount) - countIslandCells(islandCount() / 2, islandCellCountType.single), 
                countIslandCells(islandCount() / 2, islandCellCountType.fullcount));

            //Find that island and create the zone on that island.
            if (cells[(int)islands[pos].y, (int)islands[pos].z] != null)
            {
                objt = createObject(obj.GetComponent<MapObject>(), GetCell(new Vector2(islands[pos].y, islands[pos].z)).GetComponent<CellClass>(), false);
            }
        }
        return objt;
    }

    /*
     * Island Manipulation functions.
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
     * Cell manipulation functions.
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

        int randWall = 0;

        //Add walls.
        if (!isUp)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            c.addWall(walls[randWall], 1, 0);
        }
        if (!isRight)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            c.addWall(walls[randWall], 2, 90);
        }
        if (!isDown)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            c.addWall(walls[randWall], 3, 180);
        }
        if (!isLeft)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            c.addWall(walls[randWall], 4, 270);
        }
    }

    public GameObject GetCell(Vector2 coordinates)
    {
        return cells[(int)coordinates.x, (int)coordinates.y];
    }
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

    private Vector2 findClosestCells(List<Vector2> island1, List<Vector2> island2)
    {
        float dist = Mathf.Infinity;
        Vector2 c = new Vector2(0, 0);

        for(int i = 0; i < island1.Count; i++)
        {
            for(int u = 0; u < island2.Count; u++)
            {
                //Get the distance between the 2 cells.
                int newDist = getDistanceBetweenCells(island1[i], island2[u]);

                if(dist > newDist)
                {
                    dist = newDist;
                    c = new Vector2(i, u);
                }
            }
        }

        return c;
    }

    private int getDistanceBetweenCells(Vector2 cell1, Vector2 cell2)
    {
        return Mathf.Abs((int)cell1.x - (int)cell2.x) + Mathf.Abs((int)cell1.y - (int)cell2.y);
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

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

    public static Vector2 focusedDir(Vector2 dir)
    {
        Vector2 newDir = new Vector2(0,0);

        float rand = (int)UnityEngine.Random.Range(0, 100);

        if(dir.x >= 0)
        {
            if(dir.y <= 0)
            {
                if (rand < 50)
                {
                    return vectors[0];
                }
                else
                {
                    return vectors[1];
                }
            } else
            {
                if (rand < 50)
                {
                    return vectors[1];
                }
                else
                {
                    return vectors[2];
                }
            }
        } else
        {
            if (dir.y <= 0)
            {
                if (rand < 50)
                {
                    return vectors[0];
                }
                else
                {
                    return vectors[3];
                }
            }
            else
            {
                if (rand < 50)
                {
                    return vectors[2];
                }
                else
                {
                    return vectors[3];
                }
            }
        }
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
    public GameObject[] lvlChests;
    public GameObject[] lvlEnemiesPoints;

    private CellClass[,] cells;
    private BridgeClass[] bridges;
    //private List<Vector3> islands;
    private Vector2[] doorPairs;
    //private List<Vector2>[] paths;

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
            //walkSize = (int)maxWalkSize;
        }

        //Create a grid and populate cells.
        cells = new CellClass[sizeX, sizeZ];

        Vector2 randCor = RandomCoordinates(0, 0, sizeX, sizeZ);
        int i = 0;

        //Create a new island.
        int isCount = 1;
        bool newIsland = true;
        //islands = new List<Vector3>();

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
                if (!newIsland && findNeighbours(isCount, (int)randCor.x, (int)randCor.y))
                {
                    CreateCell(isCount, (int)randCor.x, (int)randCor.y);
                    i++;
                }
                else
                {
                    isCount++;
                    //Create a cell as a new island.
                    CreateCell(isCount, (int)randCor.x, (int)randCor.y);
                    newIsland = false;
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
        reorderIslandsNumbers();

        //Iterate to find small rooms and try to merge them.
        for (int p = 1; p <= c; p++)
        {
            //See how many cells are withing the count.
            if (countIslandCells(p, islandCellCountType.single) < 4)
            {
                //If the amount of cells in island is less than 4, go through each cell in that island and merge to another island.
                for(int v = 0; v < sizeX; v++)
                {
                    for(int k = 0; k < sizeZ; k++)
                    {
                        if(cells[v,k] != null && cells[v,k].getIslandNum() == p)
                        {
                            mergeToNeighbour(v, k);
                        }
                    }
                }

                //Debug.Log("Merged Island: " + countIslandCells(p, islandCellCountType.single));

                //If the island still has a few cells left in it, remove the island.
                if(countIslandCells(p, islandCellCountType.single) < 4 && countIslandCells(p, islandCellCountType.single) > 0)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for(int z = 0; z < sizeZ; z++)
                        {
                            if (cells[x,z] != null && cells[x,z].getIslandNum() == p)
                            {
                                removeIsland(x,z);
                                //Debug.Log("Deleted Cell");
                            }
                        }
                    }

                    //Debug.Log("Deleted Island: " + countIslandCells(p, islandCellCountType.single));
                }
            }
        }

        //Redo the count.
        c = islandCount();
        //Debug.Log(c);

        //Reorder.
        reorderIslandsNumbers();

        GetComponent<NavMeshSurface>().BuildNavMesh();


        /*for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if (cells[x, z] != null)
                {
                    Debug.Log("Cells made: " + x + "|" + z + "|" + cells[x, z].getIslandNum());
                }
            }
        }*/

        /*
         * Set the walls for all the cells.
         */
        //Go through all the cells for that island.
        for (int v = 0; v < sizeX; v++)
        {
            for (int k = 0; k < sizeZ; k++)
            {
                if(cells[v,k] != null)
                {
                    setWalls(cells[v, k].getIslandNum(), v, k);
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

        //A function to generate a path between two
        int[] spawn_endPath = new int[islandCount() / 2];
        for(int x = 0; x < islandCount() / 2; x++)
        {
            spawn_endPath[x] = x + 1;

            //Debug.Log("Spawn Path: " + spawn_endPath[x]);
        }

        //Now, go through and get the distance between each pair.
        int[] doorType = new int[doorPairs.Length];
        List<int>[] doorConnections = new List<int>[doorPairs.Length];

        //Initiate all the lists.
        for(int x = 0; x < doorConnections.Length; x++)
        {
            doorConnections[x] = new List<int>();
            doorConnections[x].Add((int)doorPairs[x].x);
        }


        //Find the bridge type of all the pairs based on distance.
        for(int x = 0; x < doorType.Length; x++)
        {
            //If the islands are close together AND not on the main path...
             if (findIslandDistance((int)doorPairs[x].x, (int)doorPairs[x].y) == 1)
             {
                bool onPath = false;
                for(int z = 0; z < spawn_endPath.Length; z++)
                {
                    if(z < spawn_endPath.Length - 1 && (doorPairs[x].x == spawn_endPath[z] && doorPairs[x].y == spawn_endPath[z + 1]))
                    {
                        onPath = true;
                    }
                }

                if (onPath)
                {
                    //Teleporters.
                    doorType[x] = 1;
                } else
                {
                    doorType[x] = 1;
                }

             } else
             {
                //Teleporters.
                doorType[x] = 0;
             }
        }

        //Find the possible connected connections for every door type.
        for(int x = 0; x < doorPairs.Length; x++)
        {
            //First, only do this for ones that are bridge types.
            if(doorType[x] == 1)
            {
                //Iterate through again to find all points starting with the same start as this loop.
                for(int currentLoop = 0; currentLoop < doorPairs.Length; currentLoop++)
                {
                    if(doorPairs[x].x == doorPairs[currentLoop].x)
                    {
                        if (!ContainsInt((int)doorPairs[currentLoop].y, doorConnections[x]) && doorType[currentLoop] != 1)
                        {
                            //If NOT a door, add the second point.
                            doorConnections[x].Add((int)doorPairs[currentLoop].y);
                        }
                    }
                }
            }
        }

        //Debug the list.
        /*for(int x = 0; x < bridgeconnections.Length; x++)
        {
            Debug.Log("Bridge: " + bridgePairs[x] + ", type: " + bridgeType[x]);

            for(int m = 0; m < bridgeconnections[x].Count; m++)
            {
                Debug.Log("    " + bridgeconnections[x][m]);
            }
        }*/

        //Now, go through door pairs.

        //Generate the bridge for all pairs.
        for (int b = 0; b < doorPairs.Length; b++)
        {
            //Create the possible arrays.
            List<Vector2> firstArr = new List<Vector2>();
            List<Vector2> secondArr = new List<Vector2>();

            //Go through and populate the array based on the cells of each section.
            //Iterate through all islands listed in the connected bridges.
            for (int x = 0; x < doorConnections[b].Count; x++)
            {
                //Go through all islands cells.
                for (int v = 0; v < sizeX; v++)
                {
                    for (int k = 0; k < sizeZ; k++)
                    {
                        //If the cell is the same island number as one in bridge connections and still available for zone use, then add to first array.
                        if (cells[v, k] != null && cells[v, k].getIslandNum() == doorConnections[b][x] && !cells[v, k].getAllZonesUsed())
                        {
                            //Add cell to the array.
                            firstArr.Add(new Vector2(v, k));
                        }
                    }
                }
            }

            //Do the same, but with the second array. This is stuck as just the only island.
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (cells[x, z] != null && cells[x, z].getIslandNum() == doorPairs[b].y && !cells[x, z].getAllZonesUsed())
                    {
                        secondArr.Add(new Vector2(x, z));
                    }
                }
            }

            //Debug.Log(firstArr.Count + "|" + secondArr.Count);

            createDoors(firstArr, secondArr, findClosestCells(firstArr, secondArr), doorType[b]);

        }

        createObjectZones(lvlEnemiesPoints, 2);
        createObjectZones(lvlChests, 2);
        createObjectZones(lvlAdditions, 3);

        //Debug section.
        debugSection(islandCount(), doorPairs);

        /*for(int z = 0; z < doorPairs.Length; z++)
        {
            Debug.Log(doorPairs[z]);
        }*/
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
            }
            else
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

    /*
     * Pair up the created islands based on the algorithm to create bridges between them.
     */
    private void createIslandPairs()
    {
        doorPairs = new Vector2[0];

        if (islandCount() > 1)
        {
            //Create an array to give pair information.
            doorPairs = new Vector2[islandCount() - 1];

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

            //Get the first half, then generate the pair using the island, then 1 above the island.
            for (int i = 1; i < islandDivision; i++)
            {
                doorPairs[pair] = new Vector2(i, i + 1);
                pair++;
            }

            for (int i = islandDivision + 1; i <= islandCount(); i++)
            {
                int rand = UnityEngine.Random.Range(1, i);

                //Ensure the smallest number is on the left.
                if (i > rand)
                {
                    doorPairs[pair] = new Vector2(rand, i);
                }
                else
                {
                    doorPairs[pair] = new Vector2(i, rand);
                }
                //count duplicates to see if 
                /*if (!countDuplicates(rand, countIslandCells(rand, islandCellCountType.single), bridgePairs))
                {
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
                }*/
                pair++;
            }

            bool valid = false;
            int counter = 0;
            for (int i = 1; i <= islandCount(); i++)
            {
                for (int x = 0; x < doorPairs.Length; x++)
                {
                    if (doorPairs[x].x == i || doorPairs[x].y == i)
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
                for (int x = 0; x < doorPairs.Length; x++)
                {
                    if (doorPairs[x].x == i || doorPairs[x].y == i)
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

    private void createDoors(List<Vector2> Arr1, List<Vector2> Arr2, Vector2 cell, int type)
    {
        //If there is only 1 difference between the zones, remove a single wall.
        //If not, create teleporter.
        if (type == 1)
        {
            int rotation = 0;
            int index = 0;
            //Find the direction of the walls to be broken.
            //If vertical to each other and positive, the second sell is above the first.
            if (Arr1[(int)cell.x].x - Arr2[(int)cell.y].x == 0 && Arr1[(int)cell.x].y - Arr2[(int)cell.y].y > 0)
            {
                GetCell(Arr1[(int)cell.x]).GetComponent<CellClass>().removeWalls(0);
                GetCell(Arr2[(int)cell.y]).GetComponent<CellClass>().removeWalls(2);
                rotation = 0;
                index = 1;


            }
            else if (Arr1[(int)cell.x].x - Arr2[(int)cell.y].x < 0 && Arr1[(int)cell.x].y - Arr2[(int)cell.y].y == 0)
            {
                GetCell(Arr1[(int)cell.x]).GetComponent<CellClass>().removeWalls(1);
                GetCell(Arr2[(int)cell.y]).GetComponent<CellClass>().removeWalls(3);
                rotation = 90;
                index = 2;

            }
            else if (Arr1[(int)cell.x].x - Arr2[(int)cell.y].x == 0 && Arr1[(int)cell.x].y - Arr2[(int)cell.y].y < 0)
            {
                GetCell(Arr1[(int)cell.x]).GetComponent<CellClass>().removeWalls(2);
                GetCell(Arr2[(int)cell.y]).GetComponent<CellClass>().removeWalls(0);
                rotation = 180;
                index = 3;

            }
            else if (Arr1[(int)cell.x].x - Arr2[(int)cell.y].x > 0 && Arr1[(int)cell.x].y - Arr2[(int)cell.y].y == 0)
            {
                GetCell(Arr1[(int)cell.x]).GetComponent<CellClass>().removeWalls(3);
                GetCell(Arr2[(int)cell.y]).GetComponent<CellClass>().removeWalls(1);
                rotation = 270;
                index = 4;

            }

            //Now, add a package. So far, the teleporter package is all there is.
            MapObjectPackage newPackageObjects = Instantiate(lvlBridges[1], this.transform).GetComponent<MapObjectPackage>();

            //Create all pieces based on type.
            newPackageObjects.gameObject.GetComponent<DoorPieceSpawn>().createPieces();
            newPackageObjects.collectObjects();

            for (int v = 0; v < newPackageObjects.objectAmount(); v++)
            {
                if (v == 0)
                {
                    GetCell(Arr1[(int)cell.x]).GetComponent<CellClass>().addWall(newPackageObjects.getObject(v).gameObject, index, rotation, true);
                }
                else
                {
                    //Change here to go through the 'piece' and spawn it's stuff into each section.
                    int randNum = UnityEngine.Random.Range(0, Arr1.Count);
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), GetCell(Arr1[randNum]).GetComponent<CellClass>(), true);
                }
            }

        }
        else if(type == 0)
        {
            //Now, add a package. So far, the teleporter package is all there is.
            MapObjectPackage newPackageObjects = Instantiate(lvlBridges[0], this.transform).GetComponent<MapObjectPackage>();

            int randNum = 0;

            for (int v = 0; v < newPackageObjects.objectAmount(); v++)
            {
                //Find the cell position.
                if (newPackageObjects.getPairIndex() < v)
                {
                    randNum = UnityEngine.Random.Range(0, Arr1.Count);
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), cells[(int)Arr1[randNum].x, (int)Arr1[randNum].y], true);

                }
                else
                {
                    randNum = UnityEngine.Random.Range(0, Arr2.Count);
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), cells[(int)Arr2[randNum].x, (int)Arr2[randNum].y], true);
                }
            }
        } else if (type == 2)
        {
            MapObjectPackage newPackageObjects = Instantiate(lvlBridges[2], this.transform).GetComponent<MapObjectPackage>();

            //Now, create the set objecta (destination and ending in given locations on the map.
            int randNum = 0;

            for (int v = 0; v < newPackageObjects.objectAmount(); v++)
            {
                //Find the cell position.
                if (newPackageObjects.getPairIndex() < v)
                {
                    randNum = UnityEngine.Random.Range(0, Arr1.Count);
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), cells[(int)Arr1[randNum].x, (int)Arr1[randNum].y], true);

                }
                else
                {
                    randNum = UnityEngine.Random.Range(0, Arr2.Count);
                    createObject(newPackageObjects.getObject(v).GetComponent<MapObject>(), cells[(int)Arr2[randNum].x, (int)Arr2[randNum].y], true);
                }
            }

            //Now, get the first object, and make it create the bridges.
            newPackageObjects.getObject(0).GetComponent<BridgeClass>().createBridge();
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

                            //If an enemy spawn, set the enemy spawn.
                            if(instance != null && instance.GetComponent<EnemySpawn>())
                            {
                                instance.GetComponent<EnemySpawn>().setIsland(cells[i,u].getIslandNum());
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
            Vector2 pos = getRandomCoordinatesInIsland(1);

            //Find that island and create the zone on that island.
            if (cells[(int)pos.x, (int)pos.y] != null)
            {
                objt = createObject(obj.GetComponent<MapObject>(), cells[(int)pos.x, (int)pos.y], false);
            }
        } else if (type == 1)
        {
            int isl = islandCount();
            if (islandCount() > 1)
            {
                isl = islandCount() / 2;
            }

            //Find the second island, and the gridpoint within it. Get a random number of the second island.
            Vector2 pos = getRandomCoordinatesInIsland(isl);

            //Find that island and create the zone on that island.
            if (cells[(int)pos.x, (int)pos.y] != null)
            {
                objt = createObject(obj.GetComponent<MapObject>(), cells[(int)pos.x, (int)pos.y], false);
            }
        }
        return objt;
    }

    /*
     * Island Manipulation functions.
     */
    private void mergeToNeighbour(int posX, int posZ)
    {
        //Go through every island and see if it neighbours the current island.
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                //If neighbour is found, then set new island Number
                if (cells[x, z] != null && cells[x, z].getIslandNum() != cells[posX, posZ].getIslandNum() && findNeighbours(cells[x, z].getIslandNum(), posX, posZ))
                {
                    cells[posX, posZ].setIslandNum(cells[x,z].getIslandNum());
                    return;
                }
            }
        }
    }

    private void removeIsland(int posX, int posZ)
    {
        //Destroy the cell, then make the cell null, then remove from islands list.
        Destroy(cells[posX,posZ].gameObject);
        cells[posX, posZ] = null;
        //Debug.Log("Deleted: " + index);
    }

    private bool findNeighbours(int islandNum, int posX, int posZ)
    {
        /*for(int x = 0; x < (sizeX * sizeZ); x++)
        {
            Vector2 coord = getCellCoordinates()
        }*/
        //First, find a matching square.
        for (int x = 0; x < sizeX; x++)
        {
            for(int z = 0; z < sizeZ; z++)
            {
                if(cells[x,z] != null && islandNum == cells[x, z].getIslandNum())
                {
                    //Find left.
                    if (posX - 1 == x && posZ == z)
                    {
                        return true;
                    }

                    //Find right.
                    if (posX + 1 == x && posZ == z)
                    {
                        return true;
                    }

                    //Find up
                    if (posZ + 1 == z && posX == x)
                    {
                        return true;
                    }

                    //Find down
                    if (posZ - 1 == z && posX == x)
                    {
                        return true;
                    }

                }
            }
        }

        return false;
    }

    /*
     * Cell manipulation functions.
     */
    private void CreateCell(int islandNum, int x, int z)
    {
        //Create a new cell and input the cell information into the grid array.
        GameObject newCell = Instantiate(cellPrefab, this.transform);
        cells[x, z] = newCell.GetComponent<CellClass>();
        cells[x, z].setIslandNum(islandNum);
        cells[x, z].setZones();
        //Now, set the cell location based on size of cell from bounds.
        newCell.transform.localPosition = new Vector3(x * cellSizeX, 0f, z * cellSizeZ);
    }

    /*
     * A function to enforce walls to a specific cells using coordinates and island number.
     */
    private void setWalls(int islandNum, int posX, int posZ)
    {
        //Neighbour booleans.
        bool isUp = false;
        bool isRight = false;
        bool isDown = false;
        bool isLeft = false;

        //First, find a matching square.
        for (int x = 0; x < sizeX; x++)
        {
            for(int z = 0; z < sizeZ; z++)
            {
                if(cells[x,z] != null)
                {
                    //Find up (1)
                    if (posZ - 1 == z && posX == x && cells[x,z].getIslandNum() == cells[posX,posZ].getIslandNum())
                    {
                        isUp = true;
                    }

                    //Find right. (2)
                    if (posX + 1 == x && posZ == z && cells[x, z].getIslandNum() == cells[posX, posZ].getIslandNum())
                    {
                        isRight = true;
                    }

                    //Find down (3)
                    if (posZ + 1 == z && posX == x && cells[x, z].getIslandNum() == cells[posX, posZ].getIslandNum())
                    {
                        isDown = true;
                    }

                    //Find left. (4)
                    if (posX - 1 == x && posZ == z && cells[x, z].getIslandNum() == cells[posX, posZ].getIslandNum())
                    {
                        isLeft = true;
                    }
                }
            }
        }

        int randWall = 0;

        //Add walls.
        if (!isUp)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            cells[posX,posZ].addWall(walls[randWall], 1, 0, false);
        }
        if (!isRight)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            cells[posX,posZ].addWall(walls[randWall], 2, 90, false);
        }
        if (!isDown)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            cells[posX,posZ].addWall(walls[randWall], 3, 180, false);
        }
        if (!isLeft)
        {
            randWall = (UnityEngine.Random.Range(0, walls.Length));
            cells[posX,posZ].addWall(walls[randWall], 4, 270, false);
        }
    }

    public CellClass GetCell(Vector2 coordinates)
    {
        return cells[(int)coordinates.x, (int)coordinates.y];
    }

    public Vector2 getCellCoordinates(int index)
    {
        return new Vector2((int)(sizeX / index), (int)(sizeX % index));
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

    public bool ContainsInt(int num, List<int> list)
    {
        bool contain = false;

        for(int i = 0; i < list.Count; i++)
        {
            if(num == list[i])
            {
                contain = true;
            }
        }

        return contain;
    }

    /*
     * ISLAND FUNCTIONS
     */
    private int islandCount()
    {
        List<int> islands = new List<int>();
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                if(cells[x,z] != null)
                {
                    islands.Add(cells[x, z].getIslandNum());
                }
            }
        }

        islands = Enumerable.ToList(Enumerable.Distinct(islands));

        return islands.Count;
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

    private int findIslandDistance(int island1, int island2)
    {
        //The lists that would keep coordinates on all cells on both the front and back side of a bridge.
        List<Vector2> firstArr = new List<Vector2>();
        List<Vector2> secondArr = new List<Vector2>();

        //First, create two arrays of cells that are linked to each island.
        //Go through all cells.
        for (int x = 0; x < sizeX; x++)
        {
            for(int z = 0; z < sizeZ; z++)
            {
                if(cells[x,z] != null)
                {
                    //If matches island number 1 and there are zones available for use, add to first array.
                    if (cells[x, z].getIslandNum() == island1 && !cells[x, z].getAllZonesUsed())
                    {
                        firstArr.Add(new Vector2(x, z));
                    }

                    //If matches islandNumber 2 and have zones available, add to second array.
                    if(cells[x, z].getIslandNum() == island2 && !cells[x, z].getAllZonesUsed())
                    {
                        secondArr.Add(new Vector2(x, z));
                    }
                }
            }
        }

        //Get current distance between cells.
        float dist = Mathf.Infinity;

        for (int i = 0; i < firstArr.Count; i++)
        {
            for (int u = 0; u < secondArr.Count; u++)
            {
                //Get the distance between the 2 cells.
                int newDist = getDistanceBetweenCells(firstArr[i], secondArr[u]);

                if (dist > newDist)
                {
                    dist = newDist;
                }
            }
        }

        return (int)dist;
    }

    private int getDistanceBetweenCells(Vector2 cell1, Vector2 cell2)
    {
        return Mathf.Abs((int)cell1.x - (int)cell2.x) + Mathf.Abs((int)cell1.y - (int)cell2.y);
    }


    private int countIslandCells(int islandNum, islandCellCountType t)
    {
        int count = 0;

        //Go through all the all the cells.
        for(int x = 0; x < sizeX; x++)
        {
            for(int z = 0; z < sizeZ; z++)
            {
                //If the search type is single, only search through the particular island stipulated.
                if (t == islandCellCountType.single)
                {
                    if (cells[x, z] != null && cells[x,z].getIslandNum() == islandNum)
                    {
                        count++;
                    }

                
                } else if (t == islandCellCountType.fullcount)
                {

                }
            }
        }

        //Iterate through all islands.
        /*for (int i = 0; i < islands.Count; i++)
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
                        if(islands[i].x == x && b == (int)islands[i].y && z == (int)islands[i].z)
                        {
                            count++;
                        }
                    }
                }
            }
        }*/

        return count;
    }

    private Vector2 getRandomCoordinatesInIsland(int islandNum)
    {
        List<Vector2> cList = new List<Vector2>();
        for(int x = 0; x < sizeX; x++)
        {
            for(int z = 0; z < sizeZ; z++)
            {
                if(cells[x,z] != null && cells[x,z].getIslandNum() == islandNum)
                {
                    cList.Add(new Vector2(x, z));
                }
            }
        }

        //Debug.Log("Island Num: " + islandNum);
        //Debug.Log("Amount of cells: " + cList.Count);

        return cList[(int)UnityEngine.Random.Range(0, cList.Count)];
    }

    /*
     * A function that goes through that changes island numbers to 1 - how many islands.
     */
    private void reorderIslandsNumbers()
    {
        //First, find the next largest number and keep going down until you hit 1.
        float currentNumber = Mathf.Infinity;
        int newNumber = 1;

        while(newNumber <= islandCount())
        {
            //Find largest number.
            for (int x = 0; x < sizeX; x++)
            {
                for(int z = 0; z < sizeZ; z++)
                {
                    if(cells[x,z] != null && cells[x,z].getIslandNum() < currentNumber && cells[x,z].getIslandNum() > newNumber)
                    {
                        currentNumber = cells[x, z].getIslandNum();
                    }
                }
            }

            //Now go through and change all those numbers of that number. Iterate new number and start again.
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (cells[x, z] != null && cells[x, z].getIslandNum() == currentNumber)
                    {
                        cells[x, z].setIslandNum(newNumber);

                        //Debug.Log(currentNumber + "|" + cells[x,z].getIslandNum());
                    }
                }
            }

            //Iterate new number. and reset current Number.
            newNumber++;
            currentNumber = Mathf.Infinity;
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


    private void debugSection(int iscount, Vector2[] bridgePairs)
    {
        //Go through and the count island cells.
        //If there is an invalid number of cells, then test.
        for (int p = 1; p < iscount; p++)
        {
            if (countIslandCells(p, islandCellCountType.single) < 4)
            {
                Debug.LogError("Invalid Island: " + p + "| islandCount: " + islandCount() + "| amount in island: " + countIslandCells(p, islandCellCountType.single));
            }
        }

        //Go through and ensure every island has at least one pairing.
        bool found = false;
        for (int i = 1; i < iscount; i++)
        {
            for(int x = 0; x < bridgePairs.Length; x++)
            {
                if(bridgePairs[x].x == i || bridgePairs[x].y == i)
                {
                    found = true;
                }
            }

            if (!found)
            {
                Debug.LogError("Invalid Bridge connection: " + i + " has no connections.");
            }
        }

        //Now, ensure that it is possible to get to any island from the start based on positions of the chests / terminals.

        //Create a step process that works through going to all the pairs, staring with island 1.
        int currentIsland = 1;

        //Debug.Log(iscount);
        int unexploredIslands = iscount - 1;
        int steps = 100;
        List<int> exploredIslands = new List<int>() { 1 };
        List<int> islandWalk = new List<int>() { 1 };

        //Start the walk.
        while(unexploredIslands > 0 && steps > 0)
        {
            /*Debug.Log("--------");
            Debug.Log("Current Island: " + currentIsland);
            Debug.Log("Steps: " + steps);
            for (int z = 0; z < exploredIslands.Count; z++)
            {
                Debug.Log("Explored islands" + exploredIslands[z]);
            }
            for (int z = 0; z < islandWalk.Count; z++)
            {
                Debug.Log("Wall Path" + exploredIslands[z]);
            }
            Debug.Log("Unexplored Islands: " + unexploredIslands);
            Debug.Log("--------");*/
            bool moved = false;
            //Take a step using one of the pairs.
            for(int i = 0; i < bridgePairs.Length; i++)
            {
                //If the pair starts on your island, and moves to an island yet unexplored, then explore that island.
                if(!moved && bridgePairs[i].x == currentIsland)
                {
                    bool hasIsland = false;
                    for(int f = 0; f < exploredIslands.Count; f++)
                    {
                        if(exploredIslands[f] == bridgePairs[i].y)
                        {
                            hasIsland = true;
                        }
                    }

                    //Debug.Log((int)bridgePairs[i].y + " in explored island: " + hasIsland);

                    if (!hasIsland)
                    {
                        //Debug.Log("enter");
                        //New island has been explored.
                        exploredIslands.Add((int)bridgePairs[i].y);
                        currentIsland = (int)bridgePairs[i].y;
                        islandWalk.Add(currentIsland);
                        steps--;
                        unexploredIslands--;
                        moved = true;
                    }
                }
            }

            //If you made it this far, there are no more valid movements to make. 
            if (!moved)
            {
                islandWalk.RemoveAt(islandWalk.Count - 1);
                steps--;
                currentIsland = islandWalk[^1];
            }
        }

        /*for(int i = 0; i < exploredIslands.Count; i++)
        {
            Debug.Log("Explored Islands: " + exploredIslands[i]);
        }*/

        //Now to see if all islands were visited.
        if(unexploredIslands > 0)
        {
            Debug.LogError("Invalid Island pairs: Not all islands reachable from start");
        }
    }
}

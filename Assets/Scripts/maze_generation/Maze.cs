using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Maze : MonoBehaviour
{
    // World dimensions
    static public int rows = 30;
    static public int cols = 30;
    static public float mapHeight = 100;
    static public float mapWidth = 100;

    // Objects
    public Transform wallHolder;
    public Transform wallPrefab;
    public GameObject floor;
    public Transform agent;
    public Transform goal;
    public Transform collectible;
    public Transform enemy;

    // Private members
    [HideInInspector]
    public static bool[,] occupancy;
    private List<Wall> walls = new List<Wall>();

    // public void Maze(int r, int c) {
    //     rows = r;
    //     cols = c;
    // }

    void Start()
    {
        int numWalls = 0;
        int numCollectibles = 5;
        int numEnenmies = 3;
        ElementPositions elems = InstantiateMap(numWalls, numCollectibles, numEnenmies);

        Debug.Log(occupancy);
    }

    private ElementPositions InstantiateMap(int numWalls, int numCollectibles, int numEnenmies)
    {
        occupancy = new bool[rows, cols];

        // Build walls
        for (int i = 0; i < numWalls; i++)
        {
            Wall w = new Wall();
            walls.Add(w);
            w.InstantiateWall(wallHolder, wallPrefab);
            occupancy = w.markOccupancy();
        }
        printOccupancy();

        // Sample 
        Cell root = new Cell(); // Random Valid cell
        root.rootDistance = 0;

        // Get accessible cells
        List<Cell> accessibleCells = ComputeAccessibility(root);
        int accessibleCount = accessibleCells.Count;

        // Sample ElementPositions
        ElementPositions elems = new ElementPositions();
        elems.startPos = root;
        elems.finishPos = accessibleCells[UnityEngine.Random.Range(2*accessibleCount/3, accessibleCount)];
        return elems;
    }

    private List<Cell> ComputeAccessibility(Cell root)
    {
        List<Cell> accessibleCells = new List<Cell>();
        Queue<Cell> q = new Queue<Cell>();
        q.Enqueue(root);

        bool[,] visibility = new bool[rows, cols];
        visibility[root.a, root.b] = true;

        int[] deltaA = {0, 0, -1, 1};
        int[] deltaB = {-1, 1, 0, 0};
        int i, newA, newB; // Avoid re-allocating memory
        while(q.Count != 0)
        {
            Cell c = q.Dequeue();
            accessibleCells.Add(c);

            // Get neighbors
            for (i = 0; i < 4; i++)
            {
                newA = c.a + deltaA[i];
                newB = c.b + deltaB[i];
                // If its valid and hasn't been visited, we add it to the queue
                if (!Cell.IsOccupied(newA, newB) && !visibility[newA, newB])
                {
                    Cell neighbor = new Cell(newA, newB, c.rootDistance + 1);
                    q.Enqueue(neighbor);
                    visibility[newA, newB] = true;
                }
            }
        }
        string mapstr = "";
        for (i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                if (i == root.a && j == root.b) mapstr += " X "; 
                else if (visibility[i, j]) mapstr += "  .  ";
                else mapstr += " 0 ";
            }
            mapstr += "\n";
        }
        Debug.Log(mapstr);

        return accessibleCells;
    }


    void printOccupancy()
    {
        string mapstr = "";
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                if (occupancy[i, j]) mapstr += " X ";
                else mapstr += "  .  ";
            }
            mapstr += "\n";
        }
        Debug.Log(mapstr);
    }

    public class Wall
    {
        private Cell cell;
        private float length;
        private float theta;

        // If no arguments passed to the constructor, it sets everything random
        public Wall()
        {
            cell = new Cell();
            length = UnityEngine.Random.Range(20, 60);
            theta = UnityEngine.Random.Range(-90, 90);
        }

        public Wall(Cell loc, float len, float angle)
        {
            cell = loc;
            length = len;
            theta = angle;
        }

        public void InstantiateWall(Transform wallHolder, Transform wallPrefab)
        {
            Transform instantiatedWall = Instantiate(wallPrefab);
            instantiatedWall.parent = wallHolder;
            instantiatedWall.localScale = new Vector3(1, 5, length);
            instantiatedWall.rotation = Quaternion.Euler(0, 90-theta, 0);
            instantiatedWall.localPosition = new Vector3(cell.x, 2, cell.z);
        }

        // Returns a bool 2D multi-array considering current wall
        public bool[,] markOccupancy() {
            bool[,] occup = new bool[Maze.rows, Maze.cols];
            occup = Maze.occupancy; //NOTE: I hope this performs a deep copy
            Cell origin = new Cell(cell.x + length*Mathf.Cos(Mathf.PI*theta/180.0f)/2, 
                                cell.z + length*Mathf.Sin(Mathf.PI*theta/180.0f)/2);
            Cell final = new Cell(cell.x - length*Mathf.Cos(Mathf.PI*theta/180.0f)/2, 
                                cell.z - length*Mathf.Sin(Mathf.PI*theta/180.0f)/2);
            
            // Iterate over rows
            int sign = (origin.a <= final.a) ? 1 : -1; 
            int i = origin.a;
            int j;
            while (i != final.a)
            {
                j = (int) (origin.b + (final.b - origin.b)*(i - origin.a)/((float) (final.a - origin.a)));
                if (Cell.IsValid(i, j))
                {
                    occup[i, j] = true;
                }
                i += sign;
            }

            // Iterate over cols
            sign = (origin.b <= final.b) ? 1 : -1; 
            j = origin.b;
            while (j != final.b)
            {
                i = (int) (origin.a + (final.a - origin.a)*(j - origin.b)/((float) (final.b - origin.b)));
                if (Cell.IsValid(i, j))
                {
                    occup[i, j] = true;
                }
                j += sign;
            }
            return occup;
        }
    }
}

public class Cell
{
    public int a, b;
    public float x, z;
    public int rootDistance;

    // If nothing passed, it randomly picks a valid value
    public Cell()
    {
        int aCoord, bCoord;
        do
        {
            aCoord = UnityEngine.Random.Range(0, Maze.cols);
            bCoord = UnityEngine.Random.Range(0, Maze.rows);
        }
        while(IsOccupied(aCoord, bCoord));
        
        SetCoord(aCoord, bCoord);
    }

    public Cell(int aCoord, int bCoord)
    {
        SetCoord(aCoord, bCoord);
    }

    public Cell(int aCoord, int bCoord, int distToRoot)
    {
        SetCoord(aCoord, bCoord);
        rootDistance = distToRoot;
    }

    public Cell(float xPos, float zPos)
    {
        SetPos(xPos, zPos);
    }

    public void SetCoord(int aCoord, int bCoord)
    {
        a = aCoord;
        b = bCoord;
        z = (float) (-aCoord + (Maze.rows/2))*Maze.mapHeight/Maze.rows;
        x = (float) (bCoord - (Maze.cols/2))*Maze.mapWidth/Maze.cols;
    }

    public void SetPos(float xPos, float zPos)
    {
        x = xPos;
        z = zPos;
        a = (int) ((Maze.rows/2) - zPos*Maze.rows/Maze.mapHeight);
        b = (int) ((Maze.cols/2) + xPos*Maze.cols/Maze.mapWidth);
    }

    public static bool IsValid(int aCoord, int bCoord)
    {
        return 0 <= aCoord && aCoord < Maze.rows && 0 <= bCoord && bCoord < Maze.cols;
    }

    public static bool IsOccupied(int aCoord, int bCoord)
    {
        return !IsValid(aCoord, bCoord) || Maze.occupancy[aCoord, bCoord];
    }
}

public class ElementPositions
{
    public Cell startPos;
    public Cell finishPos;
    public List<Cell> collectiblesPositions;
    public List<Cell> enemiesPositions;
}
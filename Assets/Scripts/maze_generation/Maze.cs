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
        int numWalls = 6;
        int numCollectibles = 5;
        int numEnenmies = 3;
        InstantiateMap(numWalls, numCollectibles, numEnenmies);

        Debug.Log(occupancy);
    }

    private void InstantiateMap(int numWalls, int numCollectibles, int numEnenmies)
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

        // Get accessible cells
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
            length = UnityEngine.Random.Range(10, 50);
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
    public int distance;

    // If nothing passed, it randomly picks a valid value
    public Cell()
    {
        float aCoord, bCoord;
        do
        {
            aCoord = UnityEngine.Random.Range(0, Maze.cols);
            bCoord = UnityEngine.Random.Range(0, Maze.rows);
        }
        while(IsOccupied(aCoord, bCoord))     
        
        SetCoord(aCoord, bCoord);
    }

    public Cell(int aCoord, int bCoord)
    {
        SetCoord(aCoord, bCoord);
    }

    public Cell(int aCoord, int bCoord, int distToRoot)
    {
        SetCoord(aCoord, bCoord);
        distance = distToRoot;
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
        return IsValid(aCoord, bCoord) && Maze.occupancy[aCoord, bCoord] == 0;
    }
}
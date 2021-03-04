using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapDesigner
{
    // World dimensions
    static public int rows = 40;
    static public int cols = 40;
    static public float mapHeight = 70;
    static public float mapWidth = 70;

    // Private members
    public static bool[,] occupancy;

    // public MapDesigner(int r, int c, float height, float width)
    // {

    // }

    public MapDescription CreateMap(int numWalls, int numCollectibles, int numEnemies)
    {
        occupancy = new bool[rows, cols];
        
        // Mark outer walls as not free
        for (int i = 0; i < rows; i++) {
            occupancy[i, 0] = true;
            occupancy[i, cols-1] = true;
        }
        for (int i = 0; i < cols; i++) {
            occupancy[0, i] = true;
            occupancy[rows-1, i] = true;
        }

        // Build walls
        List<Wall> walls = new List<Wall>();
        for (int i = 0; i < numWalls; i++)
        {
            Wall w = new Wall();
            walls.Add(w);
            occupancy = w.markOccupancy();
        }
        // printOccupancy();

        // Sample 
        Cell root = new Cell(); // Random Valid cell
        root.rootDistance = 0;

        // Get accessible cells
        List<Cell> accessibleCells = ComputeAccessibility(root);

        // Group map info
        MapDescription map = new MapDescription();
        map.startPos = root;
        map.finishPos = accessibleCells[UnityEngine.Random.Range(2*accessibleCells.Count/3, accessibleCells.Count)];
        accessibleCells.Remove(map.finishPos);
        map.walls = walls;
        
        // Sample collectibles
        map.collectiblesPositions = new List<Cell>();
        for (int i = 0; i < numCollectibles; i++)
        {
            Cell collectiblePos = accessibleCells[UnityEngine.Random.Range(accessibleCells.Count/3, accessibleCells.Count)];
            map.collectiblesPositions.Add(collectiblePos);
            accessibleCells.Remove(collectiblePos);
        }
        
        // Sample enemies
        map.enemiesPositions = new List<Cell>();
        for (int i = 0; i < numEnemies; i++)
        {
            Cell enemyPos = accessibleCells[UnityEngine.Random.Range(accessibleCells.Count/3, accessibleCells.Count)];
            map.enemiesPositions.Add(enemyPos);
            accessibleCells.Remove(enemyPos);
        }
        
        return map;
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
        
        // printAccessibility(root, visibility);

        return accessibleCells;
    }

    private void printOccupancy()
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

    private void printAccessibility(Cell root, bool[,] visibility)
    {
        string mapstr = "";
        for (int i = 0; i < rows; i++)
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
            aCoord = UnityEngine.Random.Range(0, MapDesigner.cols);
            bCoord = UnityEngine.Random.Range(0, MapDesigner.rows);
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
        z = (float) (-aCoord + (MapDesigner.rows/2))*MapDesigner.mapHeight/MapDesigner.rows;
        x = (float) (bCoord - (MapDesigner.cols/2))*MapDesigner.mapWidth/MapDesigner.cols;
    }

    public void SetPos(float xPos, float zPos)
    {
        x = xPos;
        z = zPos;
        a = (int) ((MapDesigner.rows/2) - zPos*MapDesigner.rows/MapDesigner.mapHeight);
        b = (int) ((MapDesigner.cols/2) + xPos*MapDesigner.cols/MapDesigner.mapWidth);
    }

    public static bool IsValid(int aCoord, int bCoord)
    {
        return 0 <= aCoord && aCoord < MapDesigner.rows && 0 <= bCoord && bCoord < MapDesigner.cols;
    }

    public static bool IsOccupied(int aCoord, int bCoord)
    {
        return !IsValid(aCoord, bCoord) || MapDesigner.occupancy[aCoord, bCoord];
    }
}

public class Wall
{
    public Cell cell;
    public float length;
    public float theta;

    // If no arguments passed to the constructor, it sets everything random
    public Wall()
    {
        cell = new Cell();
        length = UnityEngine.Random.Range(10, 40);
        theta = UnityEngine.Random.Range(-90, 90);
    }

    public Wall(Cell loc, float len, float angle)
    {
        cell = loc;
        length = len;
        theta = angle;
    }

    // Returns a bool 2D multi-array considering current wall
    public bool[,] markOccupancy() {
        bool[,] occup = new bool[MapDesigner.rows, MapDesigner.cols];
        occup = MapDesigner.occupancy; //NOTE: I hope this performs a deep copy
        Cell origin = new Cell(cell.x + length*Mathf.Cos(Mathf.PI*theta/180.0f)/2, 
                            cell.z + length*Mathf.Sin(Mathf.PI*theta/180.0f)/2);
        Cell final = new Cell(cell.x - length*Mathf.Cos(Mathf.PI*theta/180.0f)/2, 
                            cell.z - length*Mathf.Sin(Mathf.PI*theta/180.0f)/2);
        
        // Iterate over rows
        int sign = (origin.a <= final.a) ? 1 : -1; 
        int i = origin.a - sign;
        int j;
        while (i != final.a + sign)
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
        j = origin.b - sign;
        while (j != final.b + sign)
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

public class MapDescription
{
    public Cell startPos;
    public Cell finishPos;
    public List<Cell> collectiblesPositions;
    public List<Cell> enemiesPositions;
    public List<Wall> walls;
}
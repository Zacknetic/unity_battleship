using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static int rows = 10;
    public static int cols = 10;
    public float tileSize = 10.0f;
    public GameObject cube;
    public GameObject[] ships;

    public List<GameObject> shipGrids;

//    int[,] gameGrid = new int[,]
//    {
//     {1, 0, 0, 0, 0, 0, 0, 0, 0, 2},
//     {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
//    {4, 0, 0, 0, 0, 0, 0, 0, 0, 3}
//    };


    public Material newMat;
    //create a 2d array of ints with a size of rows and cols
    private GameObject[,] gameGrid = new GameObject[rows, cols];

    // private List<GridCell> gridCells = new List<GridCell>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGameGrid();
        GenerateGridCell();
        GenerateShipGrids();
        // DisplayGrid(gameGrid, 10, 10);

    }

    public void updateGameGridFromShips()
    {
        // // Clear previous ship positions on the gameGrid
        // for (int row = 0; row < rows; row++)
        // {
        //     for (int col = 0; col < cols; col++)
        //     {
        //         if (gameGrid[row, col] == 2) gameGrid[row, col] = null;
        //     }
        // }

        for (int i = 0; i < this.shipGrids.Count; i++)
        {
            ShipGrid shipGrid = this.shipGrids[i].GetComponent<ShipGrid>();
            Vector2 shipWorldPosition = shipGrid.GetWorldPosition();
            int[,] shipGridMatrix = shipGrid.GetGridMatrix();
            UpdateGameGridFromShip(shipWorldPosition, shipGridMatrix);
        }
    }

    public int[,] getGameGrid()
    {
        return gameGrid;
    }

    void UpdateGameGridFromShip(Vector2 shipWorldPosition, int[,] shipGrid)
    {


        // Mark new ship positions on the gameGrid
        int shipRows = shipGrid.GetLength(0);
        int shipCols = shipGrid.GetLength(1);
        for (int r = 0; r < shipRows; r++)
        {
            for (int c = 0; c < shipCols; c++)
            {
                if (shipGrid[r, c] > 0)
                {
                    int gameGridRow = (int)(shipWorldPosition.y + r-2);
                    int gameGridCol = (int)(shipWorldPosition.x + c-2);
                    if (gameGridRow >= 0 && gameGridRow < rows && gameGridCol >= 0 && gameGridCol < cols)
                    {
                        gameGrid[gameGridRow, gameGridCol] = 2;
                    }
                }
            }
        }
        DisplayGrid(gameGrid, rows, cols);

    }

    // public static void DisplayGrid(int[,] grid, int rows, int cols)
    // {
    //     Debug.Log("\nDisplaying Grid");

    //     string rowString = "";

    //     for (int row = 0; row < rows; row++)
    //     {

    //         rowString += "\n";
    //         for (int col = 0; col < cols; col++)
    //         {
    //             rowString += grid[row, col] + " ";
    //         }
    //     }
    //     Debug.Log(rowString);

    // }


    void Fire(RaycastHit hit)
    {
        Debug.Log("hit");
        if (hit.transform.tag == "attackSelection")
        {
            hit.transform.gameObject.GetComponent<Renderer>().material = newMat;

            Vector2 pos = hit.transform.gameObject.GetComponent<OceanGridCell>().GetPosition();
            gameGrid[(int)pos.x, (int)pos.y] = 1;

        }
        else
        {
            //rotate ship
            hit.transform.parent.gameObject.GetComponent<ShipGrid>().RotateAll();
        }


    }

    void GenerateGameGrid()
    {

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                gameGrid[row, col] = null;
            }
        }
    }

    void GenerateGridCell()
    {


        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {

                Vector3 currVector = new Vector3(row * tileSize - (rows * tileSize / 2), col * -this.tileSize + (cols * tileSize / 2), 0.0f);

                // Vector3 currVector = new Vector3(row * tileSize, col * -this.tileSize, 0.0f);
                GameObject cell = Instantiate(this.cube, currVector, Quaternion.identity);
                // cell.GetComponent<OceanGridCell>().SetPosition(row, col);
                // this.gridCells.Add(cell);
            }
        }
    }

    void GenerateShipGrids()
    {
        //this is a list of vector 3s that will be used to place the ships. This is a good configuration for a 10x10 grid, 6 ships that are max 5 long
        Vector3[] shipPositions = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 30, 0), new Vector3(0, 60, 0), new Vector3(50, 0, 0), new Vector3(50, 30, 0), new Vector3(50, 60, 0) };
        for (int i = 0; i < this.ships.Length; i++)
        {
            GameObject ship = this.ships[i];
            Vector3 currVector = shipPositions[i];
            GameObject shipGrid = Instantiate(ship, currVector, ship.transform.rotation);
            shipGrid.GetComponent<ShipGrid>().SetWorldPosition((int)currVector.x, (int)currVector.y);

            this.shipGrids.Add(shipGrid);
        }


    }
}

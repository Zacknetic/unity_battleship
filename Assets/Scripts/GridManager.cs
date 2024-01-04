using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static int rows = 10;
    public static int cols = 10;
    public float tileSize = 10.0f;
    public GameObject cube;
    public GameObject[] ships;

    private HashSet<ShipGridCell> invalidShipGridCells = new HashSet<ShipGridCell>();

    public List<ShipGrid> shipGrids;

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
    private ShipGridCell[,] gameGrid = new ShipGridCell[rows, cols];

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
        // Clear previous ship positions on the gameGrid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                gameGrid[row, col] = null;
                Debug.Log("gameGrid: " + gameGrid[row, col]);
            }
        }

        for (int i = 0; i < this.shipGrids.Count; i++)
        {
            ShipGrid shipGrid = this.shipGrids[i];

            UpdateGameGridFromShip(shipGrid);
        }
        Debug.Log("invalidShipGridCells.Count: " + invalidShipGridCells.Count);
        //iterate through the HashSet and set the material to the invalid material
        foreach (ShipGridCell invalidShipGridCell in invalidShipGridCells)
        {
            invalidShipGridCell.GetComponentInParent<ShipGrid>().manageInvalidShipPosition();
            //clear the HashSet

        }
        invalidShipGridCells.Clear();
    }

    public ShipGridCell[,] getGameGrid()
    {
        return gameGrid;
    }

    void UpdateGameGridFromShip(ShipGrid shipGrid)
    {
        Debug.Log("UpdateGameGridFromShip");
        Vector2 shipWorldPosition = shipGrid.GetWorldPosition();
        ShipGridCell[,] shipGridMatrix = shipGrid.GetShipGridMatrix();
        // Mark new ship positions on the gameGrid
        int shipRows = shipGridMatrix.GetLength(0);
        int shipCols = shipGridMatrix.GetLength(1);
        for (int currRow = 0; currRow < shipRows; currRow++)
        {
            for (int currCol = 0; currCol < shipCols; currCol++)
            {
                if (shipGridMatrix[currRow, currCol] != null)
                {
                    int gameGridRow = (int)(shipWorldPosition.y + currRow - 2);
                    int gameGridCol = (int)(shipWorldPosition.x + currCol - 2);
                    // Debug.Log("ShipWorldPosition: x:" + shipWorldPosition.x + "y:" + shipWorldPosition.y + " currRow: " + currRow + " currCol: " + currCol + " gameGridRow: " + gameGridRow + " gameGridCol: " + gameGridCol);

                    if (!(gameGridRow >= 0 && gameGridRow < rows && gameGridCol >= 0 && gameGridCol < cols))
                    {
                        invalidShipGridCells.Add(shipGridMatrix[currRow, currCol]);
                        continue;
                    }
                    else if (gameGrid[gameGridRow, gameGridCol] != null)
                    {
                        invalidShipGridCells.Add(shipGridMatrix[currRow, currCol]);
                        invalidShipGridCells.Add(gameGrid[gameGridRow, gameGridCol]);

                    }
                    else if (gameGrid[gameGridRow, gameGridCol] == null)
                    {
                        shipGrid.resetRenderShipMaterial();

                    }
                    gameGrid[gameGridRow, gameGridCol] = shipGridMatrix[currRow, currCol];
                }



                //     shipGrid.GetComponent<ShipGrid>().setRenderShipMaterial();
                //     gameCell.GetComponentInParent<ShipGrid>().setRenderShipMaterial();
                //     return;

                // }
            }
        }

    }


    // DisplayGrid(gameGrid, rows, cols);



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
            GameObject shipGameObj = Instantiate(ship, currVector, ship.transform.rotation);
            ShipGrid shipGrid = shipGameObj.GetComponent<ShipGrid>();
            shipGrid.SetWorldPosition((int)currVector.x, (int)currVector.y);
            this.shipGrids.Add(shipGrid);
        }


    }
}

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static int rows = 10;
    public static int cols = 10;
    public float tileSize = 10.0f;
    public GameObject cube;
    public GameObject[] ships;

    int[,] gameGrid = new int[,]
    {
    {1, 0, 0, 0, 0, 0, 0, 0, 0, 2},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 3}
    };


    public Material newMat;
    //create a 2d array of ints with a size of rows and cols
    // private int[,] gameGrid = new int[rows, cols];

    // private List<GridCell> gridCells = new List<GridCell>();

    // Start is called before the first frame update
    void Start()
    {
        // GenerateGameGrid();
        GenerateGridCell();
        GenerateShipGrids();
        DisplayGrid(gameGrid, 10, 10);

    }

    void Update()
    {
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit))
        // {
        //     if (Input.GetButtonDown("Fire1"))
        //     {
        //         // if (hit.transform.tag == "attackSelection")
        //         Fire(hit);
        //         // else
        //         // {
        //         //     ShipGrid obj = hit.transform.gameObject.GetComponent<ShipGrid>();
        //         //     obj.RotateAll();
        //         // }

        //     }


        // }
    }

    public void UpdateGameGridFromShip(Vector2 shipWorldPosition, int[,] shipGrid)
    {
        // Clear previous ship positions on the gameGrid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (gameGrid[row, col] > 4) gameGrid[row, col] = 0;
            }
        }

        // Mark new ship positions on the gameGrid
        int shipRows = shipGrid.GetLength(0);
        int shipCols = shipGrid.GetLength(1);
        for (int r = 0; r < shipRows; r++)
        {
            for (int c = 0; c < shipCols; c++)
            {
                if (shipGrid[r, c] > 4)
                {
                    int gameGridRow = (int)(shipWorldPosition.y + r);
                    int gameGridCol = (int)(shipWorldPosition.x + c);
                    if (gameGridRow >= 0 && gameGridRow < rows && gameGridCol >= 0 && gameGridCol < cols)
                    {
                        gameGrid[gameGridRow, gameGridCol] = shipGrid[r, c];
                    }
                }
            }
        }
        DisplayGrid(gameGrid, rows, cols);

    }

    public static void DisplayGrid(int[,] grid, int rows, int cols)
    {
        Debug.Log("\nDisplaying Grid");

        string rowString = "";

        for (int row = 0; row < rows; row++)
        {

            rowString += "\n";
            for (int col = 0; col < cols; col++)
            {
                rowString += grid[row, col] + " ";
            }
        }
        Debug.Log(rowString);

    }


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
                gameGrid[row, col] = 0;
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
                cell.GetComponent<OceanGridCell>().SetPosition(row, col);
                // this.gridCells.Add(cell);
            }
        }
    }

    void GenerateShipGrids()
    {
        for (int i = 0; i < this.ships.Length; i++)
        {
            GameObject ship = this.ships[i];
            Vector3 currVector = new Vector3((i - 1) * 30, (i - 1) * 30, 0.0f);
            GameObject shipCells = Instantiate(ship, currVector, ship.transform.rotation);
            shipCells.GetComponent<ShipGrid>().SetWorldPosition((int)currVector.x, (int)currVector.y);
        }


    }
}

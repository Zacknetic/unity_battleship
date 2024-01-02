using UnityEngine;

public class ShipGrid : MonoBehaviour
{
    private int rows;
    private int cols;
    private Vector3 screenPoint;
    private Vector3 initialMousePosition;
    private Vector3 offset;
    private bool isDragging;
    private const float clickDurationThreshold = 0.2f; // Threshold for distinguishing a click from a drag
    private const float dragDistanceThreshold = .1f;  // Threshold for how much movement counts as a drag
    private float mouseDownTime;


    private Vector2 worldPosition = new Vector2(0, 0);

    public Material damagedMat;
    //create a 2d array of ints with a size of rows and cols
    private int[,] shipGrid;

    //     int[,] shipGrid = new int[,]
    // {
    //     {5, 0, 0, 6},
    //     {7, 0, 0, 8}
    // };

    private int partID = 5;

    // private List<GridCell> gridCells = new List<GridCell>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ShipGrid Start");

        InitializeGridSize();
        InitializeMatrixValue();
        DisplayGrid();
        Debug.Log("ShipGrid End");

    }

    void HandleMouseDown(GameObject child)
    {
        initialMousePosition = Input.mousePosition;
        mouseDownTime = Time.time;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        isDragging = false;
    }

    void HandleMouseDrag(GameObject child)
    {
        // if (!isDragging)
        // {
        float distance = Vector3.Distance(Input.mousePosition, initialMousePosition);
        if (distance > dragDistanceThreshold)
        {
            // Start dragging
            isDragging = true;
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            transform.position = cursorPosition;
        }
        // }
    }

    void HandleMouseUp(GameObject child)
    {
        if (!isDragging && Time.time - mouseDownTime <= clickDurationThreshold)
        {
            // Short click detected
            RotateAll();
        }
        else if (isDragging)
        {
            // End dragging
            SnapToGrid();
        }

        // Reset values
        isDragging = false;
        mouseDownTime = 0;
    }
    void SnapToGrid()
    {
        float tileSize = 10.0f; // Assuming tileSize is known

        int x = Mathf.RoundToInt((transform.position.x + (GridManager.rows * tileSize / 2)) / tileSize);
        int y = Mathf.RoundToInt((-transform.position.y + (GridManager.cols * tileSize / 2)) / tileSize);

        Vector3 snappedPosition = new Vector3(x * tileSize - (GridManager.rows * tileSize / 2), y * -tileSize + (GridManager.cols * tileSize / 2), 0);
        transform.position = snappedPosition;

        UpdateWorldPosition();
        MarkShipOnGameGrid();
    }
    void InitializeGridSize()
    {
        int row = 0;
        int col = 0;
        foreach (Transform child in this.transform)
        {
            if (child.localPosition.x + 1 > col)
            {
                col = (int)child.localPosition.x + 1;
            }
            if (child.localPosition.y + 1 > row)
            {
                row = (int)child.localPosition.y + 1;
            }
        }
        rows = row;
        cols = col;
        Debug.Log("rows: " + rows);
        Debug.Log("cols: " + cols);
        this.shipGrid = new int[row, col];

    }

    void InitializeMatrixValue()
    {

        foreach (Transform child in this.transform)
        {

            shipGrid[(int)child.localPosition.y, (int)child.localPosition.x] = partID;
            partID++;

            ShipGridCell cell = child.GetComponent<ShipGridCell>();

            if (cell != null)
            {
                cell.OnMouseDownEvent += HandleMouseDown;
                cell.OnMouseUpEvent += HandleMouseUp;
                cell.OnMouseDragEvent += HandleMouseDrag;
            }
        }
    }


    void DisplayGrid()
    {
        GridManager.DisplayGrid(shipGrid, rows, cols);
    }

    public void RotateAll()
    {
        Debug.Log("Rotating All");
        RotateParent();
        RotateMatrix();
        DisplayGrid();

    }
    //to rotate the matrix 90 degrees clockwise use this. Keep in mind, we need to account for a non-square matrix.
void RotateMatrix()
{
    int originalRows = shipGrid.GetLength(0);
    int originalCols = shipGrid.GetLength(1);
    int[,] rotated = new int[originalCols, originalRows];

    for (int i = 0; i < originalRows; i++)
    {
        for (int j = 0; j < originalCols; j++)
        {
            rotated[j, originalRows - 1 - i] = shipGrid[i, j];
        }
    }
    this.shipGrid = rotated;
    rows = originalCols;
    cols = originalRows;

    // Adjust world position after rotation
    if (originalRows != originalCols)
    {
        float deltaRows = (originalCols - originalRows) / 2.0f;
        float deltaCols = (originalRows - originalCols) / 2.0f;
        Debug.Log("updating world position");
        Debug.Log("deltaRows: " + deltaRows);
        Debug.Log("deltaCols: " + deltaCols);
        worldPosition.x += deltaCols;
        worldPosition.y += deltaRows;
    }

    MarkShipOnGameGrid();
}


    void RotateParent()
    {
        Vector3 center = CalculateCenterOfChildren();

        // Temporarily adjust the ship's position
        Vector3 originalPosition = transform.position;
        transform.position += transform.rotation * center; // Adjust position by center

        // Rotate the ship
        transform.Rotate(new Vector3(0, 0, 1), 90);

        // Reset the position
        transform.position = originalPosition - transform.rotation * center;

        // Update worldPosition and mark the ship on the game grid
        UpdateWorldPosition();
        MarkShipOnGameGrid();
    }

    void MarkShipOnGameGrid()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.UpdateGameGridFromShip(worldPosition, shipGrid);
        }
    }

    void UpdateWorldPosition()
    {
        float tileSize = 10.0f; // Assuming tileSize is known
        worldPosition.x = Mathf.RoundToInt((transform.position.x + (GridManager.rows * tileSize / 2)) / tileSize);
        worldPosition.y = Mathf.RoundToInt((-transform.position.y + (GridManager.cols * tileSize / 2)) / tileSize);
    }


    Vector3 CalculateCenterOfChildren()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Transform child in transform)
        {
            sum += child.localPosition;
            count++;
        }

        Vector3 center = count > 0 ? sum / count : Vector3.zero;
        Debug.Log("center: " + center);
        return center;
    }



    public void SetWorldPosition(int x, int y)
    {
        this.worldPosition.x = x;
        this.worldPosition.y = y;
    }
}
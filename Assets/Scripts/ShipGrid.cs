using System;
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

    public Material incorrectMat;
    public Material selectedMat;
    public Material normalMat;
    public Material originalMat;
    //create a 2d array of ints with a size of rows and cols
    private ShipGridCell[,] shipGrid;

    //     int[,] shipGrid = new int[,]
    // {
    //     {5, 0, 0, 6},
    //     {7, 0, 0, 8}
    // };

    // private int partID = 5;
    private bool isAnimating;
    private bool hasValidPosition;

    // private List<GridCell> gridCells = new List<GridCell>();

    // Start is called before the first frame update
    void Start()
    {

        storeRenderShipMaterial();
        InitializeGridSize();
        InitializeMatrixValue();
        UpdateWorldPosition();
        MarkShipOnGameGrid();

    }

    void HandleMouseDown(GameObject child)
    {
        hasValidPosition = true;
        initialMousePosition = Input.mousePosition;
        mouseDownTime = Time.time;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isDragging = false;
    }

    void HandleMouseDrag(GameObject child)
    {
        float distance = Vector3.Distance(Input.mousePosition, initialMousePosition);
        if (distance > dragDistanceThreshold)
        {
            // Start dragging
            isDragging = true;
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            cursorPosition.z = transform.position.z; // Set the z-axis to the original z position
            transform.position = cursorPosition;
        }
    }

    public void storeRenderShipMaterial()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "renderedShip")
            {
                //get the child's child and set the material to the incorrect material
                foreach (Transform childShip in child.transform)
                {
                    originalMat = childShip.GetComponent<Renderer>().material;
                }
            }
        }
    }

    public void manageInvalidShipPosition()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "renderedShip")
            {
                //get the child's child and set the material to the incorrect material
                foreach (Transform childShip in child.transform)
                {
                    childShip.GetComponent<Renderer>().material = incorrectMat;
                    hasValidPosition = false;
                }
            }
        }
    }

    public void resetRenderShipMaterial()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "renderedShip")
            {
                foreach (Transform childShip in child.transform)
                {
                    childShip.GetComponent<Renderer>().material = originalMat;
                }
            }
        }
    }

    //test if the ship is overlapping with another ship or if it is off the grid

    void Update()
    {
        if (this.isAnimating)
        {
            HandleMouseUp(gameObject);
        }
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
            if (child.localPosition.x + 3 > col)
            {
                col = (int)child.localPosition.x + 3;
            }
            if (child.localPosition.y + 3 > row)
            {
                row = (int)child.localPosition.y + 3;
            }
        }
        rows = row;
        cols = col;
        Debug.Log("rows: " + rows);
        Debug.Log("cols: " + cols);
        this.shipGrid = new ShipGridCell[row, col];

    }

    void InitializeMatrixValue()
    {

        foreach (Transform child in this.transform)
        {

            //if the childShip has a tag of "invisBox" then we should mark it as 0
            ShipGridCell cell = child.GetComponent<ShipGridCell>();
            if (child.tag == "shipSelector")
            {
                shipGrid[(int)child.localPosition.y + 2, (int)child.localPosition.x + 2] = cell;
            }
            else if (child.tag == "shipSelectorIgnore")
            {
                shipGrid[(int)child.localPosition.y + 2, (int)child.localPosition.x + 2] = null;
            }



            if (cell != null)
            {
                cell.OnMouseDownEvent += HandleMouseDown;
                cell.OnMouseUpEvent += HandleMouseUp;
                cell.OnMouseDragEvent += HandleMouseDrag;
            }
        }
    }

    public void RotateAll()
    {
        Debug.Log("Rotating All");
        RotateParent();
        RotateMatrix();
        // DisplayGrid();

    }
    //to rotate the matrix 90 degrees clockwise use this. Keep in mind, we need to account for a non-square matrix.
    void RotateMatrix()
    {
        int originalRows = shipGrid.GetLength(0);
        int originalCols = shipGrid.GetLength(1);
        ShipGridCell[,] rotated = new ShipGridCell[originalCols, originalRows];

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

        MarkShipOnGameGrid();
    }


    void RotateParent()
    {
        //this will rotate the parent object 90 degrees clockwise
        //to rotate it around the center of the children, we need to calculate the center of the children
        transform.Rotate(Vector3.forward, 90);


    }

    void MarkShipOnGameGrid()
    {
        if (hasValidPosition)
        {
            resetRenderShipMaterial();
        }

        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.updateGameGridFromShips();
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

    public bool GetHasValidPosition()
    {
        return this.hasValidPosition;
    }

    internal Vector2 GetWorldPosition()
    {
        return this.worldPosition;
    }

    internal ShipGridCell[,] GetShipGridMatrix()
    {
        return this.shipGrid;
    }
}
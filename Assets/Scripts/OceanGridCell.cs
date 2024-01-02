using UnityEngine;

public class OceanGridCell : MonoBehaviour
{

    private int column = 0;
    private int row = 0;

    // public GridCell(int col, int row)
    // {
    //     this.column = col;
    //     this.row = row;
    // }

    public void SetPosition(int row, int col)
    {
        this.column = col;
        this.row = row;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(this.column, this.row);
    }

}

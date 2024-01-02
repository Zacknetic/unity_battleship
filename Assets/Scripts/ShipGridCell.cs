using UnityEngine;

public class ShipGridCell : MonoBehaviour
{
    public delegate void MouseEventDelegate(GameObject child);
    public event MouseEventDelegate OnMouseDownEvent;
    public event MouseEventDelegate OnMouseUpEvent;
    public event MouseEventDelegate OnMouseDragEvent;

    public bool isAttackable = false;

    void OnMouseDown()
    {
        // Debug.Log("OnMouseDown");
        OnMouseDownEvent?.Invoke(gameObject);
    }

    void OnMouseUp()
    {
        // Debug.Log("OnMouseUp");
        OnMouseUpEvent?.Invoke(gameObject);
    }

    void OnMouseDrag()
    {
        // Debug.Log("OnMouseDrag");
        OnMouseDragEvent?.Invoke(gameObject);
    }
}

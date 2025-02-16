using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2Int gridPosition;
    private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        
        gridPosition = new Vector2Int(gridManager.width / 2, gridManager.height / 2);
        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
}

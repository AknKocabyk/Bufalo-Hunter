using UnityEngine;

public class Node
{
    public Vector2Int position;  // Hücrenin pozisyonu
    public bool isWalkable;      // Hücre geçilebilir mi?
    public Node parent;          // A* için ebeveyn node
    public int gCost;            // Başlangıçtan buraya olan maliyet
    public int hCost;            // Tahmini hedef maliyet
    public int fCost => gCost + hCost;  // Toplam maliyet (G + H)

    public Node(Vector2Int pos, bool walkable)
    {
        position = pos;
        isWalkable = walkable;
        parent = null;
        gCost = 0;
        hCost = 0;
    }
}

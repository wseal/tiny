
using UnityEngine;

public class Pathfinding
{
  private int m_Width;
  private int m_Height; 
  private Vector3Int m_GridOffset;
  private Node[,] m_Grid;
  private TilemapManager m_TilemapManager;
  public Node[,] Grid => m_Grid;

  public Pathfinding(TilemapManager tilemapManager)
  {
    m_TilemapManager = tilemapManager;

    m_TilemapManager.PathfindingTilemap.CompressBounds();
    var bounds = m_TilemapManager.PathfindingTilemap.cellBounds;
    m_Width = bounds.size.x;
    m_Height = bounds.size.y;
    m_GridOffset = bounds.min;
    m_Grid = new Node[m_Width, m_Height];
    InitializeGrid();
  }
  
  void InitializeGrid()
  {
    Vector3 cellSize = m_TilemapManager.PathfindingTilemap.cellSize;
    // var max = m_TilemapManager.PathfindingTilemap.cellBounds.max;
    // Debug.Log($"bounds --> ({m_TilemapManager.PathfindingTilemap.cellBounds.size.x}, {m_TilemapManager.PathfindingTilemap.cellBounds.size.y}). ");
    // Debug.Log($"half --> ({halfSize.x}, {halfSize.y}).");
    // Debug.Log($"min --> ({offset.x}, {offset.y}).");
    // Debug.Log($"max --> ({max.x}, {max.y}).");
    for (int x = 0; x < m_Width; x++)
    {
      for (int y = 0; y < m_Height; y++)
      {
        Vector3Int leftBottomPos = new Vector3Int(x + m_GridOffset.x, y + m_GridOffset.y);
        // Vector3 centerPos = leftBottomPos + halfSize;
        var isWalkable = m_TilemapManager.CanWalkAtTile(leftBottomPos);
        var node = new Node(leftBottomPos, cellSize, isWalkable);
        m_Grid[x, y] = node;

        Debug.Log($"Node at [{x}, {y}] --> ({node.x}, {node.y}).");
      }
    }
  }
}

using System.Collections.Generic;
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
        // Debug.Log($"Node at [{x}, {y}] --> ({node.x}, {node.y}).");
      }
    }
  }

  public void FindPath(Vector3 startPosition, Vector3 destinationPosition)
  {
    var startNode = FindNode(startPosition);
    var endNode = FindNode(destinationPosition);

    if (startNode == null || endNode == null)
    {
      Debug.Log("Can't find the path!");
      return;
    }

    List<Node> openList = new();
    HashSet<Node> closeList = new();

    openList.Add(startNode);

    while (openList.Count > 0)
    {
      var currNode = GetLowestFCostNode(openList);
      if (currNode == endNode)
      {
        Debug.Log("Path Found!!!");
        return;
      }

      openList.Remove(currNode);
      closeList.Add(currNode);
    }
  }

  Node GetLowestFCostNode(List<Node> openList)
  {
    var n = openList[0];
    foreach (Node node in openList)
    {
      if (node.fCost < n.fCost || node.fCost == n.fCost && node.hCost < n.hCost)
      {
        n = node;
      }
    }

    return n;
  }


  Node FindNode(Vector3 position)
  {
    int x = Mathf.FloorToInt(position.x) - m_GridOffset.x;
    int y = Mathf.FloorToInt(position.y) - m_GridOffset.y;

    if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
    {
      return m_Grid[x, y];
    }

    return null;
  }
}
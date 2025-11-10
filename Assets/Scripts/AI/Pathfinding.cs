
using System;
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

  public Node FindNode(Vector3 position)
  {
    int x = Mathf.FloorToInt(position.x) - m_GridOffset.x;
    int y = Mathf.FloorToInt(position.y) - m_GridOffset.y;

    if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
    {
      return m_Grid[x, y];
    }

    return null;
  }

  public List<Node> FindPath(Vector3 startPosition, Vector3 destinationPosition)
  {
    var startNode = FindNode(startPosition);
    var endNode = FindNode(destinationPosition);

    if (startNode == null || endNode == null)
    {
      Debug.Log("Can't find the path!");
      return new List<Node>();
    }

    List<Node> openList = new();
    HashSet<Node> closeList = new();

    openList.Add(startNode);

    while (openList.Count > 0)
    {
      var currNode = GetLowestFCostNode(openList);
      if (currNode == endNode)
      {
        var path = RetracePath(startNode, endNode);
        Debug.Log("Path Found--->" + string.Join(", ", path));
        return path;
      }

      openList.Remove(currNode);
      closeList.Add(currNode);

      var neighbors = GetNeighbors(currNode);
      // Debug.Log("neighbor " + String.Join(", ", neighbors));
      foreach (Node neighbor in neighbors)
      {
        if (!neighbor.walkable || closeList.Contains(neighbor)) continue;

        float tentativeG = currNode.gCost + GetDistance(currNode, neighbor);
        if (tentativeG < neighbor.gCost || !openList.Contains(neighbor))
        {
          neighbor.gCost = tentativeG;
          neighbor.hCost = GetDistance(neighbor, endNode);
          neighbor.fCost = neighbor.gCost + neighbor.hCost;
          neighbor.parent = currNode;

          if (!openList.Contains(neighbor))
          {
            openList.Add(neighbor);
          }
        }
      }
    }

    Debug.Log("No Path Found");
    return new List<Node>();
  }

  List<Node> RetracePath(Node startNode, Node endNode)
  {
    List<Node> path = new();
    Node p = endNode;
    while (p != startNode)
    {
      path.Add(p);
      p = p.parent;
    }

    path.Add(startNode);
    path.Reverse();

    return path;
  }

  float GetDistance(Node nodeA, Node nodeb)
  {
    int disX = Mathf.Abs(nodeA.x - nodeb.x);
    int disY = Mathf.Abs(nodeA.y - nodeb.y);

    if (disX > disY)
    {
      return 14 * disY + 10 * (disX - disY);
    }

    return 14 * disX + 10 * (disY - disX);
  }

  List<Node> GetNeighbors(Node node)
  {
    List<Node> neighbors = new();
    for (int x = -1; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++)
      {
        if (x == 0 && y == 0) continue;

        int checkX = node.x + x - m_GridOffset.x;
        int checkY = node.y + y - m_GridOffset.y;
        if (checkX >= 0 && checkX < m_Width && checkY >= 0 && checkY < m_Height)
        {
          var neighbor = m_Grid[checkX, checkY];
          neighbors.Add(neighbor);
        }

      }
    }

    return neighbors;
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
}
using UnityEngine;

public class Node
{
  public int x;
  public int y;
  public float centerX;
  public float centerY;
  public bool walkable;

  public Node(Vector3Int leftBottom, Vector3 cellSize, bool walkable)
  {
    x = leftBottom.x;
    y = leftBottom.y;

    Vector3 halfSize = cellSize / 2;
    var center = leftBottom + halfSize;
    centerX = center.x;
    centerY = center.y;

    this.walkable = walkable;
  }
  public override string ToString()
  {
    return $"({x}, {y})";
  }
}
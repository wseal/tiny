

using System.IO;

public class Pathfinding
{
  private Node[,] m_Grid;
  public Node[,] Grid => m_Grid;

  public Pathfinding(int width, int height)
  {
    m_Grid = new Node[width, height];
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        m_Grid[x, y] = new Node { x = x, y = y, walkable = true };
      }
    }
  }
}
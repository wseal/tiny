
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : SingletonManager<PathfindingManager>
{
  [SerializeField] private Tilemap m_WalkableTilemap;
  private Pathfinding m_Pathfinding;
  public Pathfinding Pathfinding => m_Pathfinding;

  void Start()
  {
    var bounds = m_WalkableTilemap.cellBounds;
    m_Pathfinding = new Pathfinding(bounds.size.x, bounds.size.y);
  }
}
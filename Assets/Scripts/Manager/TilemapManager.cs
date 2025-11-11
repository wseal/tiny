
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
  [Header("Tilemaps")]
  [SerializeField] private Tilemap m_WalkableTilemap;
  [SerializeField] private Tilemap m_OverlayTilemap;
  [SerializeField] private Tilemap[] m_UnreachableTilemaps;

  public Tilemap PathfindingTilemap => m_WalkableTilemap;
  private Pathfinding m_Pathfinding;
  public Pathfinding Pathfinding => m_Pathfinding;

  void Start()
  {
    m_Pathfinding = new Pathfinding(this);
  }

  public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
  {
    return m_Pathfinding.FindPath(startPos, endPos);
  }

  public Node FindNode(Vector3 position)
  {
    return m_Pathfinding.FindNode(position);
  }

  public void UpdateNodesInArea(Vector3Int startPos, int widht, int height)
  {
    m_Pathfinding.UpdateNodesInArea(startPos, widht, height);
  }

  public bool CanWalkAtTile(Vector3Int position)
  {
    return m_WalkableTilemap.HasTile(position) &&
     !IsInUnreachableTilemap(position) &&
     !IsBlockedByBuilding(position);
  }

  public bool CanPlaceTile(Vector3Int position)
  {
    return m_WalkableTilemap.HasTile(position) &&
     !IsInUnreachableTilemap(position) &&
     !IsBlockedByGameobject(position);
  }

  public bool IsInUnreachableTilemap(Vector3Int position)
  {
    foreach (var tilemap in m_UnreachableTilemaps)
    {
      if (tilemap.HasTile(position)) return true;
    }

    return false;
  }

  public bool IsBlockedByBuilding(Vector3Int tilePosition)
  {
    Vector3 worldPosition = m_WalkableTilemap.CellToWorld(tilePosition) + m_WalkableTilemap.cellSize / 2;
    int buildingLayerMask = 1 << LayerMask.NameToLayer("Building");

    Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition, buildingLayerMask);
    foreach (var collider in colliders)
    {
      // if (collider.gameObject.tag == "Building") return true;
      if (collider.CompareTag("Building")) return true;
    }

    return false;
  }

  public bool IsBlockedByGameobject(Vector3Int tilePosition)
  {
    Vector3 tileSize = m_WalkableTilemap.cellSize;
    int unitMask = 1 << LayerMask.NameToLayer("Unit");
    Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.95f, 0f, unitMask);

    return colliders.Length > 0;
  }

  public void SetTileOverlay(Vector3Int position, Tile tile)
  {
    m_OverlayTilemap.SetTile(position, tile);
  }
}
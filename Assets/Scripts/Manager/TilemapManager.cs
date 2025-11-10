
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

  public List<Node> FindPath(Vector3 startPos, Vector3 endPos)
  {
    return m_Pathfinding.FindPath(startPos, endPos);
  }

  public Node FindNode(Vector3 position)
  {
    return m_Pathfinding.FindNode(position);
  }

  public bool CanWalkAtTile(Vector3Int position)
  {
    return m_WalkableTilemap.HasTile(position) &&
     !IsInUnreachableTilemap(position);
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

  public bool IsBlockedByGameobject(Vector3Int tilePosition)
  {
    Vector3 tileSize = m_WalkableTilemap.cellSize;
    Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.95f, 0f);

    foreach (var collider in colliders)
    {
      var layer = collider.gameObject.layer;
      if (layer == LayerMask.NameToLayer("Player"))
      {
        return true;
      }
      // if (collider.gameObject.CompareTag("Building") || collider.gameObject.CompareTag("Unit"))
      // {
      //   return true;
      // }
    }
    return false;
  }

  public void SetTileOverlay(Vector3Int position, Tile tile)
  {
    m_OverlayTilemap.SetTile(position, tile);
  }
}
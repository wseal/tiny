


using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
  private GameObject m_PlacementOutline;
  private BuildActionSO m_BuildAction;

  private Vector3Int[] m_HighlightPositions;
  private Tilemap m_WalkableTilemap;
  private Tilemap m_OverlayTilemap;
  private Sprite m_PlaceholderTileSprite;
  public PlacementProcess(BuildActionSO buildAction, Tilemap walkTilemap, Tilemap overlayTilemap)
  {
    m_PlaceholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");
    m_BuildAction = buildAction;
    m_WalkableTilemap = walkTilemap;
    m_OverlayTilemap = overlayTilemap;
  }

  public void Update()
  {
    if (m_PlacementOutline != null)
    {
      HighlightTiles(m_PlacementOutline.transform.position);
    }

    if (TouchUtils.TryGetHoldPosition(out var worldPosition))
    {
        m_PlacementOutline.transform.position = SnapToGrid(worldPosition);
    }
    // Vector3 worldPosition = TouchUtils.InputHoldWorldPosition;
    // if (worldPosition != Vector3.zero)
    // {
    //   m_PlacementOutline.transform.position = new Vector3(
    //     worldPosition.x,
    //     worldPosition.y, 0f
    //     );
    // }
  }

  public void ShowPlacementOutline()
  {
    m_PlacementOutline = new GameObject("PlacementOutline");
    var spriteRenderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = m_BuildAction.PlacementSprite;
    spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
    spriteRenderer.sortingOrder = 1000; // Ensure it's rendered on top
  }

  public Vector3 SnapToGrid(Vector3 worldPostion, float gridSize = 1f)
  {
    float x = Mathf.Round(worldPostion.x / gridSize) * gridSize;
    float y = Mathf.Round(worldPostion.y / gridSize) * gridSize;
    return new Vector3(x, y, 0);
  }

  void HighlightTiles(Vector3 outlinePosition)
  {
    Vector3Int buildSize = m_BuildAction.BuildingSize;
    Vector3 leftBottomPosition = outlinePosition + m_BuildAction.OriginOffset;
    ClearHighlightTiles();

    m_HighlightPositions = new Vector3Int[buildSize.x * buildSize.y];
    for (int x = 0; x < buildSize.x; x++)
    {
      for (int y = 0; y < buildSize.y; y++)
      {
        m_HighlightPositions[x * buildSize.y + y] = new Vector3Int((int)leftBottomPosition.x + x, (int)leftBottomPosition.y + y, 0);
      }
    }

    foreach (var pos in m_HighlightPositions)
    {
      var tile = ScriptableObject.CreateInstance<Tile>();
      tile.sprite = m_PlaceholderTileSprite;
      tile.color = new Color(0f, 0.8f, 1f, 0.4f); // Semi-transparent green
      m_OverlayTilemap.SetTile(pos, tile);
      // m_OverlayTilemap.SetTileFlags(pos, TileFlags.None);
      // m_OverlayTilemap.SetColor(pos, Color.green);
      // if (m_WalkableTilemap.HasTile(pos))
      // {
      //   m_WalkableTilemap.SetTileFlags(pos, TileFlags.None);
      //   m_WalkableTilemap.SetColor(pos, Color.green);
      // }
    }
  }

  void ClearHighlightTiles()
  {
    if (m_HighlightPositions == null) return;

    foreach (var pos in m_HighlightPositions)
    {
      m_OverlayTilemap.SetTile(pos, null);
      // if (m_WalkableTilemap.HasTile(pos))
      // {
      //   m_WalkableTilemap.SetTileFlags(pos, TileFlags.None);
      //   m_WalkableTilemap.SetColor(pos, Color.white);
      // }
    }
  }
}
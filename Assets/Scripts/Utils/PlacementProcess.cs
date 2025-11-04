


using UnityEngine;

public class PlacementProcess
{
  private GameObject m_PlacementOutline;
  private BuildActionSO m_BuildAction;
  public PlacementProcess(BuildActionSO buildAction)
  {
    m_BuildAction = buildAction;
  }

  public void Update()
  {
    if (TouchUtils.TryGetHoldPosition(out var worldPosition))
    {
        m_PlacementOutline.transform.position = new Vector3(
          worldPosition.x,
          worldPosition.y, 0f
          );
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
}
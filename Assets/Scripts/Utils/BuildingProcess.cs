
using UnityEngine;

public class BuildingProcess
{
  private BuildActionSO m_BuildAction;

  public BuildingProcess(BuildActionSO buildAction, Vector3 placementPosition)
  {
    m_BuildAction = buildAction;

    var structGo = new GameObject(m_BuildAction.ActionName);
    var renderer = structGo.AddComponent<SpriteRenderer>();
    renderer.sprite = m_BuildAction.FundationSprite;
    renderer.sortingOrder = 25;
    structGo.transform.position = placementPosition;
  }
}

using UnityEngine;

public class BuildingProcess
{
  private BuildActionSO m_BuildAction;

  public BuildingProcess(BuildActionSO buildAction, Vector3 placementPosition)
  {
    m_BuildAction = buildAction;

    var structure = Object.Instantiate(
      m_BuildAction.StructurePrefab,
      placementPosition,
      Quaternion.identity
    );
    
    structure.SpriteRenderer.sprite = m_BuildAction.FundationSprite;
    // structure.transform.position = placementPosition;
    // var structGo = new GameObject(m_BuildAction.ActionName);
    // var renderer = structGo.AddComponent<SpriteRenderer>();
    // renderer.sprite = m_BuildAction.FundationSprite;
    // renderer.sortingOrder = 25;
    // structGo.transform.position = placementPosition;
  }
}
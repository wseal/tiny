

using UnityEngine;

public class StructureUnit : Unit
{
  private BuildingProcess m_BuildingProcess;

  public override bool IsBuilding => true;
  public bool IsUnderConstruction => m_BuildingProcess != null;

  void Update()
  {
    if (IsUnderConstruction)
    {
      m_BuildingProcess.Update();
    }
  }

  void OnDestroy()
  {
    UpdateWalkablity();
  }

  public void OnConstructionComplete()
  {
    m_BuildingProcess = null;
    UpdateWalkablity();
  }

  public void RegisterProcess(BuildingProcess process)
  {
    m_BuildingProcess = process;
  }

  public void AssignWorkerToBuildProcess(WorkerUnit worker)
  {
    m_BuildingProcess?.AddWorker(worker);
  }
  public void UnassignWorkerFromBuildProcess()
  {
    m_BuildingProcess?.RemoveWorker();
  }

  void UpdateWalkablity()
  {
    int buildingWidthInTiles = 6;
    int buildingHeightInTiles = 6;

    float halfWidth = buildingWidthInTiles / 2;
    float halfHeight = buildingHeightInTiles / 2;

    Vector3Int startPos = new Vector3Int(
      Mathf.FloorToInt(transform.position.x - halfWidth),
      Mathf.FloorToInt(transform.position.y - halfHeight),
      0);

    TilemapManager.Get().UpdateNodesInArea(startPos, buildingWidthInTiles, buildingHeightInTiles);
  }
}
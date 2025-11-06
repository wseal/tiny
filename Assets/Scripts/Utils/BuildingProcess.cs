
using NUnit.Framework;
using UnityEngine;

public class BuildingProcess
{
  private BuildActionSO m_BuildAction;
  private WorkerUnit m_Worker;

  public bool HasActiveWorker => m_Worker != null;

  public BuildingProcess(BuildActionSO buildAction, Vector3 placementPosition, WorkerUnit worker)
  {
    m_BuildAction = buildAction;

    var structure = Object.Instantiate(
      m_BuildAction.StructurePrefab,
      placementPosition,
      Quaternion.identity
    );

    structure.SpriteRenderer.sprite = m_BuildAction.FundationSprite;
    structure.RegisterProcess(this);
    // structure.transform.position = placementPosition;
    // var structGo = new GameObject(m_BuildAction.ActionName);
    // var renderer = structGo.AddComponent<SpriteRenderer>();
    // renderer.sprite = m_BuildAction.FundationSprite;
    // renderer.sortingOrder = 25;
    // structGo.transform.position = placementPosition;

    worker.SendToBuild(structure);
  }

  public void Update()
  {
    // Update building progress logic here
    if (HasActiveWorker)
    {

    }
  }

  public void AddWorker(WorkerUnit worker)
  {
    if (HasActiveWorker) return;
    
    Debug.Log("Worker add from build process");
    m_Worker = worker;
  }
  
  public void RemoveWorker()
  {
    Debug.Log("Worker removed from build process");
    m_Worker = null;
  } 
}
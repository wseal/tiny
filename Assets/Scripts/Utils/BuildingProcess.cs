
using NUnit.Framework;
using UnityEngine;

public class BuildingProcess
{
  private BuildActionSO m_BuildAction;
  private WorkerUnit m_Worker;
  private StructureUnit m_Structure;
  private ParticleSystem m_ConstructionEffect;

  private float m_ProgressTimer = 0.0f;
  private bool m_IsComplete = false;

  private bool InProgress => HasActiveWorker && m_Worker.CurrentState == UnitState.Building;
  public bool HasActiveWorker => m_Worker != null;

  public BuildingProcess(BuildActionSO buildAction, Vector3 placementPosition, WorkerUnit worker, ParticleSystem constructionEffectPrefab)
  {
    m_BuildAction = buildAction;
    var effactOffset = new Vector3(0, -1.0f, 0);
    m_ConstructionEffect = Object.Instantiate(
      constructionEffectPrefab,
      placementPosition + effactOffset,
      Quaternion.identity
    );
    m_Structure = Object.Instantiate(
      m_BuildAction.StructurePrefab,
      placementPosition,
      Quaternion.identity
    );

    m_Structure.SpriteRenderer.sprite = m_BuildAction.FundationSprite;
    m_Structure.RegisterProcess(this);
    // m_Structure.transform.position = placementPosition;
    // var structGo = new GameObject(m_BuildAction.ActionName);
    // var renderer = structGo.AddComponent<SpriteRenderer>();
    // renderer.sprite = m_BuildAction.FundationSprite;
    // renderer.sortingOrder = 25;
    // structGo.transform.position = placementPosition;
    worker.SendToBuild(m_Structure);
  }

  public void Update()
  {
    if (m_IsComplete) return;

    if (InProgress)
    {
      m_ProgressTimer += Time.deltaTime;
      if (!m_ConstructionEffect.isPlaying)
      {
        m_ConstructionEffect.Play();
      } 

      if (m_ProgressTimer >= m_BuildAction.ConstructionTime)
      {
        // CompleteConstruction();
        m_IsComplete = true;
        m_Structure.SpriteRenderer.sprite = m_BuildAction.CompletionSprite;
        // m_Structure.UnassignWorkerFromBuildProcess();
        m_Worker.OnBuildingCompleted();
        m_Structure.OnConstructionComplete();
      }
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
    m_ConstructionEffect.Stop();
  } 
}
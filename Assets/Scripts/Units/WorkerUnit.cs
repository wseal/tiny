
using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
  // Worker-specific implementation

  protected override void UpdateBehaviour()
  {
    if (CurrentTask == UnitTask.Build && HasTarget)
    {
      CheckForConstruction();
    }
  }

  protected override void OnSetDestination(Vector3 destination)
  {
    // base.OnSetDestination(destination);
    ResetState();
  }

  public void OnBuildingCompleted()
  {
    ResetState();
  } 
  public void SendToBuild(StructureUnit structure)
  {
    MoveTo(structure.transform.position);
    SetTask(UnitTask.Build);
    SetTarget(structure);
  }

  void CheckForConstruction()
  {
    var distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
    if (distanceToTarget <= m_DetectionRadius && CurrentState == UnitState.Idle)
    {
      StartBuilding(Target as StructureUnit);
    }
  }

  void StartBuilding(StructureUnit structure)
  {
    SetState(UnitState.Building);
    m_Animator.SetBool("IsBuilding", true);
    structure.AssignWorkerToBuildProcess(this);
  }

  void ResetState()
  {
    SetTask(UnitTask.None);

    if (HasTarget)
    {
      ClearTarget();
    }

    m_Animator.SetBool("IsBuilding", false);
  }

  void ClearTarget()
  {
    if (Target is StructureUnit st)
    {
      // (Target as StructureUnit).UnassignWorkerFromBuildProcess();
      st.UnassignWorkerFromBuildProcess();
    }
    SetTarget(null);
  }
}

// void CheckForCloseObjects()
//   {
//     var detectedObjects = RunProximityObjectDetection();
//     foreach (var obj in detectedObjects)
//     {
//       if (obj.gameObject == this.gameObject) continue;

//       if (CurrentTask == UnitTask.Build && obj.gameObject == Target.gameObject)
//       {
//         if (obj.TryGetComponent<StructureUnit>(out var structure))
//         {
//           StartBuilding(structure);
//         }
//       }
//     }
//   }
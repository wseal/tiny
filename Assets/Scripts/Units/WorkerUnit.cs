
using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
  // Worker-specific implementation

  protected override void UpdateBehaviour()
  {
    if (CurrentTask != UnitTask.None)
    {
      CheckForCloseObjects();
    }
  }

  void CheckForCloseObjects()
  {
    var detectedObjects = RunProximityObjectDetection();
    foreach (var obj in detectedObjects)
    {
      if (obj.gameObject == this.gameObject) continue;

      if (CurrentTask == UnitTask.Build && obj.gameObject == Target.gameObject)
      {
        if (obj.TryGetComponent<StructureUnit>(out var structure))
        {
          StartBuilding(structure);
        }
      }
    }
  }
  
  void StartBuilding(StructureUnit structure)
  {
    // Start building logic
  }
}

using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Controls; // 引入新输入系统的命名空间

public enum ClickType
{
  Move, Attack, Build
}
public class GameManager : SingletonManager<GameManager>
{
  [Header("UI")]
  [SerializeField] private PointToClick m_PointToMovePrefab;
  [SerializeField] private PointToClick m_PointToBuildPrefab;
  [SerializeField] private ActionBar m_ActionBar;
  [SerializeField] private ConfirmationBar m_ConfirmationBar;
  [Header("Camera Settings")]
  [SerializeField] private float m_PanSpeed = 100;
  [SerializeField] private float m_MobilePanSpeed = 10;

  [Header("VFX")]
  [SerializeField] private ParticleSystem m_ConstructionEffectPrefab;

  public Unit ActiveUnit = null;

  private CameraController m_CameraController;
  private PlacementProcess m_PlacementProcess;

  private int m_Gold = 1000;
  private int m_Wood = 1500;
  public int Gold => m_Gold;
  public int Wood => m_Wood;


  // public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
  // public bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
  // public bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

  public bool HasActiveUnit => ActiveUnit != null;

  void Start()
  {
    m_CameraController = new CameraController(m_PanSpeed, m_MobilePanSpeed);
    ClearActionBarUI();
  }

  // old
  void Update()
  {
    m_CameraController.Update();

    if (m_PlacementProcess != null)
    {
      m_PlacementProcess.Update();
    }
    else if (TouchUtils.TryGetShortClickPosition(out var inputPosition))
    {
      DetectClick(inputPosition);
    }

    // Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    // if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    // {
    //   m_InitialTouchPosition = inputPosition;
    // }

    // if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
    // {
    //   if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 5)
    //   {
    //     DetectClick(inputPosition);
    //   }
    // }
  }

  public void StartBuildAction(BuildActionSO buildAction)
  {
    if (m_PlacementProcess != null) return;

    var tilemapManager = TilemapManager.Get();
    m_PlacementProcess = new PlacementProcess(
      buildAction,
      tilemapManager);

    m_PlacementProcess.ShowPlacementOutline();
    m_ConfirmationBar.Show(buildAction.GoldCost, buildAction.WoodCost);
    m_ConfirmationBar.SetupHooks(ConfirmBuildPlacement, CancelBuildPlacement);
    m_CameraController.LockCamera = true;
  }

  void DetectClick(Vector2 inputPosition)
  {
    // Debug.Log(inputPosition);
    if (TouchUtils.IsPointerOverUIElement())
    {
      // Debug.Log("点击在UI上,忽略");
      return;
    }

    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
    if (HasClickedOnUnit(hit, out var unit))
    {
      HandleClickOnUnit(unit);
    }
    else
    {
      HandleClickOnGround(worldPoint);
    }
  }

  bool HasClickedOnUnit(RaycastHit2D hit, out Unit unit)
  {
    if (hit.collider != null && hit.collider.TryGetComponent<Unit>(out var clickedUnit))
    {
      unit = clickedUnit;
      return true;
    }

    unit = null;
    return false;
  }

  void HandleClickOnGround(Vector2 worldPoint)
  {
    if (HasActiveUnit && IsHumanoid(ActiveUnit))
    {
      DisplayPointToClick(worldPoint, ClickType.Move);
      ActiveUnit.MoveTo(worldPoint);
    }
  }

  void HandleClickOnUnit(Unit unit)
  {
    if (HasActiveUnit)
    {
      if (HasClickedOnActiveUnit(unit))
      {
        CancelActiveUnit();
        return;
      }
      else if (HasClickedOnUnFinishedBuild(unit))
      {
        DisplayPointToClick(unit.transform.position, ClickType.Build);
        (ActiveUnit as WorkerUnit)?.SendToBuild(unit as StructureUnit);
        return;
      }
    }

    SelectUnit(unit);
  }

  bool HasClickedOnUnFinishedBuild(Unit clickedUnit)
  {
    return ActiveUnit is WorkerUnit worker &&
           clickedUnit is StructureUnit structure &&
           structure.IsUnderConstruction;
  }
  void SelectUnit(Unit unit)
  {
    if (HasActiveUnit)
    {
      ActiveUnit.UnSelect();
    }

    ActiveUnit = unit;
    ActiveUnit.Select();
    ShowUnitActions(unit);
  }

  bool HasClickedOnActiveUnit(Unit clickedUnit)
  {
    return ActiveUnit != null && clickedUnit == ActiveUnit;
  }

  bool IsHumanoid(Unit unit)
  {
    return unit is HumanoidUnit;
  }

  void CancelActiveUnit()
  {
    ActiveUnit.UnSelect();
    ActiveUnit = null;

    ClearActionBarUI();
  }

  void DisplayPointToClick(Vector2 worldPoint, ClickType clickType = ClickType.Move)
  {
    if (clickType == ClickType.Move)
    {
      Instantiate(m_PointToMovePrefab, (Vector3)worldPoint, Quaternion.identity);
    }
    else if (clickType == ClickType.Build)
    {
      Instantiate(m_PointToBuildPrefab, (Vector3)worldPoint, Quaternion.identity);
    }
  }

  void ShowUnitActions(Unit unit)
  {
    ClearActionBarUI();

    if (unit.Actions.Length == 0)
    {
      return;
    }

    // var hardCodeActions = 2;
    // for (int i = 0; i < hardCodeActions; i++)
    // {
    //   m_ActionBar.RegisterActionButton();
    // }

    foreach (var action in unit.Actions)
    {
      m_ActionBar.RegisterActionButton(action.Icon, () => action.Execute(this));
    }

    m_ActionBar.Show();
  }

  void ClearActionBarUI()
  {
    m_ActionBar.ClearActionButtons();
    m_ActionBar.Hide();
  }

  void ConfirmBuildPlacement()
  {
    Debug.Log("Confirm Build Placement");
    if (!TryDetectResources(m_PlacementProcess.GoldCost, m_PlacementProcess.WoodCost))
    {
      Debug.Log("Not enough resources to build!");
      return;
    }

    if (m_PlacementProcess.TryFinalizePlacement(out var buildPosition))
    {
      DisplayPointToClick(buildPosition, ClickType.Build);
      m_ConfirmationBar.Hide();
      var build = new BuildingProcess(
        m_PlacementProcess.BuildAction,
        buildPosition,
        (WorkerUnit)ActiveUnit,
        m_ConstructionEffectPrefab);
      // m_PlacementProcess.Cleanup();

      // ActiveUnit.MoveTo(buildPosition);
      // ActiveUnit.SetTask(UnitTask.Build);
      m_PlacementProcess = null;
      m_CameraController.LockCamera = false;
    }
    else
    {
      RevertResources(m_PlacementProcess.GoldCost, m_PlacementProcess.WoodCost);
    }
  }

  void RevertResources(int goldCost, int woodCost)
  {
    m_Gold += goldCost;
    m_Wood += woodCost;
  }

  void CancelBuildPlacement()
  {
    m_ConfirmationBar.Hide();
    m_PlacementProcess.Cleanup();
    m_PlacementProcess = null;
    m_CameraController.LockCamera = false;
  }


  bool TryDetectResources(int goldCost, int woodCost)
  {
    if (m_Gold >= goldCost && m_Wood >= woodCost)
    {
      m_Gold -= goldCost;
      m_Wood -= woodCost;
      return true;
    }

    return false;
  }

  void OnGUI()
  {
    GUI.Label(new Rect(10, 10, 200, 20), $"Gold: {m_Gold}", new GUIStyle { fontSize = 20 });
    GUI.Label(new Rect(10, 30, 200, 20), $"Wood: {m_Wood}", new GUIStyle { fontSize = 20 });

    if (ActiveUnit != null)
    {
      GUI.Label(new Rect(10, 50, 400, 20), $"State {ActiveUnit.CurrentState.ToString()}", new GUIStyle { fontSize = 20 });
      GUI.Label(new Rect(10, 70, 400, 20), $"Task {ActiveUnit.CurrentTask.ToString()}", new GUIStyle { fontSize = 20 });
    }
  }

  // protected virtual void Awake()
  // {
  //   GameManager[] managers = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
  //   if (managers.Length > 1)
  //   {
  //     Destroy(gameObject);
  //     return;
  //   }
  // }

  // public static GameManager Get()
  // {
  //   var tag = nameof(GameManager);
  //   GameObject managerObject = GameObject.FindWithTag(tag);
  //   if (managerObject != null)
  //   {
  //     return managerObject.GetComponent<GameManager>();
  //   }

  //   GameObject go = new(tag);
  //   go.tag = tag;
  //   return go.AddComponent<GameManager>();
  // }


  // void Update()
  // {
  //   Vector2 inputPosition = GetUnifiedInputPosition();

  //   // 检查是否有实际的点击或触摸发生 (新系统推荐使用事件或 Action，这里仅作演示)
  //   if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame ||
  //       Touchscreen.current != null && Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Began)
  //   {
  //     m_InitialTouchPosition = inputPosition;
  //     // Debug.Log($"统一输入位置 (屏幕坐标): {inputPosition}");

  //     // // 同样可以转换为世界坐标
  //     // float distance = 10f;
  //     // Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, distance));
  //     // Debug.Log($"转换后的世界坐标: {worldPos}");
  //   }

  //   if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame ||
  //    Touchscreen.current != null && Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended)
  //   {
  //     if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10)
  //     {
  //       DetectClick(inputPosition);
  //     }
  //   }
  // }

  // private Vector2 GetUnifiedInputPosition()
  // {
  //   // 尝试获取触摸位置
  //   if (Touchscreen.current != null) // && Touchscreen.current.primaryTouch.)
  //   {
  //     // 获取主要触摸点的位置
  //     return Touchscreen.current.primaryTouch.position.ReadValue();
  //   }
  //   // 尝试获取鼠标位置
  //   else if (Mouse.current != null)
  //   {
  //     // 获取鼠标位置
  //     return Mouse.current.position.ReadValue();
  //   }

  //   // 如果都没有，返回 (0, 0)
  //   return Vector2.zero;
  // }

}
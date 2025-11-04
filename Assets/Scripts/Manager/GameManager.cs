
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Controls; // 引入新输入系统的命名空间

public class GameManager : SingletonManager<GameManager>
{

  [Header("UI")]
  [SerializeField] private PointToClick m_PointToClickPrefab;
  [SerializeField] private ActionBar m_ActionBar;

  public Unit ActiveUnit = null;

  private PlacementProcess m_PlacementProcess;


  // public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
  // public bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
  // public bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

  public bool HasActiveUnit => ActiveUnit != null;

  void Start()
  {
    ClearActionBarUI();
  }

  // old
  void Update()
  {
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
    // Debug.Log($"Start Build Action: {buildAction.ActionName}");
    // Implement build action initiation logic here
    m_PlacementProcess = new PlacementProcess(buildAction);
    m_PlacementProcess.ShowPlacementOutline();
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
      DisplayPointToClick(worldPoint);
      ActiveUnit.MoveTo(worldPoint);
    }
  }

  void HandleClickOnUnit(Unit unit)
  {
    // if (HasActiveUnit)
    // {
    if (HasClickedOnActiveUnit(unit))
    {
      CancelActiveUnit();
      return;
    }
    // }

    SelectUnit(unit);
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

  void DisplayPointToClick(Vector2 worldPoint)
  {
    Instantiate(m_PointToClickPrefab, (Vector3)worldPoint, Quaternion.identity);
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
      m_ActionBar.RegisterActionButton(action.Icon, ()=>action.Execute(this));
    } 

    m_ActionBar.Show();
  }

  void ClearActionBarUI()
  {
    m_ActionBar.ClearActionButtons();
    m_ActionBar.Hide();
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
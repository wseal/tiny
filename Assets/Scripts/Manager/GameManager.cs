
using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Controls; // 引入新输入系统的命名空间

public class GameManager : SingletonManager<GameManager>
{

  [Header("UI")]
  [SerializeField] private PointToClick m_PointToClickPrefab;

  public Unit ActiveUnit = null;

  private Vector2 m_InitialTouchPosition;

  public bool HasActiveUnit => ActiveUnit != null;
  // old
  void Update()
  {
    Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    {
      m_InitialTouchPosition = inputPosition;
    }

    if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
    {
      if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10)
      {
        DetectClick(inputPosition);
      }
    }
  }

  void DetectClick(Vector2 inputPosition)
  {
    Debug.Log(inputPosition);

    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
    RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
    if (hasClickedOnUnit(hit, out var unit))
    {
      HandleClickOnUnit(unit);
    }
    else
    {
      HandleClickOnGround(worldPoint);
    }
  }

  bool hasClickedOnUnit(RaycastHit2D hit, out Unit unit)
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
  }

  bool IsHumanoid(Unit unit)
  {
    return unit is HumanoidUnit;
  }
  
  void DisplayPointToClick(Vector2 worldPoint)
  {
    Instantiate(m_PointToClickPrefab, (Vector3)worldPoint, Quaternion.identity);
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
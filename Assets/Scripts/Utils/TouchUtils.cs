using UnityEngine;
using UnityEngine.EventSystems;

public static class TouchUtils
{
  public static Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
  public static bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
  public static bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);


  public static Vector3 InputHoldWorldPosition => Input.touchCount > 0 ?
    Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) :
    Input.GetMouseButton(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Vector3.zero;


  private static Vector2 m_InitialTouchPosition;
  public static bool TryGetShortClickPosition(out Vector2 inputPosition, float maxDistance = 5f)
  {
    inputPosition = InputPosition;
    if (IsLeftClickOrTapDown)
    {
      m_InitialTouchPosition = inputPosition;
    }

    if (IsLeftClickOrTapUp && Vector2.Distance(m_InitialTouchPosition, inputPosition) < maxDistance)
    {
      return true;
    }

    return false;
  }

  public static bool TryGetHoldPosition(out Vector3 worldPosition)
  {
    if (Input.touchCount > 0)
    {
      worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
      return true;
    }
    else if (Input.GetMouseButton(0))
    {
      worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      return true;
    }

    worldPosition = Vector3.zero;
    return false;
  }

  public static bool IsPointerOverUIElement()
  {
    // Implement UI detection logic here
    if (Input.touchCount > 0)
    {
      // Check if any touch is over a UI element
      Touch touch = Input.GetTouch(0);
      return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
    else
    {
      // Check if mouse is over a UI element
      return EventSystem.current.IsPointerOverGameObject();
    }
  }
}
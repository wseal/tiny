using UnityEngine;

public static class TouchUtils
{
  public static Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
  public static bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
  public static bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);


  public static Vector3 InputHoldWorldPosition => Input.touchCount > 0 ?
    Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) :
    Input.GetMouseButton(0)? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Vector3.zero;

}
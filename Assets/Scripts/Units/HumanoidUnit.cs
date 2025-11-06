
using UnityEngine;

public class HumanoidUnit : Unit
{
  protected Vector2 m_Velcity;
  protected Vector3 m_LastPosition;

  public float CurrentSpeed => m_Velcity.magnitude;

  void Start()
  {
    m_LastPosition = transform.position;
  }

  void Update()
  {

    UpdateVelocity();
    UpdateBehaviour();
  }

  protected virtual void UpdateBehaviour()
  {
    // To be implemented in derived classes
  }

  protected virtual void UpdateVelocity()
  {
    m_Velcity = new Vector2(
     transform.position.x - m_LastPosition.x,
     transform.position.y - m_LastPosition.y
     ) / Time.deltaTime;

    m_LastPosition = transform.position;
    var state = m_Velcity.magnitude > 0 ? UnitState.Moving : UnitState.Idle;
    SetState(state);
    
    m_Animator?.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
  }
}
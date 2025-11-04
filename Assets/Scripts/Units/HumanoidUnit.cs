
using UnityEngine;

public class HumanoidUnit: Unit
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
    m_Velcity = new Vector2(
      transform.position.x - m_LastPosition.x,
      transform.position.y - m_LastPosition.y
      ) / Time.deltaTime;

    m_LastPosition = transform.position;
    IsMoving = m_Velcity.magnitude > 0;

    m_Animator.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
  }
}
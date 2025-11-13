
using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
  private float m_AttackCommitmentTime = 1f; // time to stay in attack state after got target
  private float m_CurrentAttackCommitmentTime = 0f;
  public override bool IsPlayer => false;


  protected override void UpdateBehaviour()
  {
    switch (CurrentState)
    {
      case UnitState.Idle:
      case UnitState.Moving:
        if (HasTarget)
        {
          if (IsTargetInRange(Target.transform))
          {
            SetState(UnitState.Attacking);
            StopMovement();
          }
          else
          {
            MoveTo(Target.transform.position);
          }
        }
        else
        {
          if (TryFindClosestFoe(out var foe))
          {
            // Debug.Log(foe.gameObject.name);
            SetTarget(foe);
            MoveTo(foe.transform.position);
            Debug.Log("Move to closest target: " + foe.gameObject.name);
          }
        }
        break;

      case UnitState.Attacking:
        if (HasTarget)
        {
          if (IsTargetInRange(Target.transform))
          {
            // Debug.Log("Attacking");
            m_CurrentAttackCommitmentTime = m_AttackCommitmentTime;
            TryAttackCurrentTarget();
          }
          else
          {
            // Debug.Log("Back to Moving state");
            m_CurrentAttackCommitmentTime -= Time.deltaTime;
            if (m_CurrentAttackCommitmentTime <= 0)
            {
              SetState(UnitState.Moving);
            }
          }
        }
        else
        {
          Debug.Log("Back to Idle state");
          SetState(UnitState.Idle);
        }
        break;
    }
  }
}
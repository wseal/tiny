
using UnityEngine;

public class SoldierUnit : HumanoidUnit
{

    private bool m_IsRetreating = false;

    protected override void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        if (newTask == UnitTask.Attack && HasTarget)
        {
            MoveTo(Target.transform.position);
        }

        base.OnSetTask(oldTask, newTask);
    }


    protected override void OnSetDestination(Vector3 destination)
    {
        // base.OnSetDestination(destination);
        if (HasTarget && (CurrentState == UnitState.Attacking || CurrentTask == UnitTask.Attack))
        {
            m_IsRetreating = true;
        }

        if (CurrentTask == UnitTask.Attack)
        {
            SetTask(UnitTask.None);
            SetTarget(null);
        }
    }

    protected override void OnDestinationReached()
    {
        // base.OnDestinationReached();
        if (m_IsRetreating)
        {
            m_IsRetreating = false;
        }
    }

    protected override void UpdateBehaviour()
    {
        if (CurrentState == UnitState.Idle || CurrentState == UnitState.Moving)
        {
            if (HasTarget)
            {
                if (IsTargetInRange(Target.transform))
                {
                    StopMovement();
                    SetState(UnitState.Attacking);
                }
            }
            else
            {
                if (!m_IsRetreating && TryFindClosestFoe(out var foe))
                {
                    SetTarget(foe);
                    SetTask(UnitTask.Attack);
                }
            }
        }
        else if (CurrentState == UnitState.Attacking)
        {
            if (HasTarget)
            {
                if (IsTargetInRange(Target.transform))
                {
                    TryAttackCurrentTarget();
                }
                else
                {
                    SetState(UnitState.Moving);
                }
            }
            else
            {
                SetState(UnitState.Idle);
            }
        }
        // base.UpdateBehaviour();
    }
}
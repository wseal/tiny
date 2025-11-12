
using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
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
                        // stop movement
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
                        Debug.Log("Attacking");
                    }
                    else
                    {
                        Debug.Log("Back to Moving state");
                        SetState(UnitState.Moving);

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
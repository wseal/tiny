using UnityEngine;

public enum UnitState
{
  Idle, Moving, Attacking, Chopping, Mining, Building
}

public enum UnitTask
{
  None, Build, Chop, Mine, Attack
}

public abstract class Unit : MonoBehaviour
{
  [SerializeField] private ActionSO[] m_Actions;
  [SerializeField] protected float m_DetectionRadius = 3.0f;
  [SerializeField] protected float m_UnitDetectionCheckRate = 0.5f;
  [SerializeField] protected float m_AttackRange = 1f;
  [SerializeField] protected float m_AutoAttackFrequency = 1.5f;

  // [SerializeField]
  // private Material m_HighlightMaterial; // set from unity
  public bool IsTargeted;
  protected GameManager m_GameManager;
  protected Animator m_Animator;
  protected AIPawn m_AIPown;

  protected SpriteRenderer m_SpriteRenderer;
  protected Material m_OriginalMaterial;
  protected Material m_HighlightMaterial; // load from resources
  protected float m_NextUnitDetectionTime;
  protected float m_NextAutoAttackTime;

  public UnitState CurrentState { get; protected set; } = UnitState.Idle;
  public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
  public Unit Target { get; protected set; }
  public virtual bool IsPlayer => true;
  public virtual bool IsBuilding => false;
  public ActionSO[] Actions => m_Actions;
  public SpriteRenderer SpriteRenderer => m_SpriteRenderer;
  public bool HasTarget => Target != null;

  protected virtual void Start()
  {
    RegisterUnit();
  }

  void Awake()
  {
    if (TryGetComponent<Animator>(out var animator))
    {
      m_Animator = animator;
    }

    if (TryGetComponent<AIPawn>(out var aiPown))
    {
      m_AIPown = aiPown;
      m_AIPown.OnNewPositionSelected += TurnToPosition;
    }

    m_GameManager = GameManager.Get();
    m_SpriteRenderer = GetComponent<SpriteRenderer>();
    m_OriginalMaterial = m_SpriteRenderer.material;
    m_HighlightMaterial = Resources.Load<Material>("Materials/Outlines");
  }

  void OnDestroy()
  {
    if (m_AIPown != null)
    {
      m_AIPown.OnNewPositionSelected -= TurnToPosition;
    }
    UnregisterUnit();
  }

  public void SetTask(UnitTask task)
  {
    OnSetTask(CurrentTask, task);
  }

  public void SetState(UnitState state)
  {
    OnSetState(CurrentState, state);
  }

  public void SetTarget(Unit target)
  {
    Target = target;
  }

  public void MoveTo(Vector3 destination)
  {
    var direction = (destination - transform.position).normalized;
    m_SpriteRenderer.flipX = direction.x < 0;

    m_AIPown.SetDestination(destination);
    OnSetDestination(destination);
  }

  public void Select()
  {
    Highlight();
    IsTargeted = true;
  }

  public void UnSelect()
  {
    UnHighlight();
    IsTargeted = false;
  }

  public void StopMovement()
  {
    m_AIPown.Stop();
  }

  protected virtual void OnSetDestination(Vector3 destination)
  {
    // To be implemented in derived classes
  }

  protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
  {
    // To be implemented in derived classes
    CurrentTask = newTask;
  }

  protected virtual void OnSetState(UnitState oldState, UnitState newState)
  {
    // To be implemented in derived classes
    CurrentState = newState;
  }

  protected virtual void RegisterUnit()
  {
    m_GameManager.RegisterUnit(this);
  }

  protected virtual void UnregisterUnit()
  {
    m_GameManager.UnregisterUnit(this);
  }

  protected virtual bool TryFindClosestFoe(out Unit foe)
  {
    if (Time.time >= m_NextUnitDetectionTime)
    {
      m_NextUnitDetectionTime = Time.time + m_UnitDetectionCheckRate;

      foe = m_GameManager.FindClosestUnit(transform.position, m_DetectionRadius, !IsPlayer);
      return foe != null;
    }

    foe = null;
    return false;
  }

  protected virtual bool TryAttackCurrentTarget()
  {
    if (Time.time >= m_NextAutoAttackTime)
    {
      m_NextAutoAttackTime += m_AutoAttackFrequency;
      PerformAttackAnimation();
      return true;
    }
    return false;
  }

  protected virtual void PerformAttackAnimation()
  {

  }

  protected bool IsTargetInRange(Transform target)
  {
    return Vector3.Distance(target.transform.position, transform.position) <= m_AttackRange;
  }

  protected Collider2D[] RunProximityObjectDetection()
  {
    return Physics2D.OverlapCircleAll(transform.position, m_DetectionRadius);
  }

  void TurnToPosition(Vector3 newPosition)
  {
    var direction = (newPosition - transform.position).normalized;
    m_SpriteRenderer.flipX = direction.x < 0;
  }
  private void Highlight()
  {
    m_SpriteRenderer.material = m_HighlightMaterial;
  }

  private void UnHighlight()
  {
    m_SpriteRenderer.material = m_OriginalMaterial;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = new Color(0, 0, 1, 0.4f);
    Gizmos.DrawSphere(transform.position, m_DetectionRadius);

    Gizmos.color = new Color(1, 0, 1, 0.4f);
    Gizmos.DrawSphere(transform.position, m_AttackRange);
  }
}

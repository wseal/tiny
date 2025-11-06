using UnityEngine;

public enum UnitState
{
  Idle, Moving, Attacking, Chopping, Mining, Building
}

public enum UnitTask
{
  None, Build, Chop, Mine,  Attack
}

public abstract class Unit : MonoBehaviour
{
  [SerializeField] private ActionSO[] m_Actions;
  [SerializeField] protected float m_DetectionRadius = 3.0f;

  // [SerializeField]
  // private Material m_HighlightMaterial; // set from unity
  public bool IsTargeted;
  protected Animator m_Animator;
  protected AIPawn m_AIPown;

  protected SpriteRenderer m_SpriteRenderer;
  protected Material m_OriginalMaterial;
  protected Material m_HighlightMaterial; // load from resources

  public UnitState CurrentState { get; protected set; } = UnitState.Idle;
  public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
  public Unit Target { get; protected set; }
  public ActionSO[] Actions => m_Actions;
  public SpriteRenderer SpriteRenderer => m_SpriteRenderer;
  public bool HasTarget=> Target != null;
  void Awake()
  {
    if (TryGetComponent<Animator>(out var animator))
    {
      m_Animator = animator;
    }

    if (TryGetComponent<AIPawn>(out var aiPown))
    {
      m_AIPown = aiPown;
    }

    m_SpriteRenderer = GetComponent<SpriteRenderer>();
    m_OriginalMaterial = m_SpriteRenderer.material;
    m_HighlightMaterial = Resources.Load<Material>("Materials/Outlines");
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

  protected Collider2D[] RunProximityObjectDetection()
  {
    return Physics2D.OverlapCircleAll(transform.position, m_DetectionRadius);
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
  }
}

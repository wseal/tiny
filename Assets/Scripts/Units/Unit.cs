using UnityEngine;

public abstract class Unit: MonoBehaviour
{
  [SerializeField] private ActionSO[] m_Actions;

  // [SerializeField]
  // private Material m_HighlightMaterial; // set from unity
  public bool IsMoving;
  public bool IsTargeted;
  protected Animator m_Animator;
  protected AIPawn m_AIPown;

  protected SpriteRenderer m_SpriteRenderer;
  protected Material m_OriginalMaterial;
  protected Material m_HighlightMaterial; // load from resources

  public ActionSO[] Actions => m_Actions;
  public SpriteRenderer SpriteRenderer => m_SpriteRenderer;
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

  public void MoveTo(Vector3 destination)
  {
    var direction = (destination - transform.position).normalized;
    m_SpriteRenderer.flipX = direction.x < 0;

    m_AIPown.SetDestination(destination);
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

  private void Highlight()
  {
    m_SpriteRenderer.material = m_HighlightMaterial;
  }
  
  private void UnHighlight()
  {
    m_SpriteRenderer.material = m_OriginalMaterial;
  }
}

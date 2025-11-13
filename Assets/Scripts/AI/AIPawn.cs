using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
  [SerializeField] private float m_Speed = 3f;
  private List<Vector3> m_CurrentPath = new();
  private TilemapManager m_TilemapManager;
  private int m_CurrentNodeIndex;

  public UnityAction<Vector3> OnNewPositionSelected = delegate { };
  public UnityAction OnDestinationReached = delegate { };

  void Start()
  {
    m_TilemapManager = TilemapManager.Get();
  }

  void Update()
  {
    // direct move
    // if (m_Destination.HasValue)
    // {
    //   var dir = m_Destination.Value - transform.position;
    //   transform.position += dir.normalized * Time.deltaTime * m_Speed;

    //   var distanceToDestination = Vector3.Distance(transform.position, m_Destination.Value);
    //   if (distanceToDestination < 0.1f)
    //   {
    //     m_Destination = null;
    //   }
    // }

    // A star
    if (!IsPathValid())
    {
      return;
    }

    Vector3 targetPos = m_CurrentPath[m_CurrentNodeIndex];
    var direction = (targetPos - transform.position).normalized;

    transform.position += direction * m_Speed * Time.deltaTime;
    if (Vector3.Distance(transform.position, targetPos) <= 0.15f)
    {
      if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
      {
        Debug.Log("Destination Reached!");
        OnDestinationReached.Invoke();
        m_CurrentPath = new();
      }
      else
      {
        m_CurrentNodeIndex += 1;
        OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
      }
    }
  }
  public void SetDestination(Vector3 dest)
  {
    if (m_CurrentPath.Count > 0)
    {
      Node newEndNode = m_TilemapManager.FindNode(dest);
      Vector3 endPos = new Vector3(newEndNode.centerX, newEndNode.centerY);
      var distance = Vector3.Distance(endPos, m_CurrentPath[^1]);
      if (distance < 0.1f)
      {
        return;
      }
    }

    m_CurrentPath = m_TilemapManager.FindPath(transform.position, dest);
    m_CurrentNodeIndex = 0;
    OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
  }

  public void Stop()
  {
    m_CurrentPath.Clear();
    m_CurrentNodeIndex = 0;
  }

  bool IsPathValid()
  {
    return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
  }
}
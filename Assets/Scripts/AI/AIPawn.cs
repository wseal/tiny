using System.Collections.Generic;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
  private float m_Speed = 3f;

  private Vector3? m_Destination;
  private List<Node> m_CurrentPath = new();
  private TilemapManager m_TilemapManager;
  private int m_CurrentNodeIndex;
  public Vector3? Destination => m_Destination;
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
      m_Destination = null;
      return;
    }

    Node curNode = m_CurrentPath[m_CurrentNodeIndex];
    Vector3 targetPos = new Vector3(curNode.centerX, curNode.centerY);
    var direction = (targetPos - transform.position).normalized;

    transform.position += direction * m_Speed * Time.deltaTime;
    if (Vector3.Distance(transform.position, targetPos) <= 0.15f)
    {
      m_CurrentNodeIndex += 1;
    }
  }
  public void SetDestination(Vector3 dest)
  {
    m_CurrentPath = m_TilemapManager.FindPath(transform.position, dest);
    m_CurrentNodeIndex = 0;
    m_Destination = dest;
  }

  bool IsPathValid()
  {
    return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
  }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
  private float m_Speed = 3f;

  private List<Node> m_CurrentPath = new();
  private TilemapManager m_TilemapManager;
  private int m_CurrentNodeIndex;
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

    Node curNode = m_CurrentPath[m_CurrentNodeIndex];
    Vector3 targetPos = new Vector3(curNode.centerX, curNode.centerY);
    var direction = (targetPos - transform.position).normalized;

    transform.position += direction * m_Speed * Time.deltaTime;
    if (Vector3.Distance(transform.position, targetPos) <= 0.15f)
    {
      if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
      {
        Debug.Log("Destination Reached!");
        m_CurrentPath = new();
      }
      else
      {
        m_CurrentNodeIndex += 1;
      }
    }
  }
  public void SetDestination(Vector3 dest)
  {
    if (m_CurrentPath.Count > 0)
    {
      Node newEndNode = m_TilemapManager.FindNode(dest);
      // if (newEndNode == m_CurrentPath.Last())
      if (newEndNode == m_CurrentPath[^1])
      {
        return;
      }
    }

    m_CurrentPath = m_TilemapManager.FindPath(transform.position, dest);
    m_CurrentNodeIndex = 0;
  }

  bool IsPathValid()
  {
    return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
  }
}
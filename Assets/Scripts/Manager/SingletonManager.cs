
using UnityEngine;

public class SingletonManager<T>: MonoBehaviour where T: MonoBehaviour
{

  protected virtual void Awake()
  {
    T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);
    if (managers.Length > 1)
    {
      Destroy(gameObject);
      return;
    }
  }

  public static T Get()
  {
    // var tag = nameof(GameManager);
    var tag = typeof(T).Name; 
    GameObject managerObject = GameObject.FindWithTag(tag);
    if (managerObject != null)
    {
      return managerObject.GetComponent<T>();
    }

    GameObject go = new GameObject(tag);
    go.tag = tag;
    return go.AddComponent<T>();
  }
  
}


using UnityEngine;

public abstract class ActionSO : ScriptableObject
{
  public string ActionName;
  public Sprite Icon;
  public string Guid = System.Guid.NewGuid().ToString();
  public abstract void Execute(GameManager m);
}
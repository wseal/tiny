
using UnityEngine;

[CreateAssetMenu(fileName = "BuildAction", menuName = "SObjects/Actions/BuildAction", order = 1)]
public class BuildActionSO : ActionSO
{
  [SerializeField] private Sprite m_PlacementSprite;
  [SerializeField] private Sprite m_FundationSprite;
  [SerializeField] private Sprite m_CompletionSprite;

  [SerializeField] private int m_GoldCost;
  [SerializeField] private int m_WoodCost;

  public Sprite PlacementSprite => m_PlacementSprite;
  public Sprite FundationSprite => m_FundationSprite;
  public Sprite CompletionSprite => m_CompletionSprite;

  public int GoldCost => m_GoldCost;
  public int WoodCost => m_WoodCost;


  public override void Execute(GameManager m)
  {
    // Implementation for build action
    m.StartBuildAction(this);
  }
}
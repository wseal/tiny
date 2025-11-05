

using TMPro;
using UnityEngine;

public class ResourceRequirementsDisplay : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI m_GoldText;
  [SerializeField] private TextMeshProUGUI m_WoodText;

  public void Show(int goldCost, int woodCost)
  {
    m_GoldText.text = goldCost.ToString();
    m_WoodText.text = woodCost.ToString();
    UpdateColorRequirements(goldCost, woodCost);
  }

  public void UpdateColorRequirements(int goldCost, int woodCost)
  {
    var mgr = GameManager.Get();

    var green = new Color(0f, 0.8f, 0f, 1f);
    m_GoldText.color = mgr.Gold >= goldCost ? green : Color.red;
    m_WoodText.color = mgr.Wood >= woodCost ? green : Color.red;
  }
}
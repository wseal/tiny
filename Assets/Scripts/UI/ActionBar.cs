
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ActionBar : MonoBehaviour
{
  // ActionBar related code here
  [SerializeField] private Image m_BackgroundImage;
  [SerializeField] private ActionButton m_ActionButtonPrefab;
  private Color m_OriginalBackgroundColor;
  private List<ActionButton> m_ActionButtons = new List<ActionButton>();


  void Awake()
  {
    m_OriginalBackgroundColor = m_BackgroundImage.color;
    // Hide();
  }

  public void RegisterActionButton(/*Sprite iconSprite*/)
  {
    ActionButton newButton = Instantiate(m_ActionButtonPrefab, transform);
    // 设置按钮图标
    // newButton.SetIcon(iconSprite);
    m_ActionButtons.Add(newButton);
  }

  public  void ClearActionButtons()
  {
    for (int i = m_ActionButtons.Count - 1; i >= 0; i--)
    {
      Destroy(m_ActionButtons[i].gameObject);
      m_ActionButtons.RemoveAt(i);
    }

    // foreach (var button in m_ActionButtons)
    // {
    //   Destroy(button.gameObject);
    // }
    // m_ActionButtons.Clear();
  }

  public void Show()
  {
    m_BackgroundImage.color = m_OriginalBackgroundColor;
  }
  
  public void Hide()
  {
    //m_BackgroundImage.color = new Color(0, 0, 0, 0);

    Color transparentColor = m_OriginalBackgroundColor;
    transparentColor.a = 0f;
    m_BackgroundImage.color = transparentColor;
  }
}
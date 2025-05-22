using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject[] panels;
    [SerializeField] private Button[] _resetButtons;
    
    protected virtual void InitializeButtons()
    {
        CloseAllPanel();
        foreach (Button button in _resetButtons)
        {
            button.onClick.AddListener(ReturnToDefaultPanel);
        }
    }
    
    /// <summary>
    /// 지정한 이름의 패널만 활성화하고, 나머지 패널은 비활성화하는 함수
    /// </summary>
    /// <param name="panelName"></param>
    public void OpenPanel(string panelName, bool closeOthers = true)
    {
        foreach (GameObject panel in panels)
        {
            if (closeOthers == true)
            {
                // 이름이 일치하는 패널만 활성화 (나머지는 비활성화)
                panel.SetActive(panel.name == panelName);
            }
            //다른패널은 종료하지 않고, 프로퍼티로 받은 패널 활성화
            else
            {
                if (panel.name == panelName)
                {
                    panel.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 지정한 이름의 패널만 비활성화하는 함수
    /// </summary>
    public void ClosePanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            if (panel.name == panelName)
            {
                panel.SetActive(false);
                break;
            }
            // 전부 비활성화
            // panel.SetActive(false);
        }
    }
    
    public void CloseAllPanel()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false); 
        }
    }

    public void ReturnToDefaultPanel()
    {
        CloseAllPanel();
    }
}

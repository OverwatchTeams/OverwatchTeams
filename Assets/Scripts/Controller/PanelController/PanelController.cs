using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject[] panels;
    [SerializeField] private Button[] _returnToDefaultButtons;
    [SerializeField] private Button[] _returnToParentButtons;
    
    protected virtual void InitializeListeners()
    {
        CloseAllPanel();
        foreach (Button button in _returnToDefaultButtons)
        {
            button.onClick.RemoveListener(CloseAllPanel);
            button.onClick.AddListener(CloseAllPanel);
        }
        foreach (Button button in _returnToParentButtons)
        {
            button.onClick.RemoveListener(ReturnToParentPanel);
            button.onClick.AddListener(ReturnToParentPanel);
        }
        
    }
    
    /// <summary>
    /// 지정한 이름의 패널만 활성화하고, 나머지 패널은 비활성화하는 함수
    /// </summary>
    /// <param name="panelName"></param>
    public GameObject OpenPanel(string panelName, bool closeOthers = true)
    {
        GameObject resultPanel = null;
        foreach (GameObject panel in panels)
        {
            if (closeOthers == true)
            {
                // 이름이 일치하는 패널만 활성화 (나머지는 비활성화)
                if (panel.name == panelName)
                {
                    panel.SetActive(true);
                    resultPanel = panel;
                }
                else
                {
                    panel.SetActive(false);
                }
            }
            //다른패널은 종료하지 않고, 프로퍼티로 받은 패널 활성화
            else
            {
                if (panel.name == panelName)
                {
                    panel.SetActive(true);
                    resultPanel = panel;
                }
            }
        }
        return resultPanel;
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
    
    public void  CloseAllPanel()
    {
        foreach (GameObject panel in panels)
        {
            if (!panel.activeSelf) continue;
            if (panel.TryGetComponent(out ErrorMessage errorMessage))
                if (!errorMessage.IsOwner(this.gameObject))
                    return;
            if (panel.TryGetComponent(out FitterMessage fitterMessage))
                if (!fitterMessage.IsOwner(this.gameObject))
                    return;
            if (panel.TryGetComponent(out FitterMessage finishMessage))
                if (!finishMessage.IsOwner(this.gameObject))
                    return;   
            if (panel.TryGetComponent(out NetworkingMessage networkingMessage))
                if (!networkingMessage.IsOwner(this.gameObject))
                    return; 
        }

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    protected virtual void Initialize()
    {
        CloseAllPanel();
    }
    
    protected virtual void ReturnToParentPanel()
    {
        foreach (GameObject panel in panels)
        {
            if (!panel.activeSelf) continue;
            if (panel.TryGetComponent(out ErrorMessage errorMessage))
                if (!errorMessage.IsOwner(this.gameObject))
                    return;
            if (panel.TryGetComponent(out FitterMessage fitterMessage))
                if (!fitterMessage.IsOwner(this.gameObject))
                    return;
            if (panel.TryGetComponent(out FinishMessage finishMessage))
                if (!finishMessage.IsOwner(this.gameObject))
                    return;   
            if (panel.TryGetComponent(out NetworkingMessage networkingMessage))
                if (!networkingMessage.IsOwner(this.gameObject))
                    return; 
        }
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    
}

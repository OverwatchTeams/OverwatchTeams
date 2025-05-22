using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPanelController : PanelController
{
    [SerializeField] private DataController dataController;

    [SerializeField] private Button _mapButton;
    [SerializeField] private Button[] _playerButtons;
    [SerializeField] private Button[] _teamCrownButtons;
    [SerializeField] private Image[] _teamCrownIcons;
    [SerializeField] private Button _finishButton;
    [SerializeField] private Button _confirmMakeResultButton;

    private void Start()
    {
        InitializeButtons();
        Initialize();
    }
    
    private void OnEnable()
    {
        Initialize();
        dataController.Initialize();
    }

    private void Initialize()
    {
        CloseAllPanel();
        ResetButtons();
    }
    private void ResetButtons()
    {
        //모든 플레이어 리셋
        foreach (var button in _playerButtons)
        {
            button.GetComponentInChildren<TMP_Text>().text = "-";
        }

        //모든 팀 크라운 컬러 변경
        foreach (var icon in _teamCrownIcons)
        {
            Color color = icon.color;
            color.a = 0;
            icon.color = color;
        }
        //맵 리셋
        _mapButton.GetComponentInChildren<TMP_Text>().text = "-";
    }

    protected override void InitializeButtons()
    {
        base.InitializeButtons();
        foreach (var button in _playerButtons)
        {
            button.onClick.AddListener(PopupFindPlayerTab);
        }

        foreach (var button in _teamCrownButtons)
        {
            button.onClick.AddListener(delegate { ChangeCrownTeam(button);});
        }
        
        _mapButton.onClick.AddListener(PopupFindMapTab);
        _finishButton.onClick.AddListener(PopupFinish);
    }

    private void PopupFindPlayerTab()
    {
        OpenPanel("[Group] FindPlayer");
    }

    private void PopupFindMapTab()
    {
        OpenPanel("[Group] FindMap");
    }

    private void ChangeCrownTeam(Button button)
    {
        for (int i = 0; i < _teamCrownButtons.Length; i++)
        {
            if (button == _teamCrownButtons[i])
            {
                Color color = _teamCrownIcons[i].color;
                color.a = 1;
                _teamCrownIcons[i].color = color;
            }
            else
            {
                Color color = _teamCrownIcons[i].color;
                color.a = 0;
                _teamCrownIcons[i].color = color;
            }
        }
    }
    
    private void PopupFinish()
    {
        OpenPanel(_mapButton.GetComponentInChildren<TMP_Text>().text == "-"
            ? "[Popup] FinishError" : "[Popup] FinishCheck", false);
    }
    
    private void ReturnToGameResult()
    {
        CloseAllPanel();
    }
    
}

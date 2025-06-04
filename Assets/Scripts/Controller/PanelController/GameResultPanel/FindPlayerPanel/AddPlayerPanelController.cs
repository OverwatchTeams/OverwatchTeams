using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddPlayerPanelController : PanelController
{
    [SerializeField] private TMP_InputField _newPlayerName;
    
    [SerializeField] private TMP_Text _errorMessage;
    [SerializeField] private Button _addPlayerConfirmButton;
    [SerializeField] private Button _addPlayerConfirmConfirmButton;
    
    [SerializeField] private FindPlayerPanelController _findPlayerPanelController;
    
    #region initialization
    
    private void OnEnable()
    {
        Debug.Log("OnEnable AddPlayer");
        Initialize();
    }

    public void ReInitialize()
    {
        Initialize();
    }
    protected override void Initialize()
    {
        base.Initialize();
        
        //리스너 초기화
        InitializeListeners();
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        //중복 방지를 위해 이전 Listener 삭제
        _addPlayerConfirmButton.onClick.RemoveListener(OnClickAddPlayerConfirmButton);
        _addPlayerConfirmConfirmButton.onClick.RemoveListener(OnClickAddPlayerConfirmConfirmButton);
        //Listner 추가
        _addPlayerConfirmButton.onClick.AddListener(OnClickAddPlayerConfirmButton);
        _addPlayerConfirmConfirmButton.onClick.AddListener(OnClickAddPlayerConfirmConfirmButton);
    }
    #endregion
    
    private void OnClickAddPlayerConfirmConfirmButton()
    {
        PlayerData newPlayer = new PlayerData();
        newPlayer.player = _newPlayerName.text;
        newPlayer.scores = new PlayerData.Scores();
        StartCoroutine(DataController.instance.CreatePlayer(
            result => {
                if (result)
                {
                    _findPlayerPanelController.ReInitialize();
                    OpenPanel("[Popup] AddPlayerFinishMessage");
                }
                else
                    OpenPanel("[Popup] AddPlayerFailedMessage");
            }, newPlayer)
        );
        
        OpenPanel("[Popup] AddPlayerFinishMessage");
    }
    
    private void OnClickAddPlayerConfirmButton()
    {
        if (!CheckPlayerNameSavable())
        {
            OpenPanel("[Popup] AddPlayerErrorMessage");
        }
        else
        {
            OpenPanel("[Popup] AddPlayerConfirmMessage");   
        }
    }

    private bool CheckPlayerNameSavable()
    {
        if (string.IsNullOrWhiteSpace(_newPlayerName.text))
        {
            _errorMessage.text = "플레이어 이름을 기입해 주세요";
            return false;
        }
        foreach (var button in _findPlayerPanelController._playerButtons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == _newPlayerName.text)
            {
                _errorMessage.text = "이미 존재하는 플레이어 입니다.";
                return false;
            }
        }
        return true;
    }
}

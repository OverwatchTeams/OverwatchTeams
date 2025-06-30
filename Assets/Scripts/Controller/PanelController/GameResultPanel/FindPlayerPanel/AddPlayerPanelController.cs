using System;
using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddPlayerPanelController : PanelController
{
    [SerializeField] private TMP_InputField _newPlayerName;
    
    [SerializeField] private Button _addPlayerConfirmButton;
    [SerializeField] private Button _addPlayerConfirmConfirmButton;
    
    [SerializeField] private FindPlayerPanelController _findPlayerPanelController;
    
    #region initialization
    
    private void OnEnable()
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
        if (!FitterMessage.Instance.IsOwner(gameObject)) return;
        PlayerData newPlayer = new PlayerData();
        newPlayer.isClanMember = true;
        newPlayer.player = _newPlayerName.text;
        newPlayer.scores = new PlayerData.Scores();
        OpenPanel("[PopupPanel] NetworkingPopup");
        NetworkingMessage.Instance.SetOwner(this.gameObject);
        NetworkingMessage.Instance.SetDescription("저장 중 입니다.");
        StartCoroutine(DataController.instance.CreatePlayer(
            result => {
                NetworkingMessage.Instance.gameObject.SetActive(false);
                if (result)
                {
                    _findPlayerPanelController.ReInitialize();
                    OpenPanel("[PopupPanel] FinishPopup");
                    FinishMessage.Instance.SetOwner(this.gameObject);
                    FinishMessage.Instance.SetDescription("플레이어를 저장하였습니다.");
                }
                else
                {
                    OpenPanel("[PopupPanel] ErrorPopup");
                    ErrorMessage.Instance.SetOwner(this.gameObject);
                    ErrorMessage.Instance.SetDescription("통신 중 문제가 발생했습니다.");
                }
            }, newPlayer)
        );
    }
    
    private void OnClickAddPlayerConfirmButton()
    {
        if (string.IsNullOrWhiteSpace(_newPlayerName.text))
        {
            OpenPanel("[PopupPanel] ErrorPopup");
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("플레이어 이름을 기입해 주세요");
            return;
        }
        foreach (var button in _findPlayerPanelController._playerButtons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == _newPlayerName.text)
            {
                OpenPanel("[PopupPanel] ErrorPopup");
                ErrorMessage.Instance.SetOwner(this.gameObject);
                ErrorMessage.Instance.SetDescription("이미 존재하는 플레이어 입니다.");
                return;
            }
        }
        OpenPanel("[PopupPanel] FitterPopup");
        FitterMessage.Instance.SetOwner(this.gameObject);
        FitterMessage.Instance.SetDescription("이대로 저장하겠습니까?");
    }
}

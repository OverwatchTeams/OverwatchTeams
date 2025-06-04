using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class FindPlayerPanelController : PanelController
{
    private string _path;

    public Action<string> OnPlayerClicked;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TMP_Text _playerNameInputFieldText;
    [SerializeField] private Button _addPlayerButton;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerContainer;
    
    
    public List<GameObject> _playerButtons = new List<GameObject>();

    private string _prevText = "";
    
    #region initialization
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        Initialize();
    }

    public void ReInitialize()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        //패널 초기화
        InitializePanel();
        
        //맵버튼 생성 및 초기화
        InstantiatePlayerButtons();
        InitializeListeners();
    }

    private void InstantiatePlayerButtons()
    {
        for (int i = 0; i < DataController.instance.Players.Length; i++)
        {
            GameObject go = Instantiate(_playerPrefab, _playerContainer.transform, false);
            go.GetComponent<PlayerButtonPrefab>().SetPlayerData(DataController.instance.Players[i]);
            go.GetComponentInChildren<TMP_Text>().text = DataController.instance.Players[i].player;
            _playerButtons.Add(go);
        }
    }
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var playerButton in _playerButtons)
        {
            playerButton.GetComponentInChildren<Button>().onClick.RemoveListener(()=>OnClickPlayer(playerButton));
            playerButton.GetComponentInChildren<Button>().onClick.AddListener(()=>OnClickPlayer(playerButton));
        }
        _addPlayerButton.onClick.RemoveListener(OnClickAddPlayer);
        _addPlayerButton.onClick.AddListener(OnClickAddPlayer);
        
        /*_dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);*/
    }
    
    private void InitializePanel()
    {
        _playerNameInputFieldText.text = "";
        _playerButtons.Clear();
        foreach (Transform child in _playerContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        _dropdown.ClearOptions();
        _dropdown.options.Add(new TMP_Dropdown.OptionData("전체"));
    }

    public void FilterSelectedPlayer(Button[] buttons)
    {
        foreach (var button in buttons)
        {
            foreach (var playerButton in _playerButtons)
            {
                if (button.GetComponentInChildren<TMP_Text>().text ==
                    playerButton.GetComponentInChildren<TMP_Text>().text)
                {
                    playerButton.SetActive(false);
                }
            }
        }
    }
    #endregion

    /*private void OnDropdownValueChanged(int value)
    {
        if (_dropdown.options[value].text == "전체")
        {
            foreach (var playerButton in _playerButtons)
            {
                playerButton.SetActive(true);
            }
        }
        foreach (var playerButton in _playerButtons)
        {
            if (playerButton.GetComponent<PlayerButtonPrefab>().PlayerData.type == _dropdown.options[value].text)
            {
                playerButton.SetActive(true);
            }
            else
            {
                playerButton.SetActive(false);
            }
        }
    }*/

    #region 업데이트
    void Update()
    {
        OnTypingPlayerName();
    }
    #endregion
    
    #region 이벤트
    
    private void OnTypingPlayerName()
    {
        if (_playerNameInputField.isFocused)
        {
            string current = _playerNameInputFieldText.text;
            if (_prevText != current)
            {
                _prevText = current;
                
                StringBuilder sb = new StringBuilder(_playerNameInputFieldText.text);
                sb.Replace("<u>", "");
                sb.Replace("</u>", "");
                sb.Replace(" ", "");
                
                string value = sb.ToString();
                foreach (var playerButton in _playerButtons)
                {
                    string playerName = playerButton.GetComponentInChildren<TMP_Text>().text;
                    if (!string.IsNullOrWhiteSpace(playerName) && !string.IsNullOrWhiteSpace(value))
                    {
                        playerName = playerName.Trim().ToLowerInvariant();
                        value = value.Trim().ToLowerInvariant();
                        value = value.Replace("\u200B", "");
                        if (playerName.Contains(value))
                        {
                            playerButton.SetActive(true);
                        }
                        else
                        {
                            playerButton.SetActive(false);
                        }
                    }
                    if (value == "")
                    {
                        playerButton.SetActive(true);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 플레이어 선택 시
    /// </summary>
    private void OnClickPlayer(GameObject go)
    {
        OnPlayerClicked?.Invoke(go.GetComponentInChildren<TMP_Text>().text);
    }
    /// <summary>
    /// 플레이어 추가 시
    /// </summary>
    private void OnClickAddPlayer()
    {
        OpenPanel("[Popup] AddPlayer");
    }
    #endregion
}

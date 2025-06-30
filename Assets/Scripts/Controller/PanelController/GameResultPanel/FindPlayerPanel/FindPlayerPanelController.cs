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
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private bool _isClanMember = true;
    
    public List<GameObject> _playerButtons = new List<GameObject>();

    private string _prevText = "";
    
    #region initialization
    private void OnEnable()
    {
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
        
        //플레이어 버튼 생성 및 초기화
        InstantiatePlayerButtons();
        
        InitializeListeners();
    }

    private void InstantiatePlayerButtons()
    {
        PlayerData[] players;
        if (_isClanMember)
        {
            players = DataController.instance.ClanPlayers;
        }
        else
        {
            players = DataController.instance.AllPlayers;
        }

        for (int i = 0; i < players.Length; i++)
        {
            GameObject go = Instantiate(_playerPrefab, _playerContainer.transform, false);
            go.GetComponent<PlayerButtonPrefab>().SetPlayerData(players[i]);
            go.GetComponentInChildren<TMP_Text>().text = players[i].player;
            _playerButtons.Add(go);
        }
        _dropdown.value = 0;
        OnDropdownValueChanged(0);
        _dropdown.RefreshShownValue();
    }
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var playerButton in _playerButtons)
        {
            playerButton.GetComponentInChildren<Button>().onClick.RemoveListener(()=>OnClickPlayer(playerButton));
            playerButton.GetComponentInChildren<Button>().onClick.AddListener(()=>OnClickPlayer(playerButton));
        }

        if (_addPlayerButton != null)
        {
            _addPlayerButton.onClick.RemoveListener(OnClickAddPlayer);
            _addPlayerButton.onClick.AddListener(OnClickAddPlayer);   
        }
        
        _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
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
        _dropdown.options.Add(new TMP_Dropdown.OptionData("최근 참여 순"));
        _dropdown.options.Add(new TMP_Dropdown.OptionData("사전 순"));
        
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1f;
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

    private void OnDropdownValueChanged(int value)
    {
        if (_dropdown.options[value].text == "최근 참여 순")
        {
            _playerButtons.Sort((a, b) =>
            {
                var aData = a.GetComponent<PlayerButtonPrefab>().PlayerData.dates;
                var bData = b.GetComponent<PlayerButtonPrefab>().PlayerData.dates;

                if (aData == null && bData == null) return 0;
                if (aData == null) return 1;   // a가 null이면 뒤로
                if (bData == null) return -1;  // b가 null이면 a가 앞으로

                return bData.lastRound.CompareTo(aData.lastRound); // 최신 순 (내림차순)
            });
        }
        else if (_dropdown.options[value].text == "사전 순")
        {
            _playerButtons.Sort((a, b) => 
                string.Compare(a.GetComponent<PlayerButtonPrefab>().PlayerData.player, b.GetComponent<PlayerButtonPrefab>().PlayerData.player, StringComparison.Ordinal));
        }

        for (int i = 0; i < _playerButtons.Count; i++)
        {
            _playerButtons[i].transform.SetSiblingIndex(i);
        }
    }

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

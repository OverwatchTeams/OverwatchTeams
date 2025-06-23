using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDataStatisticPanel : PanelController
{
    [SerializeField] private GameObject[] _editModeGameObjects;
    [SerializeField] private GameObject _playerName;
    [SerializeField] private Toggle _isClanMemberIcon;
    [SerializeField] private GameObject _subNamePrefab;
    [SerializeField] private GameObject _subNamesContainer;

    [SerializeField] private TMP_InputField _addSubNameInput;
    [SerializeField] private Button _addSubNameButton;
    private List<Button> _subNameButtons;
    
    [SerializeField] private Button _editPlayerButton;
    [SerializeField] private Button _editFinishPlayerButton;
    
    [SerializeField] private FindPlayerPanelController _findPlayerPanelController;
    
    private PlayerData _playerData;
    
    
    private void OnEnable()
    {
        _findPlayerPanelController.OnPlayerClicked -= ChangePlayer;
        _findPlayerPanelController.OnPlayerClicked += ChangePlayer;
        Initialize();
    }

    private void OnDisable()
    {
        foreach (var go in _editModeGameObjects)
        {
            go.SetActive(false);
        }
        _isClanMemberIcon.interactable = false;
        _editPlayerButton.gameObject.SetActive(true);
        foreach (Button subNameButton in _subNameButtons)
        {
            subNameButton.interactable = false;
        }

        _playerData = null;
    }

    protected override void Initialize()
    {
        base.Initialize();
        InstantiatePlayerButtons();
        InitializeListeners();
    }
    
    private void InstantiatePlayerButtons()
    {
        _subNameButtons  = new List<Button>();
        for (int i = 0; i < _subNamesContainer.transform.childCount; i++)
        {
            Destroy(_subNamesContainer.transform.GetChild(i).gameObject);
        }
        if (_playerData == null)
        {
            _playerName.GetComponentInChildren<TMP_Text>().text = "-";
            _isClanMemberIcon.isOn = true;
        }
        else
        {
            if (_playerData.subNames != null)
            {
                for (int i = 0; i < _playerData.subNames.Count; i++)
                {
                    GameObject go = Instantiate(_subNamePrefab, _subNamesContainer.transform, false);
                    go.GetComponent<SubNameButtonPrefab>().ChangeSubNameText(_playerData.subNames[i]);
                    _subNameButtons.Add(go.GetComponent<Button>());
                }
            }   
            _playerName.GetComponentInChildren<TMP_Text>().text = _playerData.player;
            _isClanMemberIcon.SetIsOnWithoutNotify(_playerData.isClanMember);
        }
        _playerName.GetComponent<Button>().interactable = false;
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _editPlayerButton.onClick.RemoveListener(OnClickEditPlayerButton);
        _editFinishPlayerButton.onClick.RemoveListener(OnClickEditFinishPlayerButton);
        _isClanMemberIcon.onValueChanged.RemoveListener(OnClickIsClanMemberIconChanged);
        _addSubNameButton.onClick.RemoveListener(OnClickAddSubNameButton);
        _editPlayerButton.onClick.AddListener(OnClickEditPlayerButton);
        _editFinishPlayerButton.onClick.AddListener(OnClickEditFinishPlayerButton);
        _isClanMemberIcon.onValueChanged.AddListener(OnClickIsClanMemberIconChanged);
        _addSubNameButton.onClick.AddListener(OnClickAddSubNameButton);
        if (_playerData == null) return;
        if (_playerData.subNames != null)
        {
            foreach (Button subNameButton in _subNameButtons)
            {
                subNameButton.onClick.RemoveListener(OnClickSubNameButton);
                subNameButton.onClick.AddListener(OnClickSubNameButton);
            }   
        }
        
    }

    private void OnClickEditPlayerButton()
    {
        foreach (var go in _editModeGameObjects)
        {
            go.SetActive(true);
        }
        
        _isClanMemberIcon.interactable = true;

        foreach (Button subNameButton in _subNameButtons)
        {
            subNameButton.interactable = true;
        }
    }

    private void OnClickEditFinishPlayerButton()
    {
        foreach (var go in _editModeGameObjects)
        {
            go.SetActive(false);
        }
        _isClanMemberIcon.interactable = false;
        _editPlayerButton.gameObject.SetActive(true);
        foreach (Button subNameButton in _subNameButtons)
        {
            subNameButton.interactable = false;
        }
        StartCoroutine(UpdatePlayerInform());
    }

    private void OnClickAddSubNameButton()
    {
        _playerData.subNames.Add(_addSubNameInput.text);
        GameObject go = Instantiate(_subNamePrefab, _subNamesContainer.transform, false);
        go.GetComponent<SubNameButtonPrefab>().ChangeSubNameText(_playerData.subNames.Last());
        _subNameButtons.Add(go.GetComponent<Button>());
        go.GetComponent<Button>().onClick.AddListener(OnClickSubNameButton);
    }

    private IEnumerator UpdatePlayerInform()
    {
        yield return StartCoroutine(DataController.instance.CreatePlayer(
            result => {
                if (!result) OpenPanel("[Popup] UpdatePlayerErrorMessage");
            }, _playerData)
        );
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "false";
        playerURLData.fields = new List<string> { "player", "isClanMember", "subNames" };
        playerURLData.sortType = "recent";
        yield return StartCoroutine(DataController.instance.GetAllPlayerDatas(playerURLData));
        Initialize();
    }

    private void OnClickSubNameButton()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        _playerData.subNames.Remove(button.GetComponentInChildren<TMP_Text>().text);
        _subNameButtons.Remove(button.GetComponent<Button>());
    }

    private void OnClickIsClanMemberIconChanged(bool value)
    {
        if(value) _playerData.isClanMember = true;
        else _playerData.isClanMember = false;
    }

    private void ChangePlayer(string playerName)
    {
        foreach (var player in DataController.instance.AllPlayers)
        {
            if (player.player != playerName) continue;
            _playerData = player;
            break;
        }
        _editPlayerButton.gameObject.SetActive(true);
        Initialize();
    }
    
}

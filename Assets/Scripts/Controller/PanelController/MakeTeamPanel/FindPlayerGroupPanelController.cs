using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FindPlayerGroupPanelController : FindPlayerPanelController
{
    [SerializeField] private List<GameObject> _playerPool;
    public Action<string[]> OnPoolUpdated;
    
    [SerializeField] private Button _dailyRecordButton;
    [SerializeField] private DailyRecordPanelController _dailyRecordPanel;
    [SerializeField] private PredictionMatchManager _predictionMatchManager;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Toggle _isScoresViewable;

    private PlayerButtonPrefab _selectedButton01;
    private PlayerButtonPrefab _selectedButton02;
    private string _currentMap = "";
    private string _currentDay = "";

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        for (int index = 0; index < _playerPool.Count; index++)
        {
            var obj = _playerPool[index];
            obj.GetComponent<PlayerButtonPrefab>().OnRightClick -= OnClickUndoPlayer;
            obj.GetComponent<PlayerButtonPrefab>().OnRightClick += OnClickUndoPlayer;
            obj.GetComponent<Button>().onClick.RemoveListener(OnClickChangePlayer);
            obj.GetComponent<Button>().onClick.AddListener(OnClickChangePlayer);
            if (index == 0 || index == 1 || index == 2 || index == 3 || index == 4)
            {
                obj.gameObject.GetComponent<PlayerButtonPrefab>().SetBadgePosition(true);   
            }
            else if (index == 5 || index == 6 || index == 7 || index == 8 || index == 9)
            {
                obj.gameObject.GetComponent<PlayerButtonPrefab>().SetBadgePosition(false);   
            }
            _isScoresViewable.onValueChanged.RemoveListener(ScoresViewableToggleChanged);
            _isScoresViewable.onValueChanged.AddListener(ScoresViewableToggleChanged);
        }

        _dailyRecordButton.onClick.RemoveListener(OnClickDailyRecordButton);
        _dailyRecordButton.onClick.AddListener(OnClickDailyRecordButton);
        DataController.instance.OnCreatePlayerFininshed -= StartUpdatePlayerPool;
        DataController.instance.OnCreatePlayerFininshed += StartUpdatePlayerPool;
        Debug.Log("Enable StartUpdatePlayerPool");
        _resetButton.onClick.RemoveListener(OnClickResetButton);
        _resetButton.onClick.AddListener(OnClickResetButton);
        if (UserData.instance.GetUserPermission() == Permission.ClanAdmin)
        {
            foreach (var player in _playerPool)
            {
                player.GetComponent<PlayerButtonPrefab>().SetInputfieldInteract(false);
            }
        }
    }

    private void OnDisable()
    {
        DataController.instance.OnCreatePlayerFininshed -= StartUpdatePlayerPool;
    }

    public void InitializePlayerPool(GameObject[] players, string map, string day)
    {
        _selectedButton01 = null;
        _selectedButton02 = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponentInChildren<TMP_Text>().text == "-")
            {
                _playerPool[i].gameObject.GetComponent<PlayerButtonPrefab>().SetPlayerData(new PlayerData
                    { player = players[i].GetComponentInChildren<TMP_Text>().text });
            }
            else
            {
                foreach (var player in DataController.instance.ClanPlayers)
                {
                    if (player.player == players[i].GetComponentInChildren<TMP_Text>().text)
                        _playerPool[i].gameObject.GetComponent<PlayerButtonPrefab>().SetPlayerData(player);
                }
            }
        }
        FilterSelectedPlayer(players);
        _currentMap = map;
        _currentDay = day;
        PlayerButtonPrefab[] playerDatas = _playerPool.Select(p => p.GetComponent<PlayerButtonPrefab>()).ToArray();
        _predictionMatchManager.UpdatePrediction(playerDatas, _currentMap, _currentDay);
    }
    
    protected override void OnClickPlayer(GameObject go)
    {
        //선택한 PlayerData를 변수로 저장
        PlayerData playerData = go.GetComponent<PlayerButtonPrefab>().PlayerData;
        //현재 플레이어 풀을 배열로 변환
        Button[] buttons = _playerPool
            .Select(obj => obj.GetComponentInChildren<Button>())
            .Where(button => button != null) // null 필터링
            .ToArray();
        //플레이어 풀을 처음부터 순차적으로 확인하며 "-"인 경우 PlayerData를 수정
        foreach (var player in _playerPool)
        {
            var text = player.GetComponent<PlayerButtonPrefab>().PlayerData.player;
            if (text != "-") continue;
            player.GetComponent<PlayerButtonPrefab>().SetPlayerData(playerData);
            //플레이어 풀이 수정되었으니 업데이트 함
            StartUpdatePlayerPool();
            return;
        }
        //플레이어 풀이 모두 꽉찬 경우 아무런 변경 없음 //차후 오류 메시지 정도는 출력
    }

    private void OnClickUndoPlayer(GameObject go)
    {
        //가져온 버튼의 PlayerData를 초기화 함
        go.GetComponent<PlayerButtonPrefab>().SetPlayerData(new PlayerData { player = "-" });
        //플레이어 풀이 수정되었으니 업데이트 함
        StartUpdatePlayerPool();
    }

    private void OnClickChangePlayer()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        //가져온 버튼의 PlayerData를 초기화 함
        if (_selectedButton01 == null)
        {
            _selectedButton01 = button.GetComponent<PlayerButtonPrefab>();
            _selectedButton01.SelectMask.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_selectedButton01.gameObject);
        }
        else if (_selectedButton02 == null)
        {
            PlayerData tempData;
            _selectedButton02 = button.GetComponent<PlayerButtonPrefab>();
            _selectedButton01.SelectMask.SetActive(false);
            tempData = _selectedButton01.PlayerData;
            _selectedButton01.SetPlayerData(_selectedButton02.PlayerData);
            _selectedButton02.SetPlayerData(tempData);
            EventSystem.current.SetSelectedGameObject(null);
            _selectedButton01 = null;
            _selectedButton02 = null;
            //현재 플레이어 풀을 배열로 변환
            //플레이어 풀이 수정되었으니 업데이트 함
            StartUpdatePlayerPool();
        }
        
    }

    private void OnClickResetButton()
    {
        _selectedButton01 = null;
        _selectedButton02 = null;
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var player in _playerPool)
        {
            player.gameObject.GetComponent<PlayerButtonPrefab>().SetPlayerData(new PlayerData
                { player = "-" });
        }
        
        StartUpdatePlayerPool();
    }

    private void ScoresViewableToggleChanged(bool value)
    {
        foreach (var player in _playerPool)
        {
            player.GetComponent<PlayerButtonPrefab>().SetScoreMask(value);
        }
        _predictionMatchManager.SetScoreMask(value);
    }
    
    public void StartUpdatePlayerPool()
    {
        StartCoroutine(UpdatePlayerPoolRoutine());
    }
    
    private IEnumerator UpdatePlayerPoolRoutine()
    {
        ReInitialize(); // 초기화는 한 번에

        // 프레임 분산 필터
        FilterSelectedPlayer(_playerPool.Select(b => b.gameObject).ToArray());
        yield return null;

        // 이름 추출 (10개씩 나눠서)
        List<string> names = new List<string>();
        for (int i = 0; i < _playerPool.Count; i++)
        {
            names.Add(_playerPool[i].GetComponent<PlayerButtonPrefab>().PlayerData.player);
            if (i % 10 == 0)
                yield return null;
        }

        OnPoolUpdated.Invoke(names.ToArray());
        yield return null;

        InitializePlayerPool(_playerPool.ToArray(), _currentMap, _currentDay);
    }
    
    private void OnClickDailyRecordButton()
    {
        string[] players = _playerPool
            .Select(btn => btn.GetComponentInChildren<TMP_Text>().text)
            .ToArray();
        OpenPanel("[PopupPanel] DailyRecord");
        _dailyRecordPanel.SetDailyMatchSupporter(players);
    }
}

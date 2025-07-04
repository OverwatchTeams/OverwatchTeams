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
    [SerializeField] private List<Button> _playerPool;
    public Action<string[]> OnPoolUpdated;

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var button in _playerPool)
        {
            button.onClick.RemoveListener(OnClickUndoPlayer);
            button.onClick.AddListener(OnClickUndoPlayer);
        }
    }

    public void InitializePlayerPool(Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            _playerPool[i].gameObject.GetComponent<PlayerButtonPrefab>().SetPlayerData(new PlayerData
                { player = buttons[i].GetComponentInChildren<TMP_Text>().text });
        }
        FilterSelectedPlayer(buttons);
    }
    
    protected override void OnClickPlayer(GameObject go)
    {
        //선택한 PlayerData를 변수로 저장
        PlayerData playerData = go.GetComponent<PlayerButtonPrefab>().PlayerData;
        //현재 플레이어 풀을 배열로 변환
        Button[] buttons = _playerPool.ToArray();
        //플레이어 풀을 처음부터 순차적으로 확인하며 "-"인 경우 PlayerData를 수정
        foreach (var button in buttons)
        {
            var text = button.GetComponent<PlayerButtonPrefab>().PlayerData.player;
            if (text != "-") continue;
            button.GetComponent<PlayerButtonPrefab>().SetPlayerData(playerData);
            //플레이어 풀이 수정되었으니 업데이트 함
            UpdatePlayerPool(buttons);
            return;
        }
        //플레이어 풀이 모두 꽉찬 경우 아무런 변경 없음 //차후 오류 메시지 정도는 출력
    }

    private void OnClickUndoPlayer()
    {
        //선택한 버튼을 가져옴(PlayerPool)
        GameObject button = EventSystem.current.currentSelectedGameObject;
        //가져온 버튼의 PlayerData를 초기화 함
        button.GetComponent<PlayerButtonPrefab>().SetPlayerData(new PlayerData { player = "-" });
        //현재 플레이어 풀을 배열로 변환
        Button[] buttons = _playerPool.ToArray();
        //플레이어 풀이 수정되었으니 업데이트 함
        UpdatePlayerPool(buttons);
    }

    private void UpdatePlayerPool(Button[] buttons)
    {
        //FinPlayerGroupPanelController
        ReInitialize();
        FilterSelectedPlayer(buttons);
        string[] names = buttons
            .Select(btn => btn.gameObject.GetComponent<PlayerButtonPrefab>().PlayerData.player)
            .ToArray();
        OnPoolUpdated.Invoke(names);
    }
}

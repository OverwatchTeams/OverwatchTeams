using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminStatisticPanelController : PanelController
{
    [SerializeField] private Toggle _roleToggle;
    [SerializeField] private Toggle _gameDataToggle;
    [SerializeField] private Toggle _playerDataToggle;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        // 모든 토글 비활성화 후
        _gameDataToggle.SetIsOnWithoutNotify(false);
        _playerDataToggle.SetIsOnWithoutNotify(false);

        // Role Toggle을 true로 설정
        _roleToggle.SetIsOnWithoutNotify(true);
        OnRoleToggleValueChanged(true); // 직접 호출
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _roleToggle.onValueChanged.RemoveListener(OnRoleToggleValueChanged);
        _gameDataToggle.onValueChanged.RemoveListener(OnGameDataToggleValueChanged);
        _playerDataToggle.onValueChanged.RemoveListener(OnPlayerDataToggleValueChanged);
        _roleToggle.onValueChanged.AddListener(OnRoleToggleValueChanged);
        _gameDataToggle.onValueChanged.AddListener(OnGameDataToggleValueChanged);
        _playerDataToggle.onValueChanged.AddListener(OnPlayerDataToggleValueChanged);
    }

    private void OnRoleToggleValueChanged(bool value)
    {
        Debug.Log($"after : {_roleToggle.isOn}");
        if (!value) return;
        RoleStatisticPanelController roleStatisticPanelController = OpenPanel("[Panel] RoleStatisticPanel").GetComponent<RoleStatisticPanelController>();
    }

    private void OnGameDataToggleValueChanged(bool value)
    {
        if (value) OpenPanel("[Panel] GameDataStatisticPanel");
    }

    private void OnPlayerDataToggleValueChanged(bool value)
    {
        if (value) StartCoroutine(PlayerDataSetting());
    }

    private IEnumerator PlayerDataSetting()
    {
        PlayerURLData playerURLData = new PlayerURLData();
        playerURLData.isClanMember = "false";
        playerURLData.fields = new List<string> { "player", "isClanMember", "subNames" };
        playerURLData.sortType = "recent";
        yield return StartCoroutine(DataController.instance.GetAllPlayerDatas(playerURLData));
        OpenPanel("[Panel] PlayerDataStatisticPanel");
    }
}

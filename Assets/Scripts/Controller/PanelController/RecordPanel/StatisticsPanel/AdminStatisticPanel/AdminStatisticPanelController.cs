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
        OnRoleToggleValueChanged(true);
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
        if (value)
        {
            RoleStatisticPanelController roleStatisticPanelController = OpenPanel("[Panel] RoleStatisticPanel").GetComponent<RoleStatisticPanelController>();
            
        }
    }

    private void OnGameDataToggleValueChanged(bool value)
    {
        if (value)
        {
            OpenPanel("[Panel] GameDataStatisticPanel");
        }
    }

    private void OnPlayerDataToggleValueChanged(bool value)
    {
        if (value)
        {
            OpenPanel("[Panel] PlayerDataStatisticPanel");
        }
    }
}

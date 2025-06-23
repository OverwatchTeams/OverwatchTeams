using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanelController : PanelController
{
    [SerializeField] private Toggle _yearlyToggle;
    [SerializeField] private Toggle _monthlyToggle;
    [SerializeField] private Toggle _adminToggle;
    
    private void OnEnable()
    {
        Initialize();
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        _monthlyToggle.SetIsOnWithoutNotify(false);
        _adminToggle.SetIsOnWithoutNotify(false);
        _yearlyToggle.SetIsOnWithoutNotify(true);
        OnYearlyToggleValueChanged(_yearlyToggle.isOn);
    }
    
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _yearlyToggle.onValueChanged.RemoveListener(OnYearlyToggleValueChanged);
        _monthlyToggle.onValueChanged.RemoveListener(OnMonthlyToggleValueChanged);
        _adminToggle.onValueChanged.RemoveListener(OnAdminToggleValueChanged);
        _yearlyToggle.onValueChanged.AddListener(OnYearlyToggleValueChanged);
        _monthlyToggle.onValueChanged.AddListener(OnMonthlyToggleValueChanged);
        _adminToggle.onValueChanged.AddListener(OnAdminToggleValueChanged);
    }
    
    private void OnYearlyToggleValueChanged(bool value)
    {
        if (value)
        {
            DateStatisticPanelController dateStatisticPanelController = OpenPanel("[Group] DateStatisticPanelGroup").GetComponent<DateStatisticPanelController>();
            dateStatisticPanelController.ChangeDateSetting("year");
        }
    }

    private void OnMonthlyToggleValueChanged(bool value)
    {
        if (value)
        {
            DateStatisticPanelController dateStatisticPanelController = OpenPanel("[Group] DateStatisticPanelGroup").GetComponent<DateStatisticPanelController>();
            dateStatisticPanelController.ChangeDateSetting("month");
        }
    }
    
    private void OnAdminToggleValueChanged(bool value)
    {
        if (value)
        {
            AdminStatisticPanelController adminStatisticPanelController = OpenPanel("[Group] AdminStatisticPanelGroup", true).GetComponent<AdminStatisticPanelController>();
        }
    }
}

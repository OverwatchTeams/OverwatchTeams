using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddMapPanelController : PanelController
{
    [SerializeField] private TMP_Text _newMapName;
    [SerializeField] private TMP_Dropdown _mapType;

    [SerializeField] private Button _addMapConfirmButton;
    [SerializeField] private Button _addMapConfirmConfirmButton;
    
    #region initialization
    
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        Initialize();
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        //패널 초기화
        InitializePanel();
        
        //리스너 초기화
        InitializeListeners();
    }
    
    private void InitializePanel()
    {
        _newMapName.text = "";
        foreach (var type in DataController.instance.MapTypes)
        {
            string typeName = type.name;
            _mapType.options.Add(new TMP_Dropdown.OptionData(typeName));
        }
        _mapType.options.Add(new TMP_Dropdown.OptionData("+"));
    }
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        //중복 방지를 위해 이전 Listener 삭제
        _addMapConfirmButton.onClick.RemoveListener(OnClickAddMapConfirmButton);
        _addMapConfirmConfirmButton.onClick.RemoveListener(OnClickAddMapConfirmConfirmButton);
        _mapType.onValueChanged.RemoveListener(OnClickAddTypeButton);
        //Listner 추가
        _addMapConfirmButton.onClick.AddListener(OnClickAddMapConfirmButton);
        _addMapConfirmConfirmButton.onClick.AddListener(OnClickAddMapConfirmConfirmButton);
        _mapType.onValueChanged.AddListener(OnClickAddTypeButton);
    }
    #endregion
    
    private void OnClickAddMapConfirmButton()
    {
        OpenPanel("[Popup] AddMapConfirmMessage");
    }
    
    private void OnClickAddMapConfirmConfirmButton()
    {
        MapData newMap = new MapData();
        newMap.index = DataController.instance.Maps.Length;
        newMap.name = _newMapName.text;
        newMap.type = _mapType.options[_mapType.value].text;

        StartCoroutine(DataController.instance.CreateMap(
            result => {
                if (result)
                    OpenPanel("[Popup] AddMapFinishMessage");
                else
                    OpenPanel("[Popup] AddMapFailedMessage");
            }, newMap)
        );
    }
    
    
    /// <summary>
    /// 맵 타입 추가 시
    /// </summary>
    /// <param name="value"></param>
    private void OnClickAddTypeButton(int value)
    {
        if (value == _mapType.options.Count - 1)
        {
            OpenPanel("[Popup] AddMapType", false);
        }
    }

}

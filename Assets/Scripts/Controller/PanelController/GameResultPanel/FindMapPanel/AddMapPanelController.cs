using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddMapPanelController : PanelController
{
    [SerializeField] private TMP_InputField _newMapName;
    [SerializeField] private TMP_Dropdown _mapType;
    
    [SerializeField] private TMP_Text _errorMessage;
    [SerializeField] private Button _addMapConfirmButton;
    [SerializeField] private Button _addMapConfirmConfirmButton;
    
    [SerializeField] private FindMapPanelController _findMapPanelController;
    
    #region initialization
    
    private void OnEnable()
    {
        Debug.Log("OnEnable AddMap");
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
        
        //리스너 초기화
        InitializeListeners();
    }
    
    private void InitializePanel()
    {
        _mapType.ClearOptions();
        _newMapName.text = string.Empty;
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
    
    private void OnClickAddMapConfirmConfirmButton()
    {
        MapData newMap = new MapData();
        newMap.index = DataController.instance.Maps.Length;
        newMap.name = _newMapName.text;
        newMap.type = _mapType.options[_mapType.value].text;

        StartCoroutine(DataController.instance.CreateMap(
            result => {
                if (result)
                {
                    _findMapPanelController.ReInitialize();
                    OpenPanel("[Popup] AddMapFinishMessage");
                }
                else
                    OpenPanel("[Popup] AddMapFailedMessage");
            }, newMap)
        );
    }
    
    private void OnClickAddMapConfirmButton()
    {
        if (!CheckMapNameSavable())
        {
            _errorMessage.text = "맵 이름을 기입해 주세요";
            OpenPanel("[Popup] AddMapErrorMessage");
        }
        else
        {
            OpenPanel("[Popup] AddMapConfirmMessage");   
        }
    }

    private bool CheckMapNameSavable()
    {
        if (string.IsNullOrWhiteSpace(_newMapName.text))
            return false;
        return true;
    }
    
    /// <summary>
    /// 맵 타입 추가버튼을 눌렀을 시
    /// </summary>
    /// <param name="value"></param>
    private void OnClickAddTypeButton(int value)
    {
        if (value == _mapType.options.Count - 1)
        {
            OpenPanel("[Popup] AddMapType", false);
            _mapType.value = 0;
        }
    }
    
    

}

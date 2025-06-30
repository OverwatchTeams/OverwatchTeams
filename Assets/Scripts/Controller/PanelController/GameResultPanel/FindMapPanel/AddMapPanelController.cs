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
        if (!FitterMessage.Instance.IsOwner(gameObject)) return;
        MapData newMap = new MapData();
        newMap.index = DataController.instance.Maps.Length;
        newMap.name = _newMapName.text;
        newMap.type = _mapType.options[_mapType.value].text;

        OpenPanel("[PopupPanel] NetworkingPopup");
        NetworkingMessage.Instance.SetOwner(this.gameObject);
        NetworkingMessage.Instance.SetDescription("저장 중 입니다.");
        StartCoroutine(DataController.instance.CreateMap(
            result => {
                NetworkingMessage.Instance.gameObject.SetActive(false);
                if (result)
                {
                    _findMapPanelController.ReInitialize();
                    OpenPanel("[PopupPanel] FinishPopup");
                    FinishMessage.Instance.SetOwner(this.gameObject);
                    FinishMessage.Instance.SetDescription("맵을 저장하였습니다.");
                }
                else
                {
                    OpenPanel("[PopupPanel] ErrorPopup");
                    ErrorMessage.Instance.SetOwner(this.gameObject);
                    ErrorMessage.Instance.SetDescription("통신 중 문제가 발생했습니다.");   
                }
            }, newMap)
        );
    }
    
    private void OnClickAddMapConfirmButton()
    {
        if (!string.IsNullOrWhiteSpace(_newMapName.text))
        {
            OpenPanel("[PopupPanel] ErrorPopup");
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("맵 이름을 기입해주세요");
        }
        else
        {
            OpenPanel("[PopupPanel] FitterPopup");
            FitterMessage.Instance.SetOwner(this.gameObject);
            FitterMessage.Instance.SetDescription("이대로 저장하겠습니까?");
        }
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

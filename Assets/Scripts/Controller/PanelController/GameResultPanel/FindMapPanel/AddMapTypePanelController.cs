using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AddMapTypePanelController : PanelController
{
    [SerializeField] private TMP_InputField _newMapTypeInputField;
    [SerializeField] private Toggle _isAttackDefenseTypeToggle;

    [SerializeField] private TMP_Text _errorMessage;
    [SerializeField] private Button _addMapTypeConfirmButton;
    [SerializeField] private Button _confirmAddNewMapTypeButton;

    [SerializeField] private AddMapPanelController _addMapPanelController;
    private void OnEnable()
    {
        Debug.Log("OnEnable AddMapType");
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializePanel();
        InitializeListeners();
    }
    
    private void InitializePanel()
    {
        _newMapTypeInputField.text = string.Empty;
        _isAttackDefenseTypeToggle.isOn = false;
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        //중복 방지를 위해 이전 Listener 삭제
        _addMapTypeConfirmButton.onClick.RemoveListener(OnClickAddMapTypeConfirmButton);
        _confirmAddNewMapTypeButton.onClick.RemoveListener(CreateMapType);
        //Listner 추가
        _addMapTypeConfirmButton.onClick.AddListener(OnClickAddMapTypeConfirmButton);
        _confirmAddNewMapTypeButton.onClick.AddListener(CreateMapType);
    }

    private void CreateMapType()
    {
        MapTypeData newMapType = new MapTypeData();
        newMapType.index = DataController.instance.MapTypes.Length;
        newMapType.name = _newMapTypeInputField.text;
        newMapType.isAtkDef = _isAttackDefenseTypeToggle.isOn;
        StartCoroutine(DataController.instance.CreateMapType(
            result => {
                if (result)
                {
                    _addMapPanelController.ReInitialize();
                    OpenPanel("[Popup] AddMapTypeFinishMessage");
                }
                else
                    OpenPanel("[Popup] AddMapTypeFailedMessage");
            }, newMapType)
        );
    }
    
    private void OnClickAddMapTypeConfirmButton()
    {
        Debug.Log("OnClickAddMapTypeConfirmButton");
        if (!CheckMapTypeNameSavable())
        {
            _errorMessage.text = "맵타입 이름을 기입해 주세요";
            OpenPanel("[Popup] AddMapTypeErrorMessage");
        }
        else
        {
            OpenPanel("[Popup] AddMapTypeConfirmMessage");   
        }
    }
    
    private bool CheckMapTypeNameSavable()
    {
        if (string.IsNullOrWhiteSpace(_newMapTypeInputField.text))
            return false;
        return true;
    }
}

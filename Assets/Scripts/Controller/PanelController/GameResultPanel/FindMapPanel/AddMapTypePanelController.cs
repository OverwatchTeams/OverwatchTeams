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
    
    [SerializeField] private Button _addMapTypeConfirmButton;
    [SerializeField] private Button _confirmAddNewMapTypeButton;

    [SerializeField] private AddMapPanelController _addMapPanelController;
    private void OnEnable()
    {
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
        
        OpenPanel("[PopupPanel] NetworkingPopup");
        NetworkingMessage.Instance.SetOwner(this.gameObject);
        NetworkingMessage.Instance.SetDescription("저장 중 입니다.");
        StartCoroutine(DataController.instance.CreateMapType(
            result => {
                NetworkingMessage.Instance.gameObject.SetActive(false);
                if (result)
                {
                    _addMapPanelController.ReInitialize();
                    OpenPanel("[PopupPanel] FinishPopup");
                    FinishMessage.Instance.SetOwner(this.gameObject);
                    FinishMessage.Instance.SetDescription("맵타입을 저장하였습니다.");
                }
                else
                {
                    OpenPanel("[PopupPanel] ErrorPopup");
                    ErrorMessage.Instance.SetOwner(this.gameObject);
                    ErrorMessage.Instance.SetDescription("통신 중 문제가 발생했습니다.");
                }
            }, newMapType)
        );
    }
    
    private void OnClickAddMapTypeConfirmButton()
    {
        Debug.Log("OnClickAddMapTypeConfirmButton");
        if (!CheckMapTypeNameSavable())
        {
            OpenPanel("[PopupPanel] ErrorPopup");
            ErrorMessage.Instance.SetOwner(this.gameObject);
            ErrorMessage.Instance.SetDescription("맵 타입 이름을 기입해주세요");
        }
        else
        {
            OpenPanel("[PopupPanel] FitterPopup");
            FitterMessage.Instance.SetOwner(this.gameObject);
            FitterMessage.Instance.SetDescription("이대로 저장하겠습니까?"); 
        }
    }
    
    private bool CheckMapTypeNameSavable()
    {
        if (string.IsNullOrWhiteSpace(_newMapTypeInputField.text))
            return false;
        return true;
    }
}

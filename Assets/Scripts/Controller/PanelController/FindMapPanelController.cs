using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindMapPanelController : PanelController
{
    private string _path;

    public Action<string> OnMapClicked;
    [SerializeField] private TMP_Text _mapNameInputField;
    [SerializeField] private Button _addMapButton;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _mapPrefab;
    [SerializeField] private GameObject _mapContainer;

    [SerializeField] private TMP_Text _newMapName;
    [SerializeField] private TMP_Dropdown _newMapType;

    [SerializeField] private TMP_Text _newMapTypeName;
    [SerializeField] private Toggle _isAttackDefenseTypeToggle;
    
    private List<GameObject> _maps = new List<GameObject>();

    private void OnEnable()
    {
        if (DataManager.instance.isLoadDone)
        {
            Initialize();
        }
        else
        {
            throw new Exception("맵 정보가 동기화 되지 않았습니다.");
        }
        _newMapType.onValueChanged.AddListener(OnClickAddTypeButton);
    }
    
    private void Initialize()
    {
        CloseAllPanel();
        MakeMapObjectButtons();
        InitializeButtons();
        ResetAddMap();
    }
    private void MakeMapObjectButtons()
    {
        for (int i = 0; i < DataManager.instance.MapList.Count; i++)
        {
            GameObject go = Instantiate(_mapPrefab, _mapContainer.transform, false);
            go.GetComponentInChildren<TMP_Text>().text = DataManager.instance.MapList[i].MapName;
            _maps.Add(go);
        }
    }

    private void ResetAddMap()
    {
        _newMapName.text = "";
        foreach (var type in DataManager.instance.MapTypeList)
        {
            string typeName = type.TypeName;
            _newMapType.options.Add(new TMP_Dropdown.OptionData(typeName));
        }
        _newMapType.options.Add(new TMP_Dropdown.OptionData("+"));
    }
    
    protected override void InitializeButtons()
    {
        base.InitializeButtons();
        foreach (var map in _maps)
        {
            map.GetComponentInChildren<Button>().onClick.AddListener(()=>OnClickMap(map));
        }
        _addMapButton.onClick.AddListener(OnClickAddMap);
    }

    /// <summary>
    /// 맵 선택 시
    /// </summary>
    private void OnClickMap(GameObject go)
    {
        OnMapClicked?.Invoke(go.GetComponentInChildren<TMP_Text>().text);
    }
    /// <summary>
    /// 맵 추가 시
    /// </summary>
    private void OnClickAddMap()
    {
        OpenPanel("[Popup] AddMap");
    }
    
    /// <summary>
    /// 맵 타입 추가 시
    /// </summary>
    /// <param name="value"></param>
    private void OnClickAddTypeButton(int value)
    {
        if (value == _newMapType.options.Count - 1)
        {
            OpenPanel("[Popup] AddMapType", false);
        }
    }


}

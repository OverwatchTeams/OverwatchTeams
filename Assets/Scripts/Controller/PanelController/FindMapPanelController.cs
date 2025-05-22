using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindMapPanelController : PanelController
{
    private string _path;

    public Action<string> OnMapClicked;
    [SerializeField] private TMP_InputField _mapNameInputField;
    [SerializeField] private TMP_Text _mapNameInputFieldText;
    [SerializeField] private Button _addMapButton;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _mapPrefab;
    [SerializeField] private GameObject _mapContainer;

    [SerializeField] private TMP_Text _newMapName;
    [SerializeField] private TMP_Dropdown _newMapType;

    [SerializeField] private TMP_Text _newMapTypeName;
    [SerializeField] private Toggle _isAttackDefenseTypeToggle;
    
    private List<GameObject> _maps = new List<GameObject>();

    private string _prevText = "";
    
    void Update()
    {
        if (_mapNameInputField.isFocused)
        {
            string current = _mapNameInputFieldText.text;
            if (_prevText != current)
            {
                _prevText = current;

                Debug.Log("Changed");
                StringBuilder sb = new StringBuilder(_mapNameInputFieldText.text);
                sb.Replace("<u>", "");
                sb.Replace("</u>", "");
                sb.Replace(" ", "");
                // TMP_InputField에 강제로 자모를 추가하고 싶다면
                OnTypingMapName(sb.ToString());
            }

        }
    }
    
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
        _mapNameInputField.onValueChanged.AddListener(OnTypingMapName);
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

    private void OnTypingMapName(string value)
    {
        foreach (var map in _maps)
        {
            string mapName = map.GetComponentInChildren<TMP_Text>().text;
            if (!string.IsNullOrWhiteSpace(mapName) && !string.IsNullOrWhiteSpace(value))
            {
                mapName = mapName.Trim().ToLowerInvariant();
                value = value.Trim().ToLowerInvariant();
                value = value.Replace("\u200B", "");
                if (mapName.Contains(value))
                {
                    map.SetActive(true);
                }
                else
                {
                    map.SetActive(false);
                }
            }
            if (value == "")
            {
                map.SetActive(true);
            }
        }
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

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
    
    
    private List<GameObject> _mapButtons = new List<GameObject>();

    private string _prevText = "";
    
    #region initialization
    private void OnEnable()
    {
        Debug.Log("OnEnable");
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
        
        //맵버튼 생성 및 초기화
        InstantiateMapButtons();
        InitializeListeners();
    }

    private void InstantiateMapButtons()
    {
        for (int i = 0; i < DataController.instance.Maps.Length; i++)
        {
            GameObject go = Instantiate(_mapPrefab, _mapContainer.transform, false);
            go.GetComponent<MapButtonPrefab>().SetMapData(DataController.instance.Maps[i]);
            go.GetComponentInChildren<TMP_Text>().text = DataController.instance.Maps[i].name;
            _mapButtons.Add(go);
        }
    }
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        foreach (var mapButton in _mapButtons)
        {
            mapButton.GetComponentInChildren<Button>().onClick.RemoveListener(()=>OnClickMap(mapButton));
            mapButton.GetComponentInChildren<Button>().onClick.AddListener(()=>OnClickMap(mapButton));
        }
        _addMapButton.onClick.RemoveListener(OnClickAddMap);
        _addMapButton.onClick.AddListener(OnClickAddMap);
        
        _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    
    private void InitializePanel()
    {
        _mapNameInputFieldText.text = "";
        _mapButtons.Clear();
        foreach (Transform child in _mapContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        _dropdown.ClearOptions();
        _dropdown.options.Add(new TMP_Dropdown.OptionData("전체"));
        foreach (var type in DataController.instance.MapTypes)
        {
            string typeName = type.name;
            _dropdown.options.Add(new TMP_Dropdown.OptionData(typeName));
        }
    }
    #endregion

    private void OnDropdownValueChanged(int value)
    {
        if (_dropdown.options[value].text == "전체")
        {
            foreach (var mapButton in _mapButtons)
            {
                mapButton.SetActive(true);
            }
        }
        else
        {
            foreach (var mapButton in _mapButtons)
            {
                if (mapButton.GetComponent<MapButtonPrefab>().MapData.type == _dropdown.options[value].text)
                {
                    mapButton.SetActive(true);
                }
                else
                {
                    mapButton.SetActive(false);
                }
            }   
        }
    }

    #region 업데이트
    void Update()
    {
        OnTypingMapName();
    }
    #endregion
    
    #region 이벤트
    
    private void OnTypingMapName()
    {
        if (_mapNameInputField.isFocused)
        {
            string current = _mapNameInputFieldText.text;
            if (_prevText != current)
            {
                _prevText = current;
                
                StringBuilder sb = new StringBuilder(_mapNameInputFieldText.text);
                sb.Replace("<u>", "");
                sb.Replace("</u>", "");
                sb.Replace(" ", "");
                
                string value = sb.ToString();
                foreach (var mapButton in _mapButtons)
                {
                    string mapName = mapButton.GetComponentInChildren<TMP_Text>().text;
                    if (!string.IsNullOrWhiteSpace(mapName) && !string.IsNullOrWhiteSpace(value))
                    {
                        mapName = mapName.Trim().ToLowerInvariant();
                        value = value.Trim().ToLowerInvariant();
                        value = value.Replace("\u200B", "");
                        if (mapName.Contains(value))
                        {
                            mapButton.SetActive(true);
                        }
                        else
                        {
                            mapButton.SetActive(false);
                        }
                    }
                    if (value == "")
                    {
                        mapButton.SetActive(true);
                    }
                }
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
    
    #endregion

}

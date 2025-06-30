using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateStatisticPanelController : PanelController
{
    [SerializeField] TMP_Dropdown _dateDropdown;
    [SerializeField] private GameObject _mapSelectedContainer;
    [SerializeField] private GameObject _mapSelectedPrefab;
    [SerializeField] private TMP_Text _totalRoundCount;
    [SerializeField] private TMP_Text _requireRoundCount;
    [SerializeField] private ScrollRect _scrollRect;
    private string _category = "year";
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _dateDropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
        _dateDropdown.onValueChanged.AddListener(OnDropDownValueChanged);
    }

    public void ChangeDateSetting(string category)
    {
        _category = category;
        StartCoroutine(InitializeContents());
    }
    
    private IEnumerator InitializeContents()
    {
        yield return StartCoroutine(SetDropdown());
        string date = _dateDropdown.options[_dateDropdown.value].text;
        OnDropDownValueChanged(_dateDropdown.value);
    }
    
    private IEnumerator SetDropdown()
    {
        _dateDropdown.ClearOptions();
        switch (_category)
        {
            case "year":
                foreach (var year in DataController.instance.RecordDropdownDates.year)
                {
                    _dateDropdown.options.Add(new TMP_Dropdown.OptionData(year));
                }
                break;

            case "month":
                foreach (var month in DataController.instance.RecordDropdownDates.month)
                {
                    _dateDropdown.options.Add(new TMP_Dropdown.OptionData(month));
                }
                break;
        }
        _dateDropdown.value = 0;
        _dateDropdown.RefreshShownValue();
        
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1f;

        yield return null;
    }
    
    private void OnDropDownValueChanged(int value)
    {
        string date = _dateDropdown.options[value].text;
        StartCoroutine(SetDateStatistics(date));
    }

    private IEnumerator SetDateStatistics(string date)
    {
        //맵 선택률 컨테이너 초기화
        foreach (Transform child in _mapSelectedContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        //랭크 불러오기 승률
        int i = 1;
        yield return StartCoroutine(DataController.instance.GetMainGameDatas(_category, date));
        var sortedMaps = DataController.instance.GameDatas.map
            .OrderByDescending(map => map.Value)
            .ToList();
        foreach (var map in sortedMaps)
        {
            GameObject go = Instantiate(_mapSelectedPrefab, _mapSelectedContainer.transform);
            SelectedMapCountPrafab mapInfo = go.GetComponent<SelectedMapCountPrafab>();
            if (i % 2 == 0)
            {
                foreach (var image in mapInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                }
            }
            else
            {
                foreach (var image in mapInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                }
            }
            if (i is 1 or 2 or 3)
            {
                foreach (var image in mapInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                }
            }
            mapInfo._name.text = map.Key;
            mapInfo._count.text = map.Value.ToString();
            mapInfo._selectedRate.text = 
                ((int)((map.Value / (float)DataController.instance.GameDatas.totalGames) * 100)).ToString() + "%";
            i++;
        }
        _totalRoundCount.text = "전체 라운드 수 : " + DataController.instance.GameDatas.totalGames.ToString();
        _requireRoundCount.text = "규정 라운드 수 : " + DataController.instance.GameDatas.minRequiredRound.ToString();
    }
}

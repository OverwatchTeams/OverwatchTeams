using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRecordPanelController : PanelController
{
    [SerializeField] private TMP_Dropdown _dateDropdown;
    [SerializeField] private GameObject _winRateRankContainer;
    [SerializeField] private GameObject _winRateRankPrefab;
    [SerializeField] private List<ScrollRect> _scrollRects;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        InitializePanel();
        OnDropDownValueChanged(_dateDropdown.value);
    }
    
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _dateDropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
        _dateDropdown.onValueChanged.AddListener(OnDropDownValueChanged);
    }

    private void InitializePanel()
    {
        StartCoroutine(SetDropdown("day"));
    }
    
    private IEnumerator SetDropdown(string category)
    {
        _dateDropdown.ClearOptions();
        switch (category)
        {
            case "day":
                foreach (var year in DataController.instance.RecordDropdownDates.day)
                {
                    _dateDropdown.options.Add(new TMP_Dropdown.OptionData(year));
                }
                break;
        }
        _dateDropdown.value = 0;
        _dateDropdown.RefreshShownValue();
        
        Canvas.ForceUpdateCanvases();
        foreach (var scrollRect in _scrollRects)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        yield return null;
    }
    
    private IEnumerator SetWinRateContainer(string date)
    {
        //랭크표 컨테이너 초기화
        foreach (Transform child in _winRateRankContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //랭크 불러오기 승률
        int i = 1;
        yield return StartCoroutine(DataController.instance.GetMainDailyData(date));
        foreach (var rank in DataController.instance.DailyGameData.leaderBoard.byDay[date].winRate.total)
        {
            GameObject go = Instantiate(_winRateRankPrefab, _winRateRankContainer.transform);
            WinRateRankPrefab rankInfo = go.GetComponent<WinRateRankPrefab>();
            if (i % 2 == 0)
            {
                foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table04");
                }
            }
            else
            {
                foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table03");
                }
            }

            if (rank.Value.wins - 1 > rank.Value.losses && i % 2 == 0)
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table07");
                }
            }
            else if (rank.Value.wins - 1 > rank.Value.losses && i % 2 == 1)
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table06");
                }
            }
            else if (rank.Value.losses - 1 > rank.Value.wins && i % 2 == 0)
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table09");
                }
            }
            else if (rank.Value.losses - 1 > rank.Value.wins && i % 2 == 1)
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table08");
                }
            }
            rankInfo._rank.text = i.ToString();
            rankInfo._name.text = rank.Key;
            rankInfo._win.text = rank.Value.wins.ToString();
            rankInfo._draw.text = rank.Value.draws.ToString();
            rankInfo._lose.text = rank.Value.losses.ToString();
            int winRate = (int)Mathf.Round(rank.Value.winRate);
            rankInfo._winRate.text = winRate.ToString();
            i++;
        }
    }
    
    private void OnDropDownValueChanged(int value)
    {
        string date = _dateDropdown.options[value].text;
        StartCoroutine(SetWinRateContainer(date));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRecordPanelController : PanelController
{
    [SerializeField] private TMP_Dropdown _dateDropdown;
    [SerializeField] private GameObject _winRateRankContainer;
    [SerializeField] private GameObject _dailyMatchContainer;
    [SerializeField] private GameObject _winRateRankPrefab;
    [SerializeField] private GameObject _dailyMatchPrefab;
    [SerializeField] private List<ScrollRect> _scrollRects;
    [SerializeField] private TMP_Text _totalRound;
    [SerializeField] private TMP_Text _participantCount;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        StartCoroutine(GetDropDownDates());
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

        var sortedRole =  DataController.instance.DailyGameData.leaderBoard.byDay[date].winRate.total
            .OrderByDescending(map => map.Value.wins - map.Value.losses)
            .ToList();
       
        foreach (var rank in sortedRole)
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
            //하드코딩 수정 필요(재사용 하느라 순서가 변경되어있음)
            rankInfo._lose.text = rank.Value.wins.ToString();
            rankInfo._winRate.text = rank.Value.losses.ToString();
            rankInfo._win.text = (rank.Value.wins - rank.Value.losses).ToString();
            int winRate = (int)Mathf.Round(rank.Value.winRate);
            rankInfo._draw.text = winRate.ToString();
            if (int.Parse(rankInfo._win.text) >= 3 || int.Parse(rankInfo._win.text) <= -3)
            {
                foreach (var text in rankInfo.gameObject.GetComponentsInChildren<TMP_Text>())
                {
                    text.fontStyle = FontStyles.Underline;
                }
            }
            i++;
        }
        _participantCount.text = (i-1).ToString();
    }
    private IEnumerator SetDailyMatchContainer(string date)
    {
        foreach (Transform child in _dailyMatchContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        bool isSucceed = false;
        List<RefinedMatchData> matchDatas = new List<RefinedMatchData>();
        yield return StartCoroutine(MatchDataManager.instance.GetDailyMatches(success => isSucceed = success, datas => matchDatas = datas,  date));
        yield return StartCoroutine(MainDataManager.instance.GetDropDownDates(success => isSucceed = success, DataController.instance.OnDropdownDatesLoaded));
        _totalRound.text = matchDatas.Count.ToString();
        foreach (var match in matchDatas)
        {
            GameObject go = Instantiate(_dailyMatchPrefab, _dailyMatchContainer.transform);
            DailyMatchPrefab matchObject = go.GetComponent<DailyMatchPrefab>();
            matchObject.SetDailyMatchPrefab(match);
        }
    }
    private void OnDropDownValueChanged(int value)
    {
        string date = _dateDropdown.options[value].text;
        StartCoroutine(SetWinRateContainer(date));
        StartCoroutine(SetDailyMatchContainer(date));
    }
    
    private IEnumerator GetDropDownDates()
    {
        yield return StartCoroutine(DataController.instance.GetDropdownDate(result =>
        {
            if (!result)
            {
                Debug.Log("Failed to get drop down dates");
                ReturnToParentPanel();
            }
        }));
        InitializePanel();
        OnDropDownValueChanged(_dateDropdown.value);
    }
}

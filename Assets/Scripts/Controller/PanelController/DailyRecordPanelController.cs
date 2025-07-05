using System;
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
    [SerializeField] private Button _gapButton;
    [SerializeField] private Button _streakButton;

    private List<RefinedMatchData> _matchDatas;
    private string _sortedType;
    private Dictionary<string, int> _playerStreaks;
    private bool _isMatchSupport = false;
    private string[] _players;
    
    private void OnEnable()
    {
        _isMatchSupport = false;
        _players = null;
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        _sortedType = "gap";
        StartCoroutine(GetDropDownDates());
    }
    
    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _dateDropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
        _dateDropdown.onValueChanged.AddListener(OnDropDownValueChanged);
        _gapButton.onClick.RemoveListener(OnGapButtonClicked);
        _gapButton.onClick.AddListener(OnGapButtonClicked);
        _streakButton.onClick.RemoveListener(OnStreakButtonClicked);
        _streakButton.onClick.AddListener(OnStreakButtonClicked);
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
        //랭크 불러오기 승률
        yield return StartCoroutine(DataController.instance.GetMainDailyData(date));
        yield return UpdateWinRateContainer(date);
        if(_isMatchSupport) FilterMatchPlayers();
    }

    private IEnumerator UpdateWinRateContainer(string date)
    {
        //랭크표 컨테이너 초기화
        foreach (Transform child in _winRateRankContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        int i = 1;
        var sortedWinRate = new List<KeyValuePair<string, DailyGameData.WinRateData.WinRate.PlayerWinRate>>();
        if (_sortedType == "gap")
        {
            sortedWinRate =  DataController.instance.DailyGameData.leaderBoard.byDay[date].winRate.total
                .OrderByDescending(map => map.Value.gap)
                .ToList();   
        }
        else if (_sortedType == "streak")
        {
            sortedWinRate = DataController.instance.DailyGameData.leaderBoard.byDay[date].winRate.total
                .OrderByDescending(map => map.Value.streak)
                .ToList();   
        }
        
        foreach (var rank in sortedWinRate)
        {
            GameObject go = Instantiate(_winRateRankPrefab, _winRateRankContainer.transform);
            DailyWinRateRankPrefab rankInfo = go.GetComponent<DailyWinRateRankPrefab>();
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

            rankInfo._rank.text = i.ToString();
            rankInfo._name.text = rank.Key;
            rankInfo._gap.text = (rank.Value.gap).ToString();
            rankInfo._streak.text = rank.Value.streak.ToString();
            int winRate = (int)Mathf.Round(rank.Value.winRate);
            rankInfo._winRate.text = winRate.ToString();
            rankInfo._win.text = rank.Value.wins.ToString();
            rankInfo._lose.text = rank.Value.losses.ToString();
            
            if (i % 2 == 0 && 
                (_sortedType == "gap" && int.Parse(rankInfo._gap.text) > 1 || 
                 _sortedType == "streak" && int.Parse(rankInfo._streak.text) > 1))
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table07");
                }
            }
            else if(i % 2 == 1 && 
                    (_sortedType == "gap" && int.Parse(rankInfo._gap.text) > 1 || 
                     _sortedType == "streak" && int.Parse(rankInfo._streak.text) > 1)) 
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table06");
                }
            }
            else if (i % 2 == 0 && 
                     (_sortedType == "gap" && int.Parse(rankInfo._gap.text) < -1 || 
                      _sortedType == "streak" && int.Parse(rankInfo._streak.text) < -1))
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table09");
                }
            }
            else if (i % 2 == 1 && 
                     (_sortedType == "gap" && int.Parse(rankInfo._gap.text) < -1 || 
                      _sortedType == "streak" && int.Parse(rankInfo._streak.text) < -1))
            {foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table08");
                }
            }
            i++;
        }
        _participantCount.text = (i-1).ToString();
        yield return null;
    }
    private IEnumerator SetDailyMatchContainer(string date)
    {
        foreach (Transform child in _dailyMatchContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        bool isSucceed = false;
        yield return StartCoroutine(MatchDataManager.instance.GetDailyMatches(success => isSucceed = success, datas => _matchDatas = datas,  date));
        _totalRound.text = _matchDatas.Count.ToString();
        foreach (var match in _matchDatas)
        {
            GameObject go = Instantiate(_dailyMatchPrefab, _dailyMatchContainer.transform);
            DailyMatchPrefab matchObject = go.GetComponent<DailyMatchPrefab>();
            matchObject.SetDailyMatchPrefab(match);
        }
    }

    private void OnGapButtonClicked()
    {
        _sortedType = "gap";
        StartCoroutine(UpdateWinRateContainer(_dateDropdown.options[_dateDropdown.value].text));
    }

    private void OnStreakButtonClicked()
    {
        _sortedType = "streak";
        StartCoroutine(UpdateWinRateContainer(_dateDropdown.options[_dateDropdown.value].text));
    }
    private void OnDropDownValueChanged(int value)
    {
        string date = _dateDropdown.options[value].text;
        StartCoroutine(GetDailyInform(date));
    }

    private IEnumerator GetDailyInform(string date)
    {
        yield return StartCoroutine(SetDailyMatchContainer(date));
        yield return StartCoroutine(SetWinRateContainer(date));
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

    public void SetDailyMatchSupporter(string[] players)
    {
        _players = players;
        _isMatchSupport = true;
    }

    private void FilterMatchPlayers()
    {
        foreach (Transform child in _winRateRankContainer.transform)
        {
            child.gameObject.SetActive(false);
        }
        if (!_winRateRankContainer.activeInHierarchy)
            Debug.LogWarning("_winRateRankContainer의 부모가 비활성화 상태입니다.");
        LayoutRebuilder.ForceRebuildLayoutImmediate(_winRateRankContainer.GetComponent<RectTransform>());
        if( _players == null )Debug.Log("players is null");
        foreach (string player in _players)
        {
            foreach (Transform child in _winRateRankContainer.transform)
            {
                if (child.gameObject.GetComponent<DailyWinRateRankPrefab>()._name.text == player)
                {
                    child.gameObject.SetActive(true);
                    Debug.Log(child.gameObject.GetComponent<DailyWinRateRankPrefab>()._name.text);
                    break;
                }
            }
        }
    }

}

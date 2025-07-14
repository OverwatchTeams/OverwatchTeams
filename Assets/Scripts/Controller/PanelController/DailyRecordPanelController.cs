using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RainbowArt.CleanFlatUI;
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
    [SerializeField] private ProgressBarLoop _circularProgressPopup;
    [SerializeField] private Toggle _toggleAll;
    
    private string _sortedType;
    private Dictionary<string, int> _playerStreaks;
    [SerializeField] private bool _isMatchSupport = false;
    private string[] _players;
    private bool _isInitialized = false;
    private List<RefinedMatchData> _dailyMatches;
    private DailyGameData _dailyGameData;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        InitializePanel();
        _isInitialized = false;
        _dailyGameData = DataController.instance.DailyGameData;
        _dailyMatches = DataController.instance.RefinedMatches;
        _sortedType = "gap";
        if( _isMatchSupport ) _toggleAll.isOn = false;
        else
        {
            _toggleAll.gameObject.SetActive(false);
        }
        StartCoroutine(GetDailyInform(_dateDropdown.options[_dateDropdown.value].text));
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
        _toggleAll.onValueChanged.RemoveListener(OnToggleAllClicked);
        _toggleAll.onValueChanged.AddListener(OnToggleAllClicked);
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
            sortedWinRate =  _dailyGameData.leaderBoard.byDay[date].winRate.total
                .OrderByDescending(map => map.Value.gap)
                .ToList();   
        }
        else if (_sortedType == "streak")
        {
            sortedWinRate = _dailyGameData.leaderBoard.byDay[date].winRate.total
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
            
            rankInfo._name.text = rank.Key;
            rankInfo._gap.text = (rank.Value.gap).ToString();
            rankInfo._streak.text = rank.Value.streak.ToString();
            int winRate = (int)Mathf.Round(rank.Value.winRate);
            rankInfo._winRate.text = winRate.ToString();
            rankInfo._result.text = rank.Value.wins + "/" + rank.Value.draws + "/" + rank.Value.losses;
            
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
        if(_isMatchSupport && !_toggleAll.isOn) FilterMatchPlayers();
    }
    private IEnumerator UpdateDailyMatchContainer()
    {
        foreach (Transform child in _dailyMatchContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        _totalRound.text = _dailyMatches.Count.ToString();
        foreach (var match in _dailyMatches)
        {
            GameObject go = Instantiate(_dailyMatchPrefab, _dailyMatchContainer.transform);
            DailyMatchPrefab matchObject = go.GetComponent<DailyMatchPrefab>();
            matchObject.SetDailyMatchPrefab(match);
        }

        yield return null;
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

    private void OnToggleAllClicked(bool value)
    {
        StartCoroutine(UpdateWinRateContainer(_dateDropdown.options[_dateDropdown.value].text));
    }

    private IEnumerator GetDailyInform(string date)
    {
        //첫 로딩을 제외한 경우 데이터를 새로 로드
        if (_isInitialized || !_isMatchSupport)
        {
            _circularProgressPopup.gameObject.SetActive(true);
            yield return StartCoroutine(MatchDataManager.instance.GetDailyMatches(null, matches => _dailyMatches = matches, date));
            yield return StartCoroutine(MainDataManager.instance.GetDailyGameData(null, data => _dailyGameData = data, date));
            _circularProgressPopup.gameObject.SetActive(false);
        }
        yield return StartCoroutine(UpdateDailyMatchContainer());
        yield return StartCoroutine(UpdateWinRateContainer(date));
        _isInitialized = true;
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
        foreach (string player in _players)
        {
            foreach (Transform child in _winRateRankContainer.transform)
            {
                if (child.gameObject.GetComponent<DailyWinRateRankPrefab>()._name.text == player)
                {
                    child.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

}

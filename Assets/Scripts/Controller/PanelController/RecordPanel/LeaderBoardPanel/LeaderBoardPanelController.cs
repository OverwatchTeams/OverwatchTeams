using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

public class LeaderBoardPanelController : PanelController
{
    [SerializeField] private Toggle _yearlyToggle;
    [SerializeField] private Toggle _monthlyToggle;
    [SerializeField] private TMP_Dropdown _dateDropdown;
    [SerializeField] private GameObject _winRateRankContainer;
    [SerializeField] private GameObject _participantRankContainer;
    [SerializeField] private GameObject _winRateRankPrefab;
    [SerializeField] private GameObject _participantRankPrefab;
    [SerializeField] private List<ScrollRect> _scrollRects;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        _monthlyToggle.SetIsOnWithoutNotify(false);
        _yearlyToggle.SetIsOnWithoutNotify(true);
        OnYearlyToggleValueChanged(_yearlyToggle.isOn);
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _dateDropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
        _yearlyToggle.onValueChanged.RemoveListener(OnYearlyToggleValueChanged);
        _monthlyToggle.onValueChanged.RemoveListener(OnMonthlyToggleValueChanged);
        _dateDropdown.onValueChanged.AddListener(OnDropDownValueChanged);
        _yearlyToggle.onValueChanged.AddListener(OnYearlyToggleValueChanged);
        _monthlyToggle.onValueChanged.AddListener(OnMonthlyToggleValueChanged);
    }

    private IEnumerator SetDropdown(string category)
    {
        _dateDropdown.ClearOptions();
        switch (category)
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
        foreach (var scrollRect in _scrollRects)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        yield return null;
    }

    private IEnumerator SetWinRateContainer(string category, string date)
    {
        //랭크표 컨테이너 초기화
        foreach (Transform child in _winRateRankContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _participantRankContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //랭크 불러오기 승률
        int i = 1;
        yield return StartCoroutine(DataController.instance.GetMainLeaderBoardData(category, date));
        foreach (var rank in DataController.instance.WinRateData.winRate.total)
        {
            if (i > 10) break;
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
            if (i is 1 or 2 or 3)
            {
                foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table02");
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

        //랭크 불러오기 참여율
        i = 1;
        foreach (var rank in DataController.instance.WinRateData.attendance.total)
        {
            if (i > 10) break;
            GameObject go = Instantiate(_participantRankPrefab, _participantRankContainer.transform);
            ParticipantRankPrefab rankInfo = go.GetComponent<ParticipantRankPrefab>();
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
            if (i is 1 or 2 or 3)
            {
                foreach (var image in rankInfo.gameObject.GetComponentsInChildren<Image>())
                {
                    image.sprite = Resources.Load<Sprite>("Images/backGround_table02");
                }
            }
            rankInfo._rank.text = i.ToString();
            rankInfo._name.text = rank.Key;
            rankInfo._round.text = rank.Value.playedGames.ToString();
            float a = (float)rank.Value.playedGames / rank.Value.totalGames * 100;
            int participantRate = (int)Mathf.Round(a);
            rankInfo._participant.text = participantRate.ToString();
            i++;
        }
    }
    private void OnYearlyToggleValueChanged(bool value)
    {
        Debug.Log($"OnYearlyToggleValueChanged : {value}");
        if (value)
        {
            StartCoroutine(SetDropdown("year"));
            string date = _dateDropdown.options[_dateDropdown.value].text;
            StartCoroutine(SetWinRateContainer("year", date));
        }
    }

    private void OnMonthlyToggleValueChanged(bool value)
    {
        Debug.Log($"OnMonthlyToggleValueChanged : {value}");
        if (value)
        {
            StartCoroutine(SetDropdown("month"));
            string date = _dateDropdown.options[_dateDropdown.value].text;
            StartCoroutine(SetWinRateContainer("month", date));
        }
    }

    private void OnDropDownValueChanged(int value)
    {
        string date = _dateDropdown.options[value].text;
        string category = "";
        if (_yearlyToggle.isOn)
        {
            category = "year";
        }
        else
        {
            category = "month";
        }
        StartCoroutine(SetWinRateContainer(category, date));
    }
}

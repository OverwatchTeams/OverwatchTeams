using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictionMatchManager : PanelController
{
    [SerializeField] private TMP_Text _bluePredictionScore;
    [SerializeField] private TMP_Text _redPredictionScore;
    [SerializeField] private ScoreSliderPrefab _voiceScoreSlider;
    [SerializeField] private ScoreSliderPrefab _playerScoreSlider;
    [SerializeField] private ScoreSliderPrefab _mapScoreSlider;
    [SerializeField] private GameObject _warningDescription;
    [SerializeField] private Button _settingButton;
    [SerializeField] private GameObject[] _scoreMasks;
    
    //Setting
    [SerializeField] private Slider _voiceSlider;
    [SerializeField] private Slider _roleSlider;
    [SerializeField] private Slider _mapSlider;
    [SerializeField] private Slider _dealerSlider;
    [SerializeField] private Slider _tankerSlider;
    [SerializeField] private Slider _healerSlider;
    
    private PlayerButtonPrefab[] _players;
    private PlayerData[] _playerDatas;
    private string _map;
    private string _day;

    private float[] voiceScores;
    private float[] roleScores;
    private float[] mapScores;

    private float[] voicePredictScores;
    private float[] rolePredictScores;
    private float[] mapPredictScores;
    

    [SerializeField] private TMP_Text _voiceWeight;
    [SerializeField] private TMP_Text _roleWeight;
    [SerializeField] private TMP_Text _mapWeight;
    [SerializeField] private TMP_Text _dealerWeight;
    [SerializeField] private TMP_Text _tankerWeight;
    [SerializeField] private TMP_Text _healerWeight;

    private float totalMainWeight;
    private float totalRoleWeight;
    private bool isSettingInit = true;

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
        _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _voiceSlider.onValueChanged.RemoveListener(OnVoiceSettingChanged);
        _roleSlider.onValueChanged.RemoveListener(OnRoleSettingChanged);
        _mapSlider.onValueChanged.RemoveListener(OnMapSettingChanged);
        _dealerSlider.onValueChanged.RemoveListener(OnDealerSettingChanged);
        _tankerSlider.onValueChanged.RemoveListener(OnTankerSettingChanged);
        _healerSlider.onValueChanged.RemoveListener(OnHealerSettingChanged);
        _voiceSlider.onValueChanged.AddListener(OnVoiceSettingChanged);
        _roleSlider.onValueChanged.AddListener(OnRoleSettingChanged);
        _mapSlider.onValueChanged.AddListener(OnMapSettingChanged);
        _dealerSlider.onValueChanged.AddListener(OnDealerSettingChanged);
        _tankerSlider.onValueChanged.AddListener(OnTankerSettingChanged);
        _healerSlider.onValueChanged.AddListener(OnHealerSettingChanged);
        
        isSettingInit = true;
    }

    public void UpdatePrediction(PlayerButtonPrefab[] players, string map, string day)
    {
        voiceScores = new float[2];
        roleScores = new float[2];
        mapScores = new float[2];
        voicePredictScores = new float[2];
        rolePredictScores = new float[2];
        mapPredictScores = new float[2];

        totalMainWeight = int.Parse(_roleWeight.text) + int.Parse(_mapWeight.text) + int.Parse(_voiceWeight.text);
        totalRoleWeight = int.Parse(_dealerWeight.text) + int.Parse(_tankerWeight.text) + int.Parse(_healerWeight.text);
        
        _players = players;
        _map = map;
        _day = day;

        if(map == "-") _warningDescription.SetActive(true);
        else _warningDescription.SetActive(false);

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i]._role = i switch
            {
                0 or 1 or 5 or 6 => Role.D,
                2 or 7 => Role.T,
                3 or 4 or 8 or 9 => Role.H,
                _ => _players[i]._role
            };
            if (_players[i].PlayerData.player == "-") continue;

            if (DataController.instance.DailyGameData.leaderBoard.byDay.TryGetValue(day, out var board)
                && board.winRate.total.TryGetValue(_players[i].PlayerData.player, out var rank))
                _players[i].SetBadge($"{rank.wins}승 {rank.losses}패");
            else
                _players[i].SetBadge("0승 0패");
        }

        foreach (var p in _players)
            if (p.PlayerData.player == "-") return;

        _playerDatas = players.Select(p => p.PlayerData).ToArray();
        DateTime targetDate = DateTime.Parse(day);

        var recentMonths = Enumerable.Range(0, 6)
            .Select(offset => targetDate.AddMonths(-offset).ToString("yyyy-MM"))
            .ToList();

        // 역할 점수 전체 min/max 수집
        float minScore = float.MaxValue, maxScore = float.MinValue;
        foreach (var pd in _playerDatas)
        {
            minScore = Mathf.Min(minScore, pd.scores.D, pd.scores.T, pd.scores.H);
            maxScore = Mathf.Max(maxScore, pd.scores.D, pd.scores.T, pd.scores.H);
        }

        for (int i = 0; i < _players.Length; i++)
        {
            int teamIndex = i < 5 ? 0 : 1;

            // Voice 사용 여부 (0 ~ 1 비율)
            bool? isVoiceAvailable = _playerDatas[i].isVoiceAvailable;
            float voice = (isVoiceAvailable == true) ? 1f : 0f;
            voiceScores[teamIndex] += voice;
            voicePredictScores[teamIndex] += voice;

            // 역할별 점수 + 맵 승률
            float normalizedScore = 0;
            Role role = _players[i]._role;
            switch (role)
            {
                case Role.D:
                    roleScores[teamIndex] += _playerDatas[i].scores.D;
                    rolePredictScores[teamIndex] += Normalize(_playerDatas[i].scores.D, minScore, maxScore) * int.Parse(_dealerWeight.text) / totalRoleWeight;
                    break;
                case Role.T:
                    roleScores[teamIndex] += _playerDatas[i].scores.T;
                    rolePredictScores[teamIndex] += Normalize(_playerDatas[i].scores.T, minScore, maxScore) * int.Parse(_tankerWeight.text) / totalRoleWeight;
                    break;
                case Role.H:
                    roleScores[teamIndex] += _playerDatas[i].scores.H;
                    rolePredictScores[teamIndex] += Normalize(_playerDatas[i].scores.H, minScore, maxScore) * int.Parse(_healerWeight.text) / totalRoleWeight;
                    break;
            }
            roleScores[teamIndex] += normalizedScore;

            AddMapWinRateScore(_playerDatas[i].winRates.byMonth, map, role, recentMonths,
                ref mapScores[teamIndex], ref mapPredictScores[teamIndex]);
        }

        // 슬라이더 표시용 (0~1 기준)
        _voiceScoreSlider.SetScoreSliderPrefab(voiceScores[0], voiceScores[1]);
        _playerScoreSlider.SetScoreSliderPrefab(roleScores[0] / 100, roleScores[1] / 100);
        _mapScoreSlider.SetScoreSliderPrefab(mapScores[0], mapScores[1]);

        float blueScore = (
            voicePredictScores[0] * int.Parse(_voiceWeight.text) +
            rolePredictScores[0] * int.Parse(_roleWeight.text) +
            mapPredictScores[0] * int.Parse(_mapWeight.text)) / totalMainWeight;

        float redScore = (
            voicePredictScores[1] * int.Parse(_voiceWeight.text) +
            rolePredictScores[1] * int.Parse(_roleWeight.text) +
            mapPredictScores[1] * int.Parse(_mapWeight.text)) / totalMainWeight;

        float b = Mathf.Exp(blueScore);
        float r = Mathf.Exp(redScore);
        float total = b + r;

        _bluePredictionScore.text = $"{(int)(b / total * 100)}%";
        _redPredictionScore.text = $"{ 100- (int)(b / total * 100) }%";
    }

    private float Normalize(float value, float min, float max)
    {
        if (Mathf.Abs(max - min) < 0.0001f) return 0.5f; // 방어적 처리
        return Mathf.Clamp01((value - min) / (max - min));
    }


    private void AddMapWinRateScore(
        Dictionary<string, PlayerData.WinRate.WinRateDetail> byMonth,
        string map,
        Role role,
        IEnumerable<string> recentMonths,
        ref float teamWinRateSum,
        ref float teamPredictWinRateSum)
    {
        int total = 0, wins = 0, draws = 0;

        foreach (var date in recentMonths)
        {
            if (!byMonth.TryGetValue(date, out var data)) continue;

            var roleMap = role switch
            {
                Role.D => data.roleMap.D,
                Role.T => data.roleMap.T,
                Role.H => data.roleMap.H,
                _ => null
            };
            if (roleMap == null || !roleMap.TryGetValue(map, out var stats)) continue;

            total += stats.playedGames;
            wins += stats.wins;
            draws += stats.draws;
        }

        float rawRate = total > 0 ? (wins + draws * 0.5f) / total : 0.5f;
        float confidence = Mathf.Clamp01(total / 10f);
        float trustedRate = confidence * rawRate + (1 - confidence) * 0.5f;

        teamWinRateSum += trustedRate;

        float weight = role switch
        {
            Role.D => int.Parse(_dealerWeight.text),
            Role.T => int.Parse(_tankerWeight.text),
            Role.H => int.Parse(_healerWeight.text),
            _ => 0
        };
        teamPredictWinRateSum += trustedRate * weight / totalRoleWeight;
    }

    private void OnSettingButtonClicked()
    {
        OpenPanel("[PopupPanel] Setting");
        _voiceSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Voice"));
        _roleSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Role"));
        _mapSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Map"));
        _dealerSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Dealer"));
        _tankerSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Tanker"));
        _healerSlider.gameObject.GetComponentInParent<WeightSlider>().SetSliderValue(PlayerPrefs.GetFloat("Healer"));
    }

    private void OnVoiceSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Voice", value);
        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    
    private void OnRoleSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Role", value);
        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    private void OnMapSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Map", value);
        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    private void OnDealerSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Dealer", value);

        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    private void OnTankerSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Tanker", value);
        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    private void OnHealerSettingChanged(float value)
    {
        if (isSettingInit)
        {
            isSettingInit = false;
            UpdatePrediction(_players, _map, _day);
            return;
        }
        PlayerPrefs.SetFloat("Healer", value);
        PlayerPrefs.Save();
        UpdatePrediction(_players, _map, _day);
    }
    
    public void SetScoreMask(bool isScoreViewable)
    {
        if(isScoreViewable)
        {
            foreach (var _scoreMask in _scoreMasks)
            {
                _scoreMask.SetActive(false);
            }
            
        }
        else
        {
            foreach (var _scoreMask in _scoreMasks)
            {
                _scoreMask.SetActive(true);
            }
        }
    }
}

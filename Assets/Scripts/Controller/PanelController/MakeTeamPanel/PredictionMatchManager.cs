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
    
    //Setting
    [SerializeField] private Slider _voiceSlider;
    [SerializeField] private Slider _roleSlider;
    [SerializeField] private Slider _mapSlider;
    [SerializeField] private Slider _dealerSlider;
    [SerializeField] private Slider _tankerSlider;
    [SerializeField] private Slider _healderSlider;
    
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
        _voiceSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _roleSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _mapSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _dealerSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _tankerSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _healderSlider.onValueChanged.RemoveListener(OnSettingChanged);
        _voiceSlider.onValueChanged.AddListener(OnSettingChanged);
        _roleSlider.onValueChanged.AddListener(OnSettingChanged);
        _mapSlider.onValueChanged.AddListener(OnSettingChanged);
        _dealerSlider.onValueChanged.AddListener(OnSettingChanged);
        _tankerSlider.onValueChanged.AddListener(OnSettingChanged);
        _healderSlider.onValueChanged.AddListener(OnSettingChanged);
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
            // 역할 할당
            _players[i]._role = i switch
            {
                0 or 1 or 5 or 6 => Role.D,
                2 or 7 => Role.T,
                3 or 4 or 8 or 9 => Role.H,
                _ => _players[i]._role
            };
            if (_players[i].PlayerData.player == "-") continue;
            
            if (DataController.instance.DailyGameData.leaderBoard.byDay.ContainsKey(day) &&
                DataController.instance.DailyGameData.leaderBoard.byDay[day].winRate.total
                    .ContainsKey(_players[i].PlayerData.player))
            {
                _players[i].SetBadge(DataController.instance.DailyGameData.leaderBoard.byDay[day].winRate
                    .total[_players[i].PlayerData.player].streak);
            }
            else
            {
                _players[i].SetBadge(0);
            }
        }

        foreach (PlayerButtonPrefab player in _players)
        {
            if (player.PlayerData.player == "-") return;
        }
        _playerDatas = players.Select(p => p.PlayerData).ToArray();
        DateTime targetDate = DateTime.Parse(day);

        List<string> recentMonths = new List<string>();
        for (int offset = 0; offset <= 5; offset++)  // 지난 6개월 (현재 달은 제외)
        {
            DateTime monthDate = targetDate.AddMonths(-offset);
            string key = monthDate.ToString("yyyy-MM"); // byMonth에서 사용되는 키
            recentMonths.Add(key);
        }
        
        for (int i = 0; i < _players.Length; i++)
        {
            // 팀 구분: 0 = 블루팀, 1 = 레드팀
            int teamIndex = i < 5 ? 0 : 1;

            // 보이스 점수

            bool? isVoiceAvailable = _playerDatas[i].isVoiceAvailable;
            voiceScores[teamIndex] += isVoiceAvailable != null && isVoiceAvailable.Value ? 1 : 0;
            voicePredictScores[teamIndex] += isVoiceAvailable != null && isVoiceAvailable.Value ? 100 : 0;
            
            // 역할별 점수 계산
            switch (_players[i]._role)
            {
                case Role.D:
                    roleScores[teamIndex] += _playerDatas[i].scores.D;
                    rolePredictScores[teamIndex] += (float)(_playerDatas[i].scores.D + 500) / 10 * int.Parse(_dealerWeight.text)/ totalRoleWeight;
                    
                    AddMapWinRateScore(_playerDatas[i].winRates.byMonth, map, Role.D, recentMonths, 
                        ref mapScores[teamIndex], ref mapPredictScores[teamIndex]);
                    break;

                case Role.T:
                    roleScores[teamIndex] += _playerDatas[i].scores.T;
                    rolePredictScores[teamIndex] += (float)(_playerDatas[i].scores.T + 500) / 10 * int.Parse(_tankerWeight.text)/ totalRoleWeight;
                    
                    AddMapWinRateScore(_playerDatas[i].winRates.byMonth, map, Role.T, recentMonths, 
                        ref mapScores[teamIndex], ref mapPredictScores[teamIndex]);
                    break;

                case Role.H:
                    roleScores[teamIndex] += _playerDatas[i].scores.H;
                    rolePredictScores[teamIndex] += (float)(_playerDatas[i].scores.H + 500) / 10 * int.Parse(_healerWeight.text)/ totalRoleWeight;
                    
                    AddMapWinRateScore(_playerDatas[i].winRates.byMonth, map, Role.H, recentMonths, 
                        ref mapScores[teamIndex], ref mapPredictScores[teamIndex]);
                    break;
            }
        }
        
        _voiceScoreSlider.SetScoreSliderPrefab(voiceScores[0] / 100, voiceScores[1] / 100);
        _playerScoreSlider.SetScoreSliderPrefab(roleScores[0] / 100, roleScores[1] / 100);
        _mapScoreSlider.SetScoreSliderPrefab(mapScores[0], mapScores[1]);
        
        float blueVoiceScore;
        float redVoiceScore;
        float bluePlayerScore;
        float redPlayerScore;
        float blueMapScore;
        float redMapScore;

        if (voicePredictScores[0] == 0 && voicePredictScores[1] == 0)
        {
            blueVoiceScore = 0;
            redVoiceScore = 0;
        }
        else
        {
            blueVoiceScore = voicePredictScores[0] / (voicePredictScores[0] + voicePredictScores[1]);
            Debug.Log(blueVoiceScore);
            redVoiceScore = voicePredictScores[1] / (voicePredictScores[0] + voicePredictScores[1]);
        }

        if (rolePredictScores[0] == 0 && rolePredictScores[1] == 0)
        {
            bluePlayerScore = 0;
            redPlayerScore = 0;
        }
        else
        {
            bluePlayerScore = rolePredictScores[0] / (rolePredictScores[0] + rolePredictScores[1]);
            Debug.Log(bluePlayerScore);
            redPlayerScore = rolePredictScores[1] / (rolePredictScores[0] + rolePredictScores[1]);   
        }

        if (mapPredictScores[0] == 0 && mapPredictScores[1] == 0)
        {
            blueMapScore = 0;
            redMapScore = 0;
        }
        else
        {
            blueMapScore = mapPredictScores[0] / (mapPredictScores[0] + mapPredictScores[1]);
            Debug.Log(blueMapScore);
            redMapScore = mapPredictScores[1] / (mapPredictScores[0] + mapPredictScores[1]);
        }
        
        float blueTotalScore = blueVoiceScore * int.Parse(_voiceWeight.text) + bluePlayerScore * 
            int.Parse(_roleWeight.text) + blueMapScore * int.Parse(_mapWeight.text);
        float redTotalScore = redVoiceScore * int.Parse(_voiceWeight.text) + redPlayerScore * 
            int.Parse(_roleWeight.text) + redMapScore * int.Parse(_mapWeight.text);

        if (blueTotalScore == 0 && redTotalScore == 0)
        {
            _bluePredictionScore.text = "0%";
            _redPredictionScore.text = "0%";
        }
        else
        {
            _bluePredictionScore.text = (int)((blueTotalScore / (blueTotalScore + redTotalScore)) * 100 ) + "%";
            _redPredictionScore.text = (100 - (int)((blueTotalScore / (blueTotalScore + redTotalScore)) * 100 )) + "%";   
        }
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
            if (roleMap == null) continue;
            if (!roleMap.TryGetValue(map, out var stats)) continue;
            
            total += stats.playedGames;
            wins += stats.wins;
            draws += stats.draws;
        }
        
        if (total >= 1)
        {
            teamWinRateSum += (float)wins / total + (float)draws / total * 0.5f;
            switch(role)
            {
                case Role.D:
                    teamPredictWinRateSum += (float)wins / total + (float)draws / total * 0.5f * int.Parse(_dealerWeight.text) / totalRoleWeight;
                    break;
                case Role.T:
                    teamPredictWinRateSum += (float)wins / total + (float)draws / total * 0.5f * int.Parse(_tankerWeight.text) / totalRoleWeight;
                    break;
                case Role.H:
                    teamPredictWinRateSum += (float)wins / total + (float)draws / total * 0.5f * int.Parse(_healerWeight.text) / totalRoleWeight;
                    break;
            }
        }
        else
        {
            teamWinRateSum += 0.5f; // 기본값
            switch(role)
            {
                case Role.D:
                    teamPredictWinRateSum += 0.5f * int.Parse(_dealerWeight.text) / totalRoleWeight;
                    break;
                case Role.T:
                    teamPredictWinRateSum += 0.5f * int.Parse(_tankerWeight.text) / totalRoleWeight;
                    break;
                case Role.H:
                    teamPredictWinRateSum += 0.5f * int.Parse(_healerWeight.text) / totalRoleWeight;
                    break;
            }
        }
        
    }

    private void OnSettingButtonClicked()
    {
        OpenPanel("[PopupPanel] Setting");
    }

    private void OnSettingChanged(float value)
    {
        UpdatePrediction(_players, _map, _day);
    }
}

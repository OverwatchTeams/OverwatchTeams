using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class MatchDataManager : MonoBehaviour
{
    public static MatchDataManager instance;
    private string url = "https://51g7o9m3xj.execute-api.ap-northeast-2.amazonaws.com/";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Match/";
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    public IEnumerator GetDailyMatches(Action<Response<List<RefinedMatchData>>> OnCompleted, string date)
    {
        string requestUrl = url + $"GetDailyMatches?date=" + date;
        yield return DataUtility.GetData<MatchData[]>(rawResponse =>
        {
            // MatchData[] → RefinedMatchData 가공
            List<RefinedMatchData> refinedMatches = RefineAllMatchData(rawResponse.DATA);

            var finalResponse = new Response<List<RefinedMatchData>>
            {
                isSuccess = rawResponse.isSuccess,
                statusCode = rawResponse.statusCode,
                message = rawResponse.message,
                code = rawResponse.code,
                DATA = refinedMatches,
            };

            OnCompleted?.Invoke(finalResponse);
        }, requestUrl);
    }
    
    public IEnumerator GetLastMatch(Action<Response<MatchData>> OnCompleted)
    {
        string requestUrl = url + $"GetLastMatch";
        yield return DataUtility.GetData(OnCompleted, requestUrl);
    }
    
    public IEnumerator CreateMatch(Action<Response<int>> OnCompleted, RefinedMatchData data)
    {
        string requestUrl = url + "CreateMatch";
        yield return DataUtility.AddData<MatchData[], int>(OnCompleted, requestUrl, UnrefineMatchData(data));
    }
    
    public IEnumerator DeleteMatchData(Action<Response<string>> OnCompleted, int round)
    {
        string requestUrl = url +  $"DeleteMatch?round={round}";
        yield return DataUtility.DeleteData(OnCompleted, requestUrl);
    }
    
    #region MatchData Exchanger
    
    /// <summary>
    /// MatchData[]->RefinedMatchData로 변환
    /// </summary>
    /// <param name="matchGroup"></param>
    /// <returns></returns>
    private RefinedMatchData RefineMatchData(MatchData[] matchGroup)
    {
        // 팀 필요한 구분
        List<MatchData> blueTeam;
        List<MatchData> redTeam;

        // 우선 atkdef 기준으로 팀 분류
        var atkdefGroups = matchGroup.GroupBy(m => m.atkdef).ToDictionary(g => g.Key, g => g.ToList());

        if (atkdefGroups.ContainsKey(AtkDefType.선공) && atkdefGroups.ContainsKey(AtkDefType.후공))
        {
            redTeam = atkdefGroups[AtkDefType.선공];
            blueTeam = atkdefGroups[AtkDefType.후공];
        }
        else if (atkdefGroups.ContainsKey(AtkDefType.중립))
        {
            // "동등"으로 있을 경우, index로 먼저 입력된 팀이 블루
            var byIndex = atkdefGroups[AtkDefType.중립]
                .OrderBy(m => m.index)
                .ToList();

            int half = byIndex.Count / 2;
            blueTeam = byIndex.Take(half).ToList();
            redTeam = byIndex.Skip(half).ToList();
        }
        else
        {
            Debug.LogWarning("팀 분류 불가능한 라운드: " + matchGroup.FirstOrDefault()?.round);
            return null;
        }

        // 정렬: 각 팀별 index 순
        blueTeam = blueTeam.OrderBy(m => m.index).ToList();
        redTeam = redTeam.OrderBy(m => m.index).ToList();

        // 승자 판단
        WinnerTeam winner;
        if (blueTeam.All(p => p.winlose == WinLose.승))
            winner = WinnerTeam.블루;
        else if (redTeam.All(p => p.winlose == WinLose.승))
            winner = WinnerTeam.레드;
        else
            winner = WinnerTeam.무승부;

        // 최종 RefinedMatchData 구성
        return new RefinedMatchData
        {
            beginIndex = matchGroup.Last().index + 1,
            date = Convert.ToDateTime(matchGroup.First().date),
            round = matchGroup.First().round,
            map = matchGroup.First().map,
            players = blueTeam.Select(p => (p.player, p.role))
                .Concat(redTeam.Select(p => (p.player, p.role)))
                .ToList(),
            winner = winner
        };
    }
    
    /// <summary>
    /// 입력받은 모든 라운드 정보를 변환하여 리스트로 반환
    /// </summary>
    /// <param name="groupedData"></param>
    /// <returns></returns>
    public List<RefinedMatchData> RefineAllMatchData(MatchData[] matches)
    {
        var matchGroup = matches
            .GroupBy(m => m.round)
            .OrderByDescending(g => g.Key)
            .Select(g => g.ToArray())
            .ToArray();
        var result = new List<RefinedMatchData>();
        foreach (var group in matchGroup)
        {
            var refined = RefineMatchData(group);
            if (refined != null)
                result.Add(refined);
        }
        return result;
    }
    
    /// <summary>
    /// RefinedMatchData->MatchData로 변환
    /// </summary>
    /// <param name="refined"></param>
    /// <returns></returns>
    private MatchData[] UnrefineMatchData(RefinedMatchData refined)
    {
        int totalPlayers = refined.players.Count;
        int half = totalPlayers / 2;

        var bluePlayers = refined.players.Take(half).ToList();
        var redPlayers = refined.players.Skip(half).ToList();

        // 승패 기준 판단
        WinLose blueWinlose;
        WinLose redWinlose;

        switch (refined.winner)
        {
            case WinnerTeam.블루:
                blueWinlose = WinLose.승;
                redWinlose = WinLose.패;
                break;
            case WinnerTeam.레드:
                blueWinlose = WinLose.패;
                redWinlose = WinLose.승;
                break;
            default:
                blueWinlose = WinLose.무;
                redWinlose = WinLose.무;
                break;
        }

        // atkdef 설정
        AtkDefType blueAtkdef = AtkDefType.중립;
        AtkDefType redAtkdef = AtkDefType.중립;
        
        foreach (var map in DataController.instance.Maps)
        {
            if (map.name != refined.map) continue;
            foreach (var maptype in DataController.instance.MapTypes)
            {
                if (maptype.name != map.type) continue;
                if (maptype.isAtkDef)
                {
                    blueAtkdef = AtkDefType.후공;
                    redAtkdef = AtkDefType.선공;
                }
                else
                {
                    blueAtkdef = AtkDefType.중립;
                    redAtkdef = AtkDefType.중립;
                }
            }
        }
        
        List<MatchData> result = new List<MatchData>();
        int indexCounter = 0;

        foreach (var player in bluePlayers)
        {
            result.Add(new MatchData
            {
                index = refined.beginIndex + indexCounter,
                date = refined.date.ToString("yyyy-MM-ddT00:00:00.000Z"),
                round = refined.round,
                map = refined.map,
                role = player.Item2,
                player = player.Item1,
                atkdef = blueAtkdef,
                winlose = blueWinlose,
            });
            indexCounter++;
        }

        foreach (var player in redPlayers)
        {
            result.Add(new MatchData
            {
                index = refined.beginIndex + indexCounter,
                date = refined.date.ToString("yyyy-MM-ddThh:mm:ssZ"),
                round = refined.round,
                map = refined.map,
                role = player.Item2,
                player = player.Item1,
                atkdef = redAtkdef,
                winlose = redWinlose,
            });
            indexCounter++;
        }

        return result.ToArray();
    }
    
    #endregion
}
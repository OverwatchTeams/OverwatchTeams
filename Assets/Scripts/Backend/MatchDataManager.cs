using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class MatchDataManager : DataManager<MatchData>
{
    public static MatchDataManager instance;

    #region Override Methods
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            url += "Match/";
            base.Awake();
        }
        else
            Destroy(gameObject);
    }
    
    #region AddData

    public  IEnumerator AddData(Action<bool> OnCompleted, RefinedMatchData data)
    {
        yield return StartCoroutine(AddDataCoroutine(OnCompleted, UnrefineMatchData(data), null));
    }
    
    protected IEnumerator AddDataCoroutine(Action<bool> OnCompleted, MatchData[] data, string requestUrl)
    {
        requestUrl = url + "CreateMatch";
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("Serialized JSON Data: " + json);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("데이터 추가 성공: " + request.downloadHandler.text);
                OnCompleted.Invoke(true);
            }
            else
            {
                Debug.LogError("데이터 추가 실패: " + request.error);
                OnCompleted.Invoke(false);
            }
        }
    }
    #endregion
    
    #region DeleteData
    
    protected override IEnumerator DeleteDataCoroutine(Action<bool> OnCompleted, int round, string requestUrl)
    {
        return base.DeleteDataCoroutine(OnCompleted, round, url +$"DeleteMatch?round={round}");
    }
    #endregion
    
    #endregion
    #region GetDataByRounds
    public IEnumerator GetMatchByRounds(Action<List<RefinedMatchData>> OnCompleted, int page = 1, int limit = 100)
    {
        yield return StartCoroutine(GetMatchByRoundsCoroutine(OnCompleted, page, limit));
    }
    
    private IEnumerator GetMatchByRoundsCoroutine(Action<List<RefinedMatchData>> OnCompleted, int page = 1, int limit = 100)
    {
        string requestUrl = url + $"GetMatchByRounds?page={page}&limit={limit}";

        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                MatchData[] flatMatchData = JsonConvert.DeserializeObject<MatchData[]>(json);

                // 라운드별로 그룹화
                var grouped = flatMatchData
                    .GroupBy(m => m.round)
                    .OrderByDescending(g => g.Key)
                    .Select(g => g.ToArray())
                    .ToArray();
                
                Debug.Log($"불러온 라운드 수: {grouped.Length}");
                OnCompleted?.Invoke(RefineAllMatchData(grouped));
            }
            else
            {
                Debug.LogError($"데이터 불러오기 실패: {www.error}");
                OnCompleted?.Invoke(null);
            }
        }
    }
    #endregion
    
    #region GetLastData

    public IEnumerator GetLastMatch(Action<MatchData> OnCompleted)
    {
        string requestUrl = url + $"GetLastMatch";

        if (string.IsNullOrEmpty(requestUrl))
        {
            Debug.LogError("GetLastMatch 요청 URL이 null이거나 비어있습니다.");
            OnCompleted?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                MatchData result = JsonConvert.DeserializeObject<MatchData>(json);
                Debug.Log($"데이터 조회 성공");
                OnCompleted?.Invoke(result);
            }
            else
            {
                Debug.LogError($"데이터 조회 실패: {www.responseCode} - {www.error}");
                OnCompleted?.Invoke(null);
            }
        }
    }
    #endregion

    #region GetDailyMatches

    public IEnumerator GetDailyMatches(Action<bool> OnCompleted, Action<List<RefinedMatchData>> OnCompletedDatas, string date)
    {
        string requestUrl = url + $"GetDailyMatches?date=" + date;

        if (string.IsNullOrEmpty(requestUrl))
        {
            Debug.LogError("GetDailyMatches 요청 URL이 null이거나 비어있습니다.");
            OnCompleted?.Invoke(false);
            OnCompletedDatas?.Invoke(null);
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                MatchData[] result = JsonConvert.DeserializeObject<MatchData[]>(json);
                // 라운드별로 그룹화
                var grouped = result
                    .GroupBy(m => m.round)
                    .OrderByDescending(g => g.Key)
                    .Select(g => g.ToArray())
                    .ToArray();
                
                Debug.Log($"불러온 라운드 수: {grouped.Length}");
                OnCompleted?.Invoke(true);
                OnCompletedDatas?.Invoke(RefineAllMatchData(grouped));
            }
            else
            {
                Debug.LogError($"데이터 조회 실패: {www.responseCode} - {www.error}");
                OnCompleted?.Invoke(false);
                OnCompletedDatas?.Invoke(null);
            }
        }
    }
    #endregion
    
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
    public List<RefinedMatchData> RefineAllMatchData(MatchData[][] groupedData)
    {
        var result = new List<RefinedMatchData>();
        foreach (var group in groupedData)
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
    
    #region 상속받았지만 사용하지 않는 함수
    /// <summary>
    /// 사용하지 않는 함수입니다. 
    /// </summary>
    public override IEnumerator AddData(Action<bool> OnCompleted, MatchData data)
    {
        throw new System.NotImplementedException("이 클래스에서 사용할 수 없는 함수 입니다.");
    }
    /// <summary>
    /// 사용하지 않는 함수입니다.
    /// </summary>
    protected override IEnumerator AddDataCoroutine(Action<bool> OnCompleted, MatchData data, string requestUrl)
    {
        throw new System.NotImplementedException("이 클래스에서 사용할 수 없는 함수 입니다.");
    }
    
    /// <summary>
    /// 사용하지 않는 함수입니다.
    /// </summary>
    public override IEnumerator GetAllData(Action<bool> OnCompleted, Action<MatchData[]> OnCompletedDatas)
    {
        throw new System.NotImplementedException("이 클래스에서 사용할 수 없는 함수 입니다."); 
    }

    /// <summary>
    /// 사용하지 않는 함수입니다.
    /// </summary>
    protected override IEnumerator GetAllDataCoroutine(Action<bool> OnCompleted, Action<MatchData[]> OnCompletedDatas, string requestUrl)
    {
        throw new System.NotImplementedException("이 클래스에서 사용할 수 없는 함수 입니다."); 
    }
    
    
    #endregion
}
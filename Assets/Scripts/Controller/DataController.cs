using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OverwatchMatchData;
using TMPro;
using UGS;
using UnityEngine;
using UnityEngine.UI;

public class DataController : MonoBehaviour
{
    #region SerializeField
    /// <summary>
    /// Data Property
    /// </summary>
    [SerializeField] private TMP_Text _mapName;
    [SerializeField] private Text _date;
    //플레이어 입력 정보
    [SerializeField] private TMP_Text _blueDeal00Name;
    [SerializeField] private TMP_Text _blueDeal01Name;
    [SerializeField] private TMP_Text _blueTank00Name;
    [SerializeField] private TMP_Text _blueHeal00Name;
    [SerializeField] private TMP_Text _blueHeal01Name;
    [SerializeField] private TMP_Text _redDeal00Name;
    [SerializeField] private TMP_Text _redDeal01Name;
    [SerializeField] private TMP_Text _redTank00Name;
    [SerializeField] private TMP_Text _redHeal00Name;
    [SerializeField] private TMP_Text _redHeal01Name;
    [SerializeField] private TeamType _winnerTeam;
    
    [SerializeField] private FindMapPanelController _findMapPanelController;
    #endregion
    
    #region Private
    private Dictionary<string, RoleType> _blueTeamPlayerRole;
    private Dictionary<string, RoleType> _redTeamPlayerRole;
    #endregion

    private void Start()
    {
        _findMapPanelController.OnMapClicked += ChangeMap;
    }

    public void Initialize()
    {
        ResetInputValues();
    }
    private void ResetInputValues()
    {
        _mapName.text = "-";
        _blueDeal00Name.text = "-";
        _blueDeal01Name.text = "-";
        _blueTank00Name.text = "-";
        _blueHeal00Name.text = "-";
        _blueHeal01Name.text = "-";
        _redDeal00Name.text = "-";
        _redDeal01Name.text = "-";
        _redTank00Name.text = "-";
        _redHeal00Name.text = "-";
        _redHeal01Name.text = "-";
        _blueTeamPlayerRole = new Dictionary<string, RoleType>();
        _redTeamPlayerRole = new Dictionary<string, RoleType>();
        _winnerTeam = TeamType.Default;
    }

    private void ChangeMap(string mapName)
    {
        _mapName.text = mapName;
        this.GetComponent<GameResultPanelController>().CloseAllPanel();
    }
    private void MakeTeamDictionary()
    {
        _blueTeamPlayerRole = new Dictionary<string, RoleType>();
        _redTeamPlayerRole = new Dictionary<string, RoleType>();
        //블루팀 정보 저장
        _blueTeamPlayerRole.Add(_blueDeal00Name.text, RoleType.D);
        _blueTeamPlayerRole.Add(_blueDeal01Name.text, RoleType.D);
        _blueTeamPlayerRole.Add(_blueTank00Name.text, RoleType.T);
        _blueTeamPlayerRole.Add(_blueHeal00Name.text, RoleType.H);
        _blueTeamPlayerRole.Add(_blueHeal01Name.text, RoleType.H);
        //레드팀 정보 저장
        _redTeamPlayerRole.Add(_redDeal00Name.text, RoleType.D);
        _redTeamPlayerRole.Add(_redDeal01Name.text, RoleType.D);
        _redTeamPlayerRole.Add(_redTank00Name.text, RoleType.T);
        _redTeamPlayerRole.Add(_redHeal00Name.text, RoleType.H);
        _redTeamPlayerRole.Add(_redHeal01Name.text, RoleType.H);
    }
    
    #region Setting Data
    /// <summary>
    /// 한 경기의 데이터를 구성한다.
    /// </summary>
    /// <returns></returns>
    public List<Data> MakeNewData()
    {
        //팀Dictionary 초기화(중요!)
        MakeTeamDictionary();
        
        List<Data> newDataList = new List<Data>();
        //팀 최대 플레이어 수 적용
        int maxPlayerInTeam = _blueTeamPlayerRole.Count;
        
        int currentIndex = DataManager.instance.DataList.Last().Index + 1;
        int currentRound = DataManager.instance.DataList.Last().Round + 1;
        
        string typeAD = GetAttackDefenseChange(_mapName.text);
        
        //리스트 생성
        for (int i = 0; i < Constants.MAXTEAMCOUNT; i++)
        {
            for (int j = 0; j < maxPlayerInTeam; j++)
            {
                Data data = new Data();
                data.Index = currentIndex;
                data.Date = _date.text;
                data.Round = currentRound;
                data.MapName = _mapName.text;
                
                if (typeAD != null) data.AttackDefenseType = typeAD;
                
                if (_winnerTeam == TeamType.BlueTeam)
                {
                    //블루팀 승리 반영
                    if (i == 0)
                    {
                        data.PlayerName = _blueTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _blueTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "후공";
                        data.WinLoseType = WinLoseType.승;
                    }
                    //레드팀 패배 반영
                    else
                    {
                        data.PlayerName = _redTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _redTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "선공";
                        data.WinLoseType = WinLoseType.패;
                    }
                }
                else if (_winnerTeam == TeamType.RedTeam)
                {
                    //레드팀 승리 반영
                    if (i == 0)
                    {
                        data.PlayerName = _redTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _redTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "후공";
                        data.WinLoseType = WinLoseType.승;
                    }
                    //블루팀 패배 반영
                    else
                    {
                        data.PlayerName = _blueTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _blueTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "선공";
                        data.WinLoseType = WinLoseType.패;
                    }
                }
                else
                {
                    //블루팀 무승부 반영
                    if (i == 0)
                    {
                        data.PlayerName = _blueTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _blueTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "후공";
                        data.WinLoseType = WinLoseType.무;
                    }
                    //레드팀 무승부 반영
                    else
                    {
                        data.PlayerName = _redTeamPlayerRole.ElementAt(j).Key;
                        data.RoleType = _redTeamPlayerRole.ElementAt(j).Value;
                        data.AttackDefenseType ??= "선공";
                        data.WinLoseType = WinLoseType.무;
                    }
                }
                
                newDataList.Add(data);
                
                currentIndex++;
            }
        }
        return newDataList;
    }

    /// <summary>
    /// 공수교대 맵인경우 -1 return, 아닌경우 MapType이 그대로 return
    /// </summary>
    /// <param name="mapName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private string GetAttackDefenseChange(string mapName)
    {
        string currentMapType = null;
        //맵 정보에서 현재 맵의 타입을 확인함
        foreach (var map in DataManager.instance.MapList)
        {
            if (mapName != map.MapName) continue;
            
            currentMapType = map.Type;
        }
        
        foreach (var type in DataManager.instance.MapTypeList)
        {
            if (type.TypeName != currentMapType) continue;
            
            //공수전환 맵이면 null을 리턴
            if (type.IsAttackDefenseType == 1) return null;
            //공수전환 맵이 아니면 타입 값을 리턴
            return type.TypeName;
            
        }

        //MapTypeList안에 currentMapType이 무조건 존재하기 때문에 아래 null은 에러처리
        throw new Exception("MapType이 제대로 지정되지 않았습니다. 스프레드시트를 확인하세요");
    }
    #endregion
    
    #region SaveToDB
    /// <summary>
    /// 한 경기의 데이터를 모두 추가
    /// </summary>
    /// <param name="dataList"></param>
    public void AddDataToDB(List<Data> dataList)
    {
        foreach (Data data in dataList)
        {
            UnityGoogleSheet.Write(data);
        }
    }
    
    #endregion
}

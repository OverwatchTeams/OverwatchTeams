using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public int Index;                                       //인덱스
    public string Name;                                     //이름
    public bool bIsClanMember;                              //현재 클랜 멤버인지 여부
    public float AllTimeWinRate;                            //연간 승률
    public float MonthlyWinRate;                            //월간 승률
    public Dictionary<string, float> AllTimeMapWinRate;     //전체 맵별 승률
    public Dictionary<string, float> MonthlyMapWinRate;     //월간 맵별 승률
    public Dictionary<RoleType, float> AllTimeRoleWinRate;  //전체 역할군별 승률
    public Dictionary<RoleType, float> MonthlyRoleWinRate;  //월간 역할군별 승률
    public float AllTimeParticipationRate;                  //전체참여율
    public float MonthlyParticipationRate;                  //월간참여율
    public int PlayerScore;                                 //플레이어 점수
    public int RecentSeq;                                   //최근 참여순번

}

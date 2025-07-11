using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public enum Role
{
    D,
    T,
    H,
    Default
}

public enum WinLose
{
    승,
    패,
    무
}

public enum AtkDefType
{
    선공,
    후공,
    중립,
}

public enum WinnerTeam
{
    블루,
    레드,
    무승부
}

[Serializable]
public class DailyGameData
{
    public string updateDate;
    public GameDataContainer gameDatas;
    public LeaderBoardContainer leaderBoard;
    
    [Serializable]
    public class GameDataContainer
    {
        public Dictionary<string, GameDatas> byDay;
    }
    
    [Serializable]
    public class LeaderBoardContainer
    {
        public Dictionary<string, WinRateData> byDay;
    }
    
    [Serializable]
    public class WinRateData
    {
        public WinRate winRate; // 승률 데이터
        public Attendance attendance; // 출석 데이터
        
        [Serializable]
        public class WinRate
        {
            public Dictionary<string, PlayerWinRate> total; // Key는 플레이어 이름, Value는 승률 데이터
            
            [Serializable]
            public class PlayerWinRate
            {
                public int ranking;
                public float winRate;
                public int wins;
                public int draws;
                public int losses;
                public int gap;
                public int streak;
            }
        }
        
        [Serializable]
        public class Attendance
        {
            public Dictionary<string, AttendanceDetail> total; // Key는 플레이어 이름, Value는 출석 데이터
            public Dictionary<string, MapAttendance> map; // 맵별 출석 데이터
        
            [Serializable]
            public class AttendanceDetail
            {
                public int ranking;
                public int playedGames;
                public int totalGames;
                public bool isMinRequired;
            }
            [Serializable]
            public class MapAttendance
            {
                public int playedGames;
                public int totalGames;
            }
        }
    }
}
[System.Serializable]
public class MatchData
{
    public int index;
    public string date;
    public int round;
    public string map;
    [JsonConverter(typeof(StringEnumConverter))]
    public Role role;
    public string player;
    [JsonConverter(typeof(StringEnumConverter))]
    public AtkDefType atkdef;
    [JsonConverter(typeof(StringEnumConverter))]
    public WinLose winlose;
}

public class RefinedMatchData
{
    public int beginIndex;
    public DateTime date;
    public int round;
    public string map;
    public List<(string, Role)> players;
    public WinnerTeam winner;
}

[System.Serializable]
public class MapData
{
    public string _id;
    public int index;
    public string name;
    public string type;
}

[System.Serializable]
public class MapTypeData
{
    public string _id;
    public int index;
    public string name;
    public bool isAtkDef;
}

[Serializable]
public class PlayerData
{
    public string player;
    public bool? isClanMember;
    public bool? isVoiceAvailable;
    public Date dates;
    public Scores scores;
    public List<string> subNames;
    public WinRate winRates;
    public Dictionary<string, PositionSet> synergy;

    [Serializable]
    public class Date
    {
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime first;
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime last;
        [JsonConverter(typeof(DefaultIntConverter))]
        public int lastRound;
    }
    
    [Serializable]
    public class Scores
    {
        public int D = 0;
        public int T = 0;
        public int H = 0;
    }
    
    [Serializable]
    public class WinRate
    {
        public Dictionary<string, WinRateDetail> byYear;
        public Dictionary<string, WinRateDetail> byMonth;

        [Serializable]
        public class WinRateDetail
        {
            public GameDetail total;
            public Dictionary<string, GameDetail> map;
            public RoleGame role;
            public RoleMapGame roleMap; 

            public class GameDetail
            {
                public int playedGames;
                public int requiredGames;
                public int wins;
                public int draws;
                public float rate;
            }

            public class RoleGame
            {
                public GameDetail D;
                public GameDetail T;
                public GameDetail H;
            }
            public class RoleMapGame
            {
                public Dictionary<string, GameDetail> D;
                public Dictionary<string, GameDetail> T;
                public Dictionary<string, GameDetail> H;
            }
        }
    }

    [Serializable]
    public class PositionSet
    {
        [JsonProperty("D-D")] public Team D_D;
        [JsonProperty("D-T")] public Team D_T;
        [JsonProperty("D-H")] public Team D_H;
        [JsonProperty("T-D")] public Team T_D;
        [JsonProperty("T-H")] public Team T_H;
        [JsonProperty("T-T")] public Team T_T;
        [JsonProperty("H-D")] public Team H_D;
        [JsonProperty("H-T")] public Team H_T;
        [JsonProperty("H-H")] public Team H_H;
        

        public class Team
        {
            public TeamDetail sameTeam;
            public TeamDetail oppositeTeam;

            public class TeamDetail
            {
                public int games;
                public int wins;
            }
        }
    }
}

[Serializable]
public class RecordDropdownDate
{
    public List<string> year;
    public List<string> month;
    public List<string> day;
}

public class PlayerURLData
{
    public string isClanMember;
    public List<string> fields;
    public string sortType;
}

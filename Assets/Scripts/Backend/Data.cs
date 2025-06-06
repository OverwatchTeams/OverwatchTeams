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
    public Scores scores;

    [Serializable]
    public class Scores
    {
        public int D = 0;
        public int T = 0;
        public int H = 0;
    }
}

[Serializable]
public class TotalGameData
{
    
}
[Serializable]
public class FilteredGameData
{
    
}

[Serializable]
public class TotalLeaderBoardData
{
    
}

[Serializable]
public class RecordDropdownDate
{
    public List<string> year;
    public List<string> month;
}

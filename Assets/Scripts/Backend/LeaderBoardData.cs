using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WinRateByYearData
{
    public Dictionary<string, WinRateData> byYear;
}

[Serializable]
public class WinRateData
{
    public WinRate winRate; // 승률 데이터
    public Attendance attendance; // 출석 데이터
}

[Serializable]
public class WinRate
{
    public Dictionary<string, PlayerWinRate> total; // Key는 플레이어 이름, Value는 승률 데이터
    public RoleWinRate role; // 역할별 승률 데이터
    public Dictionary<string, MapWinRate> map; // 맵별 승률 데이터
    public RoleMapWinRate roleMap; // 역할-맵별 승률 데이터
}

[Serializable]
public class PlayerWinRate
{
    public int ranking;
    public float winRate;
    public int wins;
    public int draws;
    public int losses;
    public bool isMinRequired;
}

[Serializable]
public class RoleWinRate
{
    public SimpleWinRate D; // 역할 D
    public SimpleWinRate T; // 역할 T
    public SimpleWinRate H; // 역할 H
}

[Serializable]
public class SimpleWinRate
{
    public float winRate;
    public int wins;
    public int draws;
    public int losses;
}

[Serializable]
public class MapWinRate
{
    public float winRate;
    public int wins;
    public int draws;
    public int losses;
}

[Serializable]
public class RoleMapWinRate
{
    public Dictionary<string, MapWinRate> D; // 역할 D의 맵별 데이터
    public Dictionary<string, MapWinRate> T; // 역할 T의 맵별 데이터
    public Dictionary<string, MapWinRate> H; // 역할 H의 맵별 데이터
}

[Serializable]
public class Attendance
{
    public Dictionary<string, AttendanceDetail> total; // Key는 플레이어 이름, Value는 출석 데이터
    public Dictionary<string, MapAttendance> map; // 맵별 출석 데이터
}

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

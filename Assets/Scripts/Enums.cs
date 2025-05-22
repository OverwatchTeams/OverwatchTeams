using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GoogleSheet.Core.Type;
using UnityEngine;

[UGS(typeof(WinLoseType))]
public enum WinLoseType
{
    Default,
    무,
    승,
    패
}

[UGS(typeof(RoleType))]
public enum RoleType
{
    Default,
    T,
    D,
    H
}

public enum TeamType
{
    Default,
    Draw,
    BlueTeam,
    RedTeam,
}

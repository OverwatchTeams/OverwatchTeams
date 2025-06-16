using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameDatas
{
    public int totalGames;
    public Dictionary<string, int> map; //맵이름, 총 라운드 수
    public int minRequiredRound;
}

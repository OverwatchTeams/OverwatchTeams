using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OverwatchMatchData;
using UGS;
using UnityEngine;

public class MapController : MonoBehaviour
{
    #region SerializeField
    /// <summary>
    /// Map Property
    /// </summary>
    [SerializeField] private string _mapName;
    [SerializeField] private string _mapType;
    #endregion

    private void ResetInputValues()
    {
        _mapName = null;
        _mapType = null;
    }

    #region Setting Map

    public Map MakeNewMap()
    {
        Map newMap = new Map();
        newMap.Index = DataManager.instance.MapList.Last().Index + 1;
        newMap.MapName = _mapName;
        newMap.Type = _mapType;
        return newMap;
    }
    #endregion
    
    #region SaveToDB
    /// <summary>
    /// Map 추가
    /// </summary>
    /// <param name="map"></param>
    public void AddMapToDB(Map map)
    {
        UnityGoogleSheet.Write(map);
    }
    #endregion
}

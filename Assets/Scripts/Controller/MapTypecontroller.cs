using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OverwatchMatchData;
using UGS;
using UnityEngine;

public class MapTypecontroller : MonoBehaviour
{
    /// <summary>
    /// MapType Property
    /// </summary>
    [SerializeField] private string _maptypeName;
    [SerializeField] private bool _maptypeIsAttackDefenseType;
    
    /// <summary>
    /// AttackDefenseType Property
    /// </summary>
    [SerializeField] private string _attackdefensetypeName;
    
    private void ResetInputValues()
    {
        _maptypeName = null;
        _maptypeIsAttackDefenseType = false;
        _attackdefensetypeName = null;
    }
    
    #region Setting MapType

    public MapType MakeNewMapType()
    {
        MapType newMapType = new MapType();
        newMapType.Index = DataManager.instance.MapTypeList.Last().Index + 1;
        newMapType.TypeName = _maptypeName;
        newMapType.IsAttackDefenseType = Convert.ToInt32(_maptypeIsAttackDefenseType);
        return newMapType;
    }
    #endregion
    
    #region Setting AttackDefenseType

    public AttackDefenseType MakeNewAttackDefenseType()
    {
        AttackDefenseType newAttackDefenseType = new AttackDefenseType();
        newAttackDefenseType.Index = DataManager.instance.AttackDefenseTypeList.Last().Index + 1;
        newAttackDefenseType.TypeName = _attackdefensetypeName;
        return newAttackDefenseType;
    }
    #endregion
    
    /// <summary>
    /// MapType 추가
    /// </summary>
    /// <param name="mapType"></param>
    public void SaveMapTypeToDB(MapType mapType)
    {
        UnityGoogleSheet.Write(mapType);
        
        //만일 공수교대형 맵인경우 공수교대타입 추가 
        if (mapType.IsAttackDefenseType == 1) return;
        _attackdefensetypeName = mapType.TypeName;
        AttackDefenseType newAttackDefenseType = MakeNewAttackDefenseType();
        SaveAttackDefenseTypeToDB(newAttackDefenseType);
    }
    
    /// <summary>
    /// 공수교대 맵인지 확인 하고 공수교대 맵Type이 아니라면 호출
    /// </summary>
    /// <param name="attackDefenseType"></param>
    public void SaveAttackDefenseTypeToDB(AttackDefenseType attackDefenseType)
    {
        UnityGoogleSheet.Write(attackDefenseType);
    }
}

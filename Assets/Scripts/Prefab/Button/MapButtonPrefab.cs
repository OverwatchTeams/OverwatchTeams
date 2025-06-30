using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtonPrefab : MonoBehaviour
{
    private MapData mapData;
    public MapData MapData => mapData;

    public MapData SetMapData(MapData newMapData)
    {
        return mapData = newMapData;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonPrefab : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    public PlayerData PlayerData => playerData;

    public PlayerData SetPlayerData(PlayerData newPlayerData)
    {
        return playerData = newPlayerData;
    }
}

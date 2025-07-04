using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerButtonPrefab : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] private Animator _animator;
    //[SerializeField] private SwipdownTextAnimation _swipdownTextAnimation;
    [SerializeField] private TMP_Text _name;
    public PlayerData PlayerData => playerData;

    public PlayerData SetPlayerData(PlayerData newPlayerData)
    {
        //StopAnimation();
        playerData = newPlayerData;
        _name.text = newPlayerData.player;
        /*List<string> playerNames = new List<string>();
        playerNames.Add(playerData.player);
        if (playerData.subNames != null)
        {
            foreach (var pName in playerData.subNames)
            {
                playerNames.Add(pName);
            }   
        }
        _swipdownTextAnimation.SetTexts(playerNames);
        StartAnimation();*/
        return playerData;
    }

    /*private void StartAnimation()
    {
        if (playerData == null) return;
        if (playerData.subNames == null || playerData.subNames.Count == 0) return;
        if(_animator.enabled == false)
        {
            _animator.enabled = true;
        }
        _animator.Play("Anim_SwipdownText",0,0);   
    }
    private void StopAnimation()
    {
        if (playerData.subNames == null || playerData.subNames.Count == 0) return;
        _animator.enabled = false;
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        StopAnimation();
    }*/
}

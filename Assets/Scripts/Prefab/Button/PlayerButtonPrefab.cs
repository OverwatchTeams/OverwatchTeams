using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerButtonPrefab : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PlayerData playerData;

    [SerializeField] private TMP_Text _name;
    public PlayerData PlayerData => playerData;
    [SerializeField] private GameObject _leftBadge;
    [SerializeField] private GameObject _rightBadge;
    [SerializeField] private TMP_Text[] _streaks;
    [SerializeField] private TMP_InputField[] _scores;
    [SerializeField] private Image[] _voices;
    
    //MakeTeam - Prediction 관련 프로퍼티
    [SerializeField] private bool _isRightClickEnabeled = false;
    public Action<GameObject> OnRightClick;
    public Role _role;

    private void OnEnable()
    {
        foreach (var score in _scores)
        {
            score.onEndEdit.RemoveListener(OnInputFieldChanged);
            score.onEndEdit.AddListener(OnInputFieldChanged);
        }
    }

    private void OnInputFieldChanged(string value)
    {
        foreach (var score in _scores)
        {
            score.text = value + "점";
        }
        if (value != "" && playerData.player != "-")
        {
            PlayerData data = new PlayerData();
            data.scores = new PlayerData.Scores();
            data.player = playerData.player;
            float result;
            switch (_role)
            {
                case Role.D:
                    if (float.TryParse(value, out result))
                    {
                        data.scores.D = (int)(result * 100);
                        data.scores.T = playerData.scores.T;
                        data.scores.H = playerData.scores.H;
                    }
                    break;
                case Role.T:
                    if (float.TryParse(value, out result))
                    {
                        data.scores.D = playerData.scores.D;
                        data.scores.T = (int)(result * 100);
                        data.scores.H = playerData.scores.H;
                    }
                    break;
                case Role.H:
                    if (float.TryParse(value, out result))
                    {
                        data.scores.D = playerData.scores.D;
                        data.scores.T = playerData.scores.T;
                        data.scores.H = (int)(result * 100);
                    }
                    break;
            }
            Debug.Log($"{data.player} : {data.scores.D.ToString()}");
            Debug.Log($"{data.player} : {data.scores.T.ToString()}");
            Debug.Log($"{data.player} : {data.scores.H.ToString()}");
            
            StartCoroutine(DataController.instance.CreatePlayer(null, data));
        }
    }
    public void SetBadgePosition(bool isLeft)
    {
        if (isLeft)
        {
            _leftBadge.SetActive(true);
            _rightBadge.SetActive(false);
        }
        else
        {
            _leftBadge.SetActive(false);
            _rightBadge.SetActive(true);
        }

        foreach (var streak in _streaks)
        {
            streak.text = "0";   
        }
        foreach (var score in _scores)
        {
            score.text = "0점";  
        }
    }
    
    public void SetBadge(int streak)
    {
        if (streak >= 0)
        {
            foreach (var _streak in _streaks)
            {
                _streak.text = streak.ToString();  
            }
        }
        else
        {
            foreach (var _streak in _streaks)
            {
                _streak.text = streak.ToString();
            }
        }

        float score = 0.0f;
        float sValue = 0;
        switch (_role)
        {
            case Role.D:
                sValue = (float)playerData.scores.D / 100;
                score = sValue;
                break;
            case Role.T:
                sValue = (float)playerData.scores.T / 100;
                score = sValue;
                break;
            case Role.H:
                sValue = (float)playerData.scores.H / 100;
                score = sValue;
                break;
        }
        
        foreach (var _score in _scores)
        {
            _score.text = (score % 1 == 0)
                ? ((int)score) + "점"
                : string.Format($"{score:0.##}") + "점";
        }
        
        foreach (var _voice in _voices)
        {
            if (playerData.isVoiceAvailable.HasValue)
            {
                _voice.sprite = Resources.Load<Sprite>(
                    playerData.isVoiceAvailable.Value 
                        ? "Images/MicOn" 
                        : "Images/MicOff"
                );
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _isRightClickEnabeled)
        {
            OnRightClick?.Invoke(gameObject);
        }
    }

    public PlayerData SetPlayerData(PlayerData newPlayerData)
    {
        playerData = newPlayerData;
        _name.text = newPlayerData.player;
        return playerData;
    }
}

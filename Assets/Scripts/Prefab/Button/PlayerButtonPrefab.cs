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
    [SerializeField] private TMP_Text[] _winRates;
    [SerializeField] private TMP_InputField[] _scores;
    [SerializeField] private Image[] _voices;
    [SerializeField] private GameObject[] _scoreMasks;
    public GameObject SelectMask;
    
    //MakeTeam - Prediction 관련 프로퍼티
    [SerializeField] private bool _isRightClickEnabled = false;
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
            StartCoroutine(DataController.instance.CreatePlayer(null, data));
        }
    }
    public void SetBadgePosition(bool isLeft)
    {
        if (isLeft)
        {
            _leftBadge.SetActive(true);
            _rightBadge.SetActive(false);
            gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>("Images/PlayerButton01");
        }
        else
        {
            _leftBadge.SetActive(false);
            _rightBadge.SetActive(true);
            gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>("Images/PlayerButton02");
        }

        foreach (var _winRate in _winRates)
        {
            _winRate.text = "0";   
        }
        foreach (var _score in _scores)
        {
            _score.text = "0점";  
        }
    }
    
    public void SetBadge(float winRate)
    {
        if (winRate >= 0)
        {
            foreach (var _winRate in _winRates)
            {
                _winRate.text = winRate.ToString("F0") + "%";  
            }
        }
        else
        {
            foreach (var _winRate in _winRates)
            {
                _winRate.text = winRate.ToString("F0") + "%";
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
        if (eventData.button == PointerEventData.InputButton.Right && _isRightClickEnabled)
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

    public void SetScoreMask(bool isScoreViewable)
    {
        if(isScoreViewable)
        {
            foreach (var _scoreMask in _scoreMasks)
            {
                _scoreMask.SetActive(false);
            }
            
        }
        else
        {
            foreach (var _scoreMask in _scoreMasks)
            {
                _scoreMask.SetActive(true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyMatchPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text _round;
    [SerializeField] private GameObject _blueCrown;
    [SerializeField] private GameObject _redCrown;
    [SerializeField] private List<PlayerButtonPrefab> _playerNames;

    public void SetDailyMatchPrefab(RefinedMatchData matchData)
    {
        _round.text = matchData.round.ToString();
        if (matchData.winner == WinnerTeam.블루)
        {
            _blueCrown.SetActive(true);
            _redCrown.SetActive(false);
        }
        else if (matchData.winner == WinnerTeam.레드)
        {
            _blueCrown.SetActive(false);
            _redCrown.SetActive(true);
        }
        else
        {
            _blueCrown.SetActive(false);
            _redCrown.SetActive(false);
        }

        for (int i = 0; i < matchData.players.Count; i++)
        {
            foreach (var cachedPlayer in DataController.instance.ClanPlayers)
            {
                if (cachedPlayer.player != matchData.players[i].Item1) continue;
                _playerNames[i].SetPlayerData(cachedPlayer);
                break;
            }
        }
    }
}

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
        int i = 0;
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

        foreach (var player in matchData.players)
        {
            foreach (var findthing in DataController.instance.ClanPlayers)
            {
                if (findthing.player == player.Item1)
                {
                    _playerNames[i].SetPlayerData(findthing);
                    i++;
                }
            }
        }
    }
}

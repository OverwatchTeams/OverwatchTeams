using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScorePrefab : MonoBehaviour
{
    public TMP_Text _name;
    public TMP_InputField _dealerScore;
    public TMP_InputField _tankerScore;
    public TMP_InputField _healerScore;
    public Image _delerBackground;
    public Image _tankerBackground;
    public Image _healerBackground;

    private void Start()
    {
       _dealerScore.onEndEdit.AddListener(OnDealerInputFieldChanged);
       _tankerScore.onEndEdit.AddListener(OnTankerInputFieldChanged);
       _healerScore.onEndEdit.AddListener(OnHealerInputFieldChanged);
    }

    private void OnDealerInputFieldChanged(string text)
    {
        float value = float.Parse(text, CultureInfo.InvariantCulture);
        _dealerScore.text = value % 1 == 0 
            ? ((int)value).ToString(CultureInfo.InvariantCulture)
            : value.ToString("F2", CultureInfo.InvariantCulture);
    }
    private void OnTankerInputFieldChanged(string text)
    {
        float value = float.Parse(text, CultureInfo.InvariantCulture);
        _tankerScore.text = value % 1 == 0 
            ? ((int)value).ToString(CultureInfo.InvariantCulture)
            : value.ToString("F2", CultureInfo.InvariantCulture);
    }
    private void OnHealerInputFieldChanged(string text)
    {
        float value = float.Parse(text, CultureInfo.InvariantCulture);
        _healerScore.text = value % 1 == 0 
            ? ((int)value).ToString(CultureInfo.InvariantCulture)
            : value.ToString("F2", CultureInfo.InvariantCulture);
    }

    private void OnDestroy()
    {
        _dealerScore.onEndEdit.RemoveAllListeners();
        _tankerScore.onEndEdit.RemoveAllListeners();
        _healerScore.onEndEdit.RemoveAllListeners();
    }
}

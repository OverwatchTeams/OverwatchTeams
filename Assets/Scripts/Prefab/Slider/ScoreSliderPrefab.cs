using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSliderPrefab : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _backGroundColor;
    [SerializeField] private Image _sliderColor;
    [SerializeField] private TMP_Text _blueScore;
    [SerializeField] private TMP_Text _redScore;

    public void SetScoreSliderPrefab(float blueScore, float redScore)
    {
        ColorUtility.TryParseHtmlString("#213B79FF", out var blueColor);
        ColorUtility.TryParseHtmlString("#7E2D3BFF", out var redColor);
        _blueScore.text = string.Format($"{blueScore:0.##}");
        _redScore.text = string.Format($"{redScore:0.##}");
        if (blueScore == 0 && redScore == 0)
        {
            _slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            _slider.value = 0.5f;
            _sliderColor.color = blueColor;
            _backGroundColor.color = redColor;
        }
        if (blueScore > redScore)
        {
            _slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            _slider.value = blueScore / (blueScore + redScore);
            _sliderColor.color = blueColor;
            _backGroundColor.color = redColor;
        }
        else if (blueScore < redScore)
        {
            _slider.direction = UnityEngine.UI.Slider.Direction.RightToLeft;
            _slider.value = redScore / (blueScore + redScore);
            _sliderColor.color = redColor;
            _backGroundColor.color = blueColor;
        }
    }
}

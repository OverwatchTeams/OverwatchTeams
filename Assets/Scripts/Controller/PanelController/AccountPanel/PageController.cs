using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    [Header("Default")] [SerializeField] private GameObject[] _panels;
    [SerializeField] private TMP_Text _pageIndex;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _nextButton;

    private int currentPage;

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeListeners();
        CloseAllPages();
        //첫 페이지 노출
        currentPage = 1;
        _panels[currentPage].SetActive(true);
    }

    private void InitializeListeners()
    {
        _backButton.onClick.AddListener(OnClickBackButton);
        _nextButton.onClick.AddListener(OnClickBackButton);
    }

    private void OnClickBackButton()
    {
        currentPage--;
        OpenPage();
    }

    private void OnClickNextButton()
    {
        currentPage++;
        OpenPage();
    }

    private void CloseAllPages()
    {
        foreach (GameObject panel in _panels)
        {
            panel.SetActive(false);
        }
    }

    private void OpenPage()
    {
        CloseAllPages();
        _backButton.gameObject.SetActive(true);
        _nextButton.gameObject.SetActive(true);
        _panels[currentPage].SetActive(true);
        if( currentPage == _panels.Length) _nextButton.gameObject.SetActive(false);
        if( currentPage == 1) _backButton.gameObject.SetActive(false);
    }

    private void UpdatePageIndex()
    {
        _pageIndex.text = currentPage + " / " + _panels.Length;
    }

}

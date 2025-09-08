using System;
using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SignInPanelController : PanelController
{
    [Header("SignIn Panel/Main")] 
    [SerializeField] private TMP_InputField _userEmail;
    [SerializeField] private TMP_InputField _userPassword;
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private Toggle _autoSignInToggle;
    [SerializeField] private TMP_Text loginFailText;
    [SerializeField] private ProgressBarLoop _circularProgressPopup;
    
    [Header("SignIn Panel/ChangePassword")]
    [SerializeField] private Button _changePasswordButton;

    private void OnEnable()
    {
        _autoSignInToggle.isOn = false;
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
        InitializeUserData();
        if (_autoSignInToggle.isOn)
        {
            OnClickSignInButton();
        }
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _signInButton.onClick.AddListener(OnClickSignInButton);
        _signUpButton.onClick.AddListener(OnClickSignUpButton);
        _autoSignInToggle.onValueChanged.AddListener(ToggleCheckBox);
    }

    private void InitializeUserData()
    {
        _autoSignInToggle.isOn = PlayerPrefs.GetInt("AutoSignIn", 0) == 1;
        // 저장된 로그인 정보 불러오기
        if (_autoSignInToggle.isOn)
        {
            _userEmail.text = PlayerPrefs.GetString("SignInEmail", "");
            _userPassword.text = PlayerPrefs.GetString("SignInPassword", "");
        }
        else
        {
            _userEmail.text = "";
            _userPassword.text = "";
        }
        // 로그인 실패 문구 숨기기
        if (loginFailText != null) loginFailText.gameObject.SetActive(false);
    }
    
    private void ToggleCheckBox(bool value)
    {
        PlayerPrefs.SetInt("StayLoggedIn", value ? 1 : 0);
        // 체크 해제 시 저장된 정보 초기화
        if (!value)
        {
            PlayerPrefs.DeleteKey("SignInEmail");
            PlayerPrefs.DeleteKey("SignInPassword");
        }
    }

    private void SetErrorMessage(string errorMessage)
    {
        loginFailText.gameObject.SetActive(true);
        loginFailText.text = errorMessage;
    }

    private void OnClickSignInButton()
    {
        string enteredEmail = _userEmail.text.Trim();
        string enteredPw = _userPassword.text.Trim();
        
        _circularProgressPopup.gameObject.SetActive(true);
        
        StartCoroutine(UserDataManager.instance.GetUserData(
            ResponseHandlers.Create<UserData>(
                data =>
                {
                    Initialize();
                    OpenNextPanel("[Panel] MainMenuPanel");
                },
                message =>
                {
                    SetErrorMessage(message);
                }),
            enteredEmail, enteredPw));
        _circularProgressPopup.gameObject.SetActive(false);
    }

    private void OnClickSignUpButton()
    {
        OpenNextPanel("[Panel] SignUpPanel");
    }
}

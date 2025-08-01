using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanelController : PanelController
{
    [Header("SignUp Panel/Default")]
    [SerializeField] private TMP_InputField _userEmail;
    [SerializeField] private Button _emailConfirmButton;
    [SerializeField] private TMP_InputField _userPassword;
    [SerializeField] private TMP_InputField _userPasswordConfirm;
    [SerializeField] private TMP_InputField _battleTag;
    [SerializeField] private Switch _isClanMaster;
    [SerializeField] private Button _signUpButton;
    [SerializeField] private GameObject _masterGameObject;
    [SerializeField] private GameObject _memberGameObject;
    [SerializeField] private Button _signUpConfirmButton;
    [SerializeField] private TMP_Text _signUpFailedText;
    
    [Header("SignUp Panel/ClanMaster")]
    [SerializeField] private TMP_InputField _clanName;
    [SerializeField] private Button _uploadButton;
    [SerializeField] private TMP_Text _uploadPath;
    
    [Header("SignUp Panel/ClanMember")]
    [SerializeField] private TMP_InputField _clanId;
    
    private string _path;
    
    private void OnEnable()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeListeners();
    }

    protected override void InitializeListeners()
    {
        base.InitializeListeners();
        _uploadButton.onClick.AddListener(OnClickUploadButton);
        _isClanMaster.OnValueChanged.AddListener(OnChangeClanMasterSwitch);
        _signUpButton.onClick.AddListener(OnClickSignUpButton);
    }
    
    private void OnChangeClanMasterSwitch(bool value)
    {
        if (value)
        {
            _memberGameObject.SetActive(false);
            _masterGameObject.SetActive(true);
        }
        else
        {
            _memberGameObject.SetActive(true);
            _masterGameObject.SetActive(false);
        }
    }
    private void OnClickUploadButton()
    {
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };
        WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true));
    }

    private void OnClickSignUpButton()
    {
        if (_isClanMaster.IsOn)
        {
            /*StartCoroutine(UserDataManager.instance.CreateClanMasterUser(response =>
                {
                    if (response.responseCode == 200)
                    {
                        Initialize();
                        //WIP
                        OpenNextPanel("[Panel] MainMenuPanel");
                    }
                    else if (response.responseCode == 401)
                    {
                        if (response.responseText == "Email is invalid")
                        {
                            SetErrorMessage("이메일이 존재하지 않습니다.");
                        }
                        else if (response.responseText == "Password is invalid")
                        {
                            SetErrorMessage("비밀번호가 정확하지 않습니다.");
                        }
                    }
                    else if (response.responseCode == 404)
                    {
                        SetErrorMessage($"{response.responseCode} - {response.responseText}");
                    }
                }
                , enteredEmail, enteredPw));*/
        }
        else
        {
            
        }
    }
    
    private void SetErrorMessage(string errorMessage)
    {
        _signUpFailedText.gameObject.SetActive(true);
        _signUpFailedText.text = errorMessage;
    }
    
    private void WriteResult(string[] paths) {
        if (paths.Length == 0) {
            return;
        }

        _path = "";
        foreach (var p in paths) {
            _path += p + "\n";
        }
        _uploadPath.text = _path;
    }
}

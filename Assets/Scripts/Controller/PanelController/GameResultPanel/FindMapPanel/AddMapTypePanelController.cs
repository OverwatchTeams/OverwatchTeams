using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddMapTypePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text _newMapTypeName;
    [SerializeField] private Toggle _isAttackDefenseTypeToggle;

    [SerializeField] private Button _confirmAddNewMapTypeButton;
    [SerializeField] private Button _cancelAddNewMapTypeButton;
}

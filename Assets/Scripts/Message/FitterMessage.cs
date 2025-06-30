using System.Collections;
using System.Collections.Generic;
using RainbowArt.CleanFlatUI;
using UnityEngine;

public class FitterMessage : MonoBehaviour
{
    public static FitterMessage Instance { get; private set; }
    
    private GameObject _owner;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    public bool IsOwner(GameObject owner)
    {
        return _owner == owner;
    }
    
    public void SetDescription(string description)
    {
        this.gameObject.GetComponent<ModalWindow>().DescriptionValue = description;
    }
}

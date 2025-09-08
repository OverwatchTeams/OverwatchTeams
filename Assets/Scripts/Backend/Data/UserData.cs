using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Permission
{
    Admin,
    ClanMaster,
    ClanAdmin,
    ClanMember,
    None
}

[Serializable]
public class UserData : MonoBehaviour
{
    public static UserData instance;
    private string id;
    [SerializeField] private Permission permission;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public Permission GetUserPermission()
    {
        return permission;
    }
}

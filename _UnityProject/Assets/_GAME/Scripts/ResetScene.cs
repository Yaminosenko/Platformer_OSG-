﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private trap_clone[] _trapcloneref;
    [SerializeField] private SimpleDoor[] _doorRef;
    [SerializeField] private DestructiblePlatform[] _destructibleRef;
    [SerializeField] private EnergieCharge[] _CatalyserRef;


   public void SceneReset()
    {
        for (int i = 0; i < _destructibleRef.Length; i++)
        {
            _destructibleRef[i].collider.enabled = true;
            _destructibleRef[i].child.SetActive(true);
        }
        for (int i = 0; i < _doorRef.Length; i++)
        {
            _doorRef[i].Close();
        }
        for (int i = 0; i < _trapcloneref.Length; i++)
        {
            _trapcloneref[i].Reset();
        }
        for (int i = 0; i < _CatalyserRef.Length; i++)
        {
            _CatalyserRef[i]._currentCharge = 0;
        }
    }
}
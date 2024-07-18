﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnPlace : MonoBehaviour
{
    private TileType m_tileType;
    private UIBase m_motherUI;
    [SerializeField] private TMP_Text m_placeText;

    public void SetButton(TileType _tileType, UIBase _motherUI)
    {
        m_tileType = _tileType;
        m_placeText.text = m_tileType.ToString();
        m_motherUI = _motherUI;
    }
    public void OnClickPlaceEnter()
    {
        Debug.Log(m_tileType + "으로 입장");
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}

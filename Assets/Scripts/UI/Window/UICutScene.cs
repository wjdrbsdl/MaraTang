using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICutScene : UIBase
{
    
    [SerializeField] private GameObject m_attackInfo; //공격에 관련된 버프
    [SerializeField] private GameObject m_defenseInfo; //방어에 관련된 버프
    [SerializeField] private GameObject m_worldInfo; //세계 범용적인 버프들
    [SerializeField] private GameObject m_battleToknes; //사용할 배틀 토큰들
    [SerializeField] private GameObject m_statInfo;
    [SerializeField] private GameObject m_hp;
    public void ShowBattleInfo(TokenChar _player, TokenChar _opponent)
    {


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayActionToken : MonoBehaviour
{
    [SerializeField]
    private ActionTokenObject[] m_actionObjects;

    public void ShowActionTokens(TokenChar _char)
    {
        gameObject.SetActive(true); //오브젝트 켜고

        //2. 위치 잡기
        transform.SetParent(_char.GetObject().gameObject.transform); //자식으로 넣고
        transform.localPosition = new Vector3(-1, -1); //로컬 포지션값 변환

        //3. ActionToken 세팅
        List<TokenAction> actions = _char.GetActionList();
        for (int i = 0; i < actions.Count; i++)
        {
            m_actionObjects[i].SetToken(actions[i], TokenType.Action);
        }
        for(int i = actions.Count; i < m_actionObjects.Length; i++)
        {
            m_actionObjects[i].gameObject.SetActive(false);
        }
    }

    public void OffActionDisplay()
    {
        gameObject.SetActive(false);
    }
}

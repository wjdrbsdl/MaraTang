using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayActionToken : MonoBehaviour
{
    [SerializeField]
    private ActionTokenObject[] m_actionObjects;

    public void ShowActionTokens(TokenChar _char)
    {
        gameObject.SetActive(true); //������Ʈ �Ѱ�

        //2. ��ġ ���
        transform.SetParent(_char.GetObject().gameObject.transform); //�ڽ����� �ְ�
        transform.localPosition = new Vector3(-1, -1); //���� �����ǰ� ��ȯ

        //3. ActionToken ����
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

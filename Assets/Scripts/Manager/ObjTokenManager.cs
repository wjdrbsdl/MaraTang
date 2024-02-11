using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTokenManager : MgGeneric<ObjTokenManager>
{
    //오브젝트 토큰 풀 관리자
    //타일위에 표시될 자원, 몬스터, 이벤트등은 동일한 objectTokenType을 사용할 수 있으므로 풀로 관리. 
    [SerializeField]
    private ObjTokenCapital m_sample; 
    private Stack<ObjTokenCapital> m_capitalObjStack = new Stack<ObjTokenCapital>();
    
    public void RequestObjectToken(TokenTile _tile)
    {
        ObjTokenCapital capitalObj = Instantiate(m_sample).GetComponent<ObjTokenCapital>();
        capitalObj.gameObject.transform.SetParent(_tile.GetObject().transform);
        capitalObj.gameObject.transform.localPosition = new Vector3(0, 0);
    }

}

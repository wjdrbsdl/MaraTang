using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalObjManager : MgGeneric<CapitalObjManager>
{
    //캐피탈 스프라이트 렌더러 풀 관리자
    [SerializeField]
    private ObjTokenCapital m_sample; 
    private Stack<ObjTokenCapital> m_capitalObjStack = new Stack<ObjTokenCapital>();
    
    public void RequestObjToken(TokenTile _tile)
    {
        ObjTokenCapital capitalObj = Instantiate(m_sample).GetComponent<ObjTokenCapital>();
        capitalObj.gameObject.transform.SetParent(_tile.GetObject().transform);
        capitalObj.gameObject.transform.localPosition = new Vector3(0, 0);
    }

}

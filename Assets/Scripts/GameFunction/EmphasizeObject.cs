using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EmphasizeObject
{
    //대상이 되는 타겟을 표식 활성화 시키는 부분.
    //각 오브젝트마다 자신이 강조 방식을 설정을 해놓을까. 
    //얘가 다 할까 
    private List<ObjectTokenBase> m_preObjectList = new(); //기초 오브젝트 리스트

    public void AddEmphasizeObj()
    {

    }

    public void Emphasize(List<ObjectTokenBase> _tokenbases)
    {
        ResetEmphasize();
        m_preObjectList = _tokenbases;
        for (int i = 0; i < _tokenbases.Count; i++)
        {
            SpriteRenderer sprite = _tokenbases[i].GetComponent<SpriteRenderer>();
            sprite.color = Color.red;
        }
    }

    public void Emphasize(TokenBase _token)
    {
        ObjectTokenBase _emphaObject = _token.GetObject(); //강조할 오브젝트 얻기 
        if (_emphaObject == null)
            return; //강조할 대상없음. 

        Debug.Log(_token.GetItemName() + "캐릭터 고름 강조");
    }

    public void Nomalize(List<ObjectTokenBase> _tokenbases)
    {
        for (int i = 0; i < _tokenbases.Count; i++)
        {
            SpriteRenderer sprite = _tokenbases[i].GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
        m_preObjectList.Clear();
    }

    public void ResetEmphasize()
    {
        if (m_preObjectList.Count != 0)
        {
            Nomalize(m_preObjectList);
        }
    }
}
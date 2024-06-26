using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItemInfo
{
    public List<TOrderItem> ItemList; //보여주려는 아이템리스트
    public List<TOrderItem> SelectedItem; //선택한 아이템  
    public List<int> SelectedIndex;
    public SelectItemInfo(List<TOrderItem> _showList)
    {
        //만들려는 아이템 리스트를가지고 클래스 생성
        ItemList = _showList;
        SelectedItem = new List<TOrderItem>();
        SelectedIndex = new List<int>();
    }

    public void ChooseItem(TOrderItem _item)
    {
        if (SelectedItem.IndexOf(_item) > 0)
        {
            SelectedItem.Remove(_item);
            return;
        }
        SelectedItem.Add(_item);
    }
    public void AddChooseItem(int _itemIndex)
    {
        if (SelectedIndex.IndexOf(_itemIndex) >= 0)
        {
            SelectedIndex.Remove(_itemIndex);
            return;
        }
        SelectedIndex.Add(_itemIndex);
    }

}

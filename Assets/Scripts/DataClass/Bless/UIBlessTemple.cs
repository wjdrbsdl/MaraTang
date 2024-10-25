using System.Collections;
using UnityEngine;


public class UIBlessTemple : MonoBehaviour
{
    //모시는 신에 따라 은총을 갈구할 수 있다.
    //보유한 은총을 장착해제 가능한 곳 

    public void PleaseBlessBtn()
    {
        MgGodBless.GetInstance().PleaseBless();
    }
}

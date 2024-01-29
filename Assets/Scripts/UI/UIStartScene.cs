using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartScene : UIBase
{
    public void GoGame()
    {
        SceneManager.LoadScene("1.PlayScene");
    }
}

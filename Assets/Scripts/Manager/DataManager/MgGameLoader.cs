using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameLoad
{
    New, Load
}

public enum LoadMenuEnum
{
    New, Continue, Quit, Setting
}

public class MgGameLoader : MgGeneric<MgGameLoader>
{ 
    [SerializeField] private GameLoad m_gameLoad;
    [SerializeField] private GameObject m_loadScene;

    public void MasterDataLoad()
    {
        //�ε��� �ڷḦ ���� diction ��, �ε��� ���̺��� ��Ʈ�� ������ ����
        //�� ������ �ε�
        //ĳ�� ������ ������ �ε�
        //�׼� ������ ������ �ε� �� �� ��� ������ ������ �ε�
    }

    public void GameLoadButton(LoadMenuEnum _button)
    {
        switch (_button)
        {
            case LoadMenuEnum.New:
                m_gameLoad = GameLoad.New;
                LoadGame();
                break;
            case LoadMenuEnum.Continue:
                m_gameLoad = GameLoad.Load;
                LoadGame();
                break;
            case LoadMenuEnum.Quit:
                Debug.Log("���� ����");
                break;
            case LoadMenuEnum.Setting:
                Debug.Log("ȯ�� ����â �ѱ�");
                break;
        }

    }

    public void SaveGame()
    {
        DBToJson.SaveCharToken(MgToken.GetInstance().GetCharList().ToArray(), GameLoad.Load);
    }

    private void LoadGame()
    {
        SystemLoading.g_instance.PlayLoadingScene();

        SystemPause.g_instance.Play(PauseReason.Intro);
        Camera.main.orthographicSize = 4;
        m_loadScene.SetActive(false);

    }


    #region Get Set

    public GameLoad GetLoadMode()
    {
        return m_gameLoad;
    }
    #endregion
}

public class SaveData
{
    
}

public class KMP
{
    string origin = "abcdfabcggeaabcde";
    string find = "abcde";

    public void Search()
    {

        int[] pattern = {-1,0,0,0,0};
        //�� ��ġ ���� ���� ���ϼ� ã�Ƴ���

        bool isFind = false;
        int originIdx = 0; //���������� Ʋ�ȴ� �ڸ� 
        int correct = 0; //��ġ�� ���� �� 
        int startIdx = originIdx;
        while (isFind == false)
        {
            for(int oriIdx = startIdx; oriIdx<origin.Length; oriIdx++)
            {
                char oriChar = origin[oriIdx]; //���� ����
                char findChar = find[correct]; //���� ��ġ�� ���� 0 ��° ���� ����Ȯ��
                originIdx = oriIdx; //�˻��ߴ� �κ� üũ
                Debug.LogFormat("����idx{0}����{1} �����Ѽ�{2}����{3}", oriIdx, oriChar, correct, findChar);
                if(oriChar == findChar)
                {
                    correct += 1;
                    if(correct == find.Length)
                    {
                        Debug.Log(startIdx + "���翡�� ��ġ�ϴ°� ã��");
                        isFind = true;
                        break;
                    }
                    continue;
                }

                //���� �ٸ��ٸ� �ش� ������ �����ϰ� �ٽ� ������ġ�� ��輺�� �̵��ؼ� �˻�����
                
                break;
            }
            if (isFind == true)
                break;
            int back = pattern[correct]; //��輺 �ҷ�����
            //������� ��輱 ��ŭ �̵��ؼ� �ش� ��ġ���� oriIdx�� �����ϸ��. �ٵ� ��� ���ϼ���ŭ�̴ϱ� 
            //originIdx�������� correct0���ؾ������� ��ġ�� ���� back ��ŭ ��ܼ�
            correct = back; //��ġ�� ���� �����ϰ� 
            //2��ŭ ������ٸ� ������ 
            //Ʋ�ȴ� �κк��� �������� ���鼭
            //ã������ ������ ����� ��輱 �������� �ٽ� Ȯ���� ����. 
            //correct�� 0�� �Ǵ¼��� Ʋ�ȴ� �κп� find�� 0��°�� �ڸ��ϰ�, 
            if(correct == -1)
            {
                originIdx += 1;
                correct = 0; 
            }

            if (originIdx +1 >= origin.Length)
            {
                //������ ����� �غôٸ� ���� 
                Debug.Log("����");
                break;
            }

            startIdx = originIdx;
            //���� ��谪�� -1 �� ó������ Ʋ�����̶�� ������ �ٽ� �ʱ�ȭ �ؼ� ���� 
        }
    }


}

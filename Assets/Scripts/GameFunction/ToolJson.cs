using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class ToolJson
{
   
    
    public void FileSave(string _fileName, string _jsonData)
    {
        
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, _fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(_jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public string FileLoad(string _fileName)
    {

        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, _fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return jsonData;
    }

}

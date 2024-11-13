using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum JsonName
{
    CharTokenJson, SeedJson, CropInfoJson, LandInfoJson, LandInfoExplored, EnumJson, HeartCrop, PlayerStat
}

public static class DBToJson
{
    static ToolJson g_toolJson = new ToolJson();


    public static void SaveCharToken(TokenChar[] _charTokens, GameLoad _gameLoad)
    {
        CharTokenJson JsonContainer = new CharTokenJson(_charTokens);
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        string jsonData = JsonConvert.SerializeObject(JsonContainer, Formatting.Indented, settings);
        g_toolJson.FileSave(JsonName.CharTokenJson.ToString()+"_"+ _gameLoad.ToString(), jsonData);
    }

    public static void SaveTileToken(TokenTile[] _tileTokens, GameLoad _gameLoad)
    {
        TileTokenJson JsonContainer = new TileTokenJson(_tileTokens);
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        string jsonData = JsonConvert.SerializeObject(JsonContainer, Formatting.Indented, settings);
        g_toolJson.FileSave(JsonName.CharTokenJson.ToString() + "_" + _gameLoad.ToString(), jsonData);
    }

    public static void DateToJson<T>(T _file, GameLoad _gameLoad)
    {
        string file = JsonUtility.ToJson(_file, true);

        g_toolJson.FileSave(_file.ToString() + "_" + _gameLoad.ToString(), file);
    }

   

    public static T LoadToJson<T>(JsonName _name, GameLoad _gameLoad)
    {
        string loadJson = g_toolJson.FileLoad(_name.ToString()+"_"+ _gameLoad.ToString()); //부르려는 타입으로 불러드리고. 
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        T newLoad = JsonConvert.DeserializeObject<T>(loadJson, settings); //해당 타입으로 변환 여기까지만 받으면됨.

        return newLoad;
    }

    public static T LoadToJson<T>(GameLoad _gameLoad)
    {
        string loadJson = g_toolJson.FileLoad(nameof(T) + "_" + _gameLoad.ToString()); //부르려는 타입으로 불러드리고. 
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        T newLoad = JsonConvert.DeserializeObject<T>(loadJson, settings); //해당 타입으로 변환 여기까지만 받으면됨.

        return newLoad;
    }

    //땅의 경우 마이랜드 List당 할필요가 있으므로, 저장 방식이나 불러오기 방식을 따로 메서드로 다시 만들필요가 있음. 
}

#region JsonCalss
class ActionTokenJson
{
    public string[] PropertyName;
    public string[] SkillName;
   
    public ActionTokenJson(TokenAction[] _skill)
    {
   
    }
}

class CharTokenJson
{
    public TokenChar[] charTokens;
    public CharTokenJson(TokenChar[] _charTokens)
    {
        charTokens = _charTokens;
    }
}

class TileTokenJson
{
    public TokenTile[] tileTokens;
    public TileTokenJson(TokenTile[] _tiletokens)
    {
        tileTokens = _tiletokens;
    }
}
#endregion
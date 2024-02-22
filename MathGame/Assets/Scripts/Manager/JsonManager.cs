using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager : Singleton<JsonManager>
{
    public string ReadJson(string path)
    {
        if (!File.Exists(path))
        {
            return "";
        }
        var json = File.ReadAllText(path);
        return json;
    }
    public void WriteJson(string path, string json)
    {
        File.WriteAllText(path, json);
    }
    public void SaveJsonData<T>(T data, string path)
    {
        var json = JsonUtility.ToJson(data, true);
        WriteJson(path, json);
    }
    public T LoadJsonData<T>(string path)
    {
        var json = ReadJson(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;
    }
}
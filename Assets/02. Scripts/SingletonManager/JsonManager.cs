using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class JsonData
{
    public List<string> patterns = new List<string>();
}

public class JsonManager : SingletonObject<JsonManager>
{
    private string _filePath;

    public string curRecordingPattern;

    protected override void Awake()
    {
        base.Awake();

        _filePath = Path.Combine(Application.persistentDataPath, "jsonData.json");

        RecordReset();
    }

    public void RecordReset()
    {
        curRecordingPattern = string.Empty;
    }

    public void SaveToJson()
    {
        JsonData existData = LoadJsonData();

        if (existData == null)
        {
            existData = new JsonData { patterns = new List<string>() };
        }
        else if(existData.patterns.Contains(curRecordingPattern))
        {
            return;
        }

        existData.patterns.Add(curRecordingPattern);

        string newData = JsonUtility.ToJson(existData, true);

        File.WriteAllText(_filePath, newData);

        RecordReset();
    }

    public JsonData LoadJsonData()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);

            return JsonUtility.FromJson<JsonData>(json);
        }

        return null;
    }

    public string LoadPattern(int index)
    {
        JsonData existData = LoadJsonData();

        if(existData == null || index >= existData.patterns.Count)
        {
            return null;
        }

        string record = JsonUtility.ToJson(existData, true);

        File.WriteAllText(_filePath, record);

        return existData.patterns[index];
    }

    public void DeletePattern(int index)
    {
        JsonData existData = LoadJsonData();

        existData.patterns.RemoveAt(index);

        string newData = JsonUtility.ToJson(existData, true);

        File.WriteAllText(_filePath, newData);
    }

    public void DeleteAll()
    {
        JsonData newJson = new JsonData { patterns = new List<string>() };

        string newData = JsonUtility.ToJson(newJson, true);

        File.WriteAllText(_filePath, newData);
    }
}

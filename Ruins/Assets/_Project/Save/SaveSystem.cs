using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveEntry { public string id; public string json; }

[Serializable]
public class SaveData
{
    public int version;
    public string scene;
    public string savedAt;
    public List<SaveEntry> entries = new();
}

public class SaveSystem : MonoBehaviour
{
    private const int CurrentVersion = 3;

    private static string PathFor(int slot) =>
        Path.Combine(Application.persistentDataPath, $"save{slot}.json");

    public void Save(int slot)
    {
        var data = new SaveData
        {
            version = CurrentVersion,
            scene = SceneManager.GetActiveScene().name,
            savedAt = DateTime.Now.ToString("o"),
        };

        foreach (var entity in FindObjectsByType<SaveableId>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var saveable in entity.GetComponents<ISaveable>())
            {
                data.entries.Add(new SaveEntry
                {
                    id = $"{entity.Id}/{saveable.GetType().Name}",
                    json = saveable.CaptureJson(),
                });
            }
        }
        WriteAtomic(PathFor(slot), JsonUtility.ToJson(data, true));
    }

    public bool TryLoad(int slot)
    {
        string path = PathFor(slot);
        if (!File.Exists(path)) return false;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        data = SaveMigrations.Upgrade(data, CurrentVersion);

        var lookup = new Dictionary<string, string>();
        foreach (var e in data.entries) lookup[e.id] = e.json;

        foreach (var entity in FindObjectsByType<SaveableId>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var saveable in entity.GetComponents<ISaveable>())
            {
                string key = $"{entity.Id}/{saveable.GetType().Name}";
                if (lookup.TryGetValue(key, out string json))
                    saveable.RestoreJson(json);
            }
        }
        return true;
    }

    public void Delete(int slot)
    {
        string path = PathFor(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    private static void WriteAtomic(string path, string content)
    {
        string temp = path + ".tmp";
        File.WriteAllText(temp, content);
        if (File.Exists(path))
            File.Replace(temp, path, path + ".bak"); // старая версия → .bak
        else
            File.Move(temp, path);
    }
}

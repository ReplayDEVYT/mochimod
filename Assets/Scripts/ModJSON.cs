using UnityEngine;

[System.Serializable]
public class ModJSON
{
    public string name, author, description;

    public static ModJSON CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ModJSON>(jsonString);
    }
}
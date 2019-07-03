using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLevelData
{
    public static void SaveData(int[] data)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/level.data";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }
    public static int[] LoadData()
    {
        string path = Application.persistentDataPath + "/level.data";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            int[] data = binaryFormatter.Deserialize(fileStream) as int[];
            fileStream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}

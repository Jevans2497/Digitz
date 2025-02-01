using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

    public static class SongJSONSerializer {
        public static void saveToFile<T>(T data, string fileName) {
            string json = JsonUtility.ToJson(data, true); // Pretty-print for readability
            string filePath = Application.persistentDataPath + "/" + fileName;

            File.WriteAllText(filePath, json);
            Debug.Log($"Data saved to: {filePath}");
        }
}

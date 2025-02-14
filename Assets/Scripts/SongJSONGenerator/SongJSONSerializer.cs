using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SongJSONSerializer {
    public static void saveToFile<T>(T data, string fileName) {
        object processedData = data;

        //We only want timestamp, arrowDirection, and arrowSpeed so remove all other fields by creating a new simplified object to store.
        if (data is ArrowDataList arrowDataList) {
            List<SimpleArrowData> simpleArrows = new List<SimpleArrowData>();

            foreach (var arrow in arrowDataList.arrows) {
                simpleArrows.Add(new SimpleArrowData(arrow.timestamp, arrow.arrow_direction, arrow.arrow_speed));
            }

            processedData = new SimpleArrowDataList(simpleArrows);
        }

        string json = JsonUtility.ToJson(processedData, true); // Pretty-print for readability
        string filePath = Application.persistentDataPath + "/" + fileName;

        File.WriteAllText(filePath, json);
        Debug.Log($"Data saved to: {filePath}");
    }

    [System.Serializable]
    private class SimpleArrowData {
        public float timestamp;
        public string arrow_direction;
        public float arrow_speed;

        public SimpleArrowData(float timestamp, string arrow_direction, float arrow_speed) {
            this.timestamp = timestamp;
            this.arrow_direction = arrow_direction;
            this.arrow_speed = arrow_speed;
        }
    }

    [System.Serializable]
    private class SimpleArrowDataList {
        public List<SimpleArrowData> arrows;

        public SimpleArrowDataList(List<SimpleArrowData> simpleArrows) {
            this.arrows = simpleArrows;
        }
    }
}

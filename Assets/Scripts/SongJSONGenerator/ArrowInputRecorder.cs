using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowInputRecorder: MonoBehaviour {

    [SerializeField] GameManager gameManager;
    List<ArrowData> arrowDataList = new List<ArrowData>();

    Dictionary<KeyCode, string> keyToArrowMap = new Dictionary<KeyCode, string> {
        { KeyCode.LeftArrow, "left" },
        { KeyCode.UpArrow, "up" },
        { KeyCode.RightArrow, "right" },
        { KeyCode.DownArrow, "down" }
    };

    private void Update() {
        if (gameManager.isInGenerateSongJSONMode) {
            CheckForInput();
        }
    }

    private void CheckForInput() {
        foreach (var entry in keyToArrowMap) {
            if (Input.GetKeyDown(entry.Key)) {
                //I discovered a slight delay so subtracting a small amount of time
                arrowDataList.Add(new(gameManager.getSongTime() - 0.02f, entry.Value, ArrowData.ArrowEffect.regular));
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (ArrowData data in arrowDataList) {
                Debug.Log(data.arrow_direction + " and " + data.timestamp);
            }
            ArrowDataList serializableArrowDataList = new(arrowDataList);
            SongJSONSerializer.saveToFile<ArrowDataList>(serializableArrowDataList, "last_generated_song.json");
        }
    }
}

[System.Serializable]
public class ArrowDataList {
    public List<ArrowData> arrows;

    public ArrowDataList(List<ArrowData> arrows) {
        this.arrows = arrows;
    }
}
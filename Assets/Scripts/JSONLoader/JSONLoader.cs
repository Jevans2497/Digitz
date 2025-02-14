using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JSONLoader: MonoBehaviour {
    private SongPreset songPreset;

    public SongPreset LoadSong(string songName) {
        Debug.Log(songName);
        string filePath = $"SongJSONs/{songName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            songPreset = JsonUtility.FromJson<SongPreset>(jsonFile.text);
            songPreset.arrows.Sort((a, b) => a.timestamp.CompareTo(b.timestamp));
            songPreset.arrows.ForEach(arrow => arrow.applyEffectToArrow());
            return songPreset;
        } else {
            Debug.LogError($"Song file not found at {filePath}");
        }

        return null;
    }

    public List<Level> loadLevels(bool isTutorial = false) {
        string fileName = isTutorial ? "TutorialLevels" : "Levels";
        string filePath = $"Levels/{fileName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            return JsonUtility.FromJson<LevelsList>(jsonFile.text).levels;
        } else {
            Debug.LogError($"Levels file not found at {filePath}");
        }

        return null;
    }

    public List<Song> loadSongs(bool isTutorial = false) {
        string fileName = isTutorial ? "TutorialSongs" : "Songs";
        string filePath = $"MenuItems/Songs/{fileName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            SongList songList = JsonUtility.FromJson<SongList>(jsonFile.text);

            foreach (var song in songList.songs) {
                song.InitializeFromJSON();
            }

            return songList.songs;
        } else {
            Debug.LogError($"Songs file not found at {filePath}");
        }

        return null;
    }

    public List<Upgrade> loadUpgrades(bool isTutorial = false) {
        string fileName = isTutorial ? "TutorialUpgrades" : "Upgrades";
        string filePath = $"MenuItems/Upgrades/{fileName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            UpgradeList upgradeList = JsonUtility.FromJson<UpgradeList>(jsonFile.text);

            foreach (var upgrade in upgradeList.upgrades) {
                upgrade.InitializeFromJSON();
            }

            return upgradeList.upgrades;
        } else {
            Debug.LogError($"Upgrades file not found at {filePath}");
        }

        return null;
    }

    public List<Challenge> loadChallenges(bool isTutorial = false) {
        string fileName = isTutorial ? "TutorialChallenges" : "Challenges";
        string filePath = $"MenuItems/Challenges/{fileName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            ChallengeList challengeList = JsonUtility.FromJson<ChallengeList>(jsonFile.text);

            foreach (var challenge in challengeList.challenges) {
                challenge.InitializeFromJSON();
            }

            return challengeList.challenges;
        } else {
            Debug.LogError($"Challenges file not found at {filePath}");
        }

        return null;
    }
}
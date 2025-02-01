using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JSONLoader: MonoBehaviour {
    private SongPreset songPreset;

    public SongPreset LoadSong(string songName) {
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

    public List<Level> loadLevels() {
        string filePath = $"Levels/Levels";
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);

        if (jsonFile != null) {
            return JsonUtility.FromJson<LevelsList>(jsonFile.text).levels;
        } else {
            Debug.LogError($"Levels file not found at {filePath}");
        }

        return null;
    }

    public List<Song> loadSongs() {
        string filePath = $"MenuItems/Songs/Songs";
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

    public List<Upgrade> loadUpgrades() {
        string filePath = $"MenuItems/Upgrades/Upgrades";
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

    public List<Challenge> loadChallenges() {
        string filePath = $"MenuItems/Challenges/Challenges";
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
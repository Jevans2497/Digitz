using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SongList {
    public List<Song> songs;
}

[System.Serializable]
public class Song : MenuItem {

    public enum SongDifficulty
    {
        veryEasy,
        easy,
        medium,
        hard,
        veryHard
    }

    public string name;
    public string song_file_name;
    public string sprite_name;
    public string color;
    public string rarity_string;
    public string description;
    public string artist;
    public string difficulty_string;
    public SongDifficulty difficulty;

    public string Name => name;
    public string SpriteName => sprite_name;
    public string Color => color;
    public string RarityString => rarity_string;
    public string Description => description;

    public int GetRarityWeight()
    {
        return 1;
    }

    public void InitializeFromJSON()
    {
        if (!Enum.TryParse(difficulty_string, true, out difficulty))
        {
            Debug.LogWarning($"Invalid song_difficulty: {difficulty_string} for Song {name}. Defaulting to 'medium'.");
            difficulty = SongDifficulty.medium;
        }
    }
}
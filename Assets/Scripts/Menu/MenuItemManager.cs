using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MenuItemManager {

    public List<MenuGameObjects> createMenuOptions<T>(Transform menuCanvasTransform, GameObject menuObjectPrefab, List<T> items, int currentLevel = -1) where T : MenuItem {
        List<MenuGameObjects> menuGameObjects = new List<MenuGameObjects>();
        List<T> menuOptions;

        if (typeof(T) == typeof(Song)) {
            List<Song> songItems = items.Cast<Song>().ToList();
            List<Song> songOptions = getThreeDifficultyFactoredSongOptions(songItems, currentLevel);
            menuOptions = songOptions.Cast<T>().ToList();
        } else {
            menuOptions = getThreeRandomRarityFactoredMenuOptions(items);
        }

        if (menuOptions.Count >= 3) {
            menuGameObjects.Add(createMenuOption(menuObjectPrefab, menuOptions[0], menuCanvasTransform, -420.0f, 20.0f));
            menuGameObjects.Add(createMenuOption(menuObjectPrefab, menuOptions[1], menuCanvasTransform, 420.0f, 20.0f));
            menuGameObjects.Add(createMenuOption(menuObjectPrefab, menuOptions[2], menuCanvasTransform, 0.0f, 330.0f));
        }
        return menuGameObjects;
    }

    private MenuGameObjects createMenuOption(GameObject menuObjectPrefab, MenuItem menuItem, Transform menuCanvasTransform, float x, float y) {
        GameObject background = GameObject.Instantiate(menuObjectPrefab, menuCanvasTransform);
        RectTransform rectTransform = background.GetComponent<RectTransform>();
        if (rectTransform != null) {
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
        GameObject image = background.transform.Find("MenuOptionImage").gameObject;
        GameObject text = background.transform.Find("MenuOptionText").gameObject;
        return new MenuGameObjects(menuItem, background, image, text);
    }

    public List<T> getThreeRandomRarityFactoredMenuOptions<T>(List<T> items) where T : MenuItem {
        if (items == null || items.Count == 0) {
            Debug.LogError($"{typeof(T).Name} list is empty or null.");
            return null;
        }

        // Create a weighted list based on rarity where more common items are overrepresented
        List<T> weightedItems = new List<T>();
        foreach (var item in items) {
            int rarityModifier = item.GetRarityWeight();
            for (int i = 0; i < rarityModifier; i++) {
                weightedItems.Add(item);
            }
        }

        // Shuffle the weighted items
        List<T> shuffledItems = weightedItems.OrderBy(_ => UnityEngine.Random.value).ToList();

        // Use a HashSet to ensure uniqueness
        HashSet<T> uniqueItems = new HashSet<T>();
        foreach (var item in shuffledItems) {
            uniqueItems.Add(item);
            if (uniqueItems.Count == 3) break; // Stop once we have 3 unique items
        }

        return uniqueItems.ToList();
    }

    public List<Song> getThreeDifficultyFactoredSongOptions(List<Song> songs, int currentLevel) {
        List<Song> selectedSongs = new List<Song>();

        while (selectedSongs.Count < 3) {
            Song.SongDifficulty randomLevelBasedDifficulty = getDifficultyForSong(currentLevel);
            List<Song> songsWithDifficulty = songs.FindAll(song => song.difficulty == randomLevelBasedDifficulty);
            songsWithDifficulty.RemoveAll(song => selectedSongs.Contains(song));
            if (songsWithDifficulty.Count > 0) {
                selectedSongs.Add(songsWithDifficulty[UnityEngine.Random.Range(0, songsWithDifficulty.Count)]);
            }
        }

        return selectedSongs;
    }

    public Song.SongDifficulty getDifficultyForSong(int currentLevel) {
        float difficultyFactor = (currentLevel - 1) / 6f;
        float randomValue = UnityEngine.Random.value; 

        //If level is 1, difficulty factor is 0, meaning lerp will select the first value. If level is 7, will choose the second value. 
        if (randomValue < Mathf.Lerp(0.50f, 0.00f, difficultyFactor)) return Song.SongDifficulty.veryEasy;
        if (randomValue < Mathf.Lerp(0.85f, 0.05f, difficultyFactor)) return Song.SongDifficulty.easy;
        if (randomValue < Mathf.Lerp(1.00f, 0.30f, difficultyFactor)) return Song.SongDifficulty.medium;
        if (randomValue < Mathf.Lerp(1.00f, 0.70f, difficultyFactor)) return Song.SongDifficulty.hard;

        return Song.SongDifficulty.veryHard;
    }
}

public class MenuGameObjects {
    public MenuItem menuItem;
    public string path;
    public GameObject background;
    public GameObject image;
    public GameObject text;
    public bool hasChallengeBeenViewed = false;

    public MenuGameObjects(MenuItem menuItem, GameObject background, GameObject image, GameObject text) {
        this.menuItem = menuItem;
        this.path = getPathForMenuItem();
        this.background = background;
        this.image = image;
        this.text = text;
    }

    private string getPathForMenuItem() {
        if (menuItem is Upgrade upgrade) {
            return $"MenuItems/Upgrades/UpgradeIcons/{menuItem.SpriteName}";
        } else if (menuItem is Challenge challenge) {
            return $"MenuItems/Challenges/ChallengeIcons/{menuItem.SpriteName}";
        } else if (menuItem is Song song) {
            return $"MenuItems/Songs/SongIcons/{menuItem.SpriteName}";
        } else if (menuItem is LevelBonus levelBonus) {
            return $"MenuItems/LevelBonuses/LevelBonusIcons/{menuItem.SpriteName}";
        } else {
            Debug.LogWarning("Unknown MenuItem type");
            return null;
        }
    }

    public void destroy() {
        if (background != null) {
            GameObject.Destroy(background);
            background = null;
        }
        if (image != null) {
            GameObject.Destroy(image);
            image = null;
        }
        if (text != null) {
            GameObject.Destroy(text);
            text = null;
        }        
    }
}
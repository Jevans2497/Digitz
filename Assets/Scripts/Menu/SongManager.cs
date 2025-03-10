using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SongManager: MenuItemManager {

    List<Song> songs;
    private JSONLoader jsonLoader;
    private GameObject menuObjectPrefab;    

    public SongManager(JSONLoader jsonLoader, GameObject menuObjectPrefab, bool isTutorial = false) {
        this.jsonLoader = jsonLoader;
        this.menuObjectPrefab = menuObjectPrefab;
        this.songs = jsonLoader.loadSongs(isTutorial);
    }

    public List<MenuGameObjects> createSongOptions(Transform menuCanvasTransform, GameObject menuObjectPrefab, int currentLevel) {
        return createMenuOptions<Song>(menuCanvasTransform, menuObjectPrefab, songs, currentLevel);
    }

    public void removeSongFromPool(Song songToRemove) {
        songs.RemoveAll(song => song.Name == songToRemove.Name);
    }
}
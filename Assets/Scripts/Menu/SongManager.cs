using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SongManager: MenuItemManager {

    List<Song> songs;
    private JSONLoader jsonLoader;
    private GameObject menuObjectPrefab;

    public SongManager(JSONLoader jsonLoader, GameObject menuObjectPrefab) {
        this.jsonLoader = jsonLoader;
        this.menuObjectPrefab = menuObjectPrefab;
        this.songs = jsonLoader.loadSongs();
    }

    public List<MenuGameObjects> createSongOptions(Transform menuCanvasTransform, GameObject menuObjectPrefab) {
        return createMenuOptions<Song>(menuCanvasTransform, menuObjectPrefab, songs);
    }
}
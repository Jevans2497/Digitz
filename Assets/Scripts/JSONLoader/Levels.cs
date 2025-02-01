﻿using System;
using System.Collections.Generic;

[System.Serializable]
public class LevelsList {
    public List<Level> levels;
}

[System.Serializable]
public class Level {
    public string name;
    public int level_number;
    public List<LevelSprite> level_sprites;
}

[System.Serializable]
public class LevelSprite {
    public string sprite_name;
    public string altername_name;
}
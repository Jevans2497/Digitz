using System;

public interface MenuItem {
    string Name { get; }
    string SpriteName { get; }
    string Color { get; }
    string RarityString { get; }
    string Description { get; }
    void InitializeFromJSON();
    int GetRarityWeight();
}

using System;
using System.Collections.Generic;

/// <summary>
/// Class to structure in-game data storage, used to group every save/loadable game components
/// </summary>

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public Opinion finalVictimData;
    public bool isPositiveEnding;
    public List<Opinion> disagreedOpinions;

    public SessionData Session;

    public string CurrentScene;
    // TODO add more data types here, e.g. statistics
}
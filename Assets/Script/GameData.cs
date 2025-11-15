using System;

/// <summary>
/// Class to structure in-game data storage, used to group every save/loadable game components
/// </summary>

[Serializable]
public class GameData
{
    public PlayerIdData Player; //TODO coordinate with PlayerData in TopicData.cs
    public SessionData Session;
    // TODO add more data types here, e.g. statistics
}
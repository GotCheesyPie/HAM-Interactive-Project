using UnityEngine;
[System.Serializable]
public class PlayerIdData : IPersistable
{
    public int PlayerId; // Primary Key
    public string Name;
    public int Age;
    public string City;
    public int AvatarId;
    // public 
    public PlayerIdData(int id, string name, int age, string city, int avatarId)
    {
        PlayerId = id;
        Name = name;
        Age = age;
        City = city;
        AvatarId = avatarId;
    }

    public void Load(GameData data)
    {
        PlayerId = data.Player.PlayerId;
        Name = data.Player.Name;
        Age = data.Player.Age;
        City = data.Player.City;
        AvatarId = data.Player.AvatarId;
    }

    public void Save(ref GameData data)
    {
        data.Player = this;
    }



    // To prevent clash with PlayerData in TopicData.cs
    public static explicit operator PlayerIdData(PlayerData data) => new(-1, data.playerName, data.playerAge, data.playerCity, data.selectedAvatarID);
    // explicit operator allows to do:
    // PlayerData playerData = new(...);
    // PlayerIdData playerIdData = playerData;
}
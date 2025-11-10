using UnityEngine;
[System.Serializable]
public class PlayerIdData
{
    public int PlayerId; // Primary Key
    public string Name;
    public int Age;
    public string City;
    public int AvatarId;
    public PlayerIdData(int id, string name, int age, string city, int avatarId)
    {
        PlayerId = id;
        Name = name;
        Age = age;
        City = city;
        AvatarId = avatarId;
    }

    // To prevent clash with PlayerData in TopicData.cs
    public static explicit operator PlayerIdData(PlayerData data) => new(-1, data.playerName, data.playerAge, data.playerCity, data.selectedAvatarID);
    // explicit operator allows to do:
    // PlayerData playerData = new(...);
    // PlayerIdData playerIdData = playerData;
}
using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string ID;
    public string Name;
    public ItemData(string ID, string Name)
    {
       this.ID = ID;
       this.Name = Name; 
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ButtonsDictionaryItem
{
    [FormerlySerializedAs("Id")] public int id;
    [FormerlySerializedAs("Name")] public string name;
}

[CreateAssetMenu]
public class ButtonsDictionary : ScriptableObject
{
    [SerializeField] private List<ButtonsDictionaryItem> items;
}

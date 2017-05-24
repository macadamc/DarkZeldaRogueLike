using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "ShadyPixel/Create New Character Class")]
public class CharacterClass : ScriptableObject {

    public string className;
    [TextArea]
    public string description;
    public GameObject playerPrefab;




}

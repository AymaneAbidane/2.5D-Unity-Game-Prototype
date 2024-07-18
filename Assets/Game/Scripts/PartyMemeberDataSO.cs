using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemeberDataSO : CharactersAndEnemysSO
{
    public int startingLevel;
    public GameObject memberOverworldVisualPrefab;//what will be displayed in the overworld scene;
}

using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public enum AbilityType { None, Nuke, Money, Rain, Stink, Condom }
    public static AbilityType selectedAbility = AbilityType.None;

    public GameObject nukeAOE;
    public GameObject moneyAOE;
    public GameObject rainAOE;
    public GameObject stinkAOE;
    public GameObject condomAOE;

    public void SelectAbility(int index)
    {
        selectedAbility = (AbilityType)index;
        Debug.Log("Selected ability: " + selectedAbility);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectedAbility != AbilityType.None)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnAOE(worldPos, selectedAbility);
            selectedAbility = AbilityType.None;
        }
    }

    void SpawnAOE(Vector2 pos, AbilityType type)
    {
        GameObject prefab = GetAOEPrefab(type);
        if (prefab != null) Instantiate(prefab, pos, Quaternion.identity);
    }

    GameObject GetAOEPrefab(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.Nuke: return nukeAOE;
            case AbilityType.Money: return moneyAOE;
            case AbilityType.Rain: return rainAOE;
            case AbilityType.Stink: return stinkAOE;
            case AbilityType.Condom: return condomAOE;
            default: return null;
        }
    }
}

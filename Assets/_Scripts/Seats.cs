using UnityEngine;

public class Seats : MonoBehaviour
{
    private void Start()
    {
        var children = GetComponentsInChildren<Vision>();

        for (int i = 0; i < children.Length; i++)
        {
            Character character = Instantiate(PlacementDTO.Instance.sourceCharacters[i], children[i].transform);
            if (i == 0) continue;
            children[i].ProjectilePool = character.GetComponentInChildren<ProjectileObjectPooling>();
        }
    }
}
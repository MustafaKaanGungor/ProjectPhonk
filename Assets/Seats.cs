using UnityEngine;

public class Seats : MonoBehaviour
{
    private void Start()
    {
        var children = GetComponentsInChildren<Vision>();

        for (int i = 0; i <= children.Length; i++)
        {
            children[i].name = PlacementDTO.Instance.sourceNames[i];
        }
    }
}
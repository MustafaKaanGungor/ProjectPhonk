using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementDTO : MonoBehaviour
{
    public static PlacementDTO Instance { get; private set; }

    public List<Destination> destinations = new List<Destination>();

    [HideInInspector]
    public List<Source> sources = new List<Source>();

    public List<Character> sourceCharacters = new List<Character>();

    public int maxPlacements = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Copy()
    {
        foreach (var destination in destinations)
        {
            sourceCharacters.Add(destination.Source.character);
        }
    }
}
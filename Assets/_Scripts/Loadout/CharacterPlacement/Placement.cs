using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Placement : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playButtonText;

    public void AddPlacement(Source source)
    {
        if (PlacementDTO.Instance.sources.Count == PlacementDTO.Instance.maxPlacements) return;
        PlacementDTO.Instance.sources.Add(source);
        PlacementDTO.Instance.destinations.Where(d => d.Source == null).FirstOrDefault().Source = source;
        source.SetActive(false);
        PlayButtonTextUpdate();
    }

    public void RemovePlacement(Destination destination)
    {
        if (PlacementDTO.Instance.sources.Count == 0) return;

        PlacementDTO.Instance.sources.Remove(destination.Source);
        destination.Source.SetActive(true);
        destination.Source = null;
        PlayButtonTextUpdate();
    }

    private void PlayButtonTextUpdate()
    {
        playButtonText.text = PlacementDTO.Instance.sources.Count == PlacementDTO.Instance.maxPlacements ?
            "Play" :
            $"{PlacementDTO.Instance.sources.Count}/{PlacementDTO.Instance.maxPlacements} Placed";
    }

    public void PlayButtonOnClick()
    {
        if (PlacementDTO.Instance.sources.Count == PlacementDTO.Instance.maxPlacements)
        {
            PlacementDTO.Instance.Copy();
            SceneManager.LoadScene("TestScene4");
            Debug.Log("Starting game with selected placements.");
        }
        else
        {
            Debug.LogWarning("Not enough placements selected.");
        }
    }
}

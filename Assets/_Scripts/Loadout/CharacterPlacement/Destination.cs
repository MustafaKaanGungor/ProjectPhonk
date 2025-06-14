using UnityEngine;
using UnityEngine.UI;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private Image image;

    private Source source = null;

    public Source Source
    {
        get => source;
        set
        {
            source = value;
            image.sprite = source != null ? source.Sprite : null;
        }
    }
}
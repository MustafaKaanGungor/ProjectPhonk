using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour
{
    public Character character;

    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private Button button;

    [SerializeField]
    private GameObject disable;

    public Sprite Sprite
    {
        get => image.sprite;
    }

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    public void SetActive(bool value)
    {
        button.enabled = value;
        disable.SetActive(!value);
    }
}
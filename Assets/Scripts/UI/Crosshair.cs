using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text text;

    private Sprite defaultSprite;
    private string defaultText;


    public void SetInteractionText(string interactionText) {
        text.text = interactionText;
    }

    public void SetIcon(Sprite newIcon) {
        icon.sprite = newIcon;
    }

    public void Reset() {
        text.text = defaultText;
        icon.sprite = defaultSprite;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultSprite = icon.sprite;
        defaultText = text.text;
        Reset();
    }
}

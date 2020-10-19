using UnityEngine;
using UnityEngine.UI;

public class UnitButtonScript : MonoBehaviour
{
    public Button unitButton;
    [SerializeField] private Image unitImage;
    [SerializeField] private Text unitName;
    public bool selected = false;

    void Start()
    {
        unitButton.onClick.AddListener(ClickBoton);
    }

    public void CrearBoton(string name, Sprite image)
    {
        unitName.text = name;
        unitImage.sprite = image;
    }

    public void Deseleccionar() => selected = false;

    private void ClickBoton() => selected = true;
}

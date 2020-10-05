using UnityEngine;
using UnityEngine.UI;

public class BotonUnidad : MonoBehaviour
{
    public Button botonUnidad;
    [SerializeField] private Image imagenUnidad;
    [SerializeField] private Text nombreUnidad;
    public bool seleccionada = false;

    void Start()
    {
        botonUnidad.onClick.AddListener(ClickBoton);
    }

    public void CrearBoton(string nombre, Sprite imagen)
    {
        nombreUnidad.text = nombre;
        imagenUnidad.sprite = imagen;
    }

    public void Deseleccionar() => seleccionada = false;

    private void ClickBoton() => seleccionada = true;
}

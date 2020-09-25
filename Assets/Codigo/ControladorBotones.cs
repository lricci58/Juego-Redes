using UnityEngine;

public class ControladorBotones : MonoBehaviour
{
    [SerializeField] private GameObject objetoBoton;
    [SerializeField] public Sprite[] imagenUnidades;
    [SerializeField] private Transform contenedorBotones;

    private BotonUnidad boton;

    void Start()
    {
        boton = objetoBoton.GetComponent<BotonUnidad>();
        AgregarUnidades();
    }

    private void AgregarUnidades()
    {
        for (int i = 0; i < ControladorJuego.instancia.unidadesEjercito.GetLength(0); i++)
        {
            string nombreUnidad = ControladorJuego.instancia.unidadesEjercito[i];

            if (nombreUnidad == "InfanteriaHacha")
                boton.CrearBoton("Infantería con Hacha", imagenUnidades[0]);

            else if (nombreUnidad == "InfanteriaEspada")
                boton.CrearBoton("Infantería con Espada", imagenUnidades[1]);

            GameObject instancia = Instantiate(objetoBoton);
            instancia.transform.SetParent(contenedorBotones);
        }
    }


}

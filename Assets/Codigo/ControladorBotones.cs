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
        int[] listaUnidades = ControladorJuego.instancia.listaUnidades;

        for (int i = 0; i < listaUnidades.GetLength(0); i++)
        {
            int tipoUnidad = listaUnidades[i];

            if (tipoUnidad == 0)
                boton.CrearBoton("Infantería con Hacha", imagenUnidades[tipoUnidad]);

            else if (tipoUnidad == 1)
                boton.CrearBoton("Infantería con Espada", imagenUnidades[tipoUnidad]);

            GameObject instancia = Instantiate(objetoBoton);
            instancia.transform.SetParent(contenedorBotones);
        }
    }
}

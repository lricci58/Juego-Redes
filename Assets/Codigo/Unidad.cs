using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Unidad : MonoBehaviour 
{
    [SerializeField] private int radioMovimiento;
    [SerializeField] private float vida;
    [SerializeField] private float armadura;
    [SerializeField] private float ataque;

    [SerializeField] private float tiempoMov;
    [SerializeField] private float offsetPosicionX;
    [SerializeField] private float offsetPosicionY;

    private Animator animador;
    private SpriteRenderer sprite;

    private List<Vector2> tilesMovimiento;
    private List<Vector2> tilesAtaque;
    private bool seleccionada = false;

    private bool moviendo = false;
    private string direccionX = "";
    private string direccionY = "";

    void Start()
    {
        // agrega automaticamente el script a la lista de unidades
        ControladorJuego.instancia.AgregarUnidad(this);

        animador = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        tilesMovimiento = new List<Vector2>();
        tilesAtaque = new List<Vector2>();
    }

    public bool SeSelecciono()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == transform)
                seleccionada = true;
        }

        return seleccionada;
    }

    public void Mover(Vector3 posicion)
    {
        if (moviendo)
        {
            float posicionActualX = transform.position.x - offsetPosicionX;
            float posicionActualY = transform.position.y - offsetPosicionY;
            Vector3 posicionActual = new Vector3(posicionActualX, posicionActualY);

            animador.SetBool("moviendo", true);

            if (direccionX == "derecha")
            {
                // comprueba que el eje no haya llegado a su destino
                if (posicionActual.x < posicion.x)
                {
                    sprite.flipX = false;
                    // en ese caso, se ejecuta el movimiento
                    transform.Translate(tiempoMov * Time.deltaTime, 0, 0);
                }
                else if (posicionActual.x >= posicion.x)
                    direccionX = "";
            }
            else if (direccionX == "izquierda")
            {
                if (posicionActual.x > posicion.x)
                {
                    sprite.flipX = true;
                    transform.Translate(-tiempoMov * Time.deltaTime, 0, 0);
                }
                else if (posicionActual.x <= posicion.x)
                    direccionX = "";
            }

            // if (direccionX == "") // para que solo mueva en eje 'x' antes que 'y'
            if (direccionY == "arriba")
            {
                if (posicionActual.y < posicion.y)
                {
                    transform.Translate(0, tiempoMov * Time.deltaTime, 0);
                }
                else if (posicionActual.y >= posicion.y)
                    direccionY = "";
            }
            else if (direccionY == "abajo")
            {
                if (posicionActual.y > posicion.y)
                {
                    transform.Translate(0, -tiempoMov * Time.deltaTime, 0);
                }
                else if (posicionActual.y <= posicion.y)
                    direccionY = "";
            }

            // compueba que ambos ejes hayan llegado a su destino
            if (direccionX == "" && direccionY == "")
            {
                animador.SetBool("moviendo", false);
                moviendo = false;

                // posiciona al personaje en su lugar, ya que puede (most likely) que se haya pasado de su destino
                transform.position = new Vector3(posicion.x + offsetPosicionX, posicion.y + offsetPosicionY, transform.position.z);
            }
        }
    }

    public void DeterminarDireccionMovimiento(Vector3 posicion)
    {
        if (seleccionada)
        {
            // Setea la direccion 'x' e 'y' segun su objetivo

            float posicionActualX = transform.position.x - offsetPosicionX;
            float posicionActualY = transform.position.y - offsetPosicionY;
            Vector3 posicionActual = new Vector3(posicionActualX, posicionActualY);

            if (posicionActual != posicion)
            {
                if (posicionActual.x < posicion.x)
                    direccionX = "derecha";
                else if (posicionActual.x > posicion.x)
                    direccionX = "izquierda";

                if (posicionActual.y < posicion.y)
                    direccionY = "arriba";
                else if (posicionActual.y > posicion.y)
                    direccionY = "abajo";

                moviendo = true;
                seleccionada = false;
            }
            else
                moviendo = false;
        }
    }

    /// <summary>
    /// Agrega posiciones de movimiento segun el radio de movimiento de la unidad
    /// </summary>
    public void DeterminarRadioTiles(int posUnidadX, int posUnidadY)
    {
        // determina la posicion del primer tile de iteracion
        int posInicialX = posUnidadX + 1;
        int posInicialY = posUnidadY - radioMovimiento;

        // determina cuantas filas tiene el radio
        int cantFilas = ((radioMovimiento * 2) + 1) + posInicialY;
        int cantTilesPorFila = posInicialX - 1;

        for (int posY = posInicialY; posY < cantFilas; posY++)
        {
            // dependiendo de posY, setea cuantos tiles debe tener la fila
            if (posY <= posUnidadY)
            {
                cantTilesPorFila++;
                posInicialX--;
            }
            else
            {
                cantTilesPorFila--;
                posInicialX++;
            }

            for (int posX = posInicialX; posX < cantTilesPorFila; posX++)
            {
                // evita guardar la posicion del jugador
                if (posX == posUnidadX && posY == posUnidadY)
                    continue;

                tilesMovimiento.Add(new Vector2(posX, posY));
            }
        }
    }

    public bool ClickEnTileMovimiento(int x, int y)
    {
        // Comprueba que el tile clickeado este dentro de la lista de disponibles

        foreach (Vector2 posTile in tilesMovimiento)
        {
            if (posTile.x == x && posTile.y == y)
                return true;
        }

        return false;
    }

    public bool ClickEnTileAtaque(int x, int y)
    {
        // Comprueba que el tile clickeado este dentro de la lista de disponibles

        foreach (Vector2 posTile in tilesAtaque)
        {
            if (posTile.x == x && posTile.y == y)
                return true;
        }

        return false;
    }

    // @TODO: crear metodos de ataque, ser golpeado, morir, etc

    public List<Vector2> ObtenerTilesMovimiento()
    {
        return tilesMovimiento;
    }

    public List<Vector2> ObtenerTilesAtaque()
    {
        return tilesAtaque;
    }

    public void CambiarTilesMovimiento(List<Vector2> nuevosTilesMovimiento)
    {
        tilesMovimiento = nuevosTilesMovimiento;
    }

    public void CambiarTilesAtaque(List<Vector2> nuevosTilesAtaque) {
        tilesAtaque = nuevosTilesAtaque;
    }

    public int ObtenerRadioMovimiento() {
        return radioMovimiento;
    }

    public void ToggleSeleccion(bool estado) {
        seleccionada = estado;
    }

    public bool EstaSeleccionada() {
        return seleccionada;
    }

    public bool EstaMoviendo() {
        return moviendo;
    }

    public Vector3 ObtenerPosicion()  {
        return transform.position;
    }
}

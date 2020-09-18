using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Unidad : MonoBehaviour 
{
    [SerializeField] private float tiempoMov = 300f;
    [SerializeField] private float offsetPosicionX = 70f;
    [SerializeField] private float offsetPosicionY = 115f;

    [SerializeField] private int radioMov = 2;
    [SerializeField] private float vida = 0;
    [SerializeField] private float armadura = 0;
    [SerializeField] private float ataque = 0;

    private Animator animador;
    private SpriteRenderer sprite;

    private List<Vector2> tilesDisponibles;
    private bool seleccionada = false;
    private bool moviendo = false;
    private string direccionX = "";
    private string direccionY = "";

    void Start()
    {
        // agrega automaticamente el script a la lista de unidades
        ControladorJuego.instancia.AgregarUnidad(this);

        // instancia los componentes del objeto
        animador = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        tilesDisponibles = new List<Vector2>();
    }

    public void Mover(Vector3 posicion)
    {
        if (moviendo)
        {
            float posicionActualX = transform.position.x - offsetPosicionX;
            float posicionActualY = transform.position.y - offsetPosicionY;
            Vector3 posicionActual = new Vector3(posicionActualX, posicionActualY);

            // inicia la animacion de movimiento
            animador.SetBool("moviendo", true);

            // comprueba la direccion a la que va el personaje
            // esto se hace entre los opuestos (arriba - abajo | izquierda - derecha)
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
                // deja de animar al personaje
                animador.SetBool("moviendo", false);
                moviendo = false;
                // posiciona al personaje en su lugar, ya que puede que se haya pasado de su destino
                transform.position = new Vector3(posicion.x + offsetPosicionX, posicion.y + offsetPosicionY, transform.position.z);
            }
        }
    }

    public bool EstaSeleccionada() 
    {
        return seleccionada;
    }

    private void OnMouseDown()
    {
        if (!moviendo)
            // comprueba que se haya hecho click sobre el collider
            seleccionada = true;
    }

    public List<Vector2> DeterminarMovimiento(int posUnidadX, int posUnidadY)
    {
        // determina la posicion inicial de la matriz
        int posInicialX = posUnidadX - radioMov;
        int posInicialY = posUnidadY - radioMov;

        // determina cuantos tiles tiene cada fila y columna de la matriz
        int cantTilesFilas = ((radioMov * 2) + 1) + posInicialX;
        int cantTilesCols = ((radioMov * 2) + 1) + posInicialY;

        for (int posX = posInicialX; posX < cantTilesFilas; posX++)
        {
            for (int posY = posInicialY; posY < cantTilesCols; posY++)
            {
                // no guarda la posicion del jugador, solo las adyacentes
                if (posX == posUnidadX && posY == posUnidadY)
                    continue;

                tilesDisponibles.Add(new Vector2(posX, posY));
            }
        }

        return tilesDisponibles;
    }

    public bool ClickTileEsValido(int x, int y)
    {
        foreach (Vector2 posTile in tilesDisponibles)
        {
            if (posTile.x == x && posTile.y == y)
                return true;
        }

        return false;
    }

    public void DebeMover(Vector3 posicion)
    {
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

    public void SetPosicion(Vector3 posMundo)
    {
        Vector3 pos = transform.position;
        pos.x = posMundo.x + offsetPosicionX;
        pos.y = posMundo.y + offsetPosicionY;
        transform.position = pos;
    }

    public Vector3 GetPosicion()
    {
        return transform.position;
    }

    // crear metodos de ataque, ser golpeado, morir, etc
}

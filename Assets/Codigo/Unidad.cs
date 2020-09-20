using System.Collections.Generic;
using UnityEngine;

public class Unidad : MonoBehaviour 
{
    [SerializeField] private float tiempoMov;
    [SerializeField] private float offsetPosicionX;
    [SerializeField] private float offsetPosicionY;

    [SerializeField] private int radioMov;
    [SerializeField] private float vida;
    [SerializeField] private float armadura;
    [SerializeField] private float ataque;

    private Animator animador;
    private SpriteRenderer sprite;

    private List<Vector2> radioTiles;
    
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

        radioTiles = new List<Vector2>();
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

    private void OnMouseDown()
    {
        if (!moviendo)
            // comprueba que se haya hecho click sobre el collider
            seleccionada = true;
    }

    public List<Vector2> DeterminarRadioTiles(int posUnidadX, int posUnidadY)
    {
        // Crea y devuelve la lista de las posiciones de los tiles de movimiento para la unidad

        // determina la posicion del primer tile
        int posInicialX = posUnidadX + 1;
        int posInicialY = posUnidadY - radioMov;

        // determina cuantos tiles tiene cada fila y columna de la lista
        int cantFilas = ((radioMov * 2) + 1) + posInicialY;
        int cantTilesPorFila = posInicialX - 1;

        for (int posY = posInicialY; posY < cantFilas; posY++)
        {
            
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
                // check si la posicion a guardar no es la de la unidad
                if (posX == posUnidadX && posY == posUnidadY)
                    continue;

                radioTiles.Add(new Vector2(posX, posY));
            }
        }
        
        return radioTiles;
    }

    public bool ClickEnTileMovimiento(int x, int y)
    {
        // Comprueba que el tile clickeado este dentro de la lista de disponibles

        foreach (Vector2 posTile in radioTiles)
        {
            if (posTile.x == x && posTile.y == y)
                return true;
        }

        return false;
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

    public bool EstaSeleccionada()
    {
        return seleccionada;
    }

    public void DesSeleccionar()
    {
        seleccionada = false;
    }

    public Vector3 ObtenerPosicion()
    {
        return transform.position;
    }

    // @TODO: crear metodos de ataque, ser golpeado, morir, etc
}

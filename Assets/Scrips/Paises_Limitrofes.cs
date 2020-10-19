using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paises_Limitrofes : MonoBehaviour
{
    string nombrePais; //Nombre del pais que esta seleccionado
    Color limitrofe = new Color(1f, 0f, 0f);//Color de los paises limitrofes 
    private void OnTriggerEnter2D(Collider2D collision)//Cuando el collider collisiona con algo te pasa el collider del gameObject que collisiono
    {

        if (!collision.gameObject.tag.Equals("Seleccionado"))//ignoramos si collisiona con un gameObject con la tag Seleccionado
        {
            if (collision.gameObject.tag.Equals("pais"))//si collisiona con un gameObject con la tag pais lo transformamos en un limitrofe 
            {
                collision.gameObject.tag = "Limitrofe";
                collision.gameObject.GetComponent<SpriteRenderer>().color = limitrofe;
                nombrePais = collision.gameObject.name;

            }
            else//si no buscamos un gameobject que comparta parte del nombre con la colisión y lo transformamos en un limitrofe 
            {
                GameObject paisRasteado = GameObject.Find(collision.name.Substring(7));
                if (paisRasteado != null && paisRasteado.gameObject.tag.Equals("pais"))
                {
                    paisRasteado.tag = "Limitrofe";
                    paisRasteado.GetComponent<SpriteRenderer>().color = limitrofe;
                    nombrePais = paisRasteado.name;
                }
            }
        }
        

    }

}


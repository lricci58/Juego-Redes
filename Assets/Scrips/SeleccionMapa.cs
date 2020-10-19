using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class SeleccionMapa : MonoBehaviour
{
    private int escenaMenu = 0;
    private int escenaBatalla = 2;
    [SerializeField] private int numUnidades;
    [SerializeField] private string[] limitrofes;

    //creamos 2 colores para modificar los sprites
    private Color colorDeseleccionado = new Color(1, 1, 1);//colordeseleccionado es el color que tienen por defecto
    private Color colorSeleccionado = new Color(0.7f, 0.7f, 0.7f);//colorSeleccionado es el color que van a tener los países cuando se seleccionen PD: para que unity sepa que los números son float y no double es necesario poner una f 

    GameObject paisSeleccionado;//GameObject donde se guarda la referencia del país Seleccionado

    public GameObject detetectorLimitrofes;//referensia al prefab del Detector

    private void OnMouseDown()//función que se llama al pulsar un collider
    {
        ConnectionManager.instance.CmdCountrySelected(gameObject.name, limitrofes);

        //paisSeleccionado = GameObject.FindGameObjectWithTag("Seleccionado");//buscamos un GameObject con la tag "Seleccionado" y guardamos su referencia PD:si existen más de 2 gameobjects no somos capaces de saber cual nos da 

        // GameObject[] paisesLimitrofes = GameObject.FindGameObjectsWithTag("Limitrofe");//buscamos todos los GameObject con la tag "Limitrofe" y guardamos sus referencias 

        //if (paisSeleccionado != null) //si existe referencia a un GameObject
        //{
        //    if (!this.tag.Equals("Limitrofe"))//evaluamos si es limitrofe
        //    {
        //        paisSeleccionado.GetComponent<SpriteRenderer>().color = colorDeseleccionado;

        //        limpiarLimitrofes(paisesLimitrofes);
        //        GestorDetector(this.gameObject);
        //    }
        //    else{
        //        GameManager.instance.playerBattleSide = 1;
        //        SceneManager.LoadScene(escenaBatalla);

        //        paisSeleccionado.GetComponent<SpriteRenderer>().color = colorDeseleccionado;

        //        limpiarLimitrofes(paisesLimitrofes);
        //        GestorDetector(this.gameObject);
        //    }

        //    paisSeleccionado.tag = "pais";
        //}
        //else//si no existe referencia a un GameObject le cambiamos al GameObject del país que apretamos el color y el tag para que esté Seleccionado
        //{
        //    GetComponent<SpriteRenderer>().color = colorSeleccionado;
        //    this.tag = "Seleccionado";
        //    GestorDetector(this.gameObject);
        //}
    }

    //public void GestorDetector(GameObject paisSeleccionado)//funcion que instancia/destruye el GameObject que detecta los paises limitrofes 
    //{
    //    GameObject detetectorTemp = GameObject.FindGameObjectWithTag("temp");
    //    if (detetectorTemp !=null)//si existe detetectorTemp, resetea el prefab y lo destruye
    //    {
    //       Destroy(detetectorTemp.GetComponent<PolygonCollider2D>());
    //       Destroy(detetectorTemp);
    //    }
    //    else//si no configura el prefab del Detector y lo instancia
    //    {
    //        detetectorLimitrofes.GetComponent<PolygonCollider2D>().pathCount = paisSeleccionado.GetComponent<PolygonCollider2D>().pathCount;
    //        detetectorLimitrofes.GetComponent<PolygonCollider2D>().points = paisSeleccionado.GetComponent<PolygonCollider2D>().points;

    //        detetectorLimitrofes.transform.localScale = paisSeleccionado.transform.localScale + new Vector3(0.5f, 0.5f, 0);
    //        detetectorLimitrofes.tag = "temp";
    //        Instantiate(detetectorLimitrofes, paisSeleccionado.transform.localPosition - new Vector3(0, 0, 2), Quaternion.identity);
    //    }
    //}

    //public void limpiarLimitrofes(GameObject[] paisesLimitrofes)//funcion que transforma los paises limitrofes en paises
    //{
    //    for (int i = 0; i < paisesLimitrofes.Length; i++)
    //    {
    //        paisesLimitrofes[i].GetComponent<SpriteRenderer>().color = colorDeseleccionado;
    //        paisesLimitrofes[i].tag = "pais";
    //    }
    //}
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnidadManager : MonoBehaviour 
{
    bool seleccionado = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MouseClick();

        // permite mover a la unidad
        if (seleccionado)
        {
            // TODO: deberia dejar mover de tile en tile en un rango determinado

            if (Input.GetKey("right"))
            {
                gameObject.transform.Translate(200f * Time.deltaTime, 0, 0);
                gameObject.GetComponent<Animator>().SetBool("moviendo", true);
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            if (Input.GetKey("left"))
            {
                gameObject.transform.Translate(-200f * Time.deltaTime, 0, 0);
                gameObject.GetComponent<Animator>().SetBool("moviendo", true);
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (Input.GetKey("up"))
            {
                gameObject.transform.Translate(0, 200f * Time.deltaTime, 0);
                gameObject.GetComponent<Animator>().SetBool("moviendo", true);
            }
            if (Input.GetKey("down"))
            {
                gameObject.transform.Translate(0, -200f * Time.deltaTime, 0);
                gameObject.GetComponent<Animator>().SetBool("moviendo", true);
            }

            if (!Input.GetKey("right") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("down"))
            {
                gameObject.GetComponent<Animator>().SetBool("moviendo", false);
            }
        }
    }

    void MouseClick()
    {
        // detecta si hizo click
        if (Input.GetMouseButtonDown(0))
        {
            // detecta si se hizo click dentro del collider
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                seleccionado = !seleccionado;
            }
        }
    }
}

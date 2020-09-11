using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InftHachaHandler : MonoBehaviour
{
    //GameObject un1;
    // Start is called before the first frame update
    void Start()
    {
        //un1 = GameObject.Find("Unidad");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.GetComponent<Animator>().SetBool("selected", !gameObject.GetComponent<Animator>().GetBool("selected"));
        }

        if (gameObject.GetComponent<Animator>().GetBool("selected"))
        {
            if (Input.GetKey("right"))
            {
                gameObject.transform.Translate(200f * Time.deltaTime, 0, 0);
                gameObject.GetComponent<Animator>().SetBool("moving", true);
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            if (Input.GetKey("left"))
            {
                gameObject.transform.Translate(-200f * Time.deltaTime, 0, 0);
                gameObject.GetComponent<Animator>().SetBool("moving", true);
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (Input.GetKey("up"))
            {
                gameObject.transform.Translate(0, 200f * Time.deltaTime, 0);
                gameObject.GetComponent<Animator>().SetBool("moving", true);
            }
            if (Input.GetKey("down"))
            {
                gameObject.transform.Translate(0, -200f * Time.deltaTime, 0);
                gameObject.GetComponent<Animator>().SetBool("moving", true);
            }
            if (!Input.GetKey("right") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("down"))
            {
                gameObject.GetComponent<Animator>().SetBool("moving", false);
            }
        }
    }
}

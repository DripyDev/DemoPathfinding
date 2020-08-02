using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboController : MonoBehaviour
{
    public Bresenham main;
    // Start is called before the first frame update
    void Start(){
        main = GameObject.Find("MainController").GetComponent<Bresenham>();
    }
    
    //Click izquierdo cambia origen, derecho el destino
    void OnMouseDown() {
        Vector3 pos = new Vector3((int) this.transform.position.x, 1, (int)this.transform.position.z);
        main.origen = pos;
    }

    void OnMouseOver () {
        if(Input.GetMouseButtonDown(1)){
            Vector3 pos = new Vector3((int) this.transform.position.x, 1, (int)this.transform.position.z);
            main.destino = pos;
        }
        if(Input.GetMouseButtonDown(2)){
            Vector3 pos = new Vector3((int) this.transform.position.x, 1, (int)this.transform.position.z);
            print("Cambiamos caminable del cubo en: " + pos);
            main.CambioCaminable(pos);
        }
    }
}

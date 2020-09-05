using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Bresenham : MonoBehaviour {
    public bool bres = false;
    public int numeroCubos;
    public int dimensionesCubo;
    public Vector3 origen;
    public Vector3 destino;
    public List<Cubo> mundo;
    public List<Cubo> path;
    public MeshRenderer prefab;
    public Transform cubeHolder;
    private Vector3 origenAux;
    private Vector3 destinoAux;
    public int origenPrintX = 0;
    public int origenPrintY = 0;
    public int destinoPrintX = 0;
    public int destinoPrintY = 0;

    void Start() {
        origenAux = origen; destinoAux = destino;

        int raizN = (int) Math.Sqrt((double)numeroCubos);
        mundo = new List<Cubo>();
        //Radios de cubos 2D
        float radioCubo = dimensionesCubo*dimensionesCubo / numeroCubos;
        
        Vector3 pos = new Vector3(radioCubo, 1, radioCubo);//pos primer cubo
        Vector3 posX, posZ;
        posX = pos;
        //Avanzamos en el eje X
        for (int i = 0; i < raizN; i++) {
            posZ = posX;
            posX += new Vector3(dimensionesCubo, 0,0);
            //Avanzamos en el eje Z
            for (int j = 0; j < raizN; j++){
                Quaternion rot = Quaternion.Euler (0, 0, 0);
                var cub = Instantiate(prefab, posZ, rot);
                cub.transform.parent = cubeHolder;
                cub.material.color = Color.magenta;

                var cubito = new Cubo(posZ, dimensionesCubo, cub);
                mundo.Add(cubito);

                //Debug.Log("Cubo creado en: " + cubito.centro);
                posZ += new Vector3(0,0,dimensionesCubo);
            }
        }
        if(bres)
            path = BresenhamError((int) origen.x, (int) origen.z, (int) destino.x, (int) destino.z);
        else{
            LimpiarValores();
            path = AStar((int) origen.x, (int) origen.z, (int) destino.x, (int) destino.z);
            print("path.count: " + path.Count);
        }
    }

    //Dada una posicion, devuelve el correspondiente cubo de mundo
    //NOTA: SE QUE ASI TODO SE VUEVE INEFICIENTE PERO ES PARA HACERLO VISUAL
    public Cubo Encontrar(Vector3 pos){
        //Foreach devuelve copias, no? por eso necesitamos un for
        for (int i = 0; i < mundo.Count; i++){
            if(mundo[i].centro == pos)
                return mundo[i];
        }
        //Error
        print("No hemos encontrado el cubo");
        return new Cubo(new Vector3(0,0,0), dimensionesCubo, prefab);
    }

    public List<Cubo> AStar(int xI, int yI, int xF, int yF){
        print("Path desde: " + xI + "," + yI + " a: " + xF + "," + yF);
        List<Cubo> openList = new List<Cubo>();
        List<Cubo> closedList = new List<Cubo>();
        Cubo inicio = Encontrar(new Vector3(xI,1,yI));
        Cubo final = Encontrar(new Vector3(xF,1,yF));
        
        int indiceInicio = Indice(inicio);
        int indiceFinal = Indice(final);
        inicio.g=0;
        inicio.h=CalcularH(mundo[indiceInicio], mundo[indiceFinal]);
        inicio.f = inicio.h+inicio.g;
        mundo[indiceInicio] = inicio;

        
        openList.Add(mundo[indiceInicio]);
        Cubo nodoActual = mundo[indiceInicio];
        while(openList.Count > 0){
            //print("Nodo actual: " + nodoActual.centro);
            //print("openlist.Count: " + openList.Count);
            nodoActual = NodoMenorF(openList);
            int indiceActual = Indice(nodoActual);
            if(nodoActual.centro == final.centro){
                print("Hemos llegado al final");
                return CalcularPath(mundo[indiceActual]);
            }
            openList.Remove(mundo[indiceActual]);
            closedList.Add(mundo[indiceActual]);
            List<Cubo> vecinos = NodosVecinos(mundo[indiceActual]);
            //print("Numero vecinos: " + vecinos.Count);
            foreach (Cubo v in vecinos){
                print("Analizando vecinos");
                var aux = v;
                int indiceAux = Indice(aux);
                if(closedList.Contains(mundo[indiceAux])){
                    //print("closedList contiene el vecino, lo saltamos");
                    continue;
                }
                
                if(!mundo[indiceAux].caminable){
                    print("Vecino no caminable");
                    //if(openList.Contains(mundo[indiceAux]))
                    //    openList.Remove(mundo[indiceAux]);
                    closedList.Add(mundo[indiceAux]);
                    continue;
                }

                int tentativeG = mundo[indiceActual].g + CalcularH(mundo[indiceActual], mundo[indiceAux]);
                //print("TentativeG: " + tentativeG);
                //print("Valor g del vecino: " + aux.g);
                //mundo[indiceActual].prefab.material.color = Color.green;
                if(tentativeG < mundo[indiceAux].g){
                    aux.cameFrom = indiceActual;
                    aux.g = tentativeG;
                    aux.h = CalcularH(mundo[indiceAux], mundo[indiceFinal]);
                    //aux.f = mundo[indiceAux].g+mundo[indiceAux].h;
                    aux.f = aux.g+ aux.h;
                    
                    mundo[indiceAux] = aux;
                    if(!openList.Contains(mundo[indiceAux])){
                        //print("Añadimos el vecino a la openList");
                        openList.Add(mundo[indiceAux]);
                    }
                }
            }
        }
        print("No hemos encontrado camino");
        //Error, no existe camino posible
        return null;
    }

    List<Cubo> NodosVecinos(Cubo a){
        int tamaño = dimensionesCubo * (int) Math.Sqrt((double)numeroCubos);
        List<Cubo> vec = new List<Cubo>();
        if(a.centro.x -dimensionesCubo >= 0){
            vec.Add(Encontrar(new Vector3(a.centro.x-dimensionesCubo, 1, a.centro.z)));//Izquierda
            if(a.centro.z-dimensionesCubo >= 0)
                vec.Add(Encontrar(new Vector3(a.centro.x-dimensionesCubo, 1, a.centro.z-dimensionesCubo)));//Izquierda abajo
            if(a.centro.z+dimensionesCubo < tamaño)
                vec.Add(Encontrar(new Vector3(a.centro.x-dimensionesCubo, 1, a.centro.z+dimensionesCubo)));//Izquierda arriba
        }
        if(a.centro.x + dimensionesCubo < tamaño){
            vec.Add(Encontrar(new Vector3(a.centro.x+dimensionesCubo, 1, a.centro.z)));//Derecha
            if(a.centro.z-dimensionesCubo >= 0)
                vec.Add(Encontrar(new Vector3(a.centro.x+dimensionesCubo, 1, a.centro.z-dimensionesCubo)));//Derecha abajo
            if(a.centro.z+dimensionesCubo < tamaño)
                vec.Add(Encontrar(new Vector3(a.centro.x+dimensionesCubo, 1, a.centro.z+dimensionesCubo)));//Derecha arriba
        }
        if(a.centro.z-dimensionesCubo >= 0)
            vec.Add(Encontrar(new Vector3(a.centro.x, 1, a.centro.z-dimensionesCubo)));//Abajo
        if(a.centro.z+dimensionesCubo < tamaño)
            vec.Add(Encontrar(new Vector3(a.centro.x, 1, a.centro.z+dimensionesCubo)));//Abajo
        return vec;
    }

    List<Cubo> CalcularPath(Cubo a){
        List<Cubo> res = new List<Cubo>();
        res.Add(a);
        a.prefab.material.color = Color.yellow;
        Cubo actual = a;
        while(actual.cameFrom != -1){
            print("Añadimos uno en el path");
            print("CameFrom: " + actual.cameFrom);
            res.Add(mundo[actual.cameFrom]);
            mundo[actual.cameFrom].prefab.material.color = Color.red;
            actual = mundo[actual.cameFrom];
            //var aux = mundo[actual.cameFrom];
            //aux.prefab.material.color = Color.green;
            //mundo[actual.cameFrom] = aux;
        }
        if(actual.cameFrom == -1)
            actual.prefab.material.color = Color.green;
        return DarVuelta(res);
    }

    List<Cubo> DarVuelta(List<Cubo> lista){
        List<Cubo> aux = new List<Cubo>();
        for (int i = lista.Count-1; i >= 0; i--){
            aux.Add(lista[i]);
        }
        return lista;
    }

    Cubo NodoMenorF(List<Cubo> lista){
        if(lista.Count <= 0){
            print("Lista vacia en NodoMenorF");
            return new Cubo(new Vector3(-1,-1,-1), -1, prefab);
        }
        Cubo menor = lista[0];
        foreach (var n in lista){
            if(n.f < menor.f)
                menor = n;
        }
        return menor;
    }

    int CalcularH(Cubo a, Cubo b){
        //Hay que dividir entre las dimensiones del cubo
        int xD = (int) (Mathf.Abs(a.centro.x - b.centro.x)/dimensionesCubo);
        int zD = (int) (Mathf.Abs(a.centro.z - b.centro.z)/dimensionesCubo);
        int resto = (int) Mathf.Abs(xD - zD);
        return 14*Math.Min(xD,zD) + 10*resto;
    }

    //Devuelve el indice de a en el mundo
    int Indice(Cubo a){
        for (int i = 0; i < mundo.Count; i++){
            if(a.centro == mundo[i].centro)
                return i;
        }
        print("No hemos enconttrado el indice del cubo de mierda de lo0s cojones :)");
        //Error, no esta en la lista
        return -1;
    }


    public List<Cubo> BresenhamError(int x, int y, int x1, int y1){
        List<Cubo> pathAux = new List<Cubo>();
        
        int dx = Math.Abs(x1-x);
        var sx = x < x1? dimensionesCubo:-dimensionesCubo;
        int dy = -Math.Abs(y1-y);
        var sy = y < y1? dimensionesCubo:-dimensionesCubo;
        //Caso en el que sea linea horizontal o vertical no hace falta usar el algoritmo
        if(dx == 0 || dy == 0)
            return BresenhamRecto(x, y, x1, y1);
        var err = dx+dy;
        Color colorAux = Color.green;
        while(true){
            var cubito = Encontrar(new Vector3(x, 1, y));
            pathAux.Add(cubito);
            cubito.prefab.material.color = colorAux;
            colorAux = Color.yellow;

            //Somos vecinos o el mismo, salimos
            if(dx <=1 && dy <= 1)
                return null;
            //Hemos llegado al final, salimos del bucle
            if(x==x1 && y==y1)
                break;
            var e2 = 2*err;
            if(e2>=dy){
                err += dy;
                x+=sx;
            }
            if(e2 <= dx){
                err += dx;
                y+=sy;
            }
        }
        return pathAux;
    }

    public List<Cubo> BresenhamRecto(int x, int y, int x1, int y1){
        List<Cubo> pathAux = new List<Cubo>();
        var sx = x < x1? dimensionesCubo:-dimensionesCubo;
        var sy = y < y1? dimensionesCubo:-dimensionesCubo;
        Color colorAux = Color.green;
        while(true){
            var cubito = Encontrar(new Vector3(x, 1, y));
            pathAux.Add(cubito);
            cubito.prefab.material.color = colorAux;
            colorAux = Color.yellow;
            if(x==x1 && y==y1)
                break;
            if(x != x1){
                x += sx;
            }
            if(y != y1){
                y += sy;
            }
        }
        return pathAux;
    }

    void LimpiarColores(){
        foreach (var c in mundo){
            if(c.prefab.material.color != Color.grey)
                c.prefab.material.color = Color.magenta;
        }
    }
    void LimpiarValores(){
        for (int i = 0; i < mundo.Count; i++) {
            //print("Limpiamos valores del cubo de indice: " + i);
            //print("g: " + mundo[i].g + " h: " + mundo[i].h + " f: " + mundo[i].f + " cameFrom: " + mundo[i].cameFrom);
            var aux = mundo[i];
            aux.g = int.MaxValue;
            aux.h=0;
            aux.f=aux.g+aux.h;
            aux.cameFrom = -1;
            mundo[i] = aux;
            //print("g: " + mundo[i].g + " h: " + mundo[i].h + " f: " + mundo[i].f + " cameFrom: " + mundo[i].cameFrom);
        }
    }

    public void CambioCaminable(Vector3 pos){
        int indice = Indice(Encontrar(pos));
        Cubo aux = mundo[indice];
        if(mundo[indice].caminable){
            aux.caminable = false;
            aux.prefab.material.color = Color.grey;
            //No se puede modificar directamente mundo[indice] y aux es solo una copia, asi que hay que volver a reasignarselo
            mundo[indice] = aux;
        }
        else{
            aux.caminable = true;
            aux.prefab.material.color = Color.magenta;
            mundo[indice] = aux;
            
        }
    }

    // Update is called once per frame
    void Update(){
        if(origen != origenAux || destino != destinoAux){
            LimpiarColores();//Limpiamos los colores del anterior path
            if(bres)
                path = BresenhamError((int) origen.x, (int) origen.z, (int) destino.x, (int) destino.z);
            else {
                LimpiarValores();
                path = AStar((int) origen.x, (int) origen.z, (int) destino.x, (int) destino.z);
                print("path.count: " + path.Count);
            }
            origenAux = origen; destinoAux = destino;
        }
        //var aux = BresenhamErrorInt(origenPrintX, origenPrintY, destinoPrintX, destinoPrintY);
        /*print("Camino entre: " + origenPrintX + origenPrintY + " y " + destinoPrintX + destinoPrintY);
        foreach (var a in path){
            print("Path: " + a.centro);
        }*/
    }
    
    public List<(int,int)> BresenhamErrorInt(int x, int y, int x1, int y1){
        List<(int,int)> pathAux = new List<(int,int)>();
        
        int dx = Math.Abs(x1-x);
        var sx = x < x1? 1:-1;
        int dy = -Math.Abs(y1-y);
        var sy = y < y1? 1:-1;
        //Caso en el que sea linea horizontal o vertical no hace falta usar el algoritmo
        if(dx == 0 || dy == 0)
            return BresenhamRectoInt(x, y, x1, y1);
        var err = dx+dy;
        Color colorAux = Color.green;
        while(true){
            pathAux.Add((x,y));

            //Somos vecinos o el mismo, salimos
            if(dx <=1 && dy <= 1)
                return null;
            //Hemos llegado al final, salimos del bucle
            if(x==x1 && y==y1)
                break;
            var e2 = 2*err;
            if(e2>=dy){
                err += dy;
                x+=sx;
            }
            if(e2 <= dx){
                err += dx;
                y+=sy;
            }
        }
        return pathAux;
    }
    public List<(int,int)> BresenhamRectoInt(int x, int y, int x1, int y1){
        List<(int,int)> pathAux = new List<(int,int)>();
        var sx = x < x1? 1:-1;
        var sy = y < y1? 1:-1;
        while(true){
            pathAux.Add((x,y));
            if(x==x1 && y==y1)
                break;
            if(x != x1){
                x += sx;
            }
            if(y != y1){
                y += sy;
            }
        }
        return pathAux;
    }

    void OnDrawGizmosSelected() {
        Color color = Color.red; color.a = 0.1f;
        Color colorA = Color.blue; colorA.a = 0.1f;
        Gizmos.color = colorA;
        //Dibujo del mundo
        foreach (var c in mundo) {
            Gizmos.DrawCube(c.centro, c.dimensiones);
            Gizmos.color = color;
        }
        //Dibujo del path
        /*Gizmos.color = Color.yellow;
        foreach (var c in path) {
            Gizmos.DrawCube(c.centro, c.dimensiones);
        }*/
        Gizmos.color = Color.black;
        Gizmos.DrawLine(origen, destino);

        
    }
    public struct Cubo{
        public Vector3 centro;
        public Vector3 dimensiones;
        public bool caminable;
        //Coste desde el nodo de inicio a nosotros
        public int g;
        //Distancia recta desde nosotros al objetivo
        public int h;
        //f=g+h
        public int f;
        //Nodo del que proviene
        public int cameFrom;
        public MeshRenderer prefab;
        public Cubo(Vector3 pos, int dim, MeshRenderer pref){
            this.centro = pos;
            this.dimensiones = new Vector3(dim, dim, dim);
            this.prefab = pref;
            this.caminable = true;
            this.g = int.MaxValue;
            this.h=0;
            this.f=this.g+this.h;
            this.cameFrom = -1;
        }
    }
}

# DemoPathfinding
Esta es una pequeña demo para experimentar sobre algunos algoritmos de pathfinding, en concreto el de Bresenham y A*. Estos algoritmos van a ser implementados bajo el supuesto de un mundo formado por casillas cuadradas todas con el mismo peso, pero algunas de estas podrán ser obstáculos, intransitables.

## Bresenham
Este es técnicamente un algoritmo para dibujar líneas rectas entre dos puntos A y B de manera eficiente, pero como el camino mas corto entre dos puntos en un mundo bidimensional es la línea recta, no es útil. Hay que ser conscientes de que no tiene en cuenta posibles obstáculos o diferentes pesos. Se puede implementar de diferentes maneras, ya sea a través del cálculo de la pendiente o a través del cálculo del error. La segunda forma es la implementada. Para entender en mayor profundidad el algoritmo recomiendo los siguientes enlaces:
+ https://www.javatpoint.com/computer-graphics-bresenhams-line-algorithm
+ https://www.geeksforgeeks.org/bresenhams-line-generation-algorithm/

En la siguiente imagen vemos la recta generada desde el origen (cubo verde) hasta el destino (el último cubo de la recta) a través del algoritmo de Bresenham.

![Recta Bresenham]()

## A*
Similar en cierto sentido a otros algoritmos como **Dijkstra**, **Bellman Ford** o **Floyd Warshall**, **A*** o *A Star*, este algoritmo de pathfinding si que tiene en cuenta casillas no caminables e incluso pesos. Para entenderlo en mayor profundidad recomiendo los siguientes enlaces:
+ https://www.geeksforgeeks.org/a-search-algorithm/
+ https://www.simplilearn.com/tutorials/artificial-intelligence-tutorial/a-star-algorithm#how_to_implement_the_a_algorithm_in_python

O la [serie de vídeos](https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW) de Sebastian Lague donde lo explica en mucha profundidad y de manera sencilla y visual. La siguiente imagen muestra como con A* se pueden crear rutas desde un origen (casilla verde) a un destino (casilla amarilla) evitando obstáculos intransitables (casillas grises).

### Funcionamiento
El proyecto está compuesto de dos partes principales, la cámara y el pathfinding. Los controles de la cámara són:
  -w,a,s,d / up,dowa,left,rigth: Movimiento de la cámara.
  -Control / espacio: Control es para reducir la altura y espacio para aumentarla.
  -Shift: Cambia el modo de movimiento de la cámara. De forma predefinida avanza en la dirección a la que miramos, al cambiar de modo avanzamos (o retrocedemos...) de manera global sin tener en cuenta la dirección.
  
Para ver los resultados de los algoritmos, de manera predefinida se usa Bresenham, basta con hacer click izquierdo en uno de los cubos para determinar el origen y click derecho para determinar el destino. Automaticamente devolverá el camino entre los dos coloreando de amarillo la ruta. Si desde el inspector de Unity, en el GameObject "RegionManager" cambiamos el booleano 'bresen' usará A* para determinar la ruta. Al igual que antes, con marcar origen y destino devolverá el camino pero en rojo. Si hacemos click con el botón de la rueda del ratón en un cubo, se coloreará de gris y se considerará no caminable. Bresenham lo ignorará pero A* evitará esos bloques.

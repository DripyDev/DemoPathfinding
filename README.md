# DemoPathfinding
Esta es una pequeña demo para experimentar sobre el uso de los algoritmos de Bresenham y A*.
Bresenham sirve para dibujar líneas rectas pero se ha usado como algoritmo de pathfinding ya que el camino más corto entre dos puntos es la recta. Sin embargo no siempre será útil si dentro del mapa se contempla la posibilidad de que haya nodos no caminables o diferentes pesos etc.
Por ello está también implementado el algoritmo de búsqueda A* que siempre que exista una posible ruta, devolverá la óptima.
El proyecto está compuesto de dos partes principales, la cámara y el pathfinding. Los controles de la cámara són:
  -w,a,s,d / up,dowa,left,rigth: Movimiento de la cámara.
  -Control / espacio: Control es para reducir la altura y espacio para aumentarla.
  -Shift: Cambia el modo de movimiento de la cámara. De forma predefinida avanza en la dirección a la que miramos, al cambiar de modo avanzamos (o retrocedemos...) de manera global sin tener en cuenta la dirección.
Para ver los resultados de los algoritmos, de manera predefinida usa Bresenham, basta con hacer click izquierdo en uno de los cubos para determinar el origen y click derecho para determinar el destino. Automaticamente devolverá el camino entre los dos coloreando de amarillo la ruta. Si desde el inspector de Unity, en el GameObject "RegionManager" cambiamos el booleano 'bresen' usará A* para determinar la ruta. Al igual que antes, con marcar origen y destino devolverá el camino pero en rojo. Si hacemos click con el botón de la rueda del ratón en un cubo, se coloreará de gris y se considerará no caminable. Bresenham lo ignorará pero A* evitará esos bloques.

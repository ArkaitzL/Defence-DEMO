using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaboOnLite;

public class Ataque : MonoBehaviour
{
    [SerializeField] private Material area_material, centro_material;
    [SerializeField] private Tarjeta[] tarjetas = new Tarjeta[4];

    private Defensa[] defensas = new Defensa[4];
    private Defensa defensa_actual = null;

    private List<Area> area_seleccionada = new();
    private List<Casilla> casillas = new();
    private GameObject casilla_actual = null;

    private void Awake()
    {
        Controlador.TurnoEvent += Turno;
    }

    private void Start()
    {
        //Almacena las casillas jugables
        GameObject[] casillas_objetos = GameObject.FindGameObjectsWithTag("Jugable");

        foreach (var item in casillas_objetos)
        {
            casillas.Add(
                new(
                    item.GetComponent<Renderer>(),
                    item.transform.position
                )
            );
        }
    }

    private void Update()
    {
        //HOVER DE AREA DE ATAQUE
        if (defensa_actual != null)
        {
            //Rayo para comprobar hover
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(rayo, out hit) && hit.collider.CompareTag("Jugable"))
            {
                GameObject objeto = hit.collider.gameObject;

                //Si na ha cambiado return
                if (casilla_actual != objeto)
                {
                    //Vuelve a la normalidad las anteriores
                    MaterialNormal();

                    //Cambia las casillas actuales
                    foreach (var item in CalcularArea(objeto))
                    {
                        area_seleccionada.Add(
                            new Area(
                                item.render,
                                item.render?.material,
                                item.posicion
                            )
                        );

                        item.render.material = area_material;
                    }    
                }

                //Ataca
                if (Input.GetMouseButtonDown(0)) Atacar();

                casilla_actual = objeto;

            }
            else 
            {
                //Comprueba cuando no hay colicion
                if (casilla_actual == null) return;

                MaterialNormal();
                casilla_actual = null;

            }
        }

        //ONCLICK MIENTRAS SE SELECCIONA
    }

    private void Turno() 
    {
        defensas = new Defensa[4];
        defensa_actual = null;

        //SELECCIONA LAS CARTAS
        Defensa[] comunes = Almacen.get.defensas.Filter((def) => !def.especial);
        Defensa[] especiales = Almacen.get.defensas.Filter((def) => def.especial);

        for (int i = 0; i < 3; i++)
        {
            CambiarDefensa(i, comunes);
        }

        CambiarDefensa(3, especiales);
    }

    public void Seleccionar(int carta) 
    {
        //SELECCIONA LA DEFENSA ACTUAL
        defensa_actual = defensas[carta];
    }

    public void Atacar() 
    {
        //ATACA EN EL AREA DEL CLICK
        foreach (var area in area_seleccionada)
        {
            foreach (var ficha in Controlador.get.fichas)
            {
                if (ficha.Comparar(area.posicion))
                {
                    ficha.Daño(defensa_actual.daño);
                    break;
                }
            }
        }
        MaterialNormal();
        defensa_actual = null;

     
    }

    //METODOS

    public List<Casilla> CalcularArea(GameObject centro) 
    {
        List<Vector3> posiciones = new();
        List<Casilla> area = new();

        //Comprueba si en el centro hay fichas
        foreach (var ficha in Controlador.get.fichas)
        {
            if (ficha.Comparar(centro.transform.position)) return area;
        }

        //Encuentra las posiciones
        //posiciones.Add(centro.transform.position);

        //Linea X
        for (int i = defensa_actual.area.minX; i < defensa_actual.area.maxX + 1; i++)
        {
            Vector3 pos = new Vector3(
                    i + centro.transform.position.x,
                    centro.transform.position.y,
                    centro.transform.position.z
                );

            if (pos == centro.transform.position) continue;

            posiciones.Add(
                pos
            );
        }

        //Linea Y
        for (int i = defensa_actual.area.minY * -1; i < defensa_actual.area.maxY + 1 * -1; i++)
        {
            Vector3 pos = new Vector3(
                   centro.transform.position.x,
                   centro.transform.position.y,
                   i + centro.transform.position.z
               );

            if (pos == centro.transform.position) continue;

            posiciones.Add(
                pos
            );
        }

        //Otros especioales
        foreach (var area_otros in defensa_actual.area.otros)
        {
            Vector3 pos = new Vector3(
                   area_otros.x + centro.transform.position.x,
                   centro.transform.position.y,
                   area_otros.y + centro.transform.position.z
               );

            if (pos == centro.transform.position) continue;

            posiciones.Add(
                pos
            );
        }



        //Busca los renderer correspondiente a las posiciones
        foreach (var posicion in posiciones)
        {
            foreach (var casilla in casillas)
            {
                if (casilla.Comparar(posicion))
                {
                    area.Add(
                        casilla
                    );
                    break;
                }
            }
        }

        return area;
    }

    public void MaterialNormal() 
    {
        foreach (var casilla in area_seleccionada)
        {
            casilla.render.material = casilla.material;
        }
        area_seleccionada = new();
    }

    public void CambiarDefensa(int i, Defensa[] opciones) 
    {
        //Selecciona la defensa
        defensas[i] = opciones[Random.Range(0, opciones.Length)];
        defensas[i].daño = Random.Range(1, 7);

        //Cambia la carta
        tarjetas[i].area.sprite = defensas[i].carta;
        tarjetas[i].daño.text = defensas[i].daño.ToString();
    }
}

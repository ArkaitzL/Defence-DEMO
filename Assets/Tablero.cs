using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using BaboOnLite;
using TMPro;

public class Tablero : MonoBehaviour
{
    [SerializeField] private Vector3[] pos_incio = new Vector3[5];
    [SerializeField] private int fila_final;

    private Transform tablero;

    private void Awake()
    {
        Controlador.TurnoEvent += Turno;
        tablero = transform;
    }

    private void Turno() 
    {
        Nivel nivel = Almacen.get.niveles[Controlador.get.nivel_actual];

        //CONTROLA QUE QUEDEN TURNOS
        if (Controlador.get.turno < nivel.cant_turnos) 
        {
            //SPAWNEAR NUEVA FILA

            //Selecciona las podsiciones de spawneo
            int cant = UnityEngine.Random.Range(nivel.spawn_fila_min, nivel.spawn_fila_max + 1);
            List<Vector3> posiciones = new List<Vector3>(pos_incio)
                .OrderBy(x => Guid.NewGuid())
                .Take(cant)
                .ToList();

            //Selecciona los enemigos
            List<string> enemigos = Posibilidades(nivel, cant);

            if (posiciones.Count != cant && enemigos.Count != cant)
            {
                Debug.LogWarning("No se han generado bien los enemigos");
                return;
            }

            //Spawnea los enemigos
            for (int i = 0; i < cant; i++)
            {
                Enemigo enemigo = Almacen.get.enemigos.Filter((e) => e.nombre.ToLower().Trim() == enemigos[i].ToLower().Trim())[0];
                int vida = 
                    UnityEngine.Random.Range(enemigo.vida_min, enemigo.vida_max + 1)
                    *
                    ((nivel.multiplicador_vida / 100) + 1);

                Transform skin = Instantiate(enemigo.skin, posiciones[i], Quaternion.identity, tablero).transform;
                TextMeshProUGUI texto = skin.GetComponentsInChildren<TextMeshProUGUI>()[0];
                texto.text = vida.ToString();

                Controlador.get.fichas.Add(
                    new Ficha(
                        vida,
                        posiciones[i],
                        skin,
                        skin.GetComponent<Animator>(),
                        texto,
                        skin.GetComponent<Correr>()
                    )
                );
            }
        }

        //MOVER TODOS
        foreach (var ficha in Controlador.get.fichas)
        {
            ficha.Mover(Controlador.get.duracion_turno);
        }
    }
    //METODOS

    private List<string> Posibilidades(Nivel nivel, int cant) 
    {
        List<string> enemigos = new();
        int posibilidades = 0;

        foreach (var item in nivel.spawneo)
        {
            posibilidades += item.posibilidades;
        }

        for (int i = 0; i < cant; i++)
        {
            int num = 0;

            foreach (var item in nivel.spawneo)
            {
                num += item.posibilidades;
                if (num >= UnityEngine.Random.Range(0, posibilidades + 1))
                {
                    enemigos.Add(
                        item.nombre_enemigo
                    );
                    break;
                }
            }
        }

        return enemigos;
    }
}



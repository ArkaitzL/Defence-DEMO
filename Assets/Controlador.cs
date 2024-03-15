using BaboOnLite;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    [SerializeField] public float duracion_turno = 1;
    [SerializeField] public float pos_final = 7;
    [SerializeField] private TextMeshProUGUI mensajes_txt;
    [SerializeField] public float duracion_mensaje = 7;
    [SerializeField] private GameObject particulas_golpe;

    [HideInInspector] public int turno = 0;
    [HideInInspector] public int nivel_actual = 0;

    public List<Ficha> fichas = new();

    public static event Action TurnoEvent;
    public static Controlador get;

    void Start()
    {
        get = this;
        ProximoDia(true);
    }

    public void Turno(bool directo = false)
    {
        if (!directo)
        {
            if (!ControladorBG.Esperando("Turno")) return;
            ControladorBG.IniciarEspera("Turno", duracion_turno);
        }

        Nivel nivel = Almacen.get.niveles[nivel_actual];

        TurnoEvent?.Invoke();

        turno++;

        //TERMINAR TURNO
        if (turno > nivel.cant_turnos)
        {
            if (fichas.Count == 0)
            {
                ProximoDia();
            }
            return;
        }
    }

    public void ProximoDia(bool dia_uno = false) 
    {
        if(!dia_uno) nivel_actual++;
        turno = 0;
        Mensaje($"DIA {nivel_actual+1}");

        if (nivel_actual == Almacen.get.niveles.Length)
        {
            Mensaje($"VICTORIA");
            //MENU VICTORIA [***************]
            return;
        }

        Turno(true);
    }

    public void Derrota() 
    {
        Mensaje($"DERROTA");

        //MENU MUERTE [***************]
    }

    public void Mensaje(string mensaje) 
    {
        mensajes_txt.text = mensaje;
        ControladorBG.Rutina(duracion_mensaje, () => 
        {
            mensajes_txt.text = "";
        });
    }

    public void Muerte(Ficha ficha)
    {
        fichas.Remove(ficha);
        ficha.texto.color = Color.red;
        ficha.anim.SetTrigger("Muerto");

        ControladorBG.Rutina(1, () => {
            GameObject particulas = Instantiate(particulas_golpe, ficha.objeto.position.Y(.25f), Quaternion.identity, transform);
            Destroy(ficha.objeto.gameObject);
            Destroy(particulas, 4);
        });
    }
}

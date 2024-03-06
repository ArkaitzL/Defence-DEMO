using BaboOnLite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    [SerializeField] public float duracion_turno = 1;

    public List<Ficha> fichas = new();
    int turno;

    public static event Action TurnoEvent;
    public static Controlador get;

    void Start()
    {
        get = this;
        Turno();
    }

    public void Turno()
    {
        if (!ControladorBG.Esperando("Turno")) return;
        ControladorBG.IniciarEspera("Turno", duracion_turno);

        Nivel nivel = Almacen.get.niveles[Save.Data.nivel_actual];

        TurnoEvent?.Invoke();

        //TERMINAR TURNO
        if (turno == nivel.cant_turnos)
        {
            //          [Comprueba si la columna tiene una defensa]
            //          [Fin nivel]
        };

        turno++;
    }

    public void Muerte(Ficha ficha) 
    {
        fichas.Remove(ficha);
        ficha.texto.color = Color.red;
        ficha.anim.SetTrigger("Muerto");

        ControladorBG.Rutina(1, () => {
            //      [Particulas-Muerte]
            Destroy(ficha.objeto.gameObject);
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaboOnLite;
using TMPro;

public class Almacen : MonoBehaviour
{
    [SerializeField] public Enemigo[] enemigos;
    [SerializeField] public Defensa[] defensas;
    [SerializeField] public Nivel[] niveles;

    public static Almacen get;

    private void Awake()
    {
        get = this;
    }
}

[Serializable]
public class Defensa
{
    public Area area;
    public bool especial;
    [HideInInspector] public int da�o;

    [Serializable]
    public class Area {
        public int maxX, minX, maxY, minY;
        public Vector2[] otros;
    }
}

[Serializable]
public class Enemigo
{
    public string nombre;
    public int vida_max, vida_min;
    public GameObject skin;
    public Habilidad habilidad;
}

[Serializable]
public class Nivel
{
    public Spawn[] spawneo;
    public int cant_turnos;
    public int multiplicador_vida;
    [Range(1, 6)] public int spawn_fila_min, spawn_fila_max;

    [Serializable]
    public class Spawn {
        public string nombre_enemigo;
        public int posibilidades;
    }
}

public class Ficha
{
    public Ficha(int vida, Vector3 posicion, Transform objeto, Animator anim, TextMeshProUGUI texto)
    {
        this.vida = vida;
        this.posicion = posicion;
        this.objeto = objeto;
        this.anim = anim;
        this.texto = texto;
    }

    public int vida;
    public Vector3 posicion;
    public Transform objeto;
    public Animator anim;
    public TextMeshProUGUI texto;

    public bool Comparar(Vector3 nueva_posicion) => posicion.x == nueva_posicion.x && posicion.z == nueva_posicion.z;

    public void Da�o(int da�o) 
    {
        vida -= da�o;
        texto.text = (vida >= 0) 
            ? vida.ToString()
            : "0";

        //MUERTE
        if (vida <= 0)
        {
            Controlador.get.Muerte(this);
            return;
        }

        //DA�O
        anim.SetTrigger("Golpe");
    }

    public void Mover(float duracion) 
    {
        anim.SetBool("Moviendo", true);

        posicion = posicion.Z(+1);

        ControladorBG.Mover(
            objeto,
            new(duracion, posicion)
        );
        ControladorBG.Rutina(duracion, () => {
            anim.SetBool("Moviendo", false);
        });
    }
}

public class Area
{
    public Area(Renderer render, Material material, Vector3 posicion)
    {
        this.render = render;
        this.material = material;
        this.posicion = posicion;
    }

    public Renderer render;
    public Material material;
    public Vector3 posicion;
}
public class Casilla
{
    public Casilla(Renderer render, Vector3 posicion)
    {
        this.render = render;
        this.posicion = posicion;
    }

    public Renderer render;
    public Vector3 posicion;

    public bool Comparar(Vector3 nueva_posicion) => posicion.x == nueva_posicion.x && posicion.z == nueva_posicion.z;
}

public enum Habilidad
{

}
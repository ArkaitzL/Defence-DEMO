using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Correr : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [HideInInspector] public bool corriendo = false;

    private void Update()
    {
        if (!corriendo) return;

        transform.Translate(
            Vector3.forward * velocidad * Time.deltaTime
        );
    }
}

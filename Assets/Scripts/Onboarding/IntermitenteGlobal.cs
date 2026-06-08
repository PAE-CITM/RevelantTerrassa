using System.Collections;
using UnityEngine;

public class IntermitenteAnillo : MonoBehaviour
{
    private ParticleSystem ps;

    [Header("Tiempos del Intermitente")]
    [Tooltip("Cuánto tiempo se queda el anillo visible en pantalla")]
    public float tiempoEncendido = 0.5f;

    [Tooltip("Cuánto tiempo se queda apagado antes de volver a salir")]
    public float tiempoApagado = 0.5f;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = false;

        StartCoroutine(BucleAnillo());
    }

    IEnumerator BucleAnillo()
    {
        while (true)
        {
            ps.Play();
            
            yield return new WaitForSeconds(tiempoEncendido);

            yield return new WaitForSeconds(tiempoApagado);
        }
    }
}
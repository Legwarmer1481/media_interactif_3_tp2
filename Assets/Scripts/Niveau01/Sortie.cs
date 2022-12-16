using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sortie : MonoBehaviour
{
    private GestionScenes gestionScenes;
    // Start is called before the first frame update
    void Start()
    {
        gestionScenes = GameObject.Find("GestionScenes").GetComponent<GestionScenes>();
    }

    void OnTriggerEnter(Collider other){

        // Si le joueur se met devant la porte de la maison,
        // félicitation tu a fini le jeu et on passe à la scène suivante
        if(other.transform.tag == "Player"){
            
            gestionScenes.SceneSuivante();
        }
    }
}

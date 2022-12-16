using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Barriere : MonoBehaviour
{
    [SerializeField] GestionNiveau gestionNiveau;
    [SerializeField] AudioClip dialogueBloque;
    private AudioSource sourceAudio;
    private bool enTrainDeParler = false;
    

    void Start(){
        sourceAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Surveille si la première mission est réussi.
        // Si c'est la cas, on détruit la barrière vers le village
        if(gestionNiveau.objectif01 == gestionNiveau.progresObjectif01){
            Destroy(gameObject);
        }
        
    }

    void OnTriggerEnter(Collider other){

        // Si le joueur est en collision avec le mur invisible
        // le personnage dit quelque chose, mais rejouer l'audio,
        // si le personnage n'a pas fini de parler
        if(other.transform.tag == "Player" && !enTrainDeParler){
            enTrainDeParler = true;
            sourceAudio.PlayOneShot(dialogueBloque);
            Invoke("DialogueFini", 3.773f);
        }
    }

    void DialogueFini(){
        enTrainDeParler = false;
    }
}

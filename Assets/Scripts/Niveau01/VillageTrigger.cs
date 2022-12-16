using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VillageTrigger : MonoBehaviour
{
    public bool inRange = false;
    private bool villageVide = false;
    [SerializeField] Perso joueurInfo;

    [Space(10)]
    private AudioSource sourceAudio;
    [SerializeField] AudioClip foulePanique;

    void OnTriggerEnter(Collider qui){

        if(qui.transform.tag == "Player"){
            inRange = true;
        }
    }
    void OnTriggerExit(Collider qui){

        if(qui.transform.tag == "Player"){
            inRange = false;
        }
    }

    void Start(){

        sourceAudio = GetComponent<AudioSource>();
    }

    void Update(){

        QueLeCarnageCommence();
    }

    void QueLeCarnageCommence(){

        // Quand le joueur arrive dans le village avec une arme dans la main
        // on joue le son de la foule en panique
        if(inRange && joueurInfo.avecArme && !villageVide){
            
            villageVide = true;
            sourceAudio.Stop();
            sourceAudio.volume = 1;
            sourceAudio.PlayOneShot(foulePanique);

        }
    }
}

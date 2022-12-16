using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Victimes : MonoBehaviour
{

    private Animator robotAnim;
    private AudioSource sourceAudio;
    private NavMeshAgent agent;

    public bool mort = false;
    public bool peur = false;
    public bool panique = false;
    [SerializeField] public AudioClip criDeMort;
    private Perso joueur;
    private VillageTrigger village;
    public GameObject cachette;

    void Start()
    {
        robotAnim = GetComponent<Animator>();
        sourceAudio = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        
        NiveauGestionnaire contenu = GameObject.Find("GestionNiveau").GetComponent<NiveauGestionnaire>();
        village = contenu.villageProximite;
        joueur = contenu.cible.GetComponent<Perso>();

        int cachetteChoisi = Random.Range(0, contenu.cachettes.Length);
        cachette = contenu.cachettes[cachetteChoisi];
    }

    void Update()
    {
            Fuir();
    }

    // Appellé par le script « Tirer », on fait arrêter de bouger pour montrer qu'elle est morte
    public void Meurt(){
        
            agent.SetDestination(transform.position);
            CancelInvoke();
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;
    }

    // La victime commence à courir et entrer en mode panique si le joueur arrive au village avec une arme dans la main
    private void Fuir(){
        
        if(village.inRange && !panique && joueur.avecArme){
            panique = true;
            robotAnim.SetBool("Peur", true);
            Paniquer();
        }
    }

    // Dans la zone désignée par le gestionnaire de niveau, la victime cours partout dans la zone
    // en changeant de destination au NavMeshAgent à tous les 5 secondes
    private void Paniquer(){
        
        if(!mort){

            float positionX = Random.Range(cachette.transform.position.x -30, cachette.transform.position.x + 30);
            float positionZ = Random.Range(cachette.transform.position.z -30, cachette.transform.position.z + 30);
            Vector3 nouvelPosition = new Vector3(positionX, cachette.transform.position.y, positionZ);

            agent.SetDestination(nouvelPosition);
            Invoke("Paniquer", 5);
        }
    }

}

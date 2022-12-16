using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ennemi : MonoBehaviour
{

    [Header("État")]
    private bool inRange = false;
    private bool aggressif = false;
    private bool peur = false;
    public bool mort = false;
    private bool sePromene = false;
    private bool discute = false;
    [SerializeField] int tempsAlternance = 10;

    private Animator rAnim;
    private NavMeshAgent agent;

    [Header("Composants")]
    [SerializeField] private Perso persoInfo;
    [SerializeField] private GestionNiveau gestionNiveau;
    [SerializeField] private GameObject cible;
    [SerializeField] private GameObject spot;

    [Header("Doublages")]
    [SerializeField] private AudioClip paroleAmical;
    [SerializeField] private AudioClip paroleAggressif;
    [SerializeField] private AudioClip parolePeur;
    private AudioSource dialogue;


    void Start()
    {
        rAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        dialogue = GetComponent<AudioSource>();
        
        // 
        if(cible == null){
            NiveauGestionnaire contenu = GameObject.Find("GestionNiveau").GetComponent<NiveauGestionnaire>();

            cible = contenu.cible;
            persoInfo = cible.GetComponent<Perso>();
        }
    }

    void Update()
    {
        Peur();
        Aggression();
        Deplacement();
        Camaderie();
    }

    void OnCollisionEnter(Collision other){

        // Quand le robot attaque le joueur et il le touche,
        // la partie est terminée.
        if(aggressif && other.transform.tag == "Player" && !mort){
            GameObject.Find("GestionHUD").GetComponent<GestionHUD>().GameOver();
        }
        
    }

    void OnTriggerEnter(Collider other){

        // Quand le joueur entre dans la sphère de proximité,
        // le robot garde en mémoire que le joueur est à proximité
        // pour parler. Il salut le joueur.
        if(other.transform.tag == "Player"){

            inRange = true;

            if(!persoInfo.avecArme && !aggressif){
                rAnim.SetTrigger("Rencontre");
            }
        }
    }

    void OnTriggerExit(Collider other){

        // Quand le joueur sort de la sphère de proximité,
        // le robot sait maintenant que le joueur n'est plus assez proche
        // pour interagir avec le joueur. Toutefois, il fait un signe "Au revoir!"
        if(other.transform.tag == "Player"){

            inRange = false;
            discute = false;
            if(!persoInfo.avecArme && !aggressif){
                rAnim.SetTrigger("Rencontre");
            }
        }

    }

    void Aggression(){
        
        // Les robots deviennent aggressifs lorsque
        // le joueur tue un de ses congénaires.
        // C'est de la défense légitime!
        if(gestionNiveau.progresObjectif01 > 0 && !aggressif){
            
            aggressif = true;
            dialogue.clip = paroleAggressif;
            dialogue.Play();
            dialogue.maxDistance = 50;
        }

    }

    void Camaderie(){

        // Si tu n'as pas encore commi de meurtre, tu n'arrives pas
        // les mains armées et que tu es proche du robot,
        // il va te parler amicalement.
        if(inRange && !discute && !persoInfo.avecArme && !aggressif && !mort && !peur){

            dialogue.clip = paroleAmical;
            dialogue.Play();
            discute = true;            
        }
    }

    void Peur(){

        // Si tu es proche, le robot lève ses main pour montrer qu'il a
        // peur de toi. Mais, si tu ranges ton arme, il se sentira soulagé.
        if(inRange && persoInfo.avecArme && !aggressif && !mort){
            
            rAnim.SetBool("Peur", true);

            if(!peur){ // Pour éviter le bug qui fait recommencer l'audio à toutes le millisecondes
                
                discute = false;
                peur = true;
                dialogue.clip = parolePeur;
                dialogue.Play();
            }
        }else{
            
            rAnim.SetBool("Peur", false);
            peur = false;
        }
    }

    void Deplacement(){
        if(!mort){
            
            // Si le robot n'est pas mort, il pourrait:
            // 1. attaquer le joueur;
            // 2. se promener;
            // 3. ou regarder le joueur.
            if(aggressif){
                
                // Si le robot est en mode aggressif, il fonce droit sur le joueur
                agent.SetDestination(cible.transform.position);
            }else if(!inRange){
                
                // Si le robot n'est pas en mode aggressif et que le joueur n'est pas
                // proche, il se promène tranquillement dans sa zone
                if(!sePromene){
                    Invoke("PromenadeAleatoire", tempsAlternance);
                    sePromene = true;
                }
            }else{
                
                // Si le robot n'est pas en mode aggressif
                // mais que le joueur est proche,
                // il se tourne vers le joueur pour parler
                CancelInvoke();
                transform.LookAt(cible.transform);
                agent.SetDestination(transform.position);
                sePromene = false;
            }

        }else{ 
            
            // Quand le robot meurt, il arrête de bouger. Il joue l'animation qu'il tombe au sol
            // puis, il ne dit plus rien
            agent.SetDestination(transform.position);
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;
            dialogue.Stop();
        }
    }

    void PromenadeAleatoire(){

        // Pour les robots déjà placé sur la carte.
        // Ils ont un périmètre désigné et il se promène à des points aléatoires dans leur périmètre.
        if(spot != null){

            float positionX = Random.Range(spot.transform.position.x - 10, spot.transform.position.x + 10);
            float positionZ = Random.Range(spot.transform.position.z - 10, spot.transform.position.z + 10);

            Vector3 nouvelPosition = new Vector3(positionX, spot.transform.position.y, positionZ);
            agent.SetDestination(nouvelPosition);
            sePromene = false;
        }
    }
}

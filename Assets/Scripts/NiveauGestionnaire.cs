using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiveauGestionnaire : MonoBehaviour
{
    private bool vague01 = false;
    private bool vague02 = false;
    private bool arriveAuVillage = false;
    private bool accompli = false;

    private AudioSource sourceAudio;
    
    [SerializeField] GestionNiveau gestionNiveau;
    [SerializeField] int objectif01 = 10;
    [SerializeField] int objectif02 = 10;
    [SerializeField] GameObject sortie;
    
    [Header("Dialogue Personnage")]
    [SerializeField] private AudioClip paroleDepart;
    [SerializeField] private AudioClip paroleMiMission;
    [SerializeField] private AudioClip paroleAuVillage;
    [SerializeField] private AudioClip paroleFin;

    [Header("Pour les ennemi spawn")]
    public GameObject cible;
    public VillageTrigger villageProximite;
    [Space(10)]
    [SerializeField] GameObject[] npc;
    [SerializeField] GameObject[] renforts;
    [SerializeField] GameObject[] village;
    public GameObject[] cachettes;


    void Start()
    {
        // Réinitialiser objectifs
        gestionNiveau.objectif01 = objectif01;
        gestionNiveau.objectif02 = objectif02;
        gestionNiveau.progresObjectif01 = 0;
        gestionNiveau.progresObjectif02 = 0;

        // Collecter les composants
        sourceAudio = GetComponent<AudioSource>();

        // Démarrer les fonctions de départ, mais avec un délais
        Invoke("MonologueDebut", 2);

        // Activer le CursorLock pour ne plus voir la souri sur l'écran
        cible.GetComponent<PourInputs>().SetCursorState(true);
        
    }

    void Update()
    {

        // Lorsque les ennemi passe en mode aggressive,
        // les renforts arrivent!!
        RobotSeDefend();
        
        // Une fois que l'objectif de la première mission est atteinte,
        // ont fait apparaître le reste au village
        BienvenueAuVillage();

        // Le personnage confirme l'objectif quand il arrive au village
        AuVillage();

        // Une fois que la deuxième mission est atteinte,
        // on active le point de téléportation pour retourner à la base spaciale
        FinMission();
    }

    void RobotSeDefend(){
        
        if(gestionNiveau.progresObjectif01 > 0 && !vague01){ // Pour que les renforts agit seulement une fois dans le jeu
            
            vague01 = true;
            for(int i = 0; i < objectif01 - 5; i++){

                // Désigne une zone d'apparition aléatoirement.
                // Ensuite, il apparait à une position aléatoire dans la zone
                int zoneChoisi = Random.Range(0, renforts.Length);
                float positionX = Random.Range(renforts[zoneChoisi].transform.position.x - 10, renforts[zoneChoisi].transform.position.x + 10);
                float positionZ = Random.Range(renforts[zoneChoisi].transform.position.z - 10, renforts[zoneChoisi].transform.position.z + 10);
                Vector3 positionDepart = new Vector3(positionX, renforts[zoneChoisi].transform.position.y, positionZ);

                // On instancie les ennemis à travers la carte
                GameObject nouvelEnnemi = Instantiate(npc[0], positionDepart, Quaternion.identity);
                nouvelEnnemi.transform.parent = GameObject.Find("NPCs").transform;
                nouvelEnnemi.name = "Robot_M (" + (i + 6) + ")";
                
            }
        }
    }

    void BienvenueAuVillage(){
        
        // Dès que la première mission est réussi,
        // on fait apparaître le restes de robots dans le village
        if(gestionNiveau.objectif01 == gestionNiveau.progresObjectif01 && !vague02){
            
            vague02 = true;

            // Le joueur déclare sa prochaine objectif
            sourceAudio.PlayOneShot(paroleMiMission);

            // On commence par faire apparaitre les femmes qui ont le même nombre
            // que le nombre requis du premier objectif.
            
             for(int i = 0; i < objectif01; i++){
                int x = i >= village.Length ? i - village.Length : i ;

                float positionX = Random.Range(village[x].transform.position.x - 3, village[x].transform.position.x + 3);
                float positionZ = Random.Range(village[x].transform.position.z - 3, village[x].transform.position.z + 3);
                Vector3 positionDepart = new Vector3(positionX, village[x].transform.position.y, positionZ);

                // On instancie les femmes un peu partout dans le village
                GameObject nouvelVictime = Instantiate(npc[1], positionDepart, Quaternion.identity);
                nouvelVictime.transform.parent = GameObject.Find("Victimes/Femmes").transform;
                nouvelVictime.name = "Robot_F (" + (i + 1) + ")";
                nouvelVictime.transform.LookAt(village[x].transform);
            }

            // Ensuite, on faire apparaître les enfants dans le village.
            int nbEnfants = objectif02 - objectif01;

            for(int i = 0; i < nbEnfants; i++){
                
                int x = i; // L'index de la boucle est le même index pour les points d'apparitions dans le village
                while(x >= village.Length){
                    // mais, si l'index du boucle est trope grand par rapport au nombre de points d'apparitions disponible
                    // on soustrait jusqu'à ce que l'index soit correct.
                    x = x - village.Length;
                }

                float positionX = Random.Range(village[x].transform.position.x - 3, village[x].transform.position.x + 3);
                float positionZ = Random.Range(village[x].transform.position.z - 3, village[x].transform.position.z + 3);
                Vector3 positionDepart = new Vector3(positionX, village[x].transform.position.y, positionZ);

                // On instancie les enfants un peu partout dans le village
                GameObject nouvelVictime = Instantiate(npc[2], positionDepart, Quaternion.identity);
                nouvelVictime.transform.parent = GameObject.Find("Victimes/Enfants").transform;
                nouvelVictime.name = "Robot_E (" + (i + 1) + ")";
                nouvelVictime.transform.LookAt(village[x].transform);
            }
            
        }
    }
    
    void FinMission(){
        if(gestionNiveau.objectif02 == gestionNiveau.progresObjectif02 && !accompli){
            
            accompli = true;
            // Le joueur explique ce qui reste à faire pour changer de niveau
            sourceAudio.PlayOneShot(paroleFin);
            sortie.SetActive(true);
        }
    }

    void MonologueDebut(){

        // Le jeu commence, le personnage nous donne un indice de la première mission
        sourceAudio.PlayOneShot(paroleDepart);
    }

    void AuVillage(){
        
        if(villageProximite.inRange && !arriveAuVillage){

            // Quand le personnage est dans le village, il confirme sa deuxième mission
            arriveAuVillage = true;
            sourceAudio.PlayOneShot(paroleAuVillage);
        }
    }

}

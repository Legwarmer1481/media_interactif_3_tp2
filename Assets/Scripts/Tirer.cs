using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tirer : MonoBehaviour
{

    private PourInputs lesInputs;
    private Perso lePerso;
    private AudioSource sourceAudio;
    [SerializeField] GameObject mire;
    [SerializeField] GameObject regard;
    [SerializeField] GestionNiveau gestionNiveau;
    [SerializeField] GestionHUD gestionHUD;
    [SerializeField] AudioClip gunShot;

    void Start()
    {
        lesInputs = GetComponent<PourInputs>();
        lePerso = GetComponent<Perso>();
        sourceAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        Attaque();
    }

    void Attaque(){
        
        // Quand la caméra est à la première personne et qu'il est équipé avec l'arme
        // La mire se révèle, sinon on ne la voit plus
        if(lePerso.avecArme && lePerso.perspectivePP){
            mire.SetActive(true);
        }else{
            mire.SetActive(false);
        }
        
        if (lesInputs.tire && lePerso.avecArme) // Quand le joueur tire avec l'arme
        {

            // Si en 3e personne, on vise par la tête du personnage,
            // sinon on vise par la mire
            Vector3 positionDepart = lePerso.perspectivePP ? mire.transform.position : regard.transform.position ;

            RaycastHit hit;
            GameObject objetCollision;

            // Debug.DrawRay(positionDepart, transform.forward*1000, Color.red, 3);

            sourceAudio.PlayOneShot(gunShot); // On peut le son du coup de feu pour confirmer qu'on a tiré

            if(Physics.Raycast(positionDepart, mire.transform.TransformDirection(Vector3.forward), out hit, 25, -12, QueryTriggerInteraction.Ignore)){
                
                objetCollision = hit.collider.transform.gameObject;

                // Si on a touché un robot, il va jouer l'animation qu'il tombe et meurt
                // on ajoute un point de progression
                // et on confirme que le robot est mort
                if(objetCollision.tag == "Ennemi"){
                    Animator robotAnim = objetCollision.GetComponent<Animator>();
                    Ennemi robotInfo = objetCollision.GetComponent<Ennemi>();

                    if(!robotInfo.mort){
                        gestionNiveau.progresObjectif01++;
                        robotInfo.mort = true;
                        robotAnim.SetBool("Mort", true);
                    }
                    
                }
                if(objetCollision.tag == "Victime"){
                    Animator robotAnim = objetCollision.GetComponent<Animator>();
                    Victimes robotInfo = objetCollision.GetComponent<Victimes>();
                    AudioClip robotCri = robotInfo.criDeMort;
                    AudioSource robotAudio = objetCollision.GetComponent<AudioSource>();

                    if(!robotInfo.mort){
                        gestionNiveau.progresObjectif02++;
                        robotInfo.mort = true;
                        robotAnim.SetBool("Mort", true);
                        robotAudio.PlayOneShot(robotCri);
                        robotInfo.Meurt();
                    }
                    
                }
            }
        }
        
        lesInputs.tire = false; // afin de pouvoir tirer à nouveau, on le mets à false
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GestionMusique : MonoBehaviour
{

    // Pour avoir une bon flexibilité avec les msuiques,
    // j'ai créé un Scriptable Object pour que je range les clip de la même musique ensemble
    // MusiqueData est le type de ScriptableObject
    // Il contient chacun 3 tableaux pour 3 sections de la musique
    // 1. L'intro avec les calques approprié
    // 2. La "loop" les calques qui se répètent à l'infini
    // 3. La "loop" finale si j'ai une morceaux qui a une fin
    // Si j'ai une loop finale, il y a un int qui représent la position de la musique qui ne doit pas dépacer pour faire une fin consistante
    // si elle est dépassé, il finit le boucle puis on va pouvoir changer la musique pour celle qui a une fin
    [SerializeField] AudioMixer musiqueMixer;
    [SerializeField] MusiqueData snowInSummer; // La première musique
    [SerializeField] MusiqueData mourning; // La deuxième musique
    [SerializeField] GestionNiveau gestionNiveau; // Pour traquer la progression du jeu qui seront utiliser comme déclencheur
    [SerializeField] Perso joueur; // Pour la première musique qui sera déclencher quand le joueur brandi le fusil

    private AudioSource[] musiques; // Les fentes pours les calques de musique
    private int phases = 0; // Afin d'éviter des bugs, je mets des étapes qui monte quand certaines musiques est déclenchée
    private MusiqueData musiqueEnLecture; // Pour certaines fonctions qui ont besoin de savoir quelle musique est entrain de jouer
    private bool dansIntro = false; // Pour certaines fonctions qui besoin de savoir si l'intro est entrain de jouer ou non

    // Start is called before the first frame update
    void Start()
    {

        musiques = GetComponents<AudioSource>();
        
    }

    void Update()
    {

        // Les différents phases pour synchroniser la musique avec ce qui se passe en ce moment dans le jeu
        if(joueur.avecArme && phases == 0){ // La musique commence quand on brandit l'arme à feu

            phases++;
            DebuterMusique(snowInSummer, 1, 0, 0);
        }
        if(gestionNiveau.progresObjectif01 == 1 && phases == 1){ 

            // Pour faire anticipé quelle que chose de gros, on change met la voix de la chanson
            // Après avoir éliminer la première victime
            phases++;
            ChangerCalque(0, 0, 1);
        }
        if(gestionNiveau.progresObjectif01 > 1 && phases == 2){

            // Combat épique contre les robots bleus, on joue l'orchestre avec la chorale.
            phases++;
            ChangerCalque(0, 1, 1);
        }
        if(gestionNiveau.progresObjectif01 == gestionNiveau.objectif01 && phases == 3){
            
            // L'objectif 01 atteinte, on arrête la musique complètement avec un fondu
            phases++;
            ChangerCalque(0, 0, 0);
        }

        if(gestionNiveau.progresObjectif02 > 0 && phases == 4){
            
            // La musique triste commence quand le joueur tue la première victime de la
            // deuxième mission
            phases++;
            DebuterMusique(mourning);
        }
        if(gestionNiveau.progresObjectif02 == gestionNiveau.objectif02 && phases == 5){
            
            // Quand le joueur a complété le deuxième objectif,
            // On lance la dernière boucle pour permettre la musique de finir
            // comme une musique normal. Pas par un simple fondu.
            JouerBoucleFinal();
        }
        
    }

    // Quand il n'y a pas de musique qui joue, on commence une nouvelle chanson.
    // Il a besoin obligatoirement le scriptableObject du type MusiqueData
    // Si besoin, on ajuste les volumes des groupes dans le AudioMixer
    void DebuterMusique(MusiqueData song, float volumeAmbiant = 1f, float volumeOrch = 1f, float volumeVocals = 1f){
        
        dansIntro = true;
        musiqueEnLecture = song;
        // On ajuste les volumes des calques dans le audioMixer
        ChangerCalque(volumeAmbiant, volumeOrch, volumeVocals, 0.01f);

        // Ensuite, on fait un boucle pour placer et jouer la musiques dans les trois AudioSources
        // S'il n'y a pas 3 calques fourni par la MusiqueData, on met le clip à null
        for(int i = 0; i < musiques.Length; i++){
            
            // La musique ne rempli pas toujours toujours tous les calques.
            // Alors, on s'arrange que le programme n'essaira pas de mettre
            // un AudioClip qui n'existe pas dans un calque AudioSource
            if(i < song.intro.Length){

                musiques[i].clip = song.loop[i]; // le clip qui sera en boucle est inséré dans le AudioSource
                musiques[i].loop = true; // On s'assure que le paramètre de boucle est activé parce qu'il y a une fonction qui la désactive
                musiques[i].PlayOneShot(song.intro[i]); // On joue la musique intro une fois
                musiques[i].PlayScheduled(AudioSettings.dspTime + song.intro[i].length); // Quand l'intro est fini, la partie boucle prend la relève
            }else{

                musiques[i].clip = null; // On vide les autres audioSource pour éviter d'avoir des sons indésirable
            }
        }

        // On invoque une fonction pour confirmer que l'intro est terminé. Elle est pratique pour la fonction « JouerBoucleFinal() »
        Invoke("IntroFini", song.intro[0].length);
    }

    // On joue avec les volumes des groupes dans le audioMixer permettant de dynamiser la musique
    // Si on les mets tout à off, on appelle une fonction pour complètement arrêter la musique
    // afin de peut-être manger moins de ressource de la machine.
    void ChangerCalque(float volumeAmbiant = 1f, float volumeOrch = 1f, float volumeVocals = 1f, float tempsTransition = 3f){
        
        StartCoroutine(FadeMixerGroup.StartFade(musiqueMixer, "AmbiantsVolume", tempsTransition, volumeAmbiant));
        StartCoroutine(FadeMixerGroup.StartFade(musiqueMixer, "OrchVolume", tempsTransition, volumeOrch));
        StartCoroutine(FadeMixerGroup.StartFade(musiqueMixer, "VocalsVolume", tempsTransition, volumeVocals));

        if(volumeAmbiant == 0 && volumeOrch == 0 && volumeVocals == 0){

            // Si on a mis les volumes de tous les groupes dans le audioMixer à zéro
            // On arrête la musique complètement après que l'effet de fondu soit fini
            Invoke("ArreterMusique", tempsTransition + 0.5f);
        }
    }

    // Lorsqu'elle est appelée, il arrête la musique et retire les clips complètement.
    void ArreterMusique(){

        musiqueEnLecture = null; 
        dansIntro = false;
        CancelInvoke("IntroFini");
        
        for(int i = 0; i < musiques.Length; i++){

            musiques[i].Stop();
            musiques[i].clip = null;
        }
    }

    void JouerBoucleFinal(){

        // Si la musique a une partie avec une fin particulière,
        // on appelle cette fonction pour changer le clip de la boucle à celle de
        // la « boucle finale » sans s'en rendre compte.
        // On a besoin d'abord de la position de la musique.
        // Dans le scriptable object, on peut spécifier la position maximale
        // avant de pouvoir changer à la dernière boucle et on s'assure que l'intro
        // a également fini
        int timePosition;
        int timePositionThreshold = musiqueEnLecture.lastLoopSamples;

        if(musiques[0].clip != musiqueEnLecture.last[0] && !dansIntro){

            timePosition = musiques[0].timeSamples;
            if(timePosition <= timePositionThreshold){

                for(int i = 0; i < musiques.Length; i++){
                        
                    if(i < musiqueEnLecture.last.Length){
                        
                        // Quand tout est beau, on change le clip pour la boucle finale
                        // et on la joue à la position qu'il était rendu
                        // et on désactive la boucle
                        musiques[i].clip = musiqueEnLecture.last[i];
                        musiques[i].timeSamples = timePosition;
                        musiques[i].loop = false;
                        musiques[i].Play();
                            
                    }else{

                        musiques[i].clip = null;
                    }
                }

                phases++;
            }
        }
    }

    void IntroFini(){

        dansIntro = false;
    }

}

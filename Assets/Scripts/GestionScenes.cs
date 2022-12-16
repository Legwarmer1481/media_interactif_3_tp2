using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionScenes : MonoBehaviour
{

    private int sceneIndex;

    void Start()
    {

        // On vient se procurer l'index de la scène actuelle
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(sceneIndex);
        
    }

    // La fonction permet d'aller à la scène suivante
    // Si nous sommes à la dernière scène, on retourne au début
    public void SceneSuivante(){
        
        if(sceneIndex < SceneManager.sceneCountInBuildSettings - 1){
            SceneManager.LoadScene(sceneIndex + 1);
        }
        else{
            SceneManager.LoadScene(0);
        }

    }

    // Cette fonction permet de revenir à la scène précédente
    // Elle n'est jamais utilisée, je penses.
    public void ScenePrecedente(){
        
        if(sceneIndex > 0){
            SceneManager.LoadScene(sceneIndex - 1);
        }

    }

    // Pour la scène du jeu principal, il redémarre la scène pour recommencer
    public void ReinitialiserScene(){

            SceneManager.LoadScene(sceneIndex);

    }

    public void SceneCredit(){
        SceneManager.LoadScene(3);
    }

    public void SceneAccueil(){
        SceneManager.LoadScene(0);
    }
}

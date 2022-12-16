using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GestionHUD : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject mission01;
    [SerializeField] GameObject mission02;

    [Header("Textes")]
    [SerializeField] TextMeshProUGUI objectifMission01;
    [SerializeField] TextMeshProUGUI progresMission01;
    [SerializeField] TextMeshProUGUI objectifMission02;
    [SerializeField] TextMeshProUGUI progresMission02;

    [Space(10)]
    [SerializeField] GestionNiveau gestionNiveau;

    public UnityEvent e_DisableGame;
    

    void Start()
    {

        // Je met à jour le nombre visé pour les deux objectifs
        // C'est une mesure afin de m'assurer que ça sera affiché correctement à l'écran
        objectifMission01.text = "/" + gestionNiveau.objectif01.ToString();
        objectifMission02.text = "/" + gestionNiveau.objectif02.ToString();
        
    }

    void Update()
    {
        MettreAJourScore();
        
        // Quand un des objetifs commence, on révèle le nombre à atteindre et la progrssion
        if(gestionNiveau.progresObjectif01 > 0){
            mission01.SetActive(true);
        }
        if(gestionNiveau.progresObjectif02 > 0){
            mission02.SetActive(true);
        }
    }

    // Affiche l'écran Game Over
    // La fonction est appelé lorsqu'un robot a touché le joueur
    public void GameOver(){
        gameOver.SetActive(true);
        e_DisableGame.Invoke();
    }

    // Le progres s'affiche régulièrement pour tracker notre progression de l'objectif
    public void MettreAJourScore(){
        progresMission01.text = gestionNiveau.progresObjectif01.ToString();
        progresMission02.text = gestionNiveau.progresObjectif02.ToString();
    }
}

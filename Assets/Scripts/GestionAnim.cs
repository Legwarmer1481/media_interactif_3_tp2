using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionAnim : MonoBehaviour
{

    
 
    public void EnvoiMessage(float valeur){

        transform.parent.SendMessage("AnimFinie", valeur);
    }
}

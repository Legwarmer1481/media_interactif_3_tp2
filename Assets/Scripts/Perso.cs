using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Perso : MonoBehaviour
{

     [Header("Joueur")]
    [SerializeField] private float vitesseMarche = 2.0f;
    [SerializeField] private float vitesseCourse = 6.0f;
    [SerializeField] private float accelerationDeceleration = 10.0f;
    [SerializeField] private float vitesseRotation = 1.0f;
    [SerializeField] private Animator persoAnim;
    [SerializeField] private GameObject arme;
  

    [Space(5)]
    [Header("Environnement")]
    [SerializeField] private float gravite = -15.0f;
    [SerializeField] private LayerMask layerSol;

    [Space(5)]
    [Header("Pour Saut")]
    [SerializeField] private float hauteurSaut = 1.2f;
    [SerializeField] private float delaiSaut = 0.1f;
    [SerializeField] private float delaiChute = 0.15f;
    [SerializeField] private bool joueurAuSol = true;
    [SerializeField] private float distaneceDuSol = -0.14f;
    [SerializeField] private float groundedRadius = 0.5f;

    private float sautTimeout;
    private float tombeTimeout;



    [Space(5)]
    [Header("Camera")]
    [SerializeField] private GameObject cinemachineCible;
    [SerializeField] private float maxMouvCamHaut = 90.0f;
    [SerializeField] private float maxMouvCamBas = -90.0f;

    [Space(5)]
    [SerializeField] private GameObject cinemachineTP;
    [SerializeField] private GameObject cameraTP;
    [SerializeField] private GameObject cinemachinePP;

    [SerializeField] private float ajusteTempsRotation = 0.1f;
    [SerializeField] private float ajusteVelociteRotation = 0.1f;

    private float cinemachineNiveau;
    private float laVitesse;
    private float laVitesseVerticale;
    private float laVitesseFinale = 53.0f;
    private float laVitesseRotation = 53.0f;

    private const float seuil = 0.01f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private PourInputs lesInputs;

    private bool debutAnim = false;
    private bool finAnim = false;
    public bool avecArme = false;
    public bool perspectivePP = false;


    //-----------------------------------------------------------------

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lesInputs = GetComponent<PourInputs>();
        playerInput = GetComponent<PlayerInput>();

        sautTimeout = delaiSaut;
        tombeTimeout = delaiChute;
    }

    //-----------------------------------------------------------------


    void Update()
    {
        if(perspectivePP == true){
            Bouge();
        }else{
            DirectionTP();
        }
        GraviteEtSaut();
        VerifieToucheAuSol();
        PartAnime();
        ToggleArme();
        ToggleCam();
    }


    private void LateUpdate()
    {
        if(perspectivePP == true){
            RotationCamera();
        }
    }

    private void DirectionTP(){

        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;
        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;

        float directionCote = lesInputs.bouge.x;
        float directionAvantArriere = lesInputs.bouge.y;

        Vector3 direction = new Vector3(directionCote, laVitesseVerticale, directionAvantArriere);

        // ---

        float vitessseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float ecartVitesse = 0.1f;

        if (vitessseHorizontale < vitesseVoulue - ecartVitesse || vitessseHorizontale > vitesseVoulue + ecartVitesse)
        {
            laVitesse = Mathf.Lerp(vitessseHorizontale, vitesseVoulue, Time.deltaTime * accelerationDeceleration);
            // laVitesse = Mathf.Round(laVitesse * 1000f) / 1000f;
            // Debug.Log(laVitesse);
        }
        else
        {
            laVitesse = vitesseVoulue;
        }

        // ---

        if(direction.magnitude >= 0.01f && lesInputs.bouge != Vector2.zero){

            float angleVoulue = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cinemachineTP.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVoulue, ref ajusteVelociteRotation, ajusteTempsRotation);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0,angleVoulue,0) * Vector3.forward;
            controller.Move(moveDir.normalized * (laVitesse * Time.deltaTime) + new Vector3(0, laVitesseVerticale, 0) * Time.deltaTime);
        }
        else{

            controller.Move(direction.normalized * (laVitesse * Time.deltaTime) + new Vector3(0, laVitesseVerticale, 0) * Time.deltaTime);
        }

        persoAnim.SetFloat("Vitesse", laVitesse);

    }


    //-----------------------------------------------------------------

    private void Bouge()
    {
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;


        float vitessseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float ecartVitesse = 0.1f;
        float inputMagnitude = 1f;


        if (vitessseHorizontale < vitesseVoulue - ecartVitesse || vitessseHorizontale > vitesseVoulue + ecartVitesse)
        {
            laVitesse = Mathf.Lerp(vitessseHorizontale, vitesseVoulue * inputMagnitude, Time.deltaTime * accelerationDeceleration);
            laVitesse = Mathf.Round(laVitesse * 1000f) / 1000f;
        }
        else
        {
            laVitesse = vitesseVoulue;
        }


        Vector3 inputDirection = new Vector3(lesInputs.bouge.x, 0.0f, lesInputs.bouge.y).normalized;
        if (lesInputs.bouge != Vector2.zero)
        {
            inputDirection = transform.right * lesInputs.bouge.x + transform.forward * lesInputs.bouge.y;
        }

        controller.Move(inputDirection.normalized * (laVitesse * Time.deltaTime) + new Vector3(0.0f, laVitesseVerticale, 0.0f) * Time.deltaTime);

    }



    private void VerifieToucheAuSol()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - distaneceDuSol, transform.position.z);
        joueurAuSol = Physics.CheckSphere(spherePosition, groundedRadius, layerSol, QueryTriggerInteraction.Ignore);
    }


    private void GraviteEtSaut()
    {

        if (joueurAuSol)
        {
            tombeTimeout = delaiChute;

            if (laVitesseVerticale < 0.0f)
            {
                laVitesseVerticale = -2f;
            }


            if (lesInputs.saute && sautTimeout <= 0.0f && finAnim == true)
            {
                laVitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * gravite);
            }

            if (sautTimeout >= 0.0f)
            {
                sautTimeout -= Time.deltaTime;
            }
        }
        else
        {
            sautTimeout = delaiSaut;

            if (tombeTimeout >= 0.0f)
            {
                tombeTimeout -= Time.deltaTime;
            }

            lesInputs.saute = false;

            debutAnim = false;
            finAnim = false;
        }


        if (laVitesseVerticale < laVitesseFinale)
        {
            laVitesseVerticale += gravite * Time.deltaTime;
        }
    }

    private void PartAnime(){

        if(lesInputs.saute && joueurAuSol && debutAnim == false){

            debutAnim = true;
            persoAnim.SetTrigger("Saut");

        }

        
    }

    public void AnimFinie(float valeur){
        finAnim = true;
    }

    private void ToggleArme(){
        
        if(lesInputs.equipe == true){
            avecArme = !avecArme;
            arme.SetActive(avecArme);
            persoAnim.SetBool("Arme", avecArme);
            lesInputs.equipe = false;
        }

    }

    private void ToggleCam(){
        if(lesInputs.camPP == true){
            perspectivePP = !perspectivePP;
            cinemachinePP.SetActive(perspectivePP);
            cameraTP.SetActive(!perspectivePP);
            if(perspectivePP){ persoAnim.SetFloat("Vitesse", 0); }
            lesInputs.camPP = false;
        }
    }

    private void RotationCamera()
    {
        if (lesInputs.regarde.sqrMagnitude >= seuil)
        {
            float deltaTimeMultiplier = 1;
            cinemachineNiveau += lesInputs.regarde.y * vitesseRotation * deltaTimeMultiplier;
            laVitesseRotation = lesInputs.regarde.x * vitesseRotation * deltaTimeMultiplier;
            cinemachineNiveau = GrandeurAngle(cinemachineNiveau, maxMouvCamBas, maxMouvCamHaut);
            cinemachineCible.transform.localRotation = Quaternion.Euler(cinemachineNiveau, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * laVitesseRotation);
        }

    }



    private static float GrandeurAngle(float angleGauche, float minGauche, float maxGauche)
    {
        if (angleGauche < -360f) angleGauche += 360f;
        if (angleGauche > 360f) angleGauche -= 360f;
        return Mathf.Clamp(angleGauche, minGauche, maxGauche);
    }

}

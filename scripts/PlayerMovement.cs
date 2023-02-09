using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public Transform viewPoint; //empty object to which the camera is attatched to, gotta drag the empty object on this field though
    private float mouseSens = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;
    private Vector3 moveDir,movement;
    public float moveSpeed = 5f,runSpeed = 9f;
    private float activeSpeed;
    public CharacterController charCon;
    private Camera cam;
    public float jumpForce = 5f, gravityMod = 2.5f;
    private bool isGrounded;
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public GameObject bulletImpact;
    //public float timeBetweenShots = .1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10f,/* heatPerShot = 1f,*/ coolRate = 4f, overHeatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated = false;
    public guns[] allGuns;
    private int selectedGun = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//to hide the cursor and always keep it in the view   
        cam = Camera.main; //attatches to the current active camera
        UIController.instance.weaponTempSlider.maxValue = maxHeat;
        SwitchGun();
        muzzleCounter = muzzleDisplayTime;
        PhotonView ph = GetComponent<PhotonView>();
       // Transform newTrans = spawnManager.instance.SpawnPointSelect();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;
        //above lines are implemented in player spawner script
    }

   
    void Update()
    {
        if (photonView.IsMine)
        {


            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSens;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);//to look right and left
                                                                                                                                                                       //because rotation in x happens on the y axis
            verticalRotStore -= mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f); //to clamp the vertical values at points

            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);//up and down looking
                                                                                                                                        //notice we didnt apply up and down directly on the player bcz we dont want the body to move
                                                                                                                                        //we subtracted instead of adding because adding was giving inverted camera
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));



            if (Input.GetKey(KeyCode.LeftShift)) //sprint code
            {
                activeSpeed = runSpeed;
            }
            else
            {
                activeSpeed = moveSpeed;
            }
            float yVel = movement.y;
            movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeSpeed; //to get local movement based on mouse horizontal rotation.
                                                                                                                   //.normalized makes it so we don't add speed when we go diagonally     
            movement.y = yVel;

            if (charCon.isGrounded)
            {
                movement.y = 0f; //this will stop from piling up the movement vel in y
            }


            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayer);


            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                movement.y = jumpForce * gravityMod;
            }
            movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
            charCon.Move(movement * Time.deltaTime);

            if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                if (muzzleCounter < 0f)
                {
                    allGuns[selectedGun].muzzleFlash.SetActive(false);
                    muzzleCounter = muzzleDisplayTime;
                }
            }




            if (!overHeated) //shooting code
            {

                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
                if (allGuns[selectedGun].isAutomatic)
                {
                    if (Input.GetMouseButton(0)) //to check if mouse button is being held
                    {
                        shotCounter -= Time.deltaTime;
                        if (shotCounter <= 0)
                        {
                            Shoot();
                        }
                    }
                }
                heatCounter -= coolRate * Time.deltaTime;
            }
            else
            {
                heatCounter -= overHeatCoolRate * Time.deltaTime;
                if (heatCounter <= 0)
                {
                    heatCounter = 0;
                    overHeated = false;
                    UIController.instance.overHeatedMessage.gameObject.SetActive(false);
                }
            }
            if (heatCounter >= maxHeat)
            {
                heatCounter = maxHeat;
                overHeated = true;
                UIController.instance.overHeatedMessage.gameObject.SetActive(true);
            }
            if (heatCounter < 0)
            {
                heatCounter = 0;
            }
            UIController.instance.weaponTempSlider.value = heatCounter;


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f) //gunScrollWheel
            {
                if (selectedGun >= allGuns.Length - 1)
                {
                    selectedGun = 0;
                }
                else
                {
                    selectedGun++;
                }
                SwitchGun();
            }

            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (selectedGun <= 0)
                {
                    selectedGun = allGuns.Length - 1;
                }
                else
                {
                    selectedGun--;
                }
                Debug.Log(selectedGun);
                SwitchGun();
            }
        }

        
    }

    public void SwitchGun()
    {
        foreach(guns gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(false);
        allGuns[selectedGun].gameObject.SetActive(true);
    }



    
    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f ));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("We Hit" + hit.collider.gameObject.name);
            GameObject bulletImpactObject= Instantiate(bulletImpact, hit.point + hit.normal*0.002f, Quaternion.LookRotation(hit.normal, Vector3.up)); 
            //instantiate to create a new instance of an object.
            //instantiate to create a new instance of an object.(ussually prefabs are added to the public variable).
            Destroy(bulletImpactObject, 10f); //destroys after a 10 sec delay

        }
        shotCounter = allGuns[selectedGun].TimeBetweenShot;
        heatCounter += allGuns[selectedGun].heatPerShot;
        
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
        
       
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public OVRPlugin.Controller currControl = OVRPlugin.Controller.None; //variable to keep tracking of current input mechanism
    public GameObject controllerModelLeft;
    public GameObject controllerModelRight;
    public GameObject handModelLeft;
    public GameObject handModelRight;
    public GameObject leftHandIKTarget;
    public GameObject rightHandIKTarget;
    public GameObject mirrorLeftIKTarget;
    public GameObject mirrorRightIKTarget;
    public GameObject settingUICanvas;
    public GameObject debugMirror;
    public Button calibrateBtn;
    public Button mirrorNone;
    public Button mirrorLeft;
    public Button mirrorRight;
    public Button startBtn;
    public Button stopBtn;
    public Button maleBtn;
    public Button femaleBtn;
    public SphereSpawner sphereSpawner;
    public SaveData saveDataScript;

    public AudioSource touchSound;
    public TextMeshProUGUI debugText;

    private Quaternion leftHandRot = Quaternion.Euler(-90, 0, -90);
    private Quaternion rightHandRot = Quaternion.Euler(90, 0, 90);
    private Quaternion leftControllerRot = Quaternion.Euler(0, 90, 90);
    private Quaternion rightControllerRot = Quaternion.Euler(0, -90, -90);

    public static InputManager Instance; //singleton variable

    private void Awake()
    {
        if (Instance == null) Instance = this; //store singleton
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null; //delete singelton
    }

    // Start is called before the first frame update
    void Start()
    {
        calibrateBtn.onClick.AddListener(VRRig.Instance.Calibration);
        mirrorNone.onClick.AddListener(()=>VRRig.Instance.changeMirror(VRRig.MirrorMode.None));
        mirrorLeft.onClick.AddListener(() => VRRig.Instance.changeMirror(VRRig.MirrorMode.MirrorLeft));
        mirrorRight.onClick.AddListener(() => VRRig.Instance.changeMirror(VRRig.MirrorMode.MirrorRight));
        startBtn.onClick.AddListener(StartBtnClickHandler);
        maleBtn.onClick.AddListener(() => VRRig.Instance.LoadAvatarBones(VRRig.AvatarGender.Male));
        femaleBtn.onClick.AddListener(() => VRRig.Instance.LoadAvatarBones(VRRig.AvatarGender.Female));
        stopBtn.onClick.AddListener(StopRecording);
    }

    public void HideControlelr()
    {
        controllerModelLeft.SetActive(false);
        controllerModelRight.SetActive(false);
    }

    public void StartBtnClickHandler()
    {
        saveDataScript.StartRecording();
        sphereSpawner.SpawnSphere();
    }

    public void StopRecording()
    {
        saveDataScript.StopRecording();
    }

    private void DoCalibration()
    {
        VRRig.Instance.Calibration();
        //handModelLeft.gameObject.layer = LayerMask.NameToLayer("HideFromCam");
        //handModelRight.gameObject.layer = LayerMask.NameToLayer("HideFromCam");
    }

    // Update is called once per frame
    void Update()
    {
        OVRPlugin.Controller control = OVRPlugin.GetActiveController(); //get current controller scheme
        debugText.text = control.ToString();

        //testing recording
        if (Input.GetKeyDown(KeyCode.S))
        {
            //stop recording
            InputManager.Instance.StopRecording();
        }


        if (currControl != control)
        { //if current controller is different from previous
            //controller to hands
            if(control == OVRPlugin.Controller.Hands || control == OVRPlugin.Controller.LHand || control == OVRPlugin.Controller.RHand || control == OVRPlugin.Controller.None)
            {
                //disable box collider on the avatar's hands
                //debugText.text = "controller to hands";
                controllerModelLeft.SetActive(false);
                controllerModelRight.SetActive(false);

                //TODO-fix error
                
                leftHandRot = Quaternion.identity;
                leftHandRot.eulerAngles = new Vector3(-90, 0, -90);
                leftHandIKTarget.transform.localRotation = leftHandRot;
                Quaternion mirrorLeftRot = Quaternion.identity;
                mirrorLeftRot.eulerAngles = new Vector3(90, 0, 90);
                mirrorLeftIKTarget.transform.localRotation = mirrorLeftRot;


                rightHandRot = Quaternion.identity;
                rightHandRot.eulerAngles = new Vector3(90, 0, 90);
                rightHandIKTarget.transform.localRotation = rightHandRot;
                Quaternion mirrorRightRot = Quaternion.identity;
                mirrorRightRot.eulerAngles = new Vector3(-90, 0, -90);
                mirrorRightIKTarget.transform.localRotation = mirrorRightRot;


            }
            else if (control == OVRPlugin.Controller.Touch || control == OVRPlugin.Controller.LTouch || control == OVRPlugin.Controller.RTouch) //hand to controller
            {
                // enable box collider
                //debugText.text = "hands to controllers";
                if (!VRRig.Instance.isCalibrated)
                {
                    controllerModelLeft.SetActive(true);
                    controllerModelRight.SetActive(true);
                }

                //TODO-fix error
                
                leftControllerRot = Quaternion.identity;
                leftControllerRot.eulerAngles = new Vector3(0, 90, 90);
                leftHandIKTarget.transform.localRotation = leftControllerRot;
                Quaternion mirrorLeftRot = Quaternion.identity;
                mirrorLeftRot.eulerAngles = new Vector3(180, 90, -90);
                mirrorLeftIKTarget.transform.localRotation = mirrorLeftRot;

                rightControllerRot = Quaternion.identity;
                rightControllerRot.eulerAngles = new Vector3(0, -90, -90);
                rightHandIKTarget.transform.localRotation = rightControllerRot;
                Quaternion mirrorRightRot = Quaternion.identity;
                mirrorRightRot.eulerAngles = new Vector3(-180, -90, 90);
                mirrorRightIKTarget.transform.localRotation = mirrorRightRot;

                if(VRRig.Instance.avatarGender == VRRig.AvatarGender.Male)
                {
                    VRRig.Instance.maleAvatar.GetComponent<MixamoOVRHandTracking>().ResetFingerPose();
                }else if (VRRig.Instance.avatarGender == VRRig.AvatarGender.Female)
                {
                    VRRig.Instance.femaleAvatar.GetComponent<MixamoOVRHandTracking>().ResetFingerPose();
                }


            }
            currControl = control; //save current controller scheme
        }


        if (control == OVRPlugin.Controller.Hands || control == OVRPlugin.Controller.LHand || control == OVRPlugin.Controller.RHand || control == OVRPlugin.Controller.None) //None for Unity Editor
        {

        }
        else if (control == OVRPlugin.Controller.Touch || control == OVRPlugin.Controller.LTouch || control == OVRPlugin.Controller.RTouch)
        {

            //listen to button event for show/hide UI and pointers
            //check button mapping at https://developer.oculus.com/documentation/unity/unity-ovrinput/?locale=en_US
            if (OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.All) || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.All))
            {
                //show/hide UI
                if (settingUICanvas.activeSelf)
                {
                    settingUICanvas.SetActive(false);
                }
                else
                {
                    settingUICanvas.SetActive(true);
                }
                

            }
            else if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.All) || OVRInput.GetDown(OVRInput.Button.Four, OVRInput.Controller.All))
            {
                //show/hide mirror
                if (debugMirror.activeSelf)
                {
                    debugMirror.SetActive(false);
                }
                else
                {
                    debugMirror.SetActive(true);
                }
            }

        }
    }

    private void Swap(OVRPlugin.Controller controller)
    {
        

    }
}

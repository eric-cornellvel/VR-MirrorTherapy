using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    private Transform leftController;
    private Transform rightController;
    private Transform headSet;
    private Transform leftHand;
    private Transform rightHand;


    private string trackerHeader = "date-time,h_x,h_y,h_z,h_p,h_y,h_r,r_x,r_y,r_z,r_p,r_y,r_r,l_x,l_y,l_z,l_p,l_y,l_r";
    private string eventHeader = "date-time, sphere_location, left_hand_location, right_hand_location";
    protected string delimeter = ",";
    private bool isRecording = false;
    private ArrayList trackerArrayLeftController;
    private ArrayList trackerArrayRightController;
    private ArrayList trackerArrayheadset;
    private ArrayList eventArray;


    private string fixTime()
    {
        string currTime = System.DateTime.Now.ToString();
        string newString = currTime + ":" + System.DateTime.Now.Millisecond.ToString();

        return newString;
    }


    public void StartRecording()
    {
        isRecording = true;
        trackerArrayLeftController = new ArrayList();
        trackerArrayRightController = new ArrayList();
        trackerArrayheadset = new ArrayList();
        eventArray = new ArrayList();

        if(VRRig.Instance.avatarGender == VRRig.AvatarGender.Female)
        {
            leftHand = VRRig.Instance.femaleAvatar.transform.FindDeepChild("mixamorig:LeftHand");
            rightHand = VRRig.Instance.femaleAvatar.transform.FindDeepChild("mixamorig:RightHand");
        }
        else if (VRRig.Instance.avatarGender == VRRig.AvatarGender.Male)
        {
            leftHand = VRRig.Instance.maleAvatar.transform.FindDeepChild("mixamorig:LeftHand");
            rightHand = VRRig.Instance.maleAvatar.transform.FindDeepChild("mixamorig:RightHand");
        }

    }

    public void StopRecording()
    {
        isRecording = false;
  
        string saveFolder = Path.Combine(Application.persistentDataPath, "recording");
        
        if (!Directory.Exists(saveFolder)){

            Directory.CreateDirectory(saveFolder);
        }

        string id = DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss"); //use date time as unique id for each participant
        string headsetSavePath = Path.Combine(saveFolder, "movementdata_Head_" + id + ".csv");
        string leftControllerSavePath = Path.Combine(saveFolder, "movementdata_LeftController_" + id + ".csv");
        string rightControllerSavePath = Path.Combine(saveFolder, "movementdata_RightController_" + id + ".csv");
        string eventSavePath = Path.Combine(saveFolder, "collisiondata_" + id + ".csv");


        using (StreamWriter sw = new StreamWriter(headsetSavePath, false))
        {
            //write first line for header
            sw.WriteLine(trackerHeader);
            foreach (String str in trackerArrayheadset)
            {
                sw.WriteLine(str);
            }
        }

        using (StreamWriter sw = new StreamWriter(leftControllerSavePath, false))
        {
            //write first line for header
            sw.WriteLine(trackerHeader);
            foreach (String str in trackerArrayLeftController)
            {
                sw.WriteLine(str);
            }
        }

        using (StreamWriter sw = new StreamWriter(rightControllerSavePath, false))
        {
            //write first line for header
            sw.WriteLine(trackerHeader);
            foreach (String str in trackerArrayRightController)
            {
                sw.WriteLine(str);
            }
        }

        using (StreamWriter sw = new StreamWriter(eventSavePath, false))
        {
            //write first line for header
            sw.WriteLine(eventHeader);
            foreach (String str in eventArray)
            {
                sw.WriteLine(str);
            }
        }

        trackerArrayLeftController.Clear();
        trackerArrayRightController.Clear();
        trackerArrayheadset.Clear();
        eventArray.Clear();

    }


    public void LogSphereTouchedEvent(Vector3 touchedSpherePos)
    {
        if (isRecording)
        {
            string cur_time = fixTime();

            string csvdata = cur_time + ", " + "(" + touchedSpherePos.x + "; " + touchedSpherePos.y + "; " + touchedSpherePos.z + "), " +
                "(" + leftHand.position.x + "; " + leftHand.position.y + "; " + leftHand.position.z + "), "
                + "(" + rightHand.position.x + "; " + rightHand.position.y + "; " + rightHand.position.z + ")";

            eventArray.Add(csvdata);
        }
    }

    private void recordData()
    {

        Vector3 headPos = headSet.transform.position;
        Vector3 headRot = headSet.transform.rotation.eulerAngles;

        Vector3 LControllerPos = leftController.transform.position;
        Vector3 LControllerRot = leftController.transform.rotation.eulerAngles;

        Vector3 RControllerPos = rightController.transform.position;
        Vector3 RControllerRot = rightController.transform.rotation.eulerAngles;
        
        string milliString = fixTime();

        string csvDataLeftController = milliString + delimeter + LControllerPos[0] + delimeter + LControllerPos[1] +
            delimeter + LControllerPos[2] + delimeter + LControllerRot[0] + delimeter + LControllerRot[1] + delimeter + LControllerRot[2] +
            delimeter + "LeftController";

        trackerArrayLeftController.Add(csvDataLeftController);

        string csvDataRightController = milliString + delimeter + RControllerPos[0] + delimeter + RControllerPos[1] +
            delimeter + RControllerPos[2] + delimeter + RControllerRot[0] + delimeter + RControllerRot[1] + delimeter + RControllerRot[2] +
            delimeter + "RightController";

        trackerArrayRightController.Add(csvDataRightController);

        string csvDataHeadset = milliString + delimeter + headPos[0] + delimeter + headPos[1] +
            delimeter + headPos[2] + delimeter + headRot[0] + delimeter + headRot[1] + delimeter + headRot[2] +
            delimeter + "Head";

        trackerArrayheadset.Add(csvDataHeadset);


    }

    // Start is called before the first frame update
    void Start()
    {
        headSet = Utils.getHeadset().transform;
        leftController = Utils.getLeftController().transform;
        rightController = Utils.getRightController().transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording)
        {
            recordData();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSkinColor : MonoBehaviour
{
    public Button[] colorButtons = new Button[8];
    public Material[] skin_colors = new Material[8];
    private Image[] colors = new Image[8];
    public GameObject maleBody;
    public GameObject femaleBody;

    void Start()
    {

        colorButtons[0].onClick.AddListener(() => changeSkinColor(0));
        colorButtons[1].onClick.AddListener(() => changeSkinColor(1));
        colorButtons[2].onClick.AddListener(() => changeSkinColor(2));
        colorButtons[3].onClick.AddListener(() => changeSkinColor(3));
        colorButtons[4].onClick.AddListener(() => changeSkinColor(4));
        colorButtons[5].onClick.AddListener(() => changeSkinColor(5));
        colorButtons[6].onClick.AddListener(() => changeSkinColor(6));
        colorButtons[7].onClick.AddListener(() => changeSkinColor(7));
    }


    private void changeSkinColor(int index)
    {
        //Output this to console when the Button2 is clicked
        //Debug.Log("ButtonTest: "+ index);
        //int index = int.Parse(btnName.Substring(btnName.Length - 1));
        SkinnedMeshRenderer body_rend = null;
        if (VRRig.Instance.avatarGender == VRRig.AvatarGender.Male)
        {
            body_rend = maleBody.GetComponent<SkinnedMeshRenderer>();
            body_rend.material = skin_colors[index];
        }else if(VRRig.Instance.avatarGender == VRRig.AvatarGender.Female){
            body_rend = femaleBody.GetComponent<SkinnedMeshRenderer>();
            body_rend.material = skin_colors[index];
        }
        
    }

    /*
    public void changeSkinColor(int index)
    {
        Debug.Log(index);
        SkinnedMeshRenderer body_rend = body.GetComponent<SkinnedMeshRenderer>();
        body_rend.material = skin_colors[index];
    }
    */
}

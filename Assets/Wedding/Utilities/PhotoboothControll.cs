using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoboothControll : MonoBehaviour
{
    public GameObject ChoosePose;
    public GameObject CameraPanel;
    public GameObject pinchItem;
    public GameObject staticItem;
    public GameObject imageSelected;
    public Button galleryButton;
    public Button poseButton;
    public CameraScript cameraScript;
    public Transform containerImage;
    public int selectedImage;
    public string gallery;

    
    public void Restart(){
        DisableSelectModel();
        if(cameraScript.webCameraTexture != null)
            cameraScript.webCameraTexture.Stop();
        CameraPanel.SetActive(false);
        ChoosePose.SetActive(true);
        SetMedia(false);
    }

    public void SetMedia(bool value)
    {
    }
    
    // Start is called before the first frame update
    public struct UserData{
        public int wedding;
        public string name;
        public string nickname;
        public string affiliation;
        public int phoneNumber;
    }
    
    void SelectModel(int id, Transform img){
        selectedImage = id;
        DisableSelectModel();
        img.GetChild(1).GetComponent<Image>().enabled = true;
    }
    
    void DisableSelectModel(){
        for (int i = 0; i < containerImage.childCount; i++)
        {
            containerImage.GetChild(i).GetChild(1).GetComponent<Image>().enabled = false;
        }
    }
    
    void EmptyContainer(){
        for (int i = 0; i < containerImage.childCount; i++)
        {
            Destroy(containerImage.GetChild(i).gameObject);
        }
    }
    
    public void SetPose(){
        CameraPanel.SetActive(true);
        ChoosePose.SetActive(false);
        cameraScript.Init();

    }
    
}

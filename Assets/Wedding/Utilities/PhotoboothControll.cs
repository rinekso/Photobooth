using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class PhotoboothControll : MonoBehaviour
{
    public string urlVideo1;
    public string urlVideo2;
    public GameObject Panel;
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
    public Texture selectedTexture;
    public VideoPlayer selectedVideoPlayer;
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;
    public string gallery;

    private void Start() {
        StartCoroutine(DownloadFile(urlVideo1, videoPlayer1,"selfie"));
        StartCoroutine(DownloadFile(urlVideo2, videoPlayer2,"selfie2"));
    }
    IEnumerator DownloadFile(string url,VideoPlayer vP,string name) {
        string path = Path.Combine(Application.persistentDataPath, name+".mp4");
        if(!File.Exists(path)){
            var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerFile(path);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
                Debug.LogError(uwr.error);
            else{
                Debug.Log("File successfully downloaded and saved to " + path);
                vP.url = path;
                Panel.SetActive(false);
            }
        }else{
            vP.url = path;
            Panel.SetActive(false);
        }
    }
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
    
    public void SelectModel(Transform img){
        selectedImage = img.GetComponent<ImageProperties>().type;
        DisableSelectModel();
        img.GetChild(1).GetComponent<Image>().enabled = true;
        selectedTexture = img.GetChild(0).GetComponent<RawImage>().texture;
        selectedVideoPlayer = img.GetComponent<ImageProperties>().videoPlayer;
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
        if(selectedImage == 1){
            pinchItem.SetActive(true);
            StartCoroutine(LoadItem(pinchItem.GetComponent<RawImage>()));
            staticItem.SetActive(false);
        }else if(selectedImage == 0){
            staticItem.SetActive(true);
            StartCoroutine(LoadItem(staticItem.GetComponent<RawImage>()));
            pinchItem.SetActive(false);
        }
        CameraPanel.SetActive(true);
        ChoosePose.SetActive(false);
        // cameraScript.Init();

    }
    IEnumerator LoadItem(RawImage image){
        image.texture = selectedTexture;
        selectedVideoPlayer.Stop();
        yield return new WaitForSeconds(.1f);
        selectedVideoPlayer.isLooping = true;
        selectedVideoPlayer.Play();
        // media.TrySetLoop(true);
        // media.TrySetMute(true);
        // media.TryPlay();
        yield return new WaitForSeconds(3f);
        selectedVideoPlayer.Pause();
        // media.TryStop();
    }
    public void ImageReturn(){
        selectedVideoPlayer.isLooping = false;
        selectedVideoPlayer.Play();
    }
    
}

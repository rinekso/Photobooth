using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System;
using UnityEngine.EventSystems;

public class PhotoboothControll : MonoBehaviour
{
    public GameObject Panel;
    public GameObject ChoosePose;
    public GameObject CameraPanel;
    public GameObject pinchItem;
    public GameObject staticItem;
    public GameObject imagePrefabs;
    public Button galleryButton;
    public Button poseButton;
    public CameraScript cameraScript;
    public Transform containerImage;
    public int selectedImage;
    public Texture selectedTexture;
    public VideoPlayer selectedVideoPlayer;
    public GameObject videoPlayer;
    public string gallery;
    public string wedCode;
    public TMPro.TextMeshProUGUI title;
    bool isFirst = false;
    Vector2 startPos;
    Vector2 selisih;

    private void Start() {
        // CheckFiles();
    }
    public void Exit(){
        UnityMessageManager.Instance.SendMessageToFlutter("exit");
    }
    public void PointerDown(string data){
        print("selected image : "+selectedImage);
        if(selectedImage == 1){
            string[] sparator = {","};
            string[] pos = data.Split(sparator,2,StringSplitOptions.RemoveEmptyEntries);
            if(!isFirst){
                isFirst = true;
                print(pinchItem.GetComponent<RectTransform>().position);
                startPos.x = -float.Parse(pos[1]);
                startPos.y = float.Parse(pos[0]);

                selisih.x = pinchItem.GetComponent<RectTransform>().position.x-startPos.x;
                selisih.y = pinchItem.GetComponent<RectTransform>().position.y-startPos.y;
            }else{
                pinchItem.GetComponent<RectTransform>().position = new Vector2(-float.Parse(pos[1]),float.Parse(pos[0])) + selisih;
            }
        }
    }
    // public void OnPointerDown(PointerEventData ped){
    //     startPos = ped.position;
    //     selisih.x = pinchItem.GetComponent<RectTransform>().position.x-startPos.x;
    //     selisih.y = pinchItem.GetComponent<RectTransform>().position.y-startPos.y;

    //     print("touch down");
    // }
    // public void OnDrag(PointerEventData ped){
    //     pinchItem.GetComponent<RectTransform>().position = ped.position + selisih;
    // }
    public void PointerEnd(string data){
        if(selectedImage == 1){
            isFirst = false;
        }
    }
    public void Scaling(string data) {
        float scale = float.Parse(data);
        pinchItem.GetComponent<RectTransform>().localScale = new Vector3(scale,scale,scale);
    }
    public void SetWeddingCode(string data){
        print(data);
        wedCode = data;
        // title.text = data;
        CheckFiles();
    }
    void CheckFiles(){
        string path = Path.Combine(Application.persistentDataPath, "wedding_assets/"+wedCode+"/photobooth");
        print(path);
        var files = System.IO.Directory.GetFiles(path);
        if(files.Length > 0){
            foreach (string file in files)
            {
                VideoPlayer vp = Instantiate(videoPlayer,transform).GetComponent<VideoPlayer>();
                print(file);
                vp.url = file;
                RenderTexture rt = new RenderTexture(512,768,24);
                vp.targetTexture = rt;
                GameObject imageProperties = Instantiate(imagePrefabs,containerImage);
                imageProperties.GetComponent<Button>().onClick.AddListener(()=>{
                    SelectModel(imageProperties.transform);
                });
                imageProperties.GetComponentInChildren<RawImage>().texture = rt;
                imageProperties.GetComponent<ImageProperties>().videoPlayer = vp;
            }
        }else{
            print("no data photobooth AR");
        // StartCoroutine(DownloadFile(urlVideo1, videoPlayer1,"selfie"));
        // StartCoroutine(DownloadFile(urlVideo2, videoPlayer2,"selfie2"));
        }
    }
    IEnumerator DownloadFile(string url,VideoPlayer vP,string name) {
        string path = Path.Combine(Application.persistentDataPath, "wedding_assets/"+wedCode+"/");
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
        // DisableSelectModel();
        if(cameraScript.webCameraTexture != null)
            cameraScript.webCameraTexture.Stop();
        CameraPanel.SetActive(false);
        ChoosePose.SetActive(true);
    }

    public void SetMedia(bool value)
    {
    }
    
    // Start is called before the first frame update
    private void Update() {
        
    }
    public struct UserData{
        public int wedding;
        public string name;
        public string nickname;
        public string affiliation;
        public int phoneNumber;
    }
    
    public void SelectModel(Transform img){
        // selectedImage = img.GetComponent<ImageProperties>().type;
        selectedImage = 1;
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
        selectedVideoPlayer.Stop();
        selectedVideoPlayer.Play();
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

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System;
using UnityEngine.Scripting;

public class PhotoboothControll : MonoBehaviour
{
    public GameObject choosePose;
    public GameObject cameraPanel;
    public GameObject pinchItem;
    public GameObject staticItem;
    public GameObject imagePrefabs;
    public CameraScript cameraScript;
    public Transform containerImage;
    public int selectedImage;
    public Texture selectedTexture;
    public VideoPlayer selectedVideoPlayer;
    public GameObject videoPlayer;
    public string wedCode;
    bool isFirst = false;
    Vector2 startPos;
    Vector2 selisih;

    private void Start() {
        // print("start");
        // CheckFiles();
    }
    static void enableGC()
    {
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        // Trigger a collection to free memory.
        GC.Collect();
    }
    public void exit(){
        try
        {
            StopAllCoroutines();
            enableGC();
            UnityMessageManager.Instance.SendMessageToFlutter("exit");            
        }
        catch (System.Exception e)
        {
            print(e.Message);
            print("exit error");
            throw;
        }
    }
    public void toGallery(){
        try
        {
            StopAllCoroutines();
            enableGC();
            UnityMessageManager.Instance.SendMessageToFlutter("toGallery");            
        }
        catch (System.Exception e)
        {
            print(e.Message);
            print("exit error");
            throw;
        }
    }
    public void PointerDown(string data){
        try
        {
            if(selectedImage == 1){
                string[] sparator = {","};
                string[] pos = data.Split(sparator,2,StringSplitOptions.RemoveEmptyEntries);
                if(!isFirst){
                    isFirst = true;
                    // print(pinchItem.GetComponent<RectTransform>().position);
                    startPos.x = float.Parse(pos[0]);
                    startPos.y = -float.Parse(pos[1]);

                    selisih.x = pinchItem.GetComponent<RectTransform>().position.x-startPos.x;
                    selisih.y = pinchItem.GetComponent<RectTransform>().position.y-startPos.y;
                }else{
                    pinchItem.GetComponent<RectTransform>().position = new Vector2(float.Parse(pos[0]),-float.Parse(pos[1])) + selisih;
                }
            }
        }
        catch (System.Exception e)
        {
            print(e.Message);
            print("pointer down error");            
            throw;
        }
        // print("selected image : "+selectedImage);
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
        try
        {
            float scale = float.Parse(data);
            pinchItem.GetComponent<RectTransform>().localScale = new Vector3(scale,scale,scale);
        }
        catch (System.Exception e)
        {
            print(e.Message);
            print("scalling error");
            throw;
        }
    }
    public void SetWeddingCode(string data){
        if(data != wedCode){
            wedCode = data;
            CheckFiles();
        }
    }
    void CheckFiles(){
        string path = Path.Combine(Application.persistentDataPath, "wedding_assets/"+wedCode+"/photobooth");
        // print(path);
        var files = System.IO.Directory.GetFiles(path);

        DeleteVideoPlayer();
        DeleteImageProperties();

        if(files.Length > 0){
            foreach (string file in files)
            {
                try
                {
                    VideoPlayer vp = Instantiate(videoPlayer,transform).GetComponent<VideoPlayer>();
                    // print(file);
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
                catch (System.Exception e)
                {
                    print(e.Message);
                    print("check file error");
                    throw;
                }
            }
        }
    }
    void DeleteImageProperties(){
        ImageProperties[] imgprop = containerImage.GetComponentsInChildren<ImageProperties>();
        if(imgprop.Length > 0){
            for (int i = 0; i < imgprop.Length; i++)
            {
                Destroy(imgprop[i].gameObject);
            }
        }
    }
    void DeleteVideoPlayer(){
        VideoPlayer[] imgprop = transform.GetComponentsInChildren<VideoPlayer>();
        if(imgprop.Length > 0){
            for (int i = 0; i < imgprop.Length; i++)
            {
                Destroy(imgprop[i].gameObject);
            }
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
                // panel.SetActive(false);
            }
        }else{
            vP.url = path;
            // panel.SetActive(false);
        }
    }
    public void Restart(){
        VideoPlayer[] vps = GetComponentsInChildren<VideoPlayer>();
        for (int i = 0; i < vps.Length; i++)
        {
            vps[i].Stop();
            vps[i].Play();
            vps[i].isLooping = true;
        }
        // DisableSelectModel();
        if(cameraScript.webCameraTexture != null)
            cameraScript.webCameraTexture.Stop();
        cameraPanel.SetActive(false);
        choosePose.SetActive(true);
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
        cameraPanel.SetActive(true);
        choosePose.SetActive(false);
        cameraScript.LoadWebCamTexture();

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

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public WebCamTexture webCameraTexture;
    RectTransform currentRect;
    public RectTransform buttonPanel;
    public RawImage preview;
    public GameObject backButton;
    public enum Mode{
        Boomerang,Selfie,Casual
    }
    Mode currentMode;
    WebCamDevice[] devices;
    int currentDevice = 0;
    public GameObject countDown;
    public GameObject countDownDesc;
    public GameObject coverWhite;
    public PhotoboothControll PC;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        webCameraTexture = new WebCamTexture();
        devices = WebCamTexture.devices;
        // print("device length: "+WebCamTexture.devices.Length);
        LoadWebCamTexture();
        currentMode = Mode.Casual;
        currentRect = transform.parent.GetComponent<RectTransform>();
    }

    void OnDestroy()
    {
        webCameraTexture.Stop();
    }
    public void SwitchCam(){
        webCameraTexture.Stop();
        if(currentDevice+1 < devices.Length){
            currentDevice+=1;
        }else{
            currentDevice = 0;
        }
        LoadWebCamTexture();
    }
    public void LoadWebCamTexture(){
        webCameraTexture.deviceName = devices[currentDevice].name;
        // Fprint(devices[currentDevice].name);
        webCameraTexture.Play();

        // change as user rotates iPhone or Android:

        int cwNeeded = webCameraTexture.videoRotationAngle;
        // Unity helpfully returns the _clockwise_ twist needed
        // guess nobody at Unity noticed their product works in counterclockwise:
        int ccwNeeded = -cwNeeded;

        // IF the image needs to be mirrored, it seems that it
        // ALSO needs to be spun. Strange: but true.
        if ( webCameraTexture.videoVerticallyMirrored ) ccwNeeded += 180;

        // you'll be using a UI RawImage, so simply spin the RectTransform
        GetComponent<RectTransform>().localEulerAngles = new Vector3(0f,0f,ccwNeeded);

        float videoRatio = (float)webCameraTexture.width/(float)webCameraTexture.height;

        // you'll be using an AspectRatioFitter on the Image, so simply set it
        GetComponent<AspectRatioFitter>().aspectRatio = videoRatio;

        // alert, the ONLY way to mirror a RAW image, is, the uvRect.
        // changing the scale is completely broken.
        if ( webCameraTexture.videoVerticallyMirrored )
            GetComponent<RawImage>().uvRect = new Rect(1,0,-1,1);  // means flip on vertical axis
        else
            GetComponent<RawImage>().uvRect = new Rect(0,0,1,1);  // means no flip

        if(currentDevice > 0){
            GetComponent<RawImage>().uvRect = new Rect(1,0,-1,1);  // means no flip
        GetComponent<RectTransform>().localEulerAngles = new Vector3(0f,0f,-90);
        }
            
        // GetComponent<RectTransform>().sizeDelta = new Vector2(webCameraTexture.width,webCameraTexture.height);
        GetComponent<RawImage>().texture = webCameraTexture;
    }
    public void ChangeMode(int mode){
        if(mode == 0){
            currentMode = Mode.Selfie;
        }else if(mode == 1){
            currentMode = Mode.Boomerang;
        }else if(mode == 2){
            currentMode = Mode.Casual;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakePhoto(){
        PC.selectedVideoPlayer.Stop();
        PC.selectedVideoPlayer.Play();
        StartCoroutine(Take());
    }
    Texture2D[] tempTexture = new Texture2D[20];
    IEnumerator Boomerang(){
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
            Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
            Texture2D shotCrop = new Texture2D(Screen.width, Screen.height-(int) buttonPanel.rect.height);
            Color[] pixels = new Color[Screen.width * Screen.height-(int) buttonPanel.rect.height];
            int crop = (Screen.height-Screen.height-(int) buttonPanel.rect.height);
            pixels = shot.GetPixels(0, crop, shot.width, shot.height-crop, 0);
            shotCrop.SetPixels(0, 0,Screen.width,Screen.height-(int) buttonPanel.rect.height, pixels,0);
            shotCrop.Apply();
            tempTexture[i] = shotCrop;
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForEndOfFrame();
        // NativeCamera.RecordVideo(,);

    }
    
    
    IEnumerator Take(){
        yield return new WaitForEndOfFrame();
        backButton.SetActive(false);
        coverWhite.SetActive(false);
        countDown.SetActive(true);
        countDownDesc.SetActive(true);
        TMPro.TextMeshProUGUI textCount = countDown.GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI textCountDesc = countDownDesc.GetComponent<TMPro.TextMeshProUGUI>();
        for(int  i=3;i>0;i--){
            textCount.text = (i)+"";
            if(i == 3)
                textCountDesc.text = "Get Ready";
            else
                textCountDesc.text = "Steady";

            yield return new WaitForSeconds(1);
        }
        countDown.SetActive(false);
        countDownDesc.SetActive(false);
        yield return new WaitForEndOfFrame();
        int crop = (int) buttonPanel.rect.height;
        // print("crop : "+crop+" / screen height : "+Screen.height+" / rect height : "+(Screen.height-(int) buttonPanel.rect.height));
        Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D shotCrop = new Texture2D(Screen.width, Screen.height-(int) buttonPanel.rect.height);
        Color[] pixels = new Color[Screen.width * Screen.height-(int) buttonPanel.rect.height];
        pixels = shot.GetPixels(0, crop, shot.width, shot.height-crop, 0);
        shotCrop.SetPixels(0, 0, Screen.width, Screen.height-crop, pixels,0);
        shotCrop.Apply();
        preview.texture = shotCrop;
        string date = System.DateTime.Now.ToString("ss-mm-hh_d-MM-y");
        byte[] bytes = shotCrop.EncodeToJPG();
        string fullPath = Application.persistentDataPath+"/wedding_photos/"+PC.wedCode;
        if(!Directory.Exists( @fullPath ) ){
            Directory.CreateDirectory(@fullPath);
        }

        SaveImage(Application.persistentDataPath + "/wedding_photos/" + PC.wedCode + "/", bytes);
        // NativeGallery.SaveImageToGallery(bytes, "BeloveWed", PC.wedCode+"_"+date+".jpg");

        coverWhite.SetActive(true);        
        textCount.text = "";
        countDown.SetActive(false);
        yield return new WaitForSeconds(.2f);
        coverWhite.SetActive(false);
        PC.ImageReturn();
        backButton.SetActive(true);
    }
    
    void SaveImage(string path, byte[] data){
        var date = DateTime.Now.ToString("ss-mm-hh_d-MM-y");
        path = path + date + ".jpg";
        
        Debug.Log("Saving texture to: " + path);
        File.WriteAllBytes(@path, data);
    }
    
    public void GalleryScene(){
        Application.LoadLevel(1);
    }
}

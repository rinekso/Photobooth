using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PinchableScaleMove : MonoBehaviour {
    int countDown;
    Vector2 startPos;
    int startId;
    Vector2 secondPos;
    int secondId;
    float distancefirst;
    Vector2 selisih;
    Vector2[] updatePos;
    float currentScale = 1;
    float calculatDist = 1;
    bool isFirst = false;
    private void Start() {
        updatePos = new Vector2[2];
    }
    public void PointerDown(string data){
        string[] sparator = {","};
        string[] pos = data.Split(sparator,2,StringSplitOptions.RemoveEmptyEntries);
        if(!isFirst){
            isFirst = true;
            startPos.x = float.Parse(pos[0]);
            startPos.y = float.Parse(pos[1]);
            selisih.x = GetComponent<RectTransform>().position.x-startPos.x;
            selisih.y = GetComponent<RectTransform>().position.y-startPos.y;
        }else{
            GetComponent<RectTransform>().position = new Vector2(float.Parse(pos[0]),float.Parse(pos[1])) + selisih;
        }
    }
    public void PointerEnd(){
        isFirst = false;
    }
    public void Scaling(string data) {
        print(data);
        float scale = float.Parse(data);
        GetComponent<RectTransform>().localScale = new Vector3(scale,scale,scale);
    }
    // public void OnPointerDown(PointerEventData ped){
    //     countDown++;
    //     if(countDown == 1){
    //         startPos = ped.position;
    //         startId = ped.pointerId;
    //         selisih.x = GetComponent<RectTransform>().position.x-startPos.x;
    //         selisih.y = GetComponent<RectTransform>().position.y-startPos.y;
    //     }else if(countDown>=2){
    //         secondPos = ped.position;
    //         updatePos[0] = startPos;
    //         updatePos[1] = secondPos;
    //         secondId = ped.pointerId;
    //         distancefirst = Vector2.Distance(startPos,secondPos);
    //     }
    //     print("touch down");
    // }
    // public void OnPointerUp(PointerEventData ped){
    //     if(countDown >= 2){
    //         currentScale = currentScale*calculatDist;
    //     }
    //     countDown = 0;
    //     print("touch up");
    // }
    // public void OnDrag(PointerEventData ped){
    //     print("drag");
    //     if(countDown==1){
    //         GetComponent<RectTransform>().position = ped.position + selisih;
    //     }else if(countDown == 2){
    //         if(ped.pointerId == startId){
    //             updatePos[0] = ped.position;
    //         }else{
    //             updatePos[1] = ped.position;
    //         }
    //         float currentDist = Vector2.Distance(updatePos[0],updatePos[1]);
    //         calculatDist = currentDist/distancefirst;
    //         GetComponent<RectTransform>().localScale = new Vector3(currentScale*calculatDist,currentScale*calculatDist,currentScale*calculatDist);
    //     }
    // }
    void Update()
    {
    }
}
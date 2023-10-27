using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour{

    public void ChangeSoloPlay(){
        Debug.Log("ChangeScene");
        SceneManager.LoadScene("PlayScene");
    }
    public void Exit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            //ゲームプレイ終了
        #else
            Application.Quit();
            //ゲームプレイ終了
        #endif
    }
}

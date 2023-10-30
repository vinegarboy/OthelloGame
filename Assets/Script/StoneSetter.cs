using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Othello;
using TMPro;
using OthelloAI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class StoneSetter : MonoBehaviour{

    int turn_color = 1;

    public bool vsAi = false;

    AISystem ai = new AISystem(2);
    [SerializeField]
    TextMeshProUGUI AI_data;

    [SerializeField]
    TextMeshProUGUI turn_title;

    [SerializeField]
    TextMeshProUGUI Debug_Text;

    [SerializeField]
    GameObject stone;
    Board bd = new Board();

    GameObject[,] stones = new GameObject[8,8];
    void Start(){
        bd.Init_Board();

        StoneVisual sv;
        for(int x = 0;x<8;x++){
            for(int y = 0;y<8;y++){
                stones[x,y] = Instantiate(stone, Vector3.zero, Quaternion.identity, this.transform);
                stones[x,y].transform.localPosition = new Vector3(-472.5f+(x*135),472.5f-(y*135),0);
                sv = stones[x,y].GetComponent<StoneVisual>();
                sv.color = bd.board[x,y];
                sv.parentSystem = this;
                sv.pos_record = new int[]{x,y};
                sv.ChangeColor();
            }
        }

        ViewCanPut();
        Player_name();
        Show_DebugText();
    }

    void Show_DebugText(){
        int[] count = bd.Color_Count();
        ScoreResult.voidCount = count[0];
        ScoreResult.WhiteCount = count[1];
        ScoreResult.BlackCount = count[2];
        Debug_Text.text = $"Void:{count[0]}\nWhite:{count[1]}\nBlack:{count[2]}";
    }

    void Player_name(){
        if(turn_color == 1){
            turn_title.text = $"Player:White";
        }else{
            turn_title.text = $"Player:Black";
        }
    }

    void UpdateBoard(){
        StoneVisual sv;
        for(int x = 0;x<8;x++){
            for(int y = 0;y<8;y++){
                sv = stones[x,y].GetComponent<StoneVisual>();
                sv.color = bd.board[x,y];
                sv.ChangeColor();
            }
        }
        Show_DebugText();
    }

    void ViewCanPut(){
        UpdateBoard();
        List<int[]> can_put = bd.can_put_list(turn_color);
        foreach(int[] pos in can_put){
            stones[pos[0],pos[1]].GetComponent<StoneVisual>().color = 3;
        }
    }

    void color_Change(){
        if(turn_color == 1){
            turn_color = 2;
        }else{
            turn_color = 1;
        }
    }

    public void Clicked(StoneVisual cl_obj){
        if(cl_obj.color == 3){
            cl_obj.color = turn_color;
            bd.put_stone(cl_obj.pos_record[0],cl_obj.pos_record[1],turn_color);
            color_Change();
            ViewCanPut();
            Player_name();
            if(vsAi){
                AIClick();
            }
            CheckResult();
        }
    }

    public void CheckResult(){
        int who_win = bd.judge_winner();
        ScoreResult.winner = who_win;
        if(who_win != -1){
            SceneManager.LoadScene("Result");
        }
    }

    public void AIClick(){
        ai.SetBoard(bd.board);
        int[] pos = ai.most_flip_put_stone();
        bd.put_stone(pos[0],pos[1],turn_color);
        AI_data.text = $"AI Datas\nSetPosition\nx:{pos[0]}\ny:{pos[1]}";
        color_Change();
        ViewCanPut();
        Player_name();
        CheckResult();
    }
}

public static class ScoreResult{

    public static int winner = -1;

    public static int WhiteCount;
    public static int BlackCount;
    public static int voidCount;

}
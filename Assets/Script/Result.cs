using TMPro;
using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI white_stone_text;

    [SerializeField]
    TextMeshProUGUI black_stone_text;

    [SerializeField]
    TextMeshProUGUI Who_win_text;

    void Start(){
        white_stone_text.text = $"WhiteStone:{ScoreResult.WhiteCount}";
        black_stone_text.text = $"BlackStone:{ScoreResult.BlackCount}";
        if(ScoreResult.winner == 1){
            Who_win_text.text = "White Win!!";
        }else if(ScoreResult.winner == 2){
            Who_win_text.text = "Black Win!!";
        }else{
            Who_win_text.text = "Drow!!";
        }
    }
}

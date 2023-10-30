using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoneVisual : MonoBehaviour{

    public int color = 0;

    public int[] pos_record = {0,0};

    [SerializeField]
    Image img;

    public StoneSetter parentSystem;

    [SerializeField]
    Sprite[] stoneImages = new Sprite[4];

    void Start(){
        img.sprite = stoneImages[color];
    }

    public void ChangeColor(){
        img.sprite = stoneImages[color];
    }

    void Update(){
        if(img.sprite != stoneImages[color]){
            img.sprite = stoneImages[color];
        }
    }

    public void Clicked(BaseEventData eventData){
        Debug.Log($"x:{pos_record[0]} , y:{pos_record[1]}がクリックされました.");
        parentSystem.Clicked(this);
    }
}

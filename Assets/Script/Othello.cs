using System;
using System.Collections.Generic;

namespace Othello
{

    /// <summary>
    /// 通常の8*8の盤面を利用するオセロのクラス
    /// </summary>
    class Board{

        //盤面を保存する変数。0は何もおいてない状態。1が白で2が黒
        public int[,] board = new int[8,8];

        //盤面のリセットを行う関数
        public void Init_Board(){
            for(int x = 0;x<8;x++){
                for(int y = 0;y<8;y ++){
                    board[x,y] = 0;
                }
            }

            //最初の4マス分の配置
            board[3,3] = 1;
            board[3,4] = 2;
            board[4,3] = 2;
            board[4,4] = 1;
        }

        public String View_board(){
            //返す用の変数
            String ret = "";

            //変数retに追加していく
            for(int y = 0;y<8;y++){
                for(int x = 0;x<8;x++){
                    ret+=board[x,y];
                }
                //改行を加える。
                ret+="\n";
            }

            return ret;
        }

        // 指定されたx、yの位置に指定された色の石を置けるかどうかを判定し、石を裏返す処理を行う関数
        public bool put_stone(int x, int y, int color) {
            // 既に石が置かれている場合は置けない。また色が指定されている範囲内かを調べる
            if (board[x, y] != 0&&color > 0&&color<3) {
                return false;
            }

            // 置ける場所があるかどうかを判定するフラグ
            bool can_place = false;

            // 左右、上下、斜めの8方向について石を裏返す処理を行う
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    // 自分自身の場合はスキップ
                    if (i == 0 && j == 0) {
                        continue;
                    }

                    // 裏返す石を格納するリスト
                    List<int[]> flip_list = new List<int[]>();

                    // 現在の位置から指定方向に進んでいく
                    int cur_x = x + i;
                    int cur_y = y + j;

                    // 次の位置が盤面内かつ相手の石である限り石を探し続ける
                    while (cur_x >= 0 && cur_x < 8 && cur_y >= 0 && cur_y < 8 && board[cur_x, cur_y] != 0 && board[cur_x, cur_y] != color) {
                        // 裏返す石をリストに追加
                        flip_list.Add(new int[] {cur_x, cur_y});

                        // 次の位置に進む
                        cur_x += i;
                        cur_y += j;
                    }

                    // 最後の位置が自分の石である場合、石を裏返す
                    if (cur_x >= 0 && cur_x < 8 && cur_y >= 0 && cur_y < 8 && board[cur_x, cur_y] == color) {
                        can_place = true;
                        flip_list.Add(new int[]{x,y});
                        // 裏返す石を裏返す
                        foreach (int[] flip_pos in flip_list) {
                            board[flip_pos[0], flip_pos[1]] = color;
                        }
                    }
                }
            }
            // 置ける場所があるかどうかを返す
            return can_place;
        }

        // 指定された色の番号に対して、その色の石が置ける位置をList<int[]>で返す関数
        public List<int[]> can_put_list(int color) {
            // 置ける位置を格納するリスト
            List<int[]> put_list = new List<int[]>();

            // 盤面を走査して、指定された色の石が置ける場所を探す
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    // 既に石が置かれている場合はスキップ
                    if (board[x, y] != 0) {
                        continue;
                    }

                    // 置けるかどうかを判定するフラグ
                    bool can_place = false;

                    // 左右、上下、斜めの8方向について石を裏返す処理を行う
                    for (int i = -1; i <= 1; i++) {
                        for (int j = -1; j <= 1; j++) {
                            // 自分自身の場合はスキップ
                            if (i == 0 && j == 0) {
                                continue;
                            }
                            // 現在の位置から指定方向に進んでいく
                            int cur_x = x + i;
                            int cur_y = y + j;
                            
                            int def_x = x+i;
                            int def_y = y+j;
                            // 相手の石である限り石を探し続ける
                            bool can_flip = false;
                            while (cur_x >= 0 && cur_x < 8 && cur_y >= 0 && cur_y < 8) {
                                int currentCell = board[cur_x, cur_y];
                                if (currentCell == 0) {
                                    break;
                                }
                                if (currentCell == color) {
                                    if(def_x != cur_x||def_y!=cur_y){
                                        can_flip = true;
                                    }
                                    break;
                                }
                                cur_x += i;
                                cur_y += j;
                            }

                            // 最後の位置が自分の石である場合、石を置ける場所とする
                            if (can_flip) {
                                can_place = true;
                            }
                        }
                    }

                    // 石を置ける場合、座標をリストに追加
                    if (can_place) {
                        put_list.Add(new int[] {x, y});
                    }
                }
            }

            // 置ける場所を返す
            return put_list;
        }


        //どちらかが勝利しているかを判定する。
        public int judge_winner(){
            // 判定用の石の数を初期化
            int whiteCount = 0;
            int blackCount = 0;

            //石の数を比較する。
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if (board[i, j] == 1) whiteCount++;
                    else if (board[i, j] == 2) blackCount++;
                }
            }

            // 盤面上における白または黒の石が置ける場所があるかどうかをチェック
            bool whiteCanPut = can_put_list(1).Count > 0;
            bool blackCanPut = can_put_list(2).Count > 0;

            //プレイヤーの石が消失してる場合の判定チェック
            if (whiteCount == 0) return 2; // 白の石がすべて消失した場合、黒の勝ち
            else if (blackCount == 0) return 1; // 黒の石がすべて消失した場合、白の勝ち

            // どちらのプレイヤーも石を置けない場合、石の数を比較して勝敗を決定
            if (!whiteCanPut && !blackCanPut){
                if (whiteCount > blackCount) return 1; // 白の勝ち
                else if (whiteCount < blackCount) return 2; // 黒の勝ち
                else return 0; // 引き分け
            }

            // どちらかのプレイヤーが石を置ける場合、まだ勝敗が決まっていないことを示す
            return -1;
        }
    
        //現在の盤面から空白と白と黒の数を返す
        public int[] Color_Count(){
            int[] ret = new int[]{0,0,0};
            for(int x = 0;x<8;x++){
                for(int y = 0;y<8;y ++){
                    if(board[x,y]==0){
                        ret[0]++;
                    }else if(board[x,y]==1){
                        ret[1]++;
                    }else if(board[x,y]==2){
                        ret[2]++;
                    }
                }
            }
            return ret;
        }
    
    }

    /// <summary>
    /// 余白分を2マス毎に加算してカスタムされた盤面データを作成したバージョン。
    /// </summary>
    class CustomBoard{

        //盤面を保存する変数。0は何もおいてない状態。1が白で2が黒
        public int[,] board;

        //余白を記録する変数
        private int width = 0;
        private int height = 0;

        //初期化
        CustomBoard(int margin_height,int margin_width){
            width = 2+(margin_width*2);
            height = 2+(margin_height*2);
            board = new int[width,height];
        }

        //盤面のリセットを行う関数
        public void Init_Board(){
            for(int x = 0;x<width;x++){
                for(int y = 0;y<height;y ++){
                    board[x,y] = 0;
                }
            }

            //最初の4マス分の配置
            board[(width/2)-1,(height/2)-1] = 1;
            board[(width/2)-1,height/2] = 2;
            board[width/2,(height/2)-1] = 2;
            board[width/2,height/2] = 1;
        }

        public String View_board(){
            //返す用の変数
            String ret = "";

            //変数retに追加していく
            for(int y = 0;y<height;y++){
                for(int x = 0;x<width;x++){
                    ret+=board[x,y];
                }
                //改行を加える。
                ret+="\n";
            }

            return ret;
        }

        // 指定されたx、yの位置に指定された色の石を置けるかどうかを判定し、石を裏返す処理を行う関数
        public bool put_stone(int x, int y, int color) {
            // 既に石が置かれている場合は置けない。また色が指定されている範囲内かを調べる
            if (board[x, y] != 0&&color > 0&&color<3) {
                return false;
            }

            // 置ける場所があるかどうかを判定するフラグ
            bool can_place = false;

            // 左右、上下、斜めの全部の方向について石を裏返す処理を行う
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    // 自分自身の場合はスキップ
                    if (i == 0 && j == 0) {
                        continue;
                    }

                    // 裏返す石を格納するリスト
                    List<int[]> flip_list = new List<int[]>();

                    // 現在の位置から指定方向に進んでいく
                    int cur_x = x + i;
                    int cur_y = y + j;

                    // 次の位置が盤面内かつ相手の石である限り石を探し続ける
                    while (cur_x >= 0 && cur_x < width && cur_y >= 0 && cur_y < height && board[cur_x, cur_y] != 0 && board[cur_x, cur_y] != color) {
                        // 裏返す石をリストに追加
                        flip_list.Add(new int[] {cur_x, cur_y});

                        // 次の位置に進む
                        cur_x += i;
                        cur_y += j;
                    }

                    // 最後の位置が自分の石である場合、石を裏返す
                    if (cur_x >= 0 && cur_x < width && cur_y >= 0 && cur_y < height && board[cur_x, cur_y] == color) {
                        can_place = true;
                        flip_list.Add(new int[]{x,y});
                        // 裏返す石を裏返す
                        foreach (int[] flip_pos in flip_list) {
                            board[flip_pos[0], flip_pos[1]] = color;
                        }
                    }
                }
            }
            // 置ける場所があるかどうかを返す
            return can_place;
        }

        // 指定された色の番号に対して、その色の石が置ける位置をList<int[]>で返す関数
        public List<int[]> can_put_list(int color) {
            // 置ける位置を格納するリスト
            List<int[]> put_list = new List<int[]>();

            // 盤面を走査して、指定された色の石が置ける場所を探す
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // 既に石が置かれている場合はスキップ
                    if (board[x, y] != 0) {
                        continue;
                    }

                    // 置けるかどうかを判定するフラグ
                    bool can_place = false;

                    // 左右、上下、斜めのすべての方向について石を裏返す処理を行う
                    for (int i = -1; i <= 1; i++) {
                        for (int j = -1; j <= 1; j++) {
                            // 自分自身の場合はスキップ
                            if (i == 0 && j == 0) {
                                continue;
                            }
                            // 現在の位置から指定方向に進んでいく
                            int cur_x = x + i;
                            int cur_y = y + j;

                            // 次の位置が盤面内かつ相手の石である限り石を探し続ける
                            while (cur_x >= 0 && cur_x < width && cur_y >= 0 && cur_y < height && board[cur_x, cur_y] != 0 && board[cur_x, cur_y] != color) {
                                // 次の位置に進む
                                cur_x += i;
                                cur_y += j;
                            }

                            // 最後の位置が自分の石である場合、石を置ける場所とする
                            if (cur_x >= 0 && cur_x < width && cur_y >= 0 && cur_y < height && board[cur_x, cur_y] == color) {
                                can_place = true;
                            }
                        }
                    }

                    // 石を置ける場合、座標をリストに追加
                    if (can_place) {
                        put_list.Add(new int[] {x, y});
                    }
                }
            }

            // 置ける場所を返す
            return put_list;
        }

        //どちらかが勝利しているかを判定する。
        public int judge_winner(){
            // 判定用の石の数を初期化
            int whiteCount = 0;
            int blackCount = 0;

            //石の数を比較する。
            for (int i = 0; i < width; i++){
                for (int j = 0; j < height; j++){
                    if (board[i, j] == 1) whiteCount++;
                    else if (board[i, j] == 2) blackCount++;
                }
            }

            // 盤面上における白または黒の石が置ける場所があるかどうかをチェック
            bool whiteCanPut = can_put_list(1).Count > 0;
            bool blackCanPut = can_put_list(2).Count > 0;

            //プレイヤーの石が消失してる場合の判定チェック
            if (whiteCount == 0) return 2; // 白の石がすべて消失した場合、黒の勝ち
            else if (blackCount == 0) return 1; // 黒の石がすべて消失した場合、白の勝ち

            // どちらのプレイヤーも石を置けない場合、石の数を比較して勝敗を決定
            if (!whiteCanPut && !blackCanPut){
                if (whiteCount > blackCount) return 1; // 白の勝ち
                else if (whiteCount < blackCount) return 2; // 黒の勝ち
                else return 0; // 引き分け
            }

            // どちらかのプレイヤーが石を置ける場合、まだ勝敗が決まっていないことを示す
            return -1;
        }
    }

}
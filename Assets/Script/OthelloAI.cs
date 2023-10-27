using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Othello;
using System.Text;

namespace OthelloAI{

    /// <summary>
    /// 8*8の盤面データで機能する学習機能付きオセロのCPU
    /// </summary>
    class AISystem{
        private string path = @"./learnData/";

        private Random rand = new Random();

        //盤面を保存する変数0は何もおいてない状態1が白で2が黒
        public int[,] board_data = new int[8,8];

        //学習用盤面データ
        /*木構造で以下のように登録している
        前の盤面=>次の盤面=>置いた色(白|黒)=>何回この盤面になったか|勝利数

        メモ Dictionary<前盤面、新盤面辞書型> => Dictionary<新盤面,色事に分割したデータ> => int[,]{白{経過数,勝利数},黒{経過数,勝利数}}
        */
        private Dictionary<int[,] ,Dictionary<int[,],int[,]>> board_Dictionary = new Dictionary<int[,] ,Dictionary<int[,],int[,]>>();

        //自分の石の色
        int my_color = 0;

        //自分の石の情報をもらい取得する
        public AISystem(int color){
            my_color = color;
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
        }

        //ユーザーの入力によって学習データを生成する
        public void LearnFightData(LinkedList<int[,]> fight_data,int win_color,int top_color){

            //対戦の記録を連結リストによって処理する
            LinkedListNode<int[,]> node = fight_data.First;

            //選考の色を記録する
            int now_color = top_color;

            //最初から最後までの対戦を登録するため最後の一つを外す
            for(int i = 0;i<fight_data.Count-1;i++){

                //その盤面が登録済みでないなら記録用のデータを追加する
                if(!board_Dictionary.ContainsKey(node.Value)){
                    board_Dictionary.Add(node.Value,new Dictionary<int[,], int[,]>());
                }

                //次の盤面情報も確認する
                if(!board_Dictionary[node.Value].ContainsKey(node.Next.Value)){
                    board_Dictionary[node.Value].Add(node.Next.Value,new int[,]{{0,0},{0,0}});
                }

                //データを加算する
                board_Dictionary[node.Value][node.Next.Value][now_color-1,0] += 1;

                //勝利した色であるなら勝利結果を追加する
                if(win_color == now_color){
                    board_Dictionary[node.Value][node.Next.Value][now_color-1,1] += 1;
                }

                //手番を変更する
                if(now_color == 1){
                    now_color = 2;
                }else{
                    now_color = 1;
                }

                node = node.Next;
            }
        }

        //AIによる戦闘で学習させる. Othelloライブラリを使用します。
        public void LearnFightDataSelf(int most_Choice,int random_Choice){

            if(most_Choice > 0){
                int top_color = rand.Next(2)+1;
                for(int i = 0;i<most_Choice;i++){
                    var fight_data = StudyingFight(top_color,true);

                    LearnFightData(fight_data.fight_data,fight_data.win_color,top_color);

                    //手番を変更する
                    if(top_color == 1){
                        top_color = 2;
                    }else{
                        top_color = 1;
                    }
                }
            }

            if(random_Choice > 0){
                int top_color = rand.Next(2)+1;
                for(int i = 0;i<random_Choice;i++){
                    var fight_data = StudyingFight(top_color,false);

                    LearnFightData(fight_data.fight_data,fight_data.win_color,top_color);

                    //手番を変更する
                    if(top_color == 1){
                        top_color = 2;
                    }else{
                        top_color = 1;
                    }
                }
            }
        }

        //AI同士に戦闘をさせる関数
        private (LinkedList<int[,]> fight_data,int win_color) StudyingFight(int top_color, bool most_Choice){
            //ボードクラスのインスタンスの宣言
            Board bd = new Board();

            //AIクラスのインスタンス変数の宣言
            AISystem white = new AISystem(1);
            AISystem black = new AISystem(2);

            //戦闘データの記録
            LinkedList<int[,]> fight_data = new LinkedList<int[,]>();

            bd.Init_Board();
            fight_data.AddLast(bd.board);

            while(bd.judge_winner() == -1){
                if(top_color == 1){
                    white.SetBoard(bd.board);
                    //ポジションを決定させる
                    int[] white_pos;
                    if(most_Choice){
                        white_pos = white.most_flip_put_stone();
                    }else{
                        white_pos = white.non_consider_put_stone();
                    }
                    //エラー数値が返ってきていない場合は設置する。
                    if(white_pos[0] != -1&&white_pos[1] != -1){
                        bd.put_stone(white_pos[0],white_pos[1],white.getMyColor());
                    }
                    fight_data.AddLast(bd.board);

                    black.SetBoard(bd.board);
                    //ポジションを決定させる
                    int[] black_pos;
                    if(most_Choice){
                        black_pos = white.most_flip_put_stone();
                    }else{
                        black_pos = white.non_consider_put_stone();
                    }
                    //エラー数値が返ってきていない場合は設置する。
                    if(black_pos[0] != -1&&black_pos[1] != -1){
                        bd.put_stone(black_pos[0],black_pos[1],black.getMyColor());
                    }
                    fight_data.AddLast(bd.board);
                }else{
                    black.SetBoard(bd.board);
                    //ポジションを決定させる
                    int[] black_pos;
                    if(most_Choice){
                        black_pos = white.most_flip_put_stone();
                    }else{
                        black_pos = white.non_consider_put_stone();
                    }
                    //エラー数値が返ってきていない場合は設置する。
                    if(black_pos[0] != -1&&black_pos[1] != -1){
                        bd.put_stone(black_pos[0],black_pos[1],black.getMyColor());
                    }
                    fight_data.AddLast(bd.board);

                    white.SetBoard(bd.board);
                    //ポジションを決定させる
                    int[] white_pos;
                    if(most_Choice){
                        white_pos = white.most_flip_put_stone();
                    }else{
                        white_pos = white.non_consider_put_stone();
                    }
                    //エラー数値が返ってきていない場合は設置する。
                    if(white_pos[0] != -1&&white_pos[1] != -1){
                        bd.put_stone(white_pos[0],white_pos[1],white.getMyColor());
                    }
                    fight_data.AddLast(bd.board);
                }
            }

            return (fight_data,bd.judge_winner());
        }

        //盤面情報をセットする
        public void SetBoard(int[,] board_data){
            this.board_data = board_data;
        }

        //自分の色を返す
        public int getMyColor(){
            return my_color;
        }

        //盤面のデータからランダムにおける位置に置く
        public int[] non_consider_put_stone(){
            //置ける場所を取得する
            List<int[]> choice_list = can_put_list();

            //おける場所がない場合、存在しない座標を返す
            if(choice_list.Count == 0){
                return new int[]{-1,-1};
            }

            //ランダムにおける場所を返す
            return choice_list[rand.Next(choice_list.Count)];
        }

        //最も置ける場所に石を置く
        public int[] most_flip_put_stone(){
            int maxFlips = -1;
            int[] maxCoords = new int[] { -1, -1 };

            // 置ける場所を取得する
            List<int[]> choiceList = can_put_list();

            // おける場所がない場合、存在しない座標を返す
            if (choiceList.Count == 0){
                return new int[] { -1, -1 };
            }

            // 最も多くの石をひっくり返せる場所を探す.
            foreach (int[] coords in choiceList){
                int flips = CountFlipStones(coords[0], coords[1], my_color);
                if (flips > maxFlips){
                    maxFlips = flips;
                    maxCoords = coords;
                }else if(flips == maxFlips){
                    //返せる枚数が同じ場合はランダムで返す
                    if(rand.Next(1,2) == 1){
                        maxFlips = flips;
                        maxCoords = coords;
                    }
                }
            }
            return maxCoords;
        }

        //学習し保存されているデータを用いて置ける位置を算出する
        public int[] consider_flip_put_stone(){
            
            string key = board_to_learnFileName(board_data);
            int maxFlips_index = 0;
            int counter = 0;

            //学習データが存在するか確認する
            if(File.Exists(path+key)){
                StreamReader sr = new StreamReader(path+key);
                String[] data = sr.ReadToEnd().Split(',');

                //最も勝率の高い返す場所を探す
                for(int i = 0;i<data.Length;i+=5){
                    if(my_color == 1){
                        if(counter < int.Parse(data[i+2])/int.Parse(data[i+1])){
                            counter = int.Parse(data[i+2])/int.Parse(data[i+1]);
                            maxFlips_index = i;
                        }
                    }else if(my_color == 2){
                        if(counter < int.Parse(data[i+4])/int.Parse(data[i+3])){
                            counter = int.Parse(data[i+4])/int.Parse(data[i+3]);
                            maxFlips_index = i;
                        }
                    }
                }

                //差分点の座標を取得して位置に変更し返す。
                int[] pos = new int[2];
                for(int i = 0;i<key.Length;i++){
                    if(!key[i].Equals(data[maxFlips_index][i])){
                        pos = new int[]{ i/8 , i%8 };
                    }
                }
                return pos;
            }else{
                //学習データがない場合は最も多く返せる場所を返す
                return most_flip_put_stone();
            }
        }

        //学習データを保存する。重複するデータがある際は上書きする。
        public void LearnDateOverWrite(){
            StreamWriter sw;

            //変数の各要素にアクセス
            foreach (var key in board_Dictionary.Keys.ToArray()){
                if (!File.Exists(path+board_to_learnFileName(key))){
                    //ファイルが存在していない場合は盤面データを保存する用のファイルを作成
                    File.CreateText(path+board_to_learnFileName(key));
                }

                //書き込み先を選択
                sw = new StreamWriter(path+board_to_learnFileName(key),false);

                //書き込み先に要素を1つづつ書き込んでいく。
                foreach(var new_board in board_Dictionary[key].Keys.ToArray()){
                    var data = board_Dictionary[key][new_board];
                    sw.Write(board_to_learnFileName(new_board)+",");
                    sw.Write($"{data[0,0]},{data[0,1]},{data[1,0]},{data[1,1]},");
                }
            }
        }

        //現在の学習データをファイルに加算で保存します。
        public void LearnDataAdd(){
            StreamWriter sw;
            StreamReader sr;

            //変数の各要素にアクセス
            foreach (var key in board_Dictionary.Keys.ToArray()){
                if (!File.Exists(path+board_to_learnFileName(key))){
                    
                    //ファイルが存在していない場合は盤面データを保存する用のファイルを作成
                    sw = File.CreateText(path+board_to_learnFileName(key));

                    //書き込み先に要素を1つづつ書き込んでいく。
                    foreach(var new_board in board_Dictionary[key].Keys.ToArray()){
                        var data = board_Dictionary[key][new_board];
                        sw.Write(board_to_learnFileName(new_board)+",");
                        sw.Write($"{data[0,0]},{data[0,1]},{data[1,0]},{data[1,1]},");
                    }
                }else{
                    //書き込み先が存在しているため書き込み先を予め読み込む
                    sr = new StreamReader(path+board_to_learnFileName(key));
                    List<String> dates = sr.ReadToEnd().Split(',').ToList();
                    sr.Close();

                    //ファイルを読み込む
                    sw = new StreamWriter(path+board_to_learnFileName(key),false);

                    //書き込み先に盤面データが存在しているかを確認するフラグ
                    bool non_equal = true;

                    //その後の盤面データをすべて読み込んでいく。
                    foreach(var new_board in board_Dictionary[key].Keys.ToArray()){
                        non_equal = true;
                        var data = board_Dictionary[key][new_board];
                        //元々ある盤面データをすべて読み込む
                        for(int i = 0;i<dates.Count;i+=5){
                            //一致する盤面がある場合は数値を加算する。
                            if(board_to_learnFileName(new_board).Equals(dates[i])){
                                dates[i+1] = (int.Parse(dates[i+1])+data[0,0]).ToString();
                                dates[i+2] = (int.Parse(dates[i+2])+data[0,1]).ToString();
                                dates[i+3] = (int.Parse(dates[i+3])+data[1,0]).ToString();
                                dates[i+4] = (int.Parse(dates[i+4])+data[1,1]).ToString();
                                non_equal = false;
                            }
                        }
                        //存在しなかった場合、データを追加していく。
                        if(non_equal){
                            dates.Add(board_to_learnFileName(new_board));
                            dates.Add(data[0,0].ToString());
                            dates.Add(data[0,1].ToString());
                            dates.Add(data[1,0].ToString());
                            dates.Add(data[1,1].ToString());
                        }
                    }
                    //最後にすべてファイルに書き込む
                    foreach(var d in dates){
                        if(d != ""){
                            sw.Write($"{d},");
                        }
                    }
                }
                sw.Close();
            }
        }

        //学習データの初期化
        public void ResetLearnData(){
            board_Dictionary.Clear();
        }

        //石が置ける位置をList<int[]>で返す関数
        private List<int[]> can_put_list() {
            // 置ける位置を格納するリスト
            List<int[]> put_list = new List<int[]>();

            // 盤面を走査して、指定された色の石が置ける場所を探す
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    // 既に石が置かれている場合はスキップ
                    if (board_data[x, y] != 0) {
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

                            // 次の位置が盤面内かつ相手の石である限り石を探し続ける
                            while (cur_x >= 0 && cur_x < 8 && cur_y >= 0 && cur_y < 8 && board_data[cur_x, cur_y] != 0 && board_data[cur_x, cur_y] != my_color) {
                                // 次の位置に進む
                                cur_x += i;
                                cur_y += j;
                            }

                            // 最後の位置が自分の石である場合、石を置ける場所とする
                            if (cur_x >= 0 && cur_x < 8 && cur_y >= 0 && cur_y < 8 && board_data[cur_x, cur_y] == my_color) {
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

        //ひっくり返せる石の数を数える
        private int CountFlipStones(int x, int y, int color){
            //座標とかの入力値のチェック
            if (x < 0 || x >= 8 || y < 0 || y >= 8 || (color != 1 && color != 2)){
                return -1;
            }

            //石がすでに配置されていないかをチェックする
            if (board_data[x,y] != 0){
                return -1;
            }

            //カウント変数
            int count = 0;

            //走査して石を数える
            for (int dx = -1; dx <= 1; dx++){
                for (int dy = -1; dy <= 1; dy++){

                    //中心は無視
                    if (dx == 0 && dy == 0){
                        continue;
                    }

                    //探索先の設定
                    int nx = x + dx;
                    int ny = y + dy;
                    int flipCount = 0;

                    //石を置ける限り走査を続ける
                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && board_data[nx,ny] != 0 && board_data[nx,ny] != color){
                        flipCount++;
                        nx += dx;
                        ny += dy;
                    }

                    //カウントがある場合は代入する
                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && board_data[nx,ny] == color && flipCount > 0){
                        count += flipCount;
                    }
                }
            }
            return count;
        }

        //現在の盤面データをファイル名用に変換する
        private String now_board_to_learnFileName(){
            String name = "";
            //盤面情報をString型に変換する
            for(int x = 0;x<8;x++){
                for(int y = 0;y<8;y ++){
                    name += Convert.ToString(board_data[x,y]);
                }
            }
            return name;
        }

        //盤面データをファイル名用に変換する
        private String board_to_learnFileName(int[,] board){
            String name = "";
            //盤面情報をString型に変換する
            for(int x = 0;x<8;x++){
                for(int y = 0;y<8;y ++){
                    name += Convert.ToString(board[x,y]);
                }
            }
            return name;
        }

        //ファイル名用のデータを盤面データに変換する
        private int[,] LearnFileName_to_board(String name){
            int [,] board_Data = new int[8,8];
            //盤面情報をint[,]型に変換する
            for(int x = 0;x<8;x++){
                for(int y = 0;y<8;y ++){
                    board_Data[x,y] = Convert.ToInt32(name[x+y]);
                }
            }
            return board_Data;
        }
    }
}
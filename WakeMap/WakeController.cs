using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WakeMap
{
    public class WakeController
    {
        //他クラス参照用 (初期化は生成元で行う)
        public UserControlMap refUserControlMap;
        public CsToJs refCsToJs;

        //シーン
        enum Scene {
            SceneA, SceneB, SceneC
        };
        private Scene g_scene;

        //ディクショナリー
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictAWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictDTrack;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictBWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictCPlace;

        //ディクショナリー(選択用)
        private Dictionary<string, Dictionary<string, string>> g_dictSelectAWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictSelectDTrack;
        private Dictionary<string, Dictionary<string, string>> g_dictSelectBWake;
        private Dictionary<string, Dictionary<string, string>> g_dictSelectCPlace;

        //航跡のコンフィグ
        public struct WakeCongfig {
            public string layername; //レイヤ名
            public bool isPoint; //ポイント描画の有無
            public System.Drawing.Brush pointColor; //ポイント色
            public float pointSize; //ポイントサイズ
            public bool isLine; //ライン描画の有無
            public System.Drawing.Color lineColor; //ライン色
            public float lineWidth; //ラインの太さ
            public bool isLineDash; //ラインを破線にするかどうか
            public bool isLineArrow; //ラインを破線にするかどうか
            public bool isLabel; //ラベル描画の有無
            public System.Drawing.Color labelBackColor; //ラベル背景色
            public System.Drawing.Color labelForeColor; //ラベル背景色

        }
        private WakeCongfig g_cfgAWake = new WakeCongfig();
        private WakeCongfig g_cfgDTrack = new WakeCongfig();
        private WakeCongfig g_cfgBWake = new WakeCongfig();
        private WakeCongfig g_cfgCPlace = new WakeCongfig();
        //選択用
        private WakeCongfig g_cfgSelectAWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectDTrack = new WakeCongfig();
        private WakeCongfig g_cfgSelectBWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectCPlace = new WakeCongfig();

        //ラベルリスト
        public struct WakeLabel
        {
            public Label label;
            public Coordinate worldPos;
        }
        private List<WakeLabel> g_labelListAWake;
        private List<WakeLabel> g_labelListDTrack;
        private List<WakeLabel> g_labelListBWake;
        private List<WakeLabel> g_labelListDummy;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="strDictAWake"></param>
        /// <param name="strDictDTrack"></param>
        /// <param name="strDictBWake"></param>
        /// <param name="strDictCPlace"></param>
        public void InitWake(
            string scene,
            string strDictAWake,
            string strDictDTrack,
            string strDictBWake,
            string strDictCPlace
            )
        {
            //Console.WriteLine(
            //    $"scene = {scene}\n" +
            //    $"strDictAWake = {strDictAWake}\n" +
            //    $"strDictBWake = {strDictBWake}\n" +
            //    $"strDictCPlace = {strDictCPlace}\n" +
            //    $"strDictDTrack = {strDictDTrack}\n" +
            //    $"strDictEArrow = {strDictEArrow}"
            //    );

            //mapBoxの初期化
            refUserControlMap.InitLayerOtherThanBase();

            //航跡ラベルをクリア
            WakeLabelClear();

            //コンフィグを初期化
            InitWakeConfig();

            //シーン毎に生成するリストを切り替え・辞書を生成
            switch (scene)
            {
                case "SceneA":
                    //シーンを登録
                    g_scene = Scene.SceneA;
                    //辞書を生成
                    GenerateWakeDictionary(ref g_dictAWake, strDictAWake);
                    break;
                case "SceneB":
                    //シーンを登録
                    g_scene = Scene.SceneB;
                    //辞書を生成
                    GenerateWakeDictionary(ref g_dictAWake, strDictAWake);
                    GenerateWakeDictionary(ref g_dictDTrack, strDictDTrack);
                    GenerateWakeDictionary(ref g_dictBWake, strDictBWake);
                    GenerateWakeDictionary(ref g_dictCPlace, strDictCPlace);
                    break;
                case "SceneC":
                    //シーンを登録
                    g_scene = Scene.SceneC;
                    //辞書を生成
                    GenerateWakeDictionary(ref g_dictAWake, strDictAWake);
                    GenerateWakeDictionary(ref g_dictDTrack, strDictDTrack);
                    GenerateWakeDictionary(ref g_dictBWake, strDictBWake);
                    GenerateWakeDictionary(ref g_dictCPlace, strDictCPlace);
                    break;
                default:
                    break;
            }

            //描画
            switch (g_scene)
            {
                case Scene.SceneA:
                    //コンフィグ変更
                    g_cfgAWake.lineColor = System.Drawing.Color.Red;
                    g_cfgSelectAWake.pointColor = System.Drawing.Brushes.Orange;

                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);

                    break;
                case Scene.SceneB:
                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);
                    DrawLineWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack);
                    DrawLineWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake);
                    DrawLineWake(ref g_dictCPlace, ref g_labelListDummy, ref g_cfgCPlace);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);

                    break;
                case Scene.SceneC:
                    //コンフィグ変更
                    g_cfgCPlace.isLine = true;
                    g_cfgSelectCPlace.isLine = true;
                    //連動選択は点とする
                    g_cfgSelectAWake.isPoint = true;
                    g_cfgSelectAWake.isLine = false;
                    g_cfgSelectDTrack.isPoint = true;
                    g_cfgSelectDTrack.isLine = false;
                    g_cfgSelectBWake.isPoint = true;
                    g_cfgSelectBWake.isLine = false;

                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);
                    DrawLineWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack);
                    DrawLineWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake);
                    DrawArrow(ref g_dictCPlace, ref g_cfgCPlace);

                    //空(から)の選択用レイヤを生成
                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);

                    break;
                default:
                    break;
            }

        }

        //航跡ラベルをクリア
        private void WakeLabelClear()
        {
            //既存ラベルの破棄
            // パネルのControlsコレクション内のラベルを全て削除する
            if (g_labelListAWake != null)
            {
                foreach (WakeLabel wakeLabel in g_labelListAWake)
                {
                    if (wakeLabel.label != null)
                    {
                        refUserControlMap.mapBox.Controls.Remove(wakeLabel.label);
                        wakeLabel.label.Dispose(); // メモリ解放のためにDispose()メソッドを呼び出す
                    }
                }
            }
            g_labelListAWake = new List<WakeLabel>();

            if (g_labelListBWake != null)
            {
                foreach (WakeLabel wakeLabel in g_labelListBWake)
                {
                    if (wakeLabel.label != null)
                    {
                        refUserControlMap.mapBox.Controls.Remove(wakeLabel.label);
                        wakeLabel.label.Dispose(); // メモリ解放のためにDispose()メソッドを呼び出す
                    }
                }
            }
            g_labelListBWake = new List<WakeLabel>();

            if (g_labelListDTrack != null)
            {
                foreach (WakeLabel wakeLabel in g_labelListDTrack)
                {
                    if (wakeLabel.label != null)
                    {
                        refUserControlMap.mapBox.Controls.Remove(wakeLabel.label);
                        wakeLabel.label.Dispose(); // メモリ解放のためにDispose()メソッドを呼び出す
                    }
                }
            }
            g_labelListDTrack = new List<WakeLabel>();

            g_labelListDummy = new List<WakeLabel>();
        }

        //コンフィグを初期化
        private void InitWakeConfig()
        {
            //=====未選択用=====

            g_cfgAWake.layername = "layAWake";
            g_cfgAWake.isPoint = false;
            g_cfgAWake.pointColor = System.Drawing.Brushes.White;
            g_cfgAWake.pointSize = 0;
            g_cfgAWake.isLine = true;
            g_cfgAWake.lineColor = System.Drawing.Color.LightPink;
            g_cfgAWake.lineWidth = 1;
            g_cfgAWake.isLineDash = false;
            g_cfgAWake.isLineArrow = true;
            g_cfgAWake.isLabel = true;
            g_cfgAWake.labelBackColor = System.Drawing.Color.LightPink;
            g_cfgAWake.labelForeColor = System.Drawing.Color.Black;

            g_cfgDTrack.layername = "layDTrack";
            g_cfgDTrack.isPoint = false;
            g_cfgDTrack.pointColor = System.Drawing.Brushes.White;
            g_cfgDTrack.pointSize = 0;
            g_cfgDTrack.isLine = true;
            g_cfgDTrack.lineColor = System.Drawing.Color.MediumSeaGreen;
            g_cfgDTrack.lineWidth = 1;
            g_cfgDTrack.isLineDash = false;
            g_cfgDTrack.isLineArrow = true;
            g_cfgDTrack.isLabel = true;
            g_cfgDTrack.labelBackColor = System.Drawing.Color.MediumSeaGreen;
            g_cfgDTrack.labelForeColor = System.Drawing.Color.Black;

            g_cfgBWake.layername = "layBWake";
            g_cfgBWake.isPoint = false;
            g_cfgBWake.pointColor = System.Drawing.Brushes.White;
            g_cfgBWake.pointSize = 0;
            g_cfgBWake.isLine = true;
            g_cfgBWake.lineColor = System.Drawing.Color.MediumPurple;
            g_cfgBWake.lineWidth = 1;
            g_cfgBWake.isLineDash = false;
            g_cfgBWake.isLineArrow = true;
            g_cfgBWake.isLabel = true;
            g_cfgBWake.labelBackColor = System.Drawing.Color.MediumPurple;
            g_cfgBWake.labelForeColor = System.Drawing.Color.Black;

            g_cfgCPlace.layername = "layCPlace";
            g_cfgCPlace.isPoint = true;
            g_cfgCPlace.pointColor = System.Drawing.Brushes.CornflowerBlue;
            g_cfgCPlace.pointSize = 5;
            g_cfgCPlace.isLine = false;
            g_cfgCPlace.lineColor = System.Drawing.Color.CornflowerBlue;
            g_cfgCPlace.lineWidth = 1;
            g_cfgCPlace.isLineDash = false;
            g_cfgCPlace.isLineArrow = true;
            g_cfgCPlace.isLabel = false;
            g_cfgCPlace.labelBackColor = System.Drawing.Color.Empty;
            g_cfgCPlace.labelForeColor = System.Drawing.Color.Empty;

            //=====選択用=====

            g_cfgSelectAWake.layername = "laySelectAWake";
            g_cfgSelectAWake.isPoint = false;
            g_cfgSelectAWake.pointColor = System.Drawing.Brushes.Red;
            g_cfgSelectAWake.pointSize = 5;
            g_cfgSelectAWake.isLine = true;
            g_cfgSelectAWake.lineColor = System.Drawing.Color.Red;
            g_cfgSelectAWake.lineWidth = 2;
            g_cfgSelectAWake.isLineDash = false;
            g_cfgSelectAWake.isLineArrow = true;
            g_cfgSelectAWake.isLabel = false;
            g_cfgSelectAWake.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectAWake.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectDTrack.layername = "laySelectDTrack";
            g_cfgSelectDTrack.isPoint = false;
            g_cfgSelectDTrack.pointColor = System.Drawing.Brushes.Green;
            g_cfgSelectDTrack.pointSize = 5;
            g_cfgSelectDTrack.isLine = true;
            g_cfgSelectDTrack.lineColor = System.Drawing.Color.Green;
            g_cfgSelectDTrack.lineWidth = 2;
            g_cfgSelectDTrack.isLineDash = false;
            g_cfgSelectDTrack.isLineArrow = false;
            g_cfgSelectDTrack.isLabel = false;
            g_cfgSelectDTrack.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectDTrack.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectBWake.layername = "laySelectBWake";
            g_cfgSelectBWake.isPoint = false;
            g_cfgSelectBWake.pointColor = System.Drawing.Brushes.DarkViolet;
            g_cfgSelectBWake.pointSize = 5;
            g_cfgSelectBWake.isLine = true;
            g_cfgSelectBWake.lineColor = System.Drawing.Color.DarkViolet;
            g_cfgSelectBWake.lineWidth = 2;
            g_cfgSelectBWake.isLineDash = false;
            g_cfgSelectBWake.isLineArrow = true;
            g_cfgSelectBWake.isLabel = false;
            g_cfgSelectBWake.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectBWake.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectCPlace.layername = "laySelectCPlace";
            g_cfgSelectCPlace.isPoint = true;
            g_cfgSelectCPlace.pointColor = System.Drawing.Brushes.Blue;
            g_cfgSelectCPlace.pointSize = 5;
            g_cfgSelectCPlace.isLine = false;
            g_cfgSelectCPlace.lineColor = System.Drawing.Color.Blue;
            g_cfgSelectCPlace.lineWidth = 2;
            g_cfgSelectCPlace.isLineDash = false;
            g_cfgSelectCPlace.isLineArrow = true;
            g_cfgSelectCPlace.isLabel = false;
            g_cfgSelectCPlace.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectCPlace.labelForeColor = System.Drawing.Color.Empty;
        }

        //辞書を生成する
        private void GenerateWakeDictionary(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            string strDictWake
        ) {
            refDictWake = new JsonParser().ParseDictSDictSDictSS(strDictWake);

        }

        //描画する
        private void DrawLineWake(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref List<WakeLabel> refWakeLabelList,
            ref WakeCongfig refWakeCongfig
            )
        {
            string layername = refWakeCongfig.layername;

            //レイヤを生成
            refUserControlMap.GenerateLayer(layername);

            if (refDictWake == null)
            {
                return;
            }

            //点を追加
            if (refWakeCongfig.isPoint)
            {
                //wakeを取得
                foreach (var wake in refDictWake)
                {
                    //座標を取得
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            refUserControlMap.AddPointToLayer(layername, coordinate);
                            refUserControlMap.SetStylePointToLayer(layername, refWakeCongfig.pointColor, refWakeCongfig.pointSize);
                        }
                    }
                }
            }

            //線を追加
            if (refWakeCongfig.isLine)
            {
                //wakeを取得
                foreach (var wake in refDictWake)
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    //レイヤーにラインを追加
                    refUserControlMap.AddLineToLayer(layername, coordinates);
                    //スタイルを設定
                    refUserControlMap.SetStyleLineToLayer(layername, refWakeCongfig.lineColor, refWakeCongfig.lineWidth);
                    //破線を設定
                    if (refWakeCongfig.isLineDash)
                    {
                        refUserControlMap.SetLineDash(layername);
                    }
                    //矢印を設定
                    if (refWakeCongfig.isLineArrow)
                    {
                        refUserControlMap.SetLineArrow(layername);
                    }
                }
            }

            //ラベルを描画
            if (refWakeCongfig.isLabel)
            {
                //wakeを取得
                foreach (var wake in refDictWake)
                {
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key == "pos1")
                        {
                            Coordinate wpos = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            //ラベル生成
                            GenerateLabel(ref refWakeLabelList,
                                wpos,
                                Regex.Replace(wake.Key, @"[^0-9]", ""),
                                refWakeCongfig.labelBackColor,
                                refWakeCongfig.labelForeColor);
                            break;
                        }
                    }
                }
            }

            refUserControlMap.MapBoxRefresh();
        }

        // Arrow用描画処理
        private void DrawArrow(
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refWakeCongfig
            )
        {
            string layername = refWakeCongfig.layername;

            //矢印の描画
            //レイヤを生成
            refUserControlMap.GenerateLayer(layername);

            if (refDictWake == null)
            {
                return;
            }

            //線を追加
            if (refWakeCongfig.isLine)
            {
                //wakeを取得
                foreach (var wake in refDictWake)
                {
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            //開始点の取得
                            Coordinate start = new Coordinate(
                                double.Parse(pos.Value["x"]),
                                double.Parse(pos.Value["y"]));
                            //方位の取得
                            float direction = float.Parse(pos.Value["direction"]);
                            //距離の取得
                            float distance = float.Parse(pos.Value["distance"]);
                            //終点の算出
                            double radian = direction * Math.PI / 180.0;
                            double xStart = start.X;
                            double yStart = start.Y;
                            double x = xStart + distance * Math.Cos(radian);
                            double y = yStart + distance * Math.Sin(radian);
                            Coordinate end = new Coordinate(x, y);
                            //配列を作成
                            Coordinate[] coordinates = new Coordinate[2] { start, end };
                            //レイヤーにポイントを追加
                            refUserControlMap.AddPointToLayer(layername, start);
                            //レイヤーにラインを追加
                            refUserControlMap.AddLineToLayer(layername, coordinates);
                        }
                    }
                    //スタイルを設定
                    refUserControlMap.SetStylePointToLayer(layername, refWakeCongfig.pointColor, refWakeCongfig.pointSize);
                    refUserControlMap.SetStyleLineToLayer(layername, refWakeCongfig.lineColor, refWakeCongfig.lineWidth);
                    //破線を設定
                    if (refWakeCongfig.isLineDash)
                    {
                        refUserControlMap.SetLineDash(layername);
                    }
                    //矢印を設定
                    if (refWakeCongfig.isLineArrow)
                    {
                        refUserControlMap.SetLineArrow(layername);
                    }
                }
            }

            refUserControlMap.MapBoxRefresh();
        }

        //ラベル生成
        private void GenerateLabel(
            ref List<WakeLabel> refListWakeLabel,
            Coordinate worldPos,
            string text,
            System.Drawing.Color BackColor,
            System.Drawing.Color ForeColor
            )
        {
            //新しいラベルを生成 
            Label label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Location = refUserControlMap.TransPosWorldToImage(worldPos);
            label.BackColor = BackColor;
            label.ForeColor = ForeColor;
            //コントロールに追加
            refUserControlMap.mapBox.Controls.Add(label);
            //ラベルと座標の組み合わせをラベルリストに追加
            WakeLabel wakeLabel = new WakeLabel();
            wakeLabel.label = label;
            wakeLabel.worldPos = worldPos;
            refListWakeLabel.Add(wakeLabel);
        }

        // ラベルをmapboxに合わせて再配置
        public void RelocateLabel()
        {
            //Console.WriteLine("RelocateLabel");
            foreach (WakeLabel wakeLabel in g_labelListAWake)
            {
                wakeLabel.label.Location = refUserControlMap.TransPosWorldToImage(wakeLabel.worldPos);
            }
            foreach (WakeLabel wakeLabel in g_labelListBWake)
            {
                wakeLabel.label.Location = refUserControlMap.TransPosWorldToImage(wakeLabel.worldPos);
            }
            foreach (WakeLabel wakeLabel in g_labelListDTrack)
            {
                wakeLabel.label.Location = refUserControlMap.TransPosWorldToImage(wakeLabel.worldPos);
            }
        }

        //==============================================

        //選択用レイヤー生成
        private void GenerateSelectLayer(ref WakeCongfig refWakeCongfig)
        {
            //レイヤ生成
            refUserControlMap.GenerateLayer(refWakeCongfig.layername);
            if (refWakeCongfig.isPoint)
            {
                //点のスタイルを設定
                refUserControlMap.SetStylePointToLayer(refWakeCongfig.layername, refWakeCongfig.pointColor, refWakeCongfig.pointSize);
            }
            if (refWakeCongfig.isLine)
            {
                //線のスタイルを設定
                refUserControlMap.SetStyleLineToLayer(refWakeCongfig.layername, refWakeCongfig.lineColor, refWakeCongfig.lineWidth);
                //破線を設定
                if (refWakeCongfig.isLineDash)
                {
                    refUserControlMap.SetLineDash(refWakeCongfig.layername);
                }
                //矢印を設定
                if (refWakeCongfig.isLineArrow)
                {
                    refUserControlMap.SetLineArrow(refWakeCongfig.layername);
                }
            }
        }

        //
        public void mapBox_ClickSelect(System.Drawing.Point clickPos)
        {
            bool isHit = false;

            //選択処理
            switch (g_scene)
            {
                case Scene.SceneA:
                    isHit = SelectLineWake(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, clickPos);
                    break;

                case Scene.SceneB:
                    isHit = SelectLineWake(ref g_dictSelectBWake, ref g_dictBWake, ref g_cfgSelectBWake, clickPos);
                    if (isHit) {
                        //行と時刻範囲を取り出す
                        int row = -1;
                        string startTime = null;
                        string endTime = null;
                        int i = 0;
                        foreach (var key in g_dictSelectBWake)
                        {
                            if (key.Key.Contains("info")) //Keyが"info"を含む
                            {
                                row = int.Parse(key.Value["row"]);
                            }
                            if (key.Key.Contains("pos")) //Keyが"info"を含む
                            {
                                if (i == 1)
                                {
                                    startTime = key.Value["time"];
                                }
                                if (i == (g_dictSelectBWake.Count - 1))
                                {
                                    endTime = key.Value["time"];
                                }
                            }
                            i++;
                        }
                        //連動でCPlaceを選択
                        RowLinkSelect(ref g_dictSelectCPlace, ref g_dictCPlace, ref g_cfgSelectCPlace, row);
                        //連動でAWakeを選択
                        TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                        //連動でDTrackを選択
                        TimeLinkSelectD(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);
                        break;
                    }

                    isHit = SelectPointWake(ref g_dictSelectCPlace, ref g_dictCPlace, ref g_cfgSelectCPlace, clickPos);
                    if (isHit)
                    {
                        //行と時刻範囲を取り出す
                        int row = -1;
                        string startTime = null;
                        string endTime = null;
                        int i = 0;
                        foreach (var key in g_dictSelectCPlace)
                        {
                            if (key.Key.Contains("info")) //Keyが"info"を含む
                            {
                                row = int.Parse(key.Value["row"]);
                            }
                            if (key.Key.Contains("pos")) //Keyが"info"を含む
                            {
                                if (i == 1)
                                {
                                    startTime = key.Value["time"];
                                }
                                if (i == (g_dictSelectCPlace.Count - 1))
                                {
                                    endTime = key.Value["time"];
                                }
                            }
                            i++;
                        }
                        //連動でBWakeを選択
                        RowLinkSelect(ref g_dictSelectBWake, ref g_dictBWake, ref g_cfgSelectBWake, row);
                        //連動でAWakeを選択
                        TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                        //連動でDTrackを選択
                        TimeLinkSelectD(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);
                        break;
                    }

                    break;

                case Scene.SceneC:
                    isHit = SelectArrow(ref g_dictSelectCPlace, ref g_dictCPlace, ref g_cfgSelectCPlace, clickPos);
                    if (isHit)
                    {
                        //行と時刻範囲を取り出す
                        int row = -1;
                        string startTime = null;
                        string endTime = null;
                        int i = 0;
                        foreach (var key in g_dictSelectCPlace)
                        {
                            if (key.Key.Contains("info")) //Keyが"info"を含む
                            {
                                row = int.Parse(key.Value["row"]);
                            }
                            if (key.Key.Contains("pos")) //Keyが"info"を含む
                            {
                                if (i == 1)
                                {
                                    startTime = key.Value["time"];
                                }
                                if (i == (g_dictSelectCPlace.Count - 1))
                                {
                                    endTime = key.Value["time"];
                                }
                            }
                            i++;
                        }
                        //連動でAWakeを選択
                        TimeLinkSelect(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, startTime, endTime);
                        //連動でDTrackを選択
                        TimeLinkSelectD(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, startTime, endTime);
                        //連動でBWakeを選択
                        TimeLinkSelect(ref g_dictSelectBWake, ref g_dictBWake, ref g_cfgSelectBWake, startTime, endTime);
                        break;
                    }
                    break;

                default:
                    break;
            }
        }

        //航跡を選択する（線）
        private bool SelectLineWake(
            ref Dictionary<string, Dictionary<string, string>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refSelectWakeCongfig,
            System.Drawing.Point clickPos)
        {
            bool isHit = false;

            //===== 線と点の当たり判定 =====
            {
                foreach (var wake in refDictWake)
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //線の数ループ
                    for (int i = 1; i < listCoordinate.Count; i++)
                    {
                        //線と点の距離を計算
                        System.Drawing.Point start = refUserControlMap.TransPosWorldToImage(listCoordinate[i - 1]);
                        System.Drawing.Point end = refUserControlMap.TransPosWorldToImage(listCoordinate[i]);
                        int distance = DistancePointToLine(start, end, clickPos);
                        //衝突判定
                        if (distance < 5)
                        {
                            //選択用ディクショナリーに代入
                            refDictSelectWake = wake.Value;
                            isHit = true;
                            break;
                        }
                    }
                    if (isHit) { break; }
                }
            }

            //===== 線の描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                    //空のジオメトリ生成
                    Collection<IGeometry> igeoms = new Collection<IGeometry>();
                    //図形生成クラス
                    GeometryFactory gf = new GeometryFactory();
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in refDictSelectWake)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    //線をジオメトリに追加
                    igeoms.Add(gf.CreateLineString(coordinates));
                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(igeoms);
                    layer.DataSource = gpro;
                    //レイヤのインデックスを取得
                    int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                    //レイヤを更新
                    refUserControlMap.mapBox.Map.Layers[index] = layer;
                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();
                }
            }
            return isHit;
        }

        //航跡を選択する（点）
        private bool SelectPointWake(
            ref Dictionary<string, Dictionary<string, string>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refSelectWakeCongfig,
            System.Drawing.Point clickPos)
        {
            bool isHit = false;

            //===== 線と点の当たり判定 =====
            {
                foreach (var wake in refDictWake)
                {
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            //点と点の距離を計算
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            System.Drawing.Point point = refUserControlMap.TransPosWorldToImage(coordinate);
                            int distance = DistancePointToPoint(point, clickPos);
                            //衝突判定
                            if (distance < 5)
                            {
                                //選択用ディクショナリーに代入
                                refDictSelectWake = wake.Value;
                                isHit = true;
                                break;
                            }

                        }
                    }
                    if (isHit) { break; }
                }
            }

            //===== 点の描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                    //空のジオメトリ生成
                    Collection<IGeometry> igeoms = new Collection<IGeometry>();
                    //図形生成クラス
                    GeometryFactory gf = new GeometryFactory();
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in refDictSelectWake)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    //線をジオメトリに追加
                    igeoms.Add(gf.CreateMultiPointFromCoords(coordinates));
                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(igeoms);
                    layer.DataSource = gpro;
                    //レイヤのインデックスを取得
                    int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                    //レイヤを更新
                    refUserControlMap.mapBox.Map.Layers[index] = layer;
                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();
                }
            }
            return isHit;
        }

        //航跡を選択する（Arrow）
        private bool SelectArrow(
            ref Dictionary<string, Dictionary<string, string>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refSelectWakeCongfig,
            System.Drawing.Point clickPos)
        {
            bool isHit = false;

            //===== EArrowと点の当たり判定 =====
            {
                foreach (var wake in refDictWake)
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            //Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
                            //listCoordinate.Add(coordinate);

                            //===== 開始点と終点を取得 =====
                            Coordinate[] coordinates = new Coordinate[2];
                            {
                                //開始点の取得
                                Coordinate start = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                                //方位の取得
                                float direction = float.Parse(pos.Value["direction"]);
                                //距離の取得
                                float distance = float.Parse(pos.Value["distance"]);
                                //終点の算出
                                double radian = direction * Math.PI / 180.0;
                                double xStart = start.X;
                                double yStart = start.Y;
                                double x = xStart + distance * Math.Cos(radian);
                                double y = yStart + distance * Math.Sin(radian);
                                Coordinate end = new Coordinate(x, y);
                                //配列を作成
                                coordinates = new Coordinate[2] { start, end };
                            }

                            //===== 線と点の当たり判定 =====
                            {
                                //線と点の距離を計算
                                System.Drawing.Point start = refUserControlMap.TransPosWorldToImage(coordinates[0]);
                                System.Drawing.Point end = refUserControlMap.TransPosWorldToImage(coordinates[1]);
                                int distance = DistancePointToLine(start, end, clickPos);
                                //衝突判定
                                if (distance < 5)
                                {
                                    //選択用ディクショナリーに代入
                                    refDictSelectWake = wake.Value;
                                    isHit = true;
                                    break;

                                }
                            }

                        }
                    }
                    if (isHit) { break; }
                }
            }

            //===== 線の描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                    //空のジオメトリ生成
                    Collection<IGeometry> igeoms = new Collection<IGeometry>();
                    //図形生成クラス
                    GeometryFactory gf = new GeometryFactory();

                    foreach (var pos in refDictSelectWake)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            //開始点の取得
                            Coordinate start = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            //方位の取得
                            float direction = float.Parse(pos.Value["direction"]);
                            //距離の取得
                            float distance = float.Parse(pos.Value["distance"]);
                            //終点の算出
                            double radian = direction * Math.PI / 180.0;
                            double xStart = start.X;
                            double yStart = start.Y;
                            double x = xStart + distance * Math.Cos(radian);
                            double y = yStart + distance * Math.Sin(radian);
                            Coordinate end = new Coordinate(x, y);
                            //配列を作成
                            Coordinate[] coordinates = new Coordinate[2] { start, end };
                            //レイヤーにポイントを追加
                            igeoms.Add(gf.CreatePoint(start));
                            //レイヤーにラインを追加
                            igeoms.Add(gf.CreateLineString(coordinates));
                        }
                    }

                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(igeoms);
                    layer.DataSource = gpro;
                    //レイヤのインデックスを取得
                    int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                    //レイヤを更新
                    refUserControlMap.mapBox.Map.Layers[index] = layer;
                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();
                }
            }
            return isHit;
        }

        //行番号で連動選択する
        private void RowLinkSelect(
            ref Dictionary<string, Dictionary<string, string>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refSelectWakeCongfig,
            int row)
        {
            bool isHit = false;

            //===== 行のデータを取得 =====
            {
                foreach (var wake in refDictWake)
                {
                    foreach (var key in wake.Value)
                    {
                        if (key.Key.Contains("info")) //Keyが"info"を含む
                        {
                            if (int.Parse(key.Value["row"]) == row)
                            {
                                //選択用ディクショナリーに代入
                                refDictSelectWake = wake.Value;
                                isHit = true;
                                break;
                            }
                        }
                    }
                    if (isHit) { break; }
                }
            }

            //===== 線の描画 =====
            {
                if (isHit)
                {
                    //レイヤ取得(参照)
                    VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                    //空のジオメトリ生成
                    Collection<IGeometry> igeoms = new Collection<IGeometry>();
                    //図形生成クラス
                    GeometryFactory gf = new GeometryFactory();
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in refDictSelectWake)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    if (refSelectWakeCongfig.isPoint)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateMultiPointFromCoords(coordinates));
                    }
                    if (refSelectWakeCongfig.isLine)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateLineString(coordinates));
                    }
                    //ジオメトリをレイヤに反映
                    GeometryProvider gpro = new GeometryProvider(igeoms);
                    layer.DataSource = gpro;
                    //レイヤのインデックスを取得
                    int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                    //レイヤを更新
                    refUserControlMap.mapBox.Map.Layers[index] = layer;
                    //mapBoxを再描画
                    refUserControlMap.mapBox.Refresh();
                }
            }

        }

        //時刻で連動選択する
        private void TimeLinkSelect(
            ref Dictionary<string, Dictionary<string, string>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
            ref WakeCongfig refSelectWakeCongfig,
            string startTime,
            string endTime)
        {
            refDictSelectWake = new Dictionary<string, Dictionary<string, string>>();

            //===== 時刻のデータを取得 =====
            {
                foreach (var wake in refDictWake)
                {
                    Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Console.WriteLine(pos.Value["time"]);
                            Console.WriteLine((int.Parse(pos.Value["time"].Substring(0, 8).ToString())));
                            Console.WriteLine((int.Parse(pos.Value["time"].Substring(8, 6).ToString())));

                            if ((int.Parse(pos.Value["time"].Substring(0, 8)) >= int.Parse(startTime.Substring(0, 8))) && //dete
                                (int.Parse(pos.Value["time"].Substring(0, 8)) <= int.Parse(endTime.Substring(0, 8))) &&
                                (int.Parse(pos.Value["time"].Substring(8, 6)) >= int.Parse(startTime.Substring(8, 6))) && //time
                                (int.Parse(pos.Value["time"].Substring(8, 6)) <= int.Parse(endTime.Substring(8, 6)))
                                )
                            {
                                dict.Add(pos.Key, pos.Value);
                            }
                        }
                    }
                    refDictSelectWake = dict;
                }
            }

            //===== 線の描画 =====
            {
                //レイヤ取得(参照)
                VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                //空のジオメトリ生成
                Collection<IGeometry> igeoms = new Collection<IGeometry>();
                //図形生成クラス
                GeometryFactory gf = new GeometryFactory();

                
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in refDictSelectWake)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    if (refSelectWakeCongfig.isPoint)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateMultiPointFromCoords(coordinates));
                    }
                    if (refSelectWakeCongfig.isLine && coordinates.Length >= 2)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateLineString(coordinates));
                    }
                }
                //ジオメトリをレイヤに反映
                GeometryProvider gpro = new GeometryProvider(igeoms);
                layer.DataSource = gpro;
                //レイヤのインデックスを取得
                int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                //レイヤを更新
                refUserControlMap.mapBox.Map.Layers[index] = layer;
                //mapBoxを再描画
                refUserControlMap.mapBox.Refresh();
            }
        }

        //時刻で連動選択する
        private void TimeLinkSelectD(
        ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictSelectWake,
        ref Dictionary<string, Dictionary<string, Dictionary<string, string>>> refDictWake,
        ref WakeCongfig refSelectWakeCongfig,
        string startTime,
        string endTime)
        {
            refDictSelectWake = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            //===== 時刻のデータを取得 =====
            {
                foreach (var wake in refDictWake)
                {
                    Dictionary<string,Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Console.WriteLine( pos.Value["time"] );
                            Console.WriteLine( (int.Parse(pos.Value["time"].Substring(0, 8).ToString())) );
                            Console.WriteLine( (int.Parse(pos.Value["time"].Substring(8, 6).ToString())) );

                            if ((int.Parse(pos.Value["time"].Substring(0, 8)) >= int.Parse(startTime.Substring(0, 8))) && //dete
                                (int.Parse(pos.Value["time"].Substring(0, 8)) <= int.Parse(endTime.Substring(0, 8))) &&
                                (int.Parse(pos.Value["time"].Substring(8, 6)) >= int.Parse(startTime.Substring(8, 6))) && //time
                                (int.Parse(pos.Value["time"].Substring(8, 6)) <= int.Parse(endTime.Substring(8, 6)))
                                )
                            {
                                dict.Add(pos.Key, pos.Value);
                            }
                        }
                    }
                    refDictSelectWake.Add(wake.Key, dict);
                }
            }

            //===== 線の描画 =====
            {
                //レイヤ取得(参照)
                VectorLayer layer = refUserControlMap.sharpMapHelper.GetVectorLayerByName(refUserControlMap.mapBox, refSelectWakeCongfig.layername);
                //空のジオメトリ生成
                Collection<IGeometry> igeoms = new Collection<IGeometry>();
                //図形生成クラス
                GeometryFactory gf = new GeometryFactory();

                foreach (var wake in refDictSelectWake)
                {
                    //座標リストを作成
                    List<Coordinate> listCoordinate = new List<Coordinate>();
                    foreach (var pos in wake.Value)
                    {
                        if (pos.Key.Contains("pos")) //Keyが"pos"を含む
                        {
                            Coordinate coordinate = new Coordinate(double.Parse(pos.Value["x"]), double.Parse(pos.Value["y"]));
                            listCoordinate.Add(coordinate);
                        }
                    }
                    //配列に変換
                    Coordinate[] coordinates = listCoordinate.ToArray();
                    if (refSelectWakeCongfig.isPoint)
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateMultiPointFromCoords(coordinates));
                    }
                    if (refSelectWakeCongfig.isLine && coordinates.Length >=2 )
                    {
                        //線をジオメトリに追加
                        igeoms.Add(gf.CreateLineString(coordinates));
                    }
                }
                //ジオメトリをレイヤに反映
                GeometryProvider gpro = new GeometryProvider(igeoms);
                layer.DataSource = gpro;
                //レイヤのインデックスを取得
                int index = refUserControlMap.mapBox.Map.Layers.IndexOf(layer);
                //レイヤを更新
                refUserControlMap.mapBox.Map.Layers[index] = layer;
                //mapBoxを再描画
                refUserControlMap.mapBox.Refresh();
            }
        }

        //==============================================

        //線分と点の距離計算
        private int DistancePointToLine(
            System.Drawing.Point start, 
            System.Drawing.Point end, 
            System.Drawing.Point point
            )
        {
            int xA = start.X;
            int yA = start.Y;
            int xB = end.X;
            int yB = end.Y;
            int xC = point.X;
            int yC = point.Y;

            //==== 線分ABの範囲内で点Cからの距離 ====
            //線分ABの長さの二乗
            double segmentLengthSquared = (xB - xA) * (xB - xA) + (yB - yA) * (yB - yA);

            //点Cから線分ABに垂直に下ろした垂線のベクトルと線分ABのベクトルとの内積を計算
            double dotProduct = (xC - xA) * (xB - xA) + (yC - yA) * (yB - yA);
            
            //内積の値を線分ABの長さの二乗で割り、tの値を求める。tは垂線が線分AB上にある位置を示すパラメータ。
            //Math.Max(0, Math.Min(1, ...)) の部分は、tの値が0以下の場合は0、1以上の場合は1となるように制限。
            //これにより、垂線が線分ABの範囲外に出る場合でも正しい結果を得ることができる。
            double t = Math.Max(0, Math.Min(1, dotProduct / segmentLengthSquared));
            
            //tを使って垂線の交点の座標を計算。線分AB上のtの値に基づいて、x座標とy座標を求める。
            double xProjection = xA + t * (xB - xA);
            double yProjection = yA + t * (yB - yA);
            
            //点Cと垂線の交点の座標との距離を計算。2次元平面上の距離の公式を使用して、点と点の距離を求めている。
            double distance = Math.Sqrt((xC - xProjection) * (xC - xProjection) + (yC - yProjection) * (yC - yProjection));

            return (int)distance;
        }

        //点と点の距離計算
        private int DistancePointToPoint(
            System.Drawing.Point pointA,
            System.Drawing.Point pointB
            )
        {
            int xA = pointA.X;
            int yA = pointA.Y;
            int xB = pointB.X;
            int yB = pointB.Y;
            double distance = Math.Sqrt((xB - xA) * (xB - xA) + (yB - yA) * (yB - yA));
            return (int)distance;
        }
        
        //==============================================

    }
}

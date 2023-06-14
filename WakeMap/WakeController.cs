using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Algorithm.Distance;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SharpMap.Data.Providers;
using SharpMap.Forms;
using SharpMap.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictAWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictBWake;
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictCPlace;
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictDTrack;
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictEArrow;

        //ディクショナリー(選択用)
        private Dictionary<string, Dictionary<string, double>> g_dictSelectAWake;
        private Dictionary<string, Dictionary<string, double>> g_dictSelectBWake;
        private Dictionary<string, Dictionary<string, double>> g_dictSelectCPlace;
        private Dictionary<string, Dictionary<string, double>> g_dictSelectDTrack;
        private Dictionary<string, Dictionary<string, double>> g_dictSelectEArrow;

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
        private WakeCongfig g_cfgBWake = new WakeCongfig();
        private WakeCongfig g_cfgCPlace = new WakeCongfig();
        private WakeCongfig g_cfgDTrack = new WakeCongfig();
        private WakeCongfig g_cfgEArrow = new WakeCongfig();
        //選択用
        private WakeCongfig g_cfgSelectAWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectBWake = new WakeCongfig();
        private WakeCongfig g_cfgSelectCPlace = new WakeCongfig();
        private WakeCongfig g_cfgSelectDTrack = new WakeCongfig();
        private WakeCongfig g_cfgSelectEArrow = new WakeCongfig();


        //ラベルリスト
        public struct WakeLabel
        {
            public Label label;
            public Coordinate worldPos;
        }
        private List<WakeLabel> g_labelListAWake = new List<WakeLabel>();
        private List<WakeLabel> g_labelListBWake = new List<WakeLabel>();
        private List<WakeLabel> g_labelListDTrack = new List<WakeLabel>();
        private List<WakeLabel> g_labelListDummy = new List<WakeLabel>();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="strDictAWake"></param>
        /// <param name="strDictBWake"></param>
        /// <param name="strDictCPlace"></param>
        /// <param name="strDictDTrack"></param>
        /// <param name="strDictEArrow"></param>
        public void InitWake(
            string scene,
            string strDictAWake,
            string strDictBWake,
            string strDictCPlace,
            string strDictDTrack,
            string strDictEArrow
            )
        {
            Console.WriteLine(
                $"scene = {scene}\n" +
                $"strDictAWake = {strDictAWake}\n" +
                $"strDictBWake = {strDictBWake}\n" +
                $"strDictCPlace = {strDictCPlace}\n" +
                $"strDictDTrack = {strDictDTrack}\n" +
                $"strDictEArrow = {strDictEArrow}"
                );

            //mapBoxの初期化
            refUserControlMap.InitLayerOtherThanBase();

            //コンフィグを初期化
            InitWakeConfig();

            //シーン毎に生成するリストを切り替え
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
                    GenerateWakeDictionary(ref g_dictBWake, strDictBWake);
                    GenerateWakeDictionary(ref g_dictCPlace, strDictCPlace);
                    GenerateWakeDictionary(ref g_dictDTrack, strDictDTrack);
                    break;
                case "SceneC":
                    //シーンを登録
                    g_scene = Scene.SceneC;
                    //辞書を生成
                    GenerateWakeDictionary(ref g_dictAWake, strDictAWake);
                    GenerateWakeDictionary(ref g_dictBWake, strDictBWake);
                    GenerateWakeDictionary(ref g_dictCPlace, strDictCPlace);
                    GenerateWakeDictionary(ref g_dictDTrack, strDictDTrack);
                    GenerateWakeDictionary(ref g_dictEArrow, strDictEArrow);
                    break;
                default:
                    break;
            }

            //描画
            switch (g_scene)
            {
                case Scene.SceneA:
                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);

                    GenerateSelectLayer(ref g_cfgSelectAWake);

                    break;
                case Scene.SceneB:
                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);
                    DrawLineWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake);
                    DrawLineWake(ref g_dictCPlace, ref g_labelListDummy, ref g_cfgCPlace);
                    DrawLineWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack);

                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);

                    break;
                case Scene.SceneC:
                    //Mapに描画する
                    DrawLineWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake);
                    DrawLineWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake);
                    DrawLineWake(ref g_dictCPlace, ref g_labelListDummy, ref g_cfgCPlace);
                    DrawLineWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack);
                    DrawEArrow(ref g_dictEArrow, ref g_cfgEArrow);

                    GenerateSelectLayer(ref g_cfgSelectAWake);
                    GenerateSelectLayer(ref g_cfgSelectBWake);
                    GenerateSelectLayer(ref g_cfgSelectCPlace);
                    GenerateSelectLayer(ref g_cfgSelectDTrack);
                    GenerateSelectLayer(ref g_cfgSelectEArrow);

                    break;
                default:
                    break;
            }

        }

        //コンフィグを初期化
        private void InitWakeConfig()
        {
            g_cfgAWake.layername = "layAWake";
            g_cfgAWake.isPoint = false;
            g_cfgAWake.pointColor = System.Drawing.Brushes.White;
            g_cfgAWake.pointSize = 0;
            g_cfgAWake.isLine = true;
            g_cfgAWake.lineColor = System.Drawing.Color.DarkRed;
            g_cfgAWake.lineWidth = 1;
            g_cfgAWake.isLineDash = true;
            g_cfgAWake.isLineArrow = true;
            g_cfgAWake.isLabel = true;
            g_cfgAWake.labelBackColor= System.Drawing.Color.Coral;
            g_cfgAWake.labelForeColor= System.Drawing.Color.Black;

            g_cfgBWake.layername = "layBWake";
            g_cfgBWake.isPoint = false;
            g_cfgBWake.pointColor = System.Drawing.Brushes.White;
            g_cfgBWake.pointSize = 0;
            g_cfgBWake.isLine = true;
            g_cfgBWake.lineColor = System.Drawing.Color.Orange;
            g_cfgBWake.lineWidth = 1;
            g_cfgBWake.isLineDash = true;
            g_cfgBWake.isLineArrow = true;
            g_cfgBWake.isLabel = true;
            g_cfgBWake.labelBackColor = System.Drawing.Color.Yellow;
            g_cfgBWake.labelForeColor = System.Drawing.Color.Black;

            g_cfgCPlace.layername = "layCPlace";
            g_cfgCPlace.isPoint = true;
            g_cfgCPlace.pointColor = System.Drawing.Brushes.Aqua;
            g_cfgCPlace.pointSize = 5;
            g_cfgCPlace.isLine = false;
            g_cfgCPlace.lineColor = System.Drawing.Color.Empty;
            g_cfgCPlace.lineWidth = 0;
            g_cfgCPlace.isLineDash = false;
            g_cfgCPlace.isLineArrow = false;
            g_cfgCPlace.isLabel = false;
            g_cfgCPlace.labelBackColor = System.Drawing.Color.Empty;
            g_cfgCPlace.labelForeColor = System.Drawing.Color.Empty;

            g_cfgDTrack.layername = "layDTrack";
            g_cfgDTrack.isPoint = false;
            g_cfgDTrack.pointColor = System.Drawing.Brushes.White;
            g_cfgDTrack.pointSize = 0;
            g_cfgDTrack.isLine = true;
            g_cfgDTrack.lineColor = System.Drawing.Color.Green;
            g_cfgDTrack.lineWidth = 1;
            g_cfgDTrack.isLineDash = true;
            g_cfgDTrack.isLineArrow = true;
            g_cfgDTrack.isLabel = true;
            g_cfgDTrack.labelBackColor = System.Drawing.Color.LightGreen;
            g_cfgDTrack.labelForeColor = System.Drawing.Color.Black;

            g_cfgEArrow.layername = "layEArrow";
            g_cfgEArrow.isPoint = false;
            g_cfgEArrow.pointColor = System.Drawing.Brushes.White;
            g_cfgEArrow.pointSize = 0;
            g_cfgEArrow.isLine = true;
            g_cfgEArrow.lineColor = System.Drawing.Color.Aqua;
            g_cfgEArrow.lineWidth = 1;
            g_cfgEArrow.isLineDash = true;
            g_cfgEArrow.isLineArrow = true;
            g_cfgEArrow.isLabel = false;
            g_cfgEArrow.labelBackColor = System.Drawing.Color.Empty;
            g_cfgEArrow.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectAWake.layername = "laySelectAWake";
            g_cfgSelectAWake.isPoint = false;
            g_cfgSelectAWake.pointColor = System.Drawing.Brushes.White;
            g_cfgSelectAWake.pointSize = 0;
            g_cfgSelectAWake.isLine = true;
            g_cfgSelectAWake.lineColor = System.Drawing.Color.DarkRed;
            g_cfgSelectAWake.lineWidth = 2;
            g_cfgSelectAWake.isLineDash = false;
            g_cfgSelectAWake.isLineArrow = false;
            g_cfgSelectAWake.isLabel = false;
            g_cfgSelectAWake.labelBackColor= System.Drawing.Color.Empty;
            g_cfgSelectAWake.labelForeColor= System.Drawing.Color.Empty;

            g_cfgSelectBWake.layername = "laySelectBWake";
            g_cfgSelectBWake.isPoint = false;
            g_cfgSelectBWake.pointColor = System.Drawing.Brushes.White;
            g_cfgSelectBWake.pointSize = 0;
            g_cfgSelectBWake.isLine = true;
            g_cfgSelectBWake.lineColor = System.Drawing.Color.Orange;
            g_cfgSelectBWake.lineWidth = 2;
            g_cfgSelectBWake.isLineDash = false;
            g_cfgSelectBWake.isLineArrow = false;
            g_cfgSelectBWake.isLabel = false;
            g_cfgSelectBWake.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectBWake.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectCPlace.layername = "laySelectCPlace";
            g_cfgSelectCPlace.isPoint = true;
            g_cfgSelectCPlace.pointColor = System.Drawing.Brushes.Blue;
            g_cfgSelectCPlace.pointSize = 5;
            g_cfgSelectCPlace.isLine = false;
            g_cfgSelectCPlace.lineColor = System.Drawing.Color.Empty;
            g_cfgSelectCPlace.lineWidth = 0;
            g_cfgSelectCPlace.isLineDash = false;
            g_cfgSelectCPlace.isLineArrow = false;
            g_cfgSelectCPlace.isLabel = false;
            g_cfgSelectCPlace.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectCPlace.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectDTrack.layername = "laySelectDTrack";
            g_cfgSelectDTrack.isPoint = false;
            g_cfgSelectDTrack.pointColor = System.Drawing.Brushes.White;
            g_cfgSelectDTrack.pointSize = 0;
            g_cfgSelectDTrack.isLine = true;
            g_cfgSelectDTrack.lineColor = System.Drawing.Color.Green;
            g_cfgSelectDTrack.lineWidth = 2;
            g_cfgSelectDTrack.isLineDash = false;
            g_cfgSelectDTrack.isLineArrow = false;
            g_cfgSelectDTrack.isLabel = false;
            g_cfgSelectDTrack.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectDTrack.labelForeColor = System.Drawing.Color.Empty;

            g_cfgSelectEArrow.layername = "laySelectEArrow";
            g_cfgSelectEArrow.isPoint = false;
            g_cfgSelectEArrow.pointColor = System.Drawing.Brushes.White;
            g_cfgSelectEArrow.pointSize = 0;
            g_cfgSelectEArrow.isLine = true;
            g_cfgSelectEArrow.lineColor = System.Drawing.Color.Blue;
            g_cfgSelectEArrow.lineWidth = 2;
            g_cfgSelectEArrow.isLineDash = false;
            g_cfgSelectEArrow.isLineArrow = true;
            g_cfgSelectEArrow.isLabel = false;
            g_cfgSelectEArrow.labelBackColor = System.Drawing.Color.Empty;
            g_cfgSelectEArrow.labelForeColor = System.Drawing.Color.Empty;
        }

        //辞書を生成する
        private void GenerateWakeDictionary(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
            string strDictWake
        ) {
            //JSON文字列をデシリアライズして、 Dictionary<string, Dictionary<string, Dictionary<string, double>>> 型のオブジェクトに格納
            refDictWake = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, double>>>>(strDictWake);
        }

        //描画する
        private void DrawLineWake(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
                            Coordinate wpos = new Coordinate(pos.Value["x"], pos.Value["y"]);
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

        // EArrow用描画処理
        private void DrawEArrow(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
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
                            Coordinate start = new Coordinate(pos.Value["x"], pos.Value["y"]);
                            //方位の取得
                            float direction = (float)pos.Value["direction"];
                            //距離の取得
                            float distance = (float)pos.Value["distance"];
                            //終点の算出
                            double radian = direction * Math.PI / 180.0;
                            double xStart = start.X;
                            double yStart = start.Y;
                            double x = xStart + distance * Math.Cos(radian);
                            double y = yStart + distance * Math.Sin(radian);
                            Coordinate end = new Coordinate(x, y);
                            //配列を作成
                            Coordinate[] coordinates = new Coordinate[2] { start, end };
                            //レイヤーにラインを追加
                            refUserControlMap.AddLineToLayer(layername, coordinates);
                        }

                    }
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
            Label newLabel = new Label();
            newLabel.Text = text;
            newLabel.AutoSize = true;
            //配置
            newLabel.Location = refUserControlMap.TransPosWorldToImage(worldPos);
            newLabel.BackColor = BackColor;
            newLabel.ForeColor = ForeColor;
            //コントロールに追加
            refUserControlMap.mapBox.Controls.Add(newLabel);
        
            //リストに追加
            WakeLabel wakeLabel = new WakeLabel();
            wakeLabel.label = newLabel;
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
        private void GenerateSelectLayer( ref WakeCongfig refWakeCongfig )
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
            //選択処理
            switch (g_scene)
            {
                case Scene.SceneA:
                    SelectLineWake(ref g_dictSelectAWake, ref g_dictAWake, ref g_cfgSelectAWake, clickPos);
                    break;
                case Scene.SceneB:
                    SelectLineWake(ref g_dictSelectBWake, ref g_dictBWake, ref g_cfgSelectBWake, clickPos);
                    SelectPointWake(ref g_dictSelectCPlace, ref g_dictCPlace, ref g_cfgSelectCPlace, clickPos);
                    SelectLineWake(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, clickPos);
                    break;
                case Scene.SceneC:
                    SelectPointWake(ref g_dictSelectCPlace, ref g_dictCPlace, ref g_cfgSelectCPlace, clickPos);
                    //SelectLineWake(ref g_dictSelectDTrack, ref g_dictDTrack, ref g_cfgSelectDTrack, clickPos); //SceneCではDTrackの選択なし
                    SelectEArrow(ref g_dictSelectEArrow, ref g_dictEArrow, ref g_cfgSelectEArrow, clickPos);
                    break;
                default:
                    break;
            }
        }

        //航跡を選択する（線）
        private void SelectLineWake(
            ref Dictionary<string, Dictionary<string, double>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
        }

        //航跡を選択する（点）
        private void SelectPointWake(
            ref Dictionary<string, Dictionary<string, double>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
                            Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
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
        }

        //航跡を選択する（EArrow）
        private void SelectEArrow(
            ref Dictionary<string, Dictionary<string, double>> refDictSelectWake,
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
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
                                Coordinate start = new Coordinate(pos.Value["x"], pos.Value["y"]);
                                //方位の取得
                                float direction = (float)pos.Value["direction"];
                                //距離の取得
                                float distance = (float)pos.Value["distance"];
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
                            Coordinate start = new Coordinate(pos.Value["x"], pos.Value["y"]);
                            //方位の取得
                            float direction = (float)pos.Value["direction"];
                            //距離の取得
                            float distance = (float)pos.Value["distance"];
                            //終点の算出
                            double radian = direction * Math.PI / 180.0;
                            double xStart = start.X;
                            double yStart = start.Y;
                            double x = xStart + distance * Math.Cos(radian);
                            double y = yStart + distance * Math.Sin(radian);
                            Coordinate end = new Coordinate(x, y);
                            //配列を作成
                            Coordinate[] coordinates = new Coordinate[2] { start, end };
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

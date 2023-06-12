using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Algorithm.Distance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictAWake = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictBWake = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictCPlace = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictDTrack = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictEArrow = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

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

        //航跡のコンフィグ
        public struct WakeCongfig {
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
                    DrawWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake, "layAWake");
                    break;
                case Scene.SceneB:
                    //Mapに描画する
                    DrawWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake, "layAWake");
                    DrawWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake, "layBWake");
                    DrawWake(ref g_dictCPlace, ref g_labelListDummy, ref g_cfgCPlace, "layCPlace");
                    DrawWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack, "layDTrack");
                    break;
                case Scene.SceneC:
                    //Mapに描画する
                    DrawWake(ref g_dictAWake, ref g_labelListAWake, ref g_cfgAWake, "layAWake");
                    DrawWake(ref g_dictBWake, ref g_labelListBWake, ref g_cfgBWake, "layBWake");
                    DrawWake(ref g_dictCPlace, ref g_labelListDummy, ref g_cfgCPlace, "layCPlace");
                    DrawWake(ref g_dictDTrack, ref g_labelListDTrack, ref g_cfgDTrack, "layDTrack");
                    DrawEArrow(ref g_dictEArrow, ref g_cfgEArrow, "EArrow");
                    break;
                default:
                    break;
            }

        }

        //コンフィグを初期化
        private void InitWakeConfig()
        {
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
        private void DrawWake(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> refDictWake,
            ref List<WakeLabel> refWakeLabelList,
            ref WakeCongfig refWakeCongfig,
            string layername
            )
        {
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
                        Coordinate wpos = new Coordinate(pos.Value["x"], pos.Value["y"]);
                        refUserControlMap.AddPointToLayer(layername, wpos);
                        refUserControlMap.SetStylePointToLayer(layername, refWakeCongfig.pointColor, refWakeCongfig.pointSize);
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
                        Coordinate coordinate = new Coordinate(pos.Value["x"], pos.Value["y"]);
                        listCoordinate.Add(coordinate);
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
                        //refUserControlMap.SetPointOutLine(layername);
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
            ref WakeCongfig refWakeCongfig,
            string layername
            )
        {
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
                    foreach (var info in wake.Value)
                    {
                        //開始点の取得
                        Coordinate start = new Coordinate(info.Value["x"], info.Value["y"]);
                        //方位の取得
                        float direction = (float)info.Value["direction"];
                        //距離の取得
                        float distance = (float)info.Value["distance"];
                        //終点の算出
                        double radian = direction * Math.PI / 180.0;
                        double xStart = start.X;
                        double yStart = start.Y;
                        double x = xStart + distance * Math.Cos(radian);
                        double y = yStart + distance * Math.Sin(radian);
                        Coordinate end = new Coordinate(x, y);
                        //配列を作成
                        Coordinate[] coordinates = new Coordinate[2]{ start, end };
                        //レイヤーにラインを追加
                        refUserControlMap.AddLineToLayer(layername, coordinates);
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

        //線分と点の距離計算
        private int DistancePointToLine(System.Drawing.Point start, System.Drawing.Point end, System.Drawing.Point point)
        {
            int xA = start.X;
            int yA = start.Y;
            int xB = end.X;
            int yB = end.Y;
            int xC = point.X;
            int yC = point.Y;

            int numerator = (xB - xA) * (xC - xA) + (yB - yA) * (yC - yA);
            int denominator = (xB - xA) * (xB - xA) + (yB - yA) * (yB - yA);
            float t = (float)numerator / (float)denominator;

            float xIntersection = xA + t * (xB - xA);
            float yIntersection = yA + t * (yB - yA);

            float distance = (float)Math.Sqrt(
                ((float)xC - xIntersection) * ((float)xC - xIntersection) + 
                ((float)yC - yIntersection) * ((float)yC - yIntersection)
                );

            return (int)distance;
        }

        //==============================================

    }
}

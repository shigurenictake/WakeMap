using GeoAPI.Geometries;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Algorithm.Distance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
            public System.Drawing.Color pointColor; //ポイント色
            public float pointSize; //ポイントサイズ
            public bool isLine; //ライン描画の有無
            public System.Drawing.Color lineColor; //ライン色
            public float lineWidth; //ラインの太さ
            public bool isLineDash; //ラインを破線にするかどうか
            public bool isLineArrow; //ラインを破線にするかどうか
            public bool isLabel; //ライン描画の有無
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
                    DrawEArrow(ref g_dictEArrow, g_labelListDummy, g_cfgEArrow);
                    break;
                default:
                    break;
            }

        }

        //コンフィグを初期化
        private void InitWakeConfig()
        {
            g_cfgAWake.isPoint = false;
            g_cfgAWake.pointColor = System.Drawing.Color.Empty;
            g_cfgAWake.pointSize = 0;
            g_cfgAWake.isLine = true;
            g_cfgAWake.lineColor = System.Drawing.Color.DarkRed;
            g_cfgAWake.lineWidth = 1;
            g_cfgAWake.isLineDash = true;
            g_cfgAWake.isLineArrow = true;
            g_cfgAWake.isLabel = false;

            g_cfgBWake.isPoint = false;
            g_cfgBWake.pointColor = System.Drawing.Color.Empty;
            g_cfgBWake.pointSize = 0;
            g_cfgBWake.isLine = true;
            g_cfgBWake.lineColor = System.Drawing.Color.Orange;
            g_cfgBWake.lineWidth = 1;
            g_cfgBWake.isLineDash = true;
            g_cfgBWake.isLineArrow = true;
            g_cfgBWake.isLabel = false;

            g_cfgCPlace.isPoint = true;
            g_cfgCPlace.pointColor = System.Drawing.Color.LightBlue;
            g_cfgCPlace.pointSize = 3;
            g_cfgCPlace.isLine = false;
            g_cfgCPlace.lineColor = System.Drawing.Color.Empty;
            g_cfgCPlace.lineWidth = 0;
            g_cfgCPlace.isLineDash = false;
            g_cfgCPlace.isLineArrow = false;
            g_cfgCPlace.isLabel = false;

            g_cfgDTrack.isPoint = false;
            g_cfgDTrack.pointColor = System.Drawing.Color.Empty;
            g_cfgDTrack.pointSize = 0;
            g_cfgDTrack.isLine = true;
            g_cfgDTrack.lineColor = System.Drawing.Color.Green;
            g_cfgDTrack.lineWidth = 1;
            g_cfgDTrack.isLineDash = true;
            g_cfgDTrack.isLineArrow = true;
            g_cfgDTrack.isLabel = false;

            g_cfgEArrow.isPoint = false;
            g_cfgEArrow.pointColor = System.Drawing.Color.Empty;
            g_cfgEArrow.pointSize = 0;
            g_cfgEArrow.isLine = true;
            g_cfgEArrow.lineColor = System.Drawing.Color.LightBlue;
            g_cfgEArrow.lineWidth = 1;
            g_cfgEArrow.isLineDash = true;
            g_cfgEArrow.isLineArrow = true;
            g_cfgEArrow.isLabel = false;
        }


        //辞書を生成する
        private void GenerateWakeDictionary(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> dictWake,
            string strDictWake
        ) {
            // 文字列の空白文字を削除する
            //strWakeList = System.Text.RegularExpressions.Regex.Replace(strWakeList, @"[\s]+", "");
            //JSON文字列をデシリアライズして、 Dictionary<string, Dictionary<string, Dictionary<string, double>>> 型のオブジェクトに格納
            dictWake = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, double>>>>(strDictWake);

            //foreach (var wake in dictWake)
            //{
            //    Console.WriteLine("■" + wake.Key);
            //    foreach (var pos in wake.Value)
            //    {
            //        Console.Write($"　{pos.Key} : ");
            //        Console.Write($"( {pos.Value["x"]} , ");
            //        Console.Write($"{pos.Value["y"]} )\n");
            //    }
            //    Console.WriteLine();
            //}
        }

        //描画する
        private void DrawWake(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> dictWake,
            ref List<WakeLabel> wakeLabelList,
            ref WakeCongfig wakeCongfig,
            string layername
            )
        {
            //レイヤを生成
            refUserControlMap.GenerateLayer(layername);

            if (dictWake == null)
            {
                return;
            }

            //点を追加
            if (wakeCongfig.isPoint)
            {
                //wakeを取得
                foreach (var wake in dictWake)
                {
                    //座標を取得
                    foreach (var pos in wake.Value)
                    {
                        Coordinate wpos = new Coordinate(pos.Value["x"], pos.Value["y"]);
                        refUserControlMap.AddPointToLayer(layername, wpos);
                        //点を線で結ぶ
                        //refUserControlMap.AddLineConnectLast2PointsToLayer(layername);
                    }
                }
            }

            //線を追加
            if (wakeCongfig.isLine)
            {
                //wakeを取得
                foreach (var wake in dictWake)
                {
                    //座標リストを取得
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
                    refUserControlMap.SetStyleLineToLayer(layername, wakeCongfig.lineColor, wakeCongfig.lineWidth);
                    //破線を設定
                    if (wakeCongfig.isLineDash)
                    {
                        refUserControlMap.SetLineDash(layername);
                    }
                    //矢印を設定
                    if (wakeCongfig.isLineArrow)
                    {
                        refUserControlMap.SetLineArrow(layername);
                    }

                    ////終点に▲(矢印の代わり)を描画
                    ////考え中・・・
                    //double angle = GetAngle(coordinates[coordinates.Count() - 2], coordinates[coordinates.Count() - 1]); //角度
                    ////終点を基準に▲を描画
                    //refUserControlMap.AddTriangleToLayer(layername, coordinates[coordinates.Count() - 1]);
                }
            }

            ////角度を取得
            //double GetAngle(Coordinate start, Coordinate target)
            //{
            //    Coordinate dt = new Coordinate( target.X - start.X, target.Y - start.Y);
            //    double rad = Math.Atan2(dt.Y, dt.X);
            //    double angle = rad * 180 / Math.PI;
            //    return angle;
            //}

            //ラベルを描画
            if (wakeCongfig.isLabel)
            {
                //wakeを取得
                foreach (var wake in dictWake)
                {
                    foreach (var pos in wake.Value)
                    {
                        //refUserControlMap.SetPointOutLine(layername);
                        if (pos.Key == "pos1")
                        {
                            Coordinate wpos = new Coordinate(pos.Value["x"], pos.Value["y"]);
                            //ラベル名はwake.Keyの4文字目以降を切り出し
                            GenerateLabel(wpos, wake.Key.Substring(4));
                        }
                    }
                }
            }

            Console.WriteLine();
            refUserControlMap.MapBoxRefresh();
        }

        //==============================================


        //ラベル生成
        private void GenerateLabel(Coordinate worldPos, string text)
        {
            //新しいラベルを生成
            Label newLabel = new Label();
            newLabel.Text = text;
            newLabel.AutoSize = true;
            //配置
            newLabel.Location = refUserControlMap.TransPosWorldToImage(worldPos);
            //newLabel.Parent = refForm1.mapBox1; //親を設定
            //newLabel.BackColor = System.Drawing.Color.Transparent;//背景を透過
            //newLabel.Name = labelName; //識別名
            //コントロールに追加
            refUserControlMap.mapBox.Controls.Add(newLabel);
        
            //リストに追加
            WakeLabel wakeLabel = new WakeLabel();
            wakeLabel.label = newLabel;
            wakeLabel.worldPos = worldPos;
            g_labelListAWake.Add(wakeLabel);
        }

        // EArrow用描画処理
        private void DrawEArrow(
            ref Dictionary<string, Dictionary<string, Dictionary<string, double>>> dictWake,
            List<WakeLabel> wakeLabelList,
            WakeCongfig wakeCongfig
            )
        {
        }

        // ラベルをmapboxに合わせて再配置
        public void RelocateLabel()
        {
            //Console.WriteLine("RelocateLabel");
            foreach (WakeLabel wakeLabel in g_labelListAWake)
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

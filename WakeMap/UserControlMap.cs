using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap.Data.Providers;
using SharpMap;
using SharpMap.Forms;
using SharpMap.Layers;
using SharpMap.Styles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpMap.Rendering.Symbolizer;
using SharpMap.Drawing;
using System.Net;

namespace WakeMap
{
    public partial class UserControlMap : UserControl
    {
        public WakeController refWakeController;

        //SharpMap補助クラス
        public SharpMapHelper sharpMapHelper = new SharpMapHelper();

        //クラス変数
        public Coordinate g_worldPos = new Coordinate();                       //地理座標
        public System.Drawing.Point g_imagePos = new System.Drawing.Point();   //イメージ座標


        public UserControlMap()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();
        }

        //マップ初期化
        private void InitializeMap()
        {
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            //Zoom制限
            mapBox.Map.MinimumZoom = 0.1;
            mapBox.Map.MaximumZoom = 360.0;

            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox.Refresh();
        }

        //基底レイヤ初期化
        private void InitializeBaseLayer()
        {
            //Map生成
            mapBox.Map = new Map(new Size(mapBox.Width, mapBox.Height));
            mapBox.Map.BackColor = System.Drawing.Color.LightBlue;

            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            //マップにレイヤーを追加
            mapBox.Map.Layers.Add(baseLayer);
        }

        //レイヤ生成
        public void GenerateLayer(string layername)
        {
            //レイヤ生成
            VectorLayer layer = new VectorLayer(layername);
            //ジオメトリ生成
            List<IGeometry> igeoms = new List<IGeometry>();
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //layer.Style.PointColor = Brushes.Red;
            //layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
            //レイヤをmapBoxに追加
            mapBox.Map.Layers.Add(layer);
        }

        //レイヤのポイントの色を設定
        public void SetStylePointToLayer(string layername , System.Drawing.Brush color , float size )
        {
            //レイヤ取得(参照)
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ポイントの色を設定
            layer.Style.PointColor = color;
            layer.Style.PointSize = size;

            //レイヤを更新
            //int index = mapBox.Map.Layers.IndexOf(layer);
            //mapBox.Map.Layers[index] = layer;
        }

        //レイヤの点の色を設定
        public void SetStyleLineToLayer(string layername, System.Drawing.Color color, float width)
        {
            //レイヤ取得(参照)
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ラインの色を設定
            layer.Style.Line = new Pen(color, width);
            //レイヤを更新
            //int index = mapBox.Map.Layers.IndexOf(layer);
            //mapBox.Map.Layers[index] = layer;
        }

        ////イベント - 地図上でマウス移動
        private void mapBox_MouseMove(Coordinate worldPos, MouseEventArgs imagePos)
        {
            //labelに座標表示
            g_worldPos = worldPos;//地理座標系上の座標の更新
            g_imagePos = imagePos.Location;//画面上のイメージ座標の更新
        }

        ////イベント - 地図上でクリック(ボタンを離した瞬間)

        private void mapBox_Click(object sender, EventArgs e)
        {
            refWakeController.mapBox_ClickSelect(g_imagePos);
        }


        ////イベント - [Pan処理] ラジオボタン「パン」変更時
        //private void radioButtonClickModePan_CheckedChanged(object sender, EventArgs e)
        //{
        //    ////「クリックモード == パン」ならばActiveToolをPanにする
        //    //if (this.radioButtonClickModePan.Checked == true)
        //    //{
        //    //    mapBox.ActiveTool = MapBox.Tools.Pan;
        //    //}
        //    //else
        //    //{
        //    //    mapBox.ActiveTool = MapBox.Tools.None;
        //    //}
        //}
        ////イベント - [Pan処理] マウスボタンが離れた瞬間
        //private void mapBox1_MouseUp(Coordinate worldPos, MouseEventArgs imagePos)
        //{
        //    //ActiveToolがPanならばパン処理を行う
        //    if (mapBox.ActiveTool == MapBox.Tools.Pan)
        //    {
        //        //「押した瞬間のイメージ座標」から「離れた瞬間のイメージ座標」がほぼ移動していなければ、地図は動かさない
        //        if (sharpMapHelper.Distance(g_mouseDownImagePos, imagePos.Location) <= 1.0)
        //        {
        //            //ActiveToolをNoneとすることでパンさせない
        //            mapBox.ActiveTool = MapBox.Tools.None;
        //
        //            //指定時間（ミリ秒）後、Panに戻す
        //            DelayActivePan(500);
        //        }
        //    }
        //}
        ////非同期処理 - [Pan処理] 指定時間後、ActiveToolをPanにする
        //private async void DelayActivePan(int msec)
        //{
        //    await Task.Delay(msec);
        //    mapBox.ActiveTool = MapBox.Tools.Pan;
        //}

        //指定レイヤにPoint追加
        public void AddPointToLayer(string layername, Coordinate worldPos)
        {
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();
            igeoms.Add(gf.CreatePoint(worldPos));
            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤのインデックスを取得
            int index = mapBox.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox.Map.Layers[index] = layer;
            //mapBoxを再描画
            mapBox.Refresh();
        }

        //ラインを追加
        public void AddLineToLayer(string layername, Coordinate[] coordinates)
        {
            //Coordinate[]の例
            //Coordinate[] coordinates = new Coordinate[]{
            //        new Coordinate(130,30),
            //        new Coordinate(135,30),
            //        new Coordinate(135,35),
            //        new Coordinate(130,35)
            //    };

            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometrysAll(layer);
            //図形生成クラス
            GeometryFactory gf = new GeometryFactory();
            //座標リストの線を生成し、ジオメトリのコレクションに追加
            igeoms.Add(gf.CreateLineString(coordinates));

            //ジオメトリをレイヤに反映
            GeometryProvider gpro = new GeometryProvider(igeoms);
            layer.DataSource = gpro;
            //レイヤのインデックスを取得
            int index = mapBox.Map.Layers.IndexOf(layer);
            //レイヤを更新
            mapBox.Map.Layers[index] = layer;
        }

        //レイヤの線を破線にする
        public void SetLineDash(string layername)
        {
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);

            //破線にする { 破線の長さ, 間隔 }
            layer.Style.Line.DashPattern = new float[] { 3.0F, 3.0F };
        }

        //レイヤの線を矢印にする
        public void SetLineArrow(string layername)
        {
             //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);

            //矢印にする (width, height, isFilled)
            layer.Style.Line.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4f, 4f, true);
        }

        //mapBoxを再描画
        public void MapBoxRefresh()
        {
            mapBox.Refresh();
        }

        /// <summary>
        /// ベース以外の全レイヤ削除
        /// </summary>
        public void InitLayerOtherThanBase()
        {
            //ベース(0番目)以外のレイヤ削除
            while (mapBox.Map.Layers.Count > 1)
            {
                mapBox.Map.Layers.RemoveAt((mapBox.Map.Layers.Count - 1));
            }
            //mapBoxを再描画
            mapBox.Refresh();
        }

        //地図座標→イメージ座標に変換
        public System.Drawing.Point TransPosWorldToImage(Coordinate worldPos)
        {
            return System.Drawing.Point.Round(this.mapBox.Map.WorldToImage(worldPos));
        }

        //イベント - マップの中心が変更(ZoomやPanによる変更も対象)
        private void mapBox_MapCenterChanged(Coordinate center)
        {
            //ラベルの再配置
            refWakeController.RelocateLabel();
        }
    }
}

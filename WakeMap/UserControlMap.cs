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

        ////ジオメトリ情報格納用
        //struct GeomInfo
        //{
        //    public MapBox mapbox;     //MapBoxオブジェクト
        //    public string layername;  //レイヤ名
        //    public int index;      //インデックス
        //    public IGeometry igeom;      //ジオメトリ
        //    //セット関数
        //    public void Set(MapBox mb, string ln, int id, IGeometry ig)
        //    {
        //        this.mapbox = mb;
        //        this.layername = ln;
        //        this.index = id;
        //        this.igeom = ig;
        //    }
        //};
        //GeomInfo g_selectedGeom = new GeomInfo(); //選択ジオメトリ
        //GeomInfo g_selectedGeomPrev = new GeomInfo(); //前回選択ジオメトリ

        //private System.Drawing.Point g_mouseDownImagePos = new System.Drawing.Point();   //マウスを押した瞬間のイメージ座標

        public UserControlMap()
        {
            InitializeComponent();

            //SharpMap初期化
            this.InitializeMap();

            //Form1参照用
            //refWakeController.refUserControlMap = this;
        }

        //マップ初期化
        private void InitializeMap()
        {
            //baseLayerレイヤ初期化
            this.InitializeBaseLayer();

            ////pointLineLayerレイヤ生成
            //this.GenerateLayer("pointLineLayer");

            //TestLayer();

            TestLayer2();

            //Zoom制限
            mapBox.Map.MinimumZoom = 0.1;
            mapBox.Map.MaximumZoom = 360.0;

            //レイヤ全体を表示する(全レイヤの範囲にズームする)
            mapBox.Map.ZoomToExtents();

            //mapBoxを再描画
            mapBox.Refresh();
        }

        //テスト
        private void TestLayer2()

        {

        }

        ////テスト
        //private void TestLayer()
        //{
        //    //レイヤ生成
        //    VectorLayer layer = new VectorLayer("testlayer");
        //    //ジオメトリ生成
        //    List<IGeometry> igeoms = new List<IGeometry>();
        //
        //    //図形生成クラス
        //    GeometryFactory gf1 = new GeometryFactory();
        //    GeometryFactory gf2 = new GeometryFactory();
        //
        //
        //    //Pointが2つ以上ならばコレクション上、最後の2点を取得する
        //    Coordinate[] linePos1 = new Coordinate[2];
        //    linePos1[0] = new Coordinate(110, 45);
        //    linePos1[1] = new Coordinate(115, 40);
        //    ILineString ilinestring1 = gf1.CreateLineString(linePos1);
        //    igeoms.Add(ilinestring1);
        //
        //    //Pointが2つ以上ならばコレクション上、最後の2点を取得する
        //    Coordinate[] linePos2 = new Coordinate[2];
        //    linePos2[0] = new Coordinate(140, 35);
        //    linePos2[1] = new Coordinate(145, 30);
        //    ILineString ilinestring2 = gf2.CreateLineString(linePos2);
        //    igeoms.Add(ilinestring2);
        //
        //    //LineString lineStringDummy;
        //    //lineStringDummy.AsText
        //
        //    //ILineString ilinestringDummy;
        //
        //    // 多角形塗りつぶし
        //    var poly = new GeometryFactory().CreatePolygon(new Coordinate[] {
        //         new Coordinate(ilinestring1.Coordinates[0].X, ilinestring1.Coordinates[0].Y),
        //         new Coordinate(ilinestring1.Coordinates[1].X, ilinestring1.Coordinates[1].Y),
        //         new Coordinate(ilinestring2.Coordinates[1].X, ilinestring2.Coordinates[1].Y),
        //         new Coordinate(ilinestring2.Coordinates[0].X, ilinestring2.Coordinates[0].Y),
        //         new Coordinate(155,25),
        //         new Coordinate(ilinestring1.Coordinates[0].X, ilinestring1.Coordinates[0].Y)
        //    });
        //    igeoms.Add(poly);
        //
        //    Console.WriteLine($"【出力】poly.AsText() = {poly.AsText()}");
        //    //【出力】poly.AsText() = POLYGON ((110 45, 115 40, 145 30, 140 35, 155 25, 110 45))
        //
        //    //ジオメトリをレイヤに反映
        //    GeometryProvider gpro = new GeometryProvider(igeoms);
        //    layer.DataSource = gpro;
        //
        //    // レイヤーのスタイル設定
        //    var style = new VectorStyle();
        //    style.Line = new Pen(Color.Red, 2);
        //    //layer.Style.PointColor = Brushes.Blue;
        //    //layer.Style.Line = new Pen(Color.Blue, 1.0f);
        //
        //    //レイヤをmapBoxに追加
        //    mapBox.Map.Layers.Add(layer);
        //}

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

            ////点とマウスの衝突時のアクション
            //CollisionsWithPoints();//点との衝突
        }


        ////イベント - 地図上でクリック(ボタンを離した瞬間)
        //private void mapBox1_Click(object sender, EventArgs e)
        //{
        //    ////クリックモード == 点を描く
        //    //if (this.radioButtonClickModeDraw.Checked == true)
        //    //{
        //    //    AddPointToLayer("pointLineLayer", g_worldPos);//pointLineLayerレイヤに点を追加
        //    //}
        //
        //    ////クリックモード == 点を選択する
        //    //if (this.radioButtonClickModeSelect.Checked == true)
        //    //{
        //    //    //全レイヤの中からクリックした点を探索し、選択する
        //    //    LayerCollection layers = mapBox.Map.Layers;
        //    //    foreach (Layer layer in layers)
        //    //    {
        //    //        //レイヤの点が当たっていたらを選択する
        //    //        bool isSelect = SelectPoint(layer.LayerName);
        //    //        if (isSelect == true)
        //    //        {
        //    //            break;
        //    //        }
        //    //    }
        //    //    UpdateSelectLayer();//選択ジオメトリのレイヤを更新
        //    //
        //    //    //選択したものがあるならばラベルに表示
        //    //    if (g_selectedGeom.igeom != null)
        //    //    {
        //    //        this.label4.Text = $"{g_selectedGeom.layername} : [ {g_selectedGeom.index} ] : {g_selectedGeom.igeom}";
        //    //        this.richTextBox1.AppendText(this.label4.Text + "\n");
        //    //    }//選択したものがないならば"選択なし"をラベルに表示
        //    //    else
        //    //    {
        //    //        this.label4.Text = $"選択なし";
        //    //        this.richTextBox1.Clear();
        //    //    }
        //    //
        //    //    //前回選択ジオメトリを更新
        //    //    g_selectedGeomPrev = g_selectedGeom;
        //    //}
        //}
        
        ////イベント - 地図上でクリック(ボタンを離した瞬間)

        private void mapBox_Click(object sender, EventArgs e)
        {
            refWakeController.mapBox_ClickSelect(g_imagePos);
        }


        ////イベント - ラジオボタン「パン」変更時
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

        ////イベント - マウスボタンが押された瞬間
        //private void mapBox1_MouseDown(Coordinate worldPos, MouseEventArgs imagePos)
        //{
        //    //マウスボタンが押された瞬間のイメージ座標
        //    g_mouseDownImagePos = imagePos.Location;
        //}
        private void mapBox_MouseDown(Coordinate worldPos, MouseEventArgs imagePos)
        {

        }

        ////イベント - マウスボタンが離れた瞬間
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

        ////非同期処理 - 指定時間後、ActiveToolをPanにする
        //private async void DelayActivePan(int msec)
        //{
        //    await Task.Delay(msec);
        //    mapBox.ActiveTool = MapBox.Tools.Pan;
        //}

        ////イベント - button1クリック
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    sharpMapHelper.ViewWholeMap(mapBox);//全体表示
        //}
        //
        ////イベント - button2クリック
        //private void button2_Click(object sender, EventArgs e)
        //{
        //}
        //
        ////イベント - button3クリック
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    InitLayerOtherThanBase(); //ベース以外のレイヤ初期化
        //}

        ////イベント - button4クリック
        //private void button4_Click(object sender, EventArgs e)
        //{
        //    UpdeteLayerList(); //レイヤリストの更新
        //}

        ////イベント - button5クリック
        //private void button5_Click(object sender, EventArgs e)
        //{
        //    UpdeteGeometryListOfPointLineLayer(); //ジオメトリリストの更新
        //}

        ////地理座標系上の座標の更新
        //private void UpdateWorldPos(Coordinate worldPos)
        //{
        //    g_worldPos = worldPos;
        //
        //    ////地理座標→イメージ座標に変換
        //    //this.label1.Text = g_worldPos.ToString() + "\n" +
        //    //    mapBox.Map.WorldToImage(g_worldPos);
        //}

        ////画面上のイメージ座標の更新
        //private void UpdateImagePos(MouseEventArgs imagePos)
        //{
        //    g_imagePos = imagePos.Location;
        //
        //    ////イメージ座標→地理座標に変換
        //    //this.label2.Text = g_imagePos + "\n" +
        //    //    mapBox.Map.ImageToWorld(g_imagePos);
        //}

        ////点との衝突
        //private void CollisionsWithPoints()
        //{
        //    //いずれかのPointと衝突しているか判定
        //    IGeometry hitIgeome = null;
        //    int index = new int();
        //    bool ishit = sharpMapHelper.CheckHitAnyPoints(ref index, ref hitIgeome, mapBox, "pointLineLayer", g_worldPos);
        //    //if (ishit == true)
        //    //{
        //    //    string txt = $"ヒットしました : [ {index} ] : " + hitIgeome.ToString();
        //    //    this.label3.Text = txt;
        //    //}
        //    //else
        //    //{
        //    //    this.label3.Text = "ヒットなし";
        //    //}
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
            //レイヤ取得
            VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, layername);
            //ジオメトリ取得
            Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometrysAll(layer);
            //点をジオメトリに追加
            GeometryFactory gf = new GeometryFactory();

            //Coordinate[] coordinates = new Coordinate[]{
            //        new Coordinate(130,30),
            //        new Coordinate(135,30),
            //        new Coordinate(135,35),
            //        new Coordinate(130,35)
            //    };
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

        ////指定レイヤ内で、マウスカーソルに当たるPointがあれば、そのPointを選択ジオメトリにセットする
        //private bool SelectPoint(string layername)
        //{
        //    //いずれかのPointと衝突しているか判定
        //    IGeometry hitIgeome = null;
        //    int index = new int();
        //    bool isSelect = sharpMapHelper.CheckHitAnyPoints(ref index, ref hitIgeome, mapBox, layername, g_worldPos);
        //    if (isSelect == true)
        //    {
        //        //ヒットしたPointを選択ジオメトリにセットする
        //        g_selectedGeom.Set(mapBox, layername, index, hitIgeome);
        //    }//マウスカーソルに衝突するPointがないならば、選択ジオメトリを初期化
        //    else
        //    {
        //        //初期化
        //        g_selectedGeom = new GeomInfo();
        //    }
        //    return isSelect;
        //}

        ////選択ジオメトリのレイヤを更新
        //private void UpdateSelectLayer()
        //{
        //    //前回選択しているものがあったならば、そのレイヤは未選択の色を設定
        //    if (g_selectedGeomPrev.igeom != null)
        //    {
        //        //レイヤ取得
        //        VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, g_selectedGeomPrev.layername);
        //        //Pointの色を変更
        //        layer.Style.PointColor = Brushes.Red;
        //        layer.Style.Line = new Pen(Color.DarkRed, 1.0f);
        //        //レイヤのインデックスを取得
        //        int index = mapBox.Map.Layers.IndexOf(layer);
        //        //レイヤを更新
        //        mapBox.Map.Layers[index] = layer;
        //        //mapBoxを再描画
        //        mapBox.Refresh();
        //    }
        //
        //    //今選択しているものがあるならば、そのレイヤは選択中の色を設定
        //    if (g_selectedGeom.igeom != null)
        //    {
        //        //レイヤ取得
        //        VectorLayer layer = sharpMapHelper.GetVectorLayerByName(mapBox, g_selectedGeom.layername);
        //        //Pointの色を変更
        //        layer.Style.PointColor = Brushes.BlueViolet;
        //        layer.Style.Line = new Pen(Color.Blue, 1.5f);
        //        //レイヤのインデックスを取得
        //        int index = mapBox.Map.Layers.IndexOf(layer);
        //        //レイヤを更新
        //        mapBox.Map.Layers[index] = layer;
        //        //mapBoxを再描画
        //        mapBox.Refresh();
        //    }
        //}

        ////リッチテキストボックス「レイヤリスト」更新
        //private void UpdeteLayerList()
        //{
        //    //レイヤリストの取得
        //    string text = null;
        //    LayerCollection layers = mapBox.Map.Layers;
        //    for (int i = 0; i < layers.Count; i++)
        //    {
        //        text = text + $"[ {i} ] : {layers[i].LayerName}" + "\n";
        //
        //    }
        //    richTextBoxLayerList.Text = text;
        //}

        ////pointLineLayerレイヤのジオメトリ一覧更新
        //private void UpdeteGeometryListOfPointLineLayer()
        //{
        //    //レイヤ内の全ジオメトリを取得
        //    Collection<IGeometry> igeoms = sharpMapHelper.GetIGeometrysAll(sharpMapHelper.GetVectorLayerByName(mapBox, "pointLineLayer"));
        //    ////ジオメトリ一覧をラベルに表示 ( pointLineLayerレイヤ一覧表示 )
        //    //string text = string.Empty;
        //    //for (int i = 0; i < igeoms.Count; i++)
        //    //{
        //    //    text = text + $"[ {i} ] : {igeoms[i]}" + "\n";
        //    //}
        //    //this.richTextBoxPointLayerList.Text = text;
        //}

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
            //pointLineLayerレイヤ生成
            //this.GenerateLayer("pointLineLayer");

            //this.g_selectedGeom = new GeomInfo();

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

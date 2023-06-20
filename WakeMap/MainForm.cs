using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    public partial class MainForm : Form
    {
        //CsToJsクラス
        private CsToJs csToJs = new CsToJs();
        //JsToCsクラス
        private JsToCs jsToCs = new JsToCs();
        //WakeControllerクラス
        private WakeController wakeController = new WakeController();

        //コンストラクタ
        public MainForm(string path)
        {
            InitializeComponent();

            //URLを設定
            this.webView.Source = new Uri(path);

            //CsToJsクラス
            csToJs.webView = this.webView;
            csToJs.refForm = this;
            //JsToCsクラス
            jsToCs.refWakeController = this.wakeController;
            //WakeControllerクラス
            wakeController.refUserControlMap = this.userControlMap;
            //UserControlMapクラス
            this.userControlMap.refWakeController = this.wakeController;
        }

        //イベント ボタンクリック「詳細へ」
        private void buttonGoDetail_Click(object sender, EventArgs e)
        {
            csToJs.buttonClickGoDetail();
        }

        //イベント ボタンクリック「戻る」
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.webView.GoBack();
        }

        //イベント ボタンクリック「ヘルプ」
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ヘルプ");
        }

        //イベント ボタンクリック「閉じる」
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //イベント WebView2 htmlのロード完了時
        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if (webView.CoreWebView2 != null)
                {
                    //JavaScriptからC#のメソッドが実行できる様に仕込む
                    webView.CoreWebView2.AddHostObjectToScript("jsToCs", jsToCs);

                    //C#_WebView判定フラグにtrueをセット
                    csToJs.SetIsCsharpWebView();

                    //URLからファイル名を取得
                    string url = this.webView.Source.ToString();
                    string fimeneme = System.IO.Path.GetFileName(url);
                    //フォーム構成を切り変える
                    Transform(fimeneme);
                }
                else MessageBox.Show("CoreWebView2==null");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //htmlファイル名に応じてフォーム内の構成を変更する
        private void Transform(string fimeneme)
        {
            switch (fimeneme)
            {
                case "topMenu.html":
                case "patternA.html":
                case "subPatternA.html":
                    TransformA(fimeneme);
                    break;
                case "subPatternA2.html":
                    //TransformA2(fimeneme);
                    break;
                case "patternB.html":
                case "patternB1.html":
                case "patternB2.html":
                case "patternB3.html":
                case "subPatternB.html":
                    TransformB(fimeneme);
                    break;

                default:
                    Console.WriteLine("未登録のhtmlです");
                    break;
            }
        }

        //ウィンドウ構成A 全画面html
        private void TransformA(string fimeneme)
        {
            //コントロールのレイアウトを一時停止
            this.SuspendLayout();

            //htmlの閉じるボタンを削除
            this.csToJs.RemoveCloseBtn();
            //タイトルを設定
            this.csToJs.SetTitle();

            //右パネルを非表示
            this.splitContainerLR.Panel2Collapsed = true;
            //左上パネルを表示
            this.splitContainerLeftUD.Panel1Collapsed = false;
            //C#の「戻る/ヘルプ/閉じる」パネルを左上パネルに移動
            this.splitContainerRightUD.Panel1.Controls.Remove(this.panelBHC);
            this.splitContainerLeftUD.Panel1.Controls.Add(this.panelBHC);
            this.panelBHC.Location = new System.Drawing.Point(
                this.splitContainerLeftUD.Panel1.Width - this.panelBHC.Width,
                this.panelBHC.Location.Y);

            //戻るボタンの非表示・表示
            switch (fimeneme)
            {
                case "topMenu.html":
                    //戻るボタンの非表示
                    this.buttonBack.Visible = false;
                    break;
                default:
                    //戻るボタンの表示
                    this.buttonBack.Visible = true;
                    break;
            }

            //固有処理
            switch (fimeneme)
            {
                case "topMenu.html":
                    break;
                case "patternA.html":
                    this.csToJs.OperatePatternA();
                    break;
                default:
                    break;
            }

            //コントロールのレイアウトを再開
            this.ResumeLayout(false);
        }

        //ウィンドウ構成B 半画面html
        private void TransformB(string fimeneme)
        {
            //コントロールのレイアウトを一時停止
            this.SuspendLayout();

            //htmlの閉じるボタンを削除
            this.csToJs.RemoveCloseBtn();
            //タイトルを設定
            this.csToJs.SetTitle();

            //右パネルを表示
            this.splitContainerLR.Panel2Collapsed = false;
            //左上パネルを非表示
            this.splitContainerLeftUD.Panel1Collapsed = true;
            //C#の「戻る/ヘルプ/閉じる」パネルを右上パネルに移動
            this.splitContainerLeftUD.Panel1.Controls.Remove(this.panelBHC);
            this.splitContainerRightUD.Panel1.Controls.Add(this.panelBHC);
            this.panelBHC.Location = new System.Drawing.Point(
                this.splitContainerRightUD.Panel1.Width - this.panelBHC.Width,
                this.panelBHC.Location.Y);

            //戻るボタンの表示
            this.buttonBack.Visible = true;

            //スプリッターの位置を設定
            this.splitContainerLR.SplitterDistance = (int)(this.splitContainerLR.Width * 65 / 100);

            //固有処理
            switch (fimeneme)
            {
                case "patternB.html":
                case "patternB1.html":
                case "patternB2.html":
                case "patternB3.html":
                    csToJs.OperatePatternB();
                    break;
                default:
                    break;
            }

            //コントロールのレイアウトを再開
            this.ResumeLayout(false);
        }
    }
}

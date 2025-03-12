using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace YahooBrowser
{
    public partial class Form1 : Form
    {
        private TabControl tabControl;
        private List<string> history = new List<string>(); // 履歴管理
        private List<string> bookmarks = new List<string>(); // ブックマーク管理

        public Form1()
        {
            InitializeComponent();
            InitializeBrowser();
        }

        private void InitializeBrowser()
        {
            // タブコントロール
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(tabControl);

            // 最初のタブを作成
            AddNewTab("https://www.yahoo.co.jp/");

            // メニュー作成
            CreateMenu();
        }

        // 🔹 新しいタブを追加
        private void AddNewTab(string url)
        {
            var tabPage = new TabPage("新しいタブ");
            var webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // WebView2 初期化
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                webView.Source = new Uri(url);
                tabPage.Text = webView.Source.Host;
            };

            // 履歴を記録
            webView.NavigationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                {
                    history.Add(webView.Source.ToString());
                    tabPage.Text = webView.Source.Host;
                }
            };

            webView.EnsureCoreWebView2Async(null);
            tabPage.Controls.Add(webView);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
        }

        // 🔹 現在のWebView取得
        private WebView2 GetCurrentWebView()
        {
            if (tabControl.SelectedTab != null && tabControl.SelectedTab.Controls.Count > 0)
            {
                return tabControl.SelectedTab.Controls[0] as WebView2;
            }
            return null;
        }

        // 🔹 メニュー作成
        private void CreateMenu()
        {
            var menu = new MenuStrip();

            // 戻るボタン
            var backButton = new ToolStripMenuItem("◀️ 戻る");
            backButton.Click += (s, e) =>
            {
                var webView = GetCurrentWebView();
                if (webView != null && webView.CanGoBack) webView.GoBack();
            };

            // 進むボタン
            var forwardButton = new ToolStripMenuItem("▶️ 進む");
            forwardButton.Click += (s, e) =>
            {
                var webView = GetCurrentWebView();
                if (webView != null && webView.CanGoForward) webView.GoForward();
            };

            // 更新ボタン
            var refreshButton = new ToolStripMenuItem("🔄 更新");
            refreshButton.Click += (s, e) =>
            {
                var webView = GetCurrentWebView();
                webView?.Reload();
            };

            // 新しいタブボタン
            var newTabButton = new ToolStripMenuItem("📂 新しいタブ");
            newTabButton.Click += (s, e) => AddNewTab("https://www.yahoo.co.jp/");

            // 履歴ボタン
            var historyButton = new ToolStripMenuItem("📜 履歴");
            historyButton.Click += (s, e) =>
            {
                string message = string.Join("\n", history);
                MessageBox.Show(message, "履歴");
            };

            // ブックマークボタン
            var bookmarkButton = new ToolStripMenuItem("⭐ ブックマーク追加");
            bookmarkButton.Click += (s, e) =>
            {
                var webView = GetCurrentWebView();
                if (webView != null)
                {
                    bookmarks.Add(webView.Source.ToString());
                    MessageBox.Show("ブックマークに追加しました", "ブックマーク");
                }
            };

            // ブックマーク一覧ボタン
            var showBookmarksButton = new ToolStripMenuItem("📚 ブックマーク一覧");
            showBookmarksButton.Click += (s, e) =>
            {
                string message = string.Join("\n", bookmarks);
                MessageBox.Show(message, "ブックマーク");
            };

            // メニューに追加
            menu.Items.Add(backButton);
            menu.Items.Add(forwardButton);
            menu.Items.Add(refreshButton);
            menu.Items.Add(newTabButton);
            menu.Items.Add(historyButton);
            menu.Items.Add(bookmarkButton);
            menu.Items.Add(showBookmarksButton);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }
    }
}

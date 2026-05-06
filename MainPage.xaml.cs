using System.Diagnostics;
using System.Text;
using MAUI_App.Utilities;
using QRCoder;

namespace MAUI_App
{
    public class FavoritesItem
    {
        public required string Url { get; set; }
    }

    [QueryProperty(nameof(Url), "url")]
    public partial class MainPage : ContentPage
    {

        FavoritesManager fManager = new FavoritesManager();
        HistoryManager historyManager;
        //Func<string, string> deleteProtocol = (string u) => (u.Replace("http://", "")).Replace("https://", "");
        public string Url
        {
            get; set;
        }

        public MainPage()
        {
            InitializeComponent();
            Url = "https://www.google.de";
            UrlInput.Text = Url;
            MainWebView.Source = Url;
            historyManager = new HistoryManager();
            NavigatedTo += MainPage_NavigatedTo;
            MainWebView.Navigated += MainWebView_Navigated;
            Update_Favorite_Button_Icon();
            historyManager.AddNewUrl(Url);

        }

        private void MainWebView_Navigated(object? sender, WebNavigatedEventArgs e)
        {
            if (e.NavigationEvent == WebNavigationEvent.NewPage)
            {
                historyManager.AddNewUrl(e.Url);
                UrlInput.Text = e.Url;
                Url = e.Url;
                Update_Favorite_Button_Icon();
            }
        }


        private void MainPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
        {
            if (!string.IsNullOrEmpty(Url))
            {
                ChangeWebsite(Url);
            }
            Update_Favorite_Button_Icon();
        }


        private void SecondPageBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SecondPage());
        }

        private void UrlInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Update_Favorite_Button_Icon()
        {
            var url = Url;
            faviBtn.Text = fManager.ContainsKey(url) ? "★" : "☆"; // Filled star for favorite, non-filled for not favorite
        }

        private void FaviBtn_Clicked(object sender, EventArgs e)
        {
            string currentURL = UrlInput.Text;
            if (fManager.ContainsKey(currentURL))
            {
                fManager.RemoveKey(currentURL);
            }
            else
            {
                fManager.AddKey(currentURL);
            }

            Update_Favorite_Button_Icon(); // Update the button icon after adding/removing favorite
        }

        private string Complete_URL_From_Short_URL(string url)
        {
            string newurl = "http://www.";
            if (!url.StartsWith("http"))
            {
                if (!url.StartsWith("www."))
                {
                    newurl += url;
                }
                else
                {
                    newurl += url.Substring(4);
                }
            }
            else
            {
                string tmp = (url.Replace("https://", "")).Replace("http://", "");
                if (tmp.StartsWith("www."))
                {
                    newurl += tmp.Substring(4);
                }
                else
                {
                    newurl += tmp;
                }
            }
            return newurl;
        }

        private void UrlInput_Completed(object sender, EventArgs e)
        {
            Url = Complete_URL_From_Short_URL(((Entry)sender).Text);
            ChangeWebsite(Url);
        }

        private void ChangeWebsite(string url)
        {
            string shorterURL = Complete_URL_From_Short_URL(url);
            MainWebView.Source = shorterURL;
            Url = shorterURL;
            UrlInput.Text = shorterURL;
            Update_Favorite_Button_Icon();
        }

        private void MainWebView_Loaded(object sender, WebNavigatedEventArgs e)
        {
            Update_Favorite_Button_Icon();
        }


        private void HistoryBackBtn_Clicked(object sender, EventArgs e)
        {
            string previousItem = historyManager.GetPrevious();
            if (previousItem == "") return;
            Url = previousItem;
            ChangeWebsite(Url);
        }

        private void HistoryForwardBtn_Clicked(object sender, EventArgs e)
        {
            string nextItem = historyManager.GetNext();
            if (nextItem == "") return;
            Url = nextItem;
            ChangeWebsite(Url);
        }
        private void QRBtn_Clicked(object sender, EventArgs e)
        {
            var payload = new PayloadGenerator.Url(Url);
            var data = QRCodeGenerator.GenerateQrCode(payload);
            var svg_text = new SvgQRCode(data).GetGraphic();
            var html = @"
                <!DOCTYPE html>
                <html lang=""de"">
                  <head>
                    <meta charset=""utf-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title></title>
                  </head>
                  <body>
                    <svg width=""50%"" height=""50%"" style=""margin:0 auto;"">
                    TMP
                    </svg>
                  </body>
                </html>
            ";
           
            html = html.Replace("TMP", svg_text);
            MainWebView.Source = new HtmlWebViewSource { Html = html };
        }
    }

}

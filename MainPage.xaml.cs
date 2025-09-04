using System.Diagnostics;
using System.Text.Json;

namespace MAUI_App
{
    public class FavoritesItem
    {
        public required string Url { get; set; }
    }

    [QueryProperty(nameof(Url), "url")]
    public partial class MainPage : ContentPage
    {

        private List<string> history = new List<string>();
        private int historyIndex = -1;
        public string Url
        {
            get;set;
        }


        public MainPage()
        {
            InitializeComponent();
            Url = "https://www.google.de";
            UrlInput.Text = (string)Url;
            MainWebView.Source = (string)Url;
            Append_History(Url);
            NavigatedTo += MainPage_NavigatedTo;
            UpdateFavoriteButtonIcon();
           
        }
        private void Append_History(string url)
        {
            history.Add(url);
            if (history.Count > 10)
            {
                history.RemoveAt(0);
            }
            historyIndex = history.Count - 1;
        }

        private string Get_Previous_History_Item()
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                return history[historyIndex];
            }
            return UrlInput.Text;
        }

        private string Get_Next_History_Item()
        {
            if (historyIndex < history.Count - 1)
            {
                historyIndex++;
                return history[historyIndex];
            }
            return UrlInput.Text;
        }

        // This is called automatically when Shell passes query parameters
        private void MainPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
        {

            // The 'Url' property is now populated with the query parameter

            // Update the UI only if a new URL was passed
            if (!string.IsNullOrEmpty(Url))
            {
                UrlInput.Text = Url;
                MainWebView.Source = Url;
            }
            UpdateFavoriteButtonIcon();
        }


        private void SecondPageBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SecondPage());
        }

        private void UrlInput_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void UpdateFavoriteButtonIcon()
        {
            var url = UrlInput.Text;
            if (Preferences.ContainsKey("favo"))
            {
                var urls = FavoritesManager.getUrls();

                var existingItem = urls.FirstOrDefault(i => i == url);
                faviBtn.Text = existingItem != null ? "★" : "☆"; // Filled star for favorite, non-filled for not favorite
            }
            else
            {
                faviBtn.Text = "☆"; // Default to non-filled star
            }
        }

        private void faviBtn_Clicked(object sender, EventArgs e)
        {
            var url = UrlInput.Text;

            FavoritesManager.Add_Key(url); 

            UpdateFavoriteButtonIcon(); // Update the button icon after adding/removing favorite
        }

        private void UrlInput_Completed(object sender, EventArgs e)
        {
            Url = ((Entry)sender).Text;
            MainWebView.Source = ((Entry)sender).Text;
            Append_History(Url);
            UpdateFavoriteButtonIcon(); // Update the button icon when URL changes
        }

    
        private void MainWebView_Loaded(object sender, WebNavigatedEventArgs e)
        {
            UrlInput.Text = e.Url;
        }

        private void historyBackBtn_Clicked(object sender, EventArgs e)
        {
            Url = Get_Previous_History_Item();
            UrlInput.Text = Url;
            MainWebView.Source = Url;
        }

        private void historyForwardBtn_Clicked(object sender, EventArgs e)
        {
            Url = Get_Next_History_Item();
            UrlInput.Text = Url;
            MainWebView.Source = Url;
        }
    }

}

using System.Diagnostics;
using MAUI_App.Utilities;

namespace MAUI_App;

public partial class SecondPage : ContentPage
{
    private FavoritesManager fmanager = new FavoritesManager();
    public SecondPage()
    {
        InitializeComponent();
        LoadPreferenceKeys();
        this.NavigatedTo += SecondPage_NavigatedTo;

    }

    private void SecondPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        LoadPreferenceKeys();
    }

    private void LoadPreferenceKeys()
    {
        // Retrieve all keys from Preferences  
        var keys = fmanager.GetURLS();

        // Create a VerticalStackLayout to hold the buttons  
        linksStackLay.Children.Clear();
        var stackLayout = linksStackLay;

        foreach (var key in keys)
        {
            var grid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star }, // Nimmt den restlichen Platz
                        new ColumnDefinition { Width = GridLength.Auto }  // Nur so breit wie das "x"
                    }
            };
            // Create a button for each key  

            var button = new Button
            {
                Text = key,
                Margin = new Thickness(0, 10),
                BackgroundColor = (Color)Application.Current.Resources["Gray100"],
                TextColor = (Color)Application.Current.Resources["OffBlack"],
            };
            var deleteButton = new Button
            {
                Text = "x",
                Margin = new Thickness(10, 10),
                BackgroundColor = Color.FromRgb(155, 20, 20),
            };

            // Add a click event handler for the button  
            button.Clicked += async (sender, e) =>
            {
                // Navigate to MainPage and pass the URL as a parameter  
                await Shell.Current.GoToAsync($"//MainPage?url={Uri.EscapeDataString(key)}");
            };

            deleteButton.Clicked += async (sender, e) =>
            {
                fmanager.RemoveKey(key);
                stackLayout.Children.Remove(grid);
            };
            // Positionierung im Grid
            Grid.SetColumn(button, 0);
            Grid.SetColumn(deleteButton, 1);
            grid.Children.Add(button);
            grid.Children.Add(deleteButton);
            // Add the button to the stack layout  
            stackLayout.Children.Add(grid);
        }
    }

}

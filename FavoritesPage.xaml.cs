using System.Diagnostics;

namespace MAUI_App;

public partial class SecondPage : ContentPage
{
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
        var keys = FavoritesManager.getUrls();

        // Create a VerticalStackLayout to hold the buttons  
        linksStackLay.Children.Clear();
        var stackLayout = linksStackLay;

        foreach (var key in keys)
        {
            // Create a button for each key  
            var button = new Button
            {
                Text = key,
            };
            button.Margin = new Thickness(0, 10);

            // Use colors defined in colors.xaml  
            button.BackgroundColor = (Color)Application.Current.Resources["Gray100"];
            button.TextColor = (Color)Application.Current.Resources["OffBlack"];

            // Add a click event handler for the button  
            button.Clicked += async (sender, e) =>
            {
                // Navigate to MainPage and pass the URL as a parameter  
                await Shell.Current.GoToAsync($"//MainPage?url={Uri.EscapeDataString(key)}");
            };

            // Add the button to the stack layout  
            stackLayout.Children.Add(button);
        }
    }
       

    private IEnumerable<string> GetAllPreferenceKeys()
    {
        // Use reflection to retrieve all keys from Preferences.Default
        var preferencesType = Preferences.Default.GetType();
        var keysProperty = preferencesType.GetProperty("Keys", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

        if (keysProperty != null && keysProperty.GetValue(Preferences.Default) is IEnumerable<string> keys)
        {
            return keys;
        }

        return Enumerable.Empty<string>();
    }
}

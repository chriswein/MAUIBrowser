using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Maui.Controls.Platform.Compatibility;


namespace MAUI_App.Utilities
{
    public class FavoritesManager
    {
        private const string _key = "favo";

        private static volatile List<FavoritesItem>? _buffer;
        private static readonly object _lock = new object();

        public static string DeleteScheme(string url)
        {
            if (url is null) return string.Empty;

            if (!url.StartsWith("http")) { url = $"http://{url}"; }
            try
            {
                var uri = new Uri(url);
                var uriToSave = uri.Host + uri.PathAndQuery;
                return uriToSave;
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        private List<FavoritesItem> GetItems()
        {

            List<FavoritesItem> items = new List<FavoritesItem>();
            if (_buffer is null) // Double locking pattern
            {
                lock (_lock)
                {
                    if (_buffer is null)
                    {

                        string items_string = Preferences.Get(_key, "");

                        try
                        {
                            var i = JsonSerializer.Deserialize<List<FavoritesItem>>(items_string);
                            if (i is not null) items = i;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("There was an error deserializing json");
                            Debug.WriteLine(ex);
                            Debug.WriteLine(items_string);
                        }
                        _buffer = items;
                    }

                }
            }
            return _buffer;
        }
        private void SetItems(List<FavoritesItem> items)
        {
            _buffer = items;
        }

        private void Save()
        {
            lock (_lock)
            {
                if (_buffer == null) return;
                var json = JsonSerializer.Serialize(_buffer);
                Preferences.Default.Set(_key, json);
            }
        }

        /// <summary>
        /// Get all items
        /// </summary>
        /// <returns>String array of all items</returns>
        public string[] GetURLS()
        {
            var items = GetItems().Select(i => i.Url);
            return items.ToArray();
        }

        /// <summary>
        /// Add key if not already added. Will init database if empty.
        /// </summary>
        /// <param name="url">Url identifying a website</param>
        public void AddKey(string url)
        {
            var item = new FavoritesItem { Url = DeleteScheme(url) };
            if (string.IsNullOrEmpty(item.Url)) return;

            var items = new List<FavoritesItem>();
            items = GetItems();
            lock (_lock)
            {
                var existingItem = items.FirstOrDefault(i => i.Url == item.Url);
                if (existingItem == null)
                {
                    items.Add(item);
                }
                SetItems(items);
                Save();
            }
        }

        /// <summary>
        /// Removes key if it exists. Right now it will delete double occurences as well.
        /// </summary>
        /// <param name="url">Url identifying a website</param>
        public void RemoveKey(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            var item = new FavoritesItem { Url = url };
            var items = new List<FavoritesItem>();
            url = DeleteScheme(url);
            if (Preferences.ContainsKey(_key))
            {
                items = GetItems();
                lock (_lock)
                {
                    var filteredItems = items.RemoveAll(i => i.Url.Equals(url));
                    SetItems(items);
                    Save();
                }
            }
        }

        /// <summary>
        /// Checks if key is in database. 
        /// </summary>
        /// <param name="url">Url identifying a website</param>
        /// <returns></returns>
        public bool ContainsKey(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            url = DeleteScheme(url);
            var items = GetItems();
            return items.Any(i => i.Url.Equals(url));
        }

    }
}

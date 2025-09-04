using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUI_App
{
    public class FavoritesManager
    {
        private static string _key = "favo";
        public static string[] getUrls()
        {
            string keys_string = Preferences.Get(_key, "");
            var keys = new List<string>();
            if (!string.IsNullOrEmpty(keys_string))
            {
                try
                {
                    var items = System.Text.Json.JsonSerializer.Deserialize<List<FavoritesItem>>(keys_string);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            keys.Add(item.Url);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("There was an error deserializing json");
                    Debug.WriteLine(ex);
                    Debug.WriteLine(keys_string);
                }
            }
            return keys.ToArray();
        }

        public static void Add_Key(string url)
        {
            Debug.WriteLine("Add/Remove favorite button clicked");
            var item = new FavoritesItem { Url = url };
            var items = new List<FavoritesItem>();
            if (Preferences.ContainsKey("favo"))
            {
                var json = Preferences.Get("favo", "{}");
                items = JsonSerializer.Deserialize<List<FavoritesItem>>(json) ?? new List<FavoritesItem>();

                var existingItem = items.FirstOrDefault(i => i.Url == url);
                if (existingItem != null)
                {
                    items.Remove(existingItem);
                    Preferences.Set("favo", JsonSerializer.Serialize(items));
                    Debug.WriteLine($"Removed from favorites: {url}");
                }
                else
                {
                    items.Add(item);
                    Preferences.Set("favo", JsonSerializer.Serialize(items));
                    Debug.WriteLine($"Added to favorites: {url}");
                }
            }
            else
            {
                items.Add(item);
                Preferences.Set("favo", JsonSerializer.Serialize(items));
                Debug.WriteLine($"Added to favorites: {url}");
            }

        }

    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Flipper.Classes
{
    /// <summary>
    /// Static class that handles the data request for items a player can use.
    /// </summary>
    public static class Items
    {
        /// <summary>
        /// A list of items that loads it's data from a json file.
        /// </summary>
        private static List<Item> _items;

        /// <summary>
        /// Class constructor.
        /// Checks if the item list is populated and will do so if the list is empty.
        /// </summary>
        static Items()
        {
            // Check if the private item list has not yet been set.
            if (_items == null)
            {
                // Get the executing Assembly.
                Assembly assembly = Assembly.GetExecutingAssembly();
                // Set the embedded resource path and name.
                string resourceName = "Flipper.Resources.items.json";
                // Create a string to hold the retrieved json.
                string json = string.Empty;

                // Use StreamReader to load the .json file into the target string.
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    json = reader.ReadToEnd();

                // Deserialize the json into the item list.
                _items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
        }

        /// <summary>
        /// Searches the itemlist for an item that contains the passed name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Item GetItem(string name)
        {
            return _items.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Searches the itemlist for an item that contains the passed id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetItem(ushort id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    /// <summary>
    /// POCO class that holds the Item data.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The Id of the Item.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// The Name of the ITem.
        /// </summary>
        public string Name { get; set; }
    }
}

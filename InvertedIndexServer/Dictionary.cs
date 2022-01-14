using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace InvertedIndexServer
{
    internal class Dictionary
    {
        string path;
        string searchKey;
        List<string> searchResult;

        public Dictionary(string path, string searchKey)
        {
            this.path = path;
            this.searchKey = searchKey;
        }
        static Dictionary<TItem, IEnumerable<TKey>> Invert<TKey, TItem>(Dictionary<TKey, IEnumerable<TItem>> dictionary)
        {
            return dictionary
                .SelectMany(keyValuePair => keyValuePair.Value.Select(item => new KeyValuePair<TItem, TKey>(item, keyValuePair.Key)))
                .GroupBy(keyValuePair => keyValuePair.Key)
                .ToDictionary(group => group.Key, group => group.Select(keyValuePair => keyValuePair.Value));
        }
        public void Search()
        {
            var key = searchKey.ToLower();
            var files = Directory.GetFiles(path);
            var dictionary = files.ToDictionary(
                    file => Path.GetFileName(file), 
                    file => File.ReadAllText(file).ToLower().Split().AsEnumerable()
                );

            try
            {
                searchResult = Invert(dictionary)[key].Distinct().ToList();
            }
            catch
            {
                return; // The given key was not found
            }
        }
        public List<string> GetResult()
        {
            return searchResult;
        }
    }
}
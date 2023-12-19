using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace json_xml_storing.database
{
    public class UniversalDatabase<T>
    {
        private List<T> data;
        private string filePath;

        public UniversalDatabase(string filePath)
        {
            this.filePath = filePath;
            this.LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                data = JsonSerializer.Deserialize<List<T>>(json);
            }
            else
            {
                data = new List<T>();
            }
        }

        private void SaveData()
        {
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }

        public void Add(T item)
        {
            data.Add(item);
            SaveData();
        }

        public T Get(int id)
        {
            // Example: Assuming T has a property named "Id"
            return data.FirstOrDefault(x => GetPropertyValue<int>(x, "Id") == id);
        }

        public void Update(int id, T updatedItem)
        {
            // Example: Assuming T has a property named "Id"
            var existingItem = data.FirstOrDefault(x => GetPropertyValue<int>(x, "Id") == id);
            if (existingItem != null)
            {
                UpdateProperties(existingItem, updatedItem);
            }
            SaveData();
        }

        public void Delete(int id)
        {
            // Example: Assuming T has a property named "Id"
            var itemToRemove = data.FirstOrDefault(x => GetPropertyValue<int>(x, "Id") == id);
            if (itemToRemove != null)
            {
                data.Remove(itemToRemove);
            }
            SaveData();
        }

        public List<T> Filter(Func<T, bool> predicate)
        {
            return data.Where(predicate).ToList();
        }

        private void UpdateProperties(T target, T source)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(target, sourceValue);
            }
        }

        private TValue GetPropertyValue<TValue>(T obj, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return property != null ? (TValue)property.GetValue(obj) : default(TValue);
        }
    }
}

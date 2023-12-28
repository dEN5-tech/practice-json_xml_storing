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
            //возвращяет значение по  айди  
            return data.FirstOrDefault(x => GetPropertyValue<int>(x, "Id") == id);
        }

        public void Update(int id, T updatedItem)
        {
            // обновляет поля по айди 
            var existingItem = data.FirstOrDefault(x => GetPropertyValue<int>(x, "Id") == id);
            if (existingItem != null)
            {
                UpdateProperties(existingItem, updatedItem);
            }
            SaveData();
        }

        public void Delete(int id)
        {
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

        // получает поля из базового типа, устанавливает новые 
        private void UpdateProperties(T target, T source)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(target, sourceValue);
            }
        }

        // Получает значение свойства объекта по его имени.
        // Если свойство существует, возвращает его значение.
        // В противном случае возвращает значение по умолчанию для типа свойства.
        private TValue GetPropertyValue<TValue>(T obj, string propertyName)
        {
            // Получаем информацию о свойстве по его имени.
            var property = typeof(T).GetProperty(propertyName);

            // Если свойство существует, возвращаем его значение.
            if (property != null)
            {
                return (TValue)property.GetValue(obj);
            }

            // В противном случае возвращаем значение по умолчанию для типа TValue.
            return default(TValue);
        }

    }
}

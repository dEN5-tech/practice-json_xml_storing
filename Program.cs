using json_xml_storing.database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace json_xml_storing
{
    // Часть 1: Сериализация и десериализация JSON

    // Часть 1.1: Создание класса Person
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Person Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Person>(json);
        }
    }


    // Часть 2: Чтение и запись XML данных

    // Часть 2.1: Определение структуры XML документа
    class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }

    class Library
    {
        private List<Book> books = new List<Book>();

        public void AddBook(Book book)
        {
            books.Add(book);
        }

        public IEnumerable<Book> GetBooks()
        {
            return books;
        }
    }

    // Часть 3.2.1: Создание класса Employee
    class Employee
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public int Salary { get; set; }
    }

    class Program
    {
        static void Main()
        {

            // Часть 1.2: Создание консольного приложения
            List<Person> people = new List<Person>
        {
            new Person { Name = "John", Age = 25, Address = "123 Main St" },
            new Person { Name = "Alice", Age = 30, Address = "456 Oak St" },
            // Добавьте еще несколько объектов по вашему усмотрению
        };

            // Сериализация
            string json = JsonSerializer.Serialize(people);

            // Сохранение в файл
            File.WriteAllText("people.json", json);

            // Десериализация
            string jsonFromFile = File.ReadAllText("people.json");
            List<Person> deserializedPeople = JsonSerializer.Deserialize<List<Person>>(jsonFromFile);

            // Вывод на консоль
            foreach (var person in deserializedPeople)
            {
                Console.WriteLine($"Name: {person.Name}, Age: {person.Age}, Address: {person.Address}");
            }



            // Часть 2.2: Реализация классов Book и Library
            Library library = new Library();
            library.AddBook(new Book { Title = "Название книги 1", Author = "Автор 1", Year = 2000 });
            library.AddBook(new Book { Title = "Название книги 2", Author = "Автор 2", Year = 2005 });
            // Добавьте еще книги по вашему усмотрению

            // Часть 2.3: Код для чтения и записи XML данных
            using (XmlWriter writer = XmlWriter.Create("library.xml"))
            {
                writer.WriteStartElement("Library");
                foreach (var book in library.GetBooks())
                {
                    writer.WriteStartElement("Book");
                    writer.WriteElementString("Title", book.Title);
                    writer.WriteElementString("Author", book.Author);
                    writer.WriteElementString("Year", book.Year.ToString());
                    writer.WriteEndElement(); // закрываем элемент Book
                }
                writer.WriteEndElement(); // закрываем элемент Library
            }

            using (XmlReader reader = XmlReader.Create("library.xml"))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Book")
                    {
                        var book = new Book();

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "Title":
                                        book.Title = reader.ReadElementContentAsString();
                                        break;
                                    case "Author":
                                        book.Author = reader.ReadElementContentAsString();
                                        break;
                                    case "Year":
                                        book.Year = reader.ReadElementContentAsInt();
                                        break;
                                }
                            }

                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Book")
                            {
                                library.AddBook(book);
                                break;
                            }
                        }
                    }
                }
            }
            // Вывод на консоль
            foreach (var book in library.GetBooks())
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}");
            }


            // Часть 3.2: Написание программы для фильтрации и вывода данных



            // Часть 3.2.2: Загрузка данных из файла
            string employeesFileContent = File.ReadAllText("employees.json");
            List<Employee> employeesList = JsonSerializer.Deserialize<List<Employee>>(employeesFileContent);

            // Часть 3.2.3: Задание порогового значения
            int salaryThreshold = 60000;

            // Часть 3.2.4: Фильтрация данных с использованием LINQ
            var filteredEmployees = employeesList.Where(e => e.Salary > salaryThreshold).ToList();

            // Часть 3.2.5: Вывод отфильтрованных данных на консоль
            foreach (var employee in filteredEmployees)
            {
                Console.WriteLine($"Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}");
            }

            // Часть 4: Создание и использование XML схемы

            // Часть 4.2: Использование XML схемы для валидации

            // Часть 4.2.1: Создание XmlSchemaSet
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, "library.xsd");

            // Часть 4.2.2: Настройка XmlReaderSettings для включения валидации
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = schemaSet;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidationCallback;

            // Часть 4.2.3: Чтение XML данных с использованием валидации
            using (XmlReader reader = XmlReader.Create("library.xml", settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Book")
                    {
                        var book = new Book();

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "Title":
                                        book.Title = reader.ReadElementContentAsString();
                                        break;
                                    case "Author":
                                        book.Author = reader.ReadElementContentAsString();
                                        break;
                                    case "Year":
                                        book.Year = reader.ReadElementContentAsInt();
                                        break;
                                }
                            }

                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Book")
                            {
                                library.AddBook(book);
                                break;
                            }
                        }
                    }
                }
            }

            // Часть 4.2.4: Обработчик событий для вывода сообщений об ошибках валидации
            void ValidationCallback(object sender, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    Console.WriteLine($"Validation error: {e.Message}");
                }
            }

            // Часть : Разработка библиотеки файловой БД
            UniversalDatabase<Book> bookDatabase = new UniversalDatabase<Book>("books.json");

            Book newBook = new Book { Title = "Sample Book", Author = "John Doe", Year = 2023 };
            bookDatabase.Add(newBook);
            Book retrievedBook = bookDatabase.Get(1);
            bookDatabase.Update(1, new Book { Title = "Updated Book", Author = "Jane Doe", Year = 2023 });
            bookDatabase.Delete(1);

            List<Book> filteredBooks = bookDatabase.Filter(book => book.Year > 2000);

            Console.WriteLine(filteredBooks[0].Year);

        }
    }
}


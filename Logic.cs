using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace TestTask2.Model
{
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s) // Запись в файл
        {
            Dictionary<int, ListNode> Dic = new Dictionary<int, ListNode>(); // Для присвоения ключа каждому элементу
            ListNode _thisNode = Head; // Для начала раздачи ID
            string _writingLine = ""; // Записывающая строка, означающая 1 ListRand в формате string

            // Раздача ключей ID
            for (int i = 0; i < Count; i++)
            {
                Dic.Add(i, _thisNode);
                _thisNode = _thisNode.Next;
            }

            // Создание связей на уровне ID
            for (int i = 0; i < Count; i++)
            {
                _writingLine += string.Format("{0} {1} {2} {3} {4}|", i, Dic.FirstOrDefault(x => x.Value == Dic[i].Prev).Key,
                    Dic.FirstOrDefault(x => x.Value == Dic[i].Next).Key, Dic.FirstOrDefault(x => x.Value == Dic[i].Rand).Key,
                    Dic[i].Data);
            }

            // Запись
            byte[] array = Encoding.Default.GetBytes(_writingLine);
            try {
                s.Write(array, 0, array.Length);
            }
            catch (Exception e) {
                MessageBox.Show("Ошибка записи файла: {0}", e.Message);
            }
        }

        public void Deserialize(FileStream s) // Загрузка файла
        {
            try {
                // Чтение файла
                byte[] array = new byte[s.Length];
                s.Read(array, 0, array.Length);
                string _readedLine = Encoding.Default.GetString(array);

                //Разбиение на строки
                string[] _stringNodes = _readedLine.Split('|');
                // Для присвоения ключа каждому элементу
                Dictionary<int, ListNode> Dic = new Dictionary<int, ListNode>();

                //Для инициализации каждого элемента
                ListNode _thisNode; // "Буферный" объект
                string[] ListNodeFields; // Значение полей конкретного элемента ListNode в формате string

                // для собственной инициализации
                for (int i = 0; i < _stringNodes.Length; i++)
                {
                    _thisNode = new ListNode();
                    ListNodeFields = _stringNodes[i].Split();
                    _thisNode.Data = ListNodeFields[4];
                    Dic.Add(int.Parse(ListNodeFields[i]), _thisNode);
                }

                //для создания связей других элементов с данным элементом
                for (int i = 0; i < _stringNodes.Length; i++)
                {
                    ListNodeFields = _stringNodes[i].Split();
                    _thisNode = Dic[i];
                    _thisNode.Prev = Dic.FirstOrDefault(x => x.Key == int.Parse(ListNodeFields[1])).Value;
                    _thisNode.Next = Dic.FirstOrDefault(x => x.Key == int.Parse(ListNodeFields[2])).Value;
                    _thisNode.Rand = Dic.FirstOrDefault(x => x.Key == int.Parse(ListNodeFields[3])).Value;
                }
                Head = Dic[0];
                Count = _stringNodes.Length;
                Tail = Dic[Count];
            }
            catch(Exception e) {
                MessageBox.Show("Ошибка чтения файла: {0}", e.Message);
            }
        }
    }
}

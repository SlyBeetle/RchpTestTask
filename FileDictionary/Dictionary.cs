/* Автор: Пальчевский Андрей Ю.
 * e-mail: timplayer@tut.by
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Dictionary
{
    /// <summary>
    /// Представляет собой словарь пар:
    /// ключ - имя param-файла,
    /// значение - множество подходящих require-файлов.
    /// </summary>
    public class FileDictionary
    {
        /// <summary>
        /// baseAttribute задаёт атрибут,
        /// по которому определяется какие param-файлы подходят к require-файлам.
        /// </summary>
        const String baseAttribute = "name";
        /// <summary>
        /// requiredParamFileDictionary хранит пары:
        /// ключ - require-файл с требованиями,
        /// значение - множество param-файлов, удовлетворяющих требованиям.
        /// </summary>
        Dictionary<String, HashSet<String>> requiredParamFileDictionary;
        /// <summary>
        /// Возвращает массив строк с именами
        /// param-файлов, каждый из которых содержит все значения базовых атрибутов
        /// require-файла.
        /// </summary>
        /// <param name="nameOfRequireFile">Имя require-файла.</param>
        /// <returns>
        /// Массив строк с именами
        /// param-файлов, каждый из которых содержит все значения базовых атрибутов
        /// require-файла.
        /// </returns>
        public String[] GetSuitableParamFiles(ref String nameOfRequireFile)
        {
            // Если словарь пуст возвращаем null и завершаем метод.
            if (requiredParamFileDictionary.Count == 0)
            {
                return null;
            }
            /* Если словарь не пуст, то пытаемся получить множество param файлов,
             * удовлетворяющих требованиям require-файла с именем nameOfRequireFile,
             * чтобы вернуть его как результат работы метода.
             * В случае неудачной попытки возвращаем null.
             */
            try
            {
                /* namesOfSuitableParamFiles - массив param-файлов,
                 * которые подходят к require-файлу.
                 */
                String[] namesOfSuitableParamFiles = new String[requiredParamFileDictionary[nameOfRequireFile].Count];
                requiredParamFileDictionary[nameOfRequireFile].CopyTo(namesOfSuitableParamFiles);
                return namesOfSuitableParamFiles;
            }
            /* Если param-файлу с именем nameOfRequireFile не подходит ни один param-файл,
             * то возвращаем null.
             */
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
        /// <summary>
        /// Формирует словарь пар:
        /// ключ - имя require-файла,
        /// значение - множество подходящих param-файлов.
        /// 
        /// Выбрасывает библиотечные исключения IOException, UnauthorizedAccessException,
        /// ArgumentException, ArgumentNullException, PathTooLongException,
        /// DirectoryNotFoundException.
        /// </summary>
        /// <param name="nameOfParamDirectory">Имя директории с param-файлами.</param>
        /// <param name="nameOfRequireDirectory">Имя директории с require-файлами.</param>
        public FileDictionary(ref String nameOfParamDirectory, ref String nameOfRequireDirectory)
        {
            /* Строим отображение значения базового атрибута в множество param-файлов,
             * которые содержат этот атрибут. */
            #region
            {
                /* attributeFileDictionary хранит пары:
                 * ключ - значения атрибут baseAttribute,
                 * значение - множество param-файлов, которые содержат этот атрибут.
                 */
                Dictionary<String, HashSet<String>> attributeFileDictionary = new Dictionary<String, HashSet<String>>();
                // paramFileEntries - массив имен param-файлов из param-директории.
                String[] paramFileEntries = Directory.GetFiles(nameOfParamDirectory);
                /* Формируем словарь attributeFileDictionary, ставя в соответствие
                 * каждому значению базовых атрибутов param-файла имя этого файла.
                 */
                foreach (String paramFileName in paramFileEntries)
                {
                    XmlTextReader reader = new XmlTextReader(paramFileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    if (reader.Read())
                    {
                        // rootTagName запоминает имя корневого тега.
                        String rootTagName = reader.Name;
                        reader.Read();
                        /* Извлекаем значение базового атрибута у всех тегов,
                         * расположенных внутри корневого.
                         */
                        while (reader.Name != rootTagName)
                        {
                            String attributeValue = reader.GetAttribute(baseAttribute);
                            /* Пытаемся добавить имя param-файла к множеству param-файлов,
                             * которые содержат базовый атрибут со значением attributeValue.
                             */
                            try
                            {
                                attributeFileDictionary[attributeValue].Add(paramFileName);
                            }
                            /* Если не получилось добавить имя param-файла к множеству, 
                             * значит необходимо создать множество и добавить его в словарь.
                             */
                            catch (KeyNotFoundException)
                            {
                                /* paramFileNamesSet - множество имен param-файлов,
                                 * которые содержат базовый атрибут со значением attributeValue.
                                 */
                                HashSet<String> paramFileNamesSet = new HashSet<String>();
                                paramFileNamesSet.Add(paramFileName);
                                attributeFileDictionary.Add(attributeValue, paramFileNamesSet);
                            }
                            reader.Read();
                        };
                    }
                }
            }
            #endregion
            /* Строим отображение имени require-файла в множество param-файлов,
             * которые подходят к данному require-файлу. */
            #region
            {
                requiredParamFileDictionary = new Dictionary<String, HashSet<String>>();
                // requireFileEntries - массив имен require-файлов из require-директории.
                String[] requireFileEntries = Directory.GetFiles(nameOfRequireDirectory);
                // Каждому require-файлу ставим в соответствие множество подходящих param-файлов.
                foreach (String requireFileName in requireFileEntries)
                {
                    /* SuitableParamFilesSet - множесто param-файлов,
                     * которые подходят к require-файлу с именем requireFileName.
                     */
                    HashSet<String> SuitableParamFilesSet = new HashSet<String>();
                    XmlTextReader reader = new XmlTextReader(requireFileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    if (reader.Read())
                    {
                        // rootTagName запоминает имя корневого тега.
                        String rootTagName = reader.Name;
                        reader.Read();
                        /* Пытаемся получать множества param-файлов, содержащих значение
                         * базового атрибута require-файла. Если получить множество не удалось,
                         * то значит не один param-файл не подходит под требования require-файла.
                         */
                        try
                        {
                            if (reader.Name != rootTagName)
                            {
                                String attributeValue = reader.GetAttribute(baseAttribute);
                                // suitableParamFiles - массив имен param-файлов, подходящих под требования require-файла.
                                String[] suitableParamFiles = new String[attributeFileDictionary[attributeValue].Count];
                                /* Выполняем глубокое копирование множества param-файлов,
                                 * содержащих значение базового атрибута attributeValue.
                                 * Его необходимо выполнять для корректной работы алгоритма.
                                 */
                                attributeFileDictionary[attributeValue].CopyTo(suitableParamFiles);
                                SuitableParamFilesSet.UnionWith(suitableParamFiles);
                                reader.Read();
                                /* Получаем множество имен param-файлов, которые
                                 * содержат все значения базового атрибута baseAttribute
                                 * require-файла.
                                 */
                                while (reader.Name != rootTagName)
                                {
                                    attributeValue = reader.GetAttribute(baseAttribute);
                                    SuitableParamFilesSet.IntersectWith(attributeFileDictionary[attributeValue]);
                                    reader.Read();
                                };
                            }
                            else
                            {
                                SuitableParamFilesSet.UnionWith(paramFileEntries);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            SuitableParamFilesSet.Clear();
                        }
                    }
                    SuitableParamFilesSet.TrimExcess();
                    requiredParamFileDictionary.Add(requireFileName, SuitableParamFilesSet);
                }
            }
            #endregion
        }
    }
}

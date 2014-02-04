using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace IsolatedStorageDeserializator
{
    public class IsolatedStorageDeserializer<T>
    {
        public bool XmlSerialize(T obj, string fileName, bool isDeleteBeforeSerialize, string pathToFile = "")
        {
            IsolatedStorageFileStream stream;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (pathToFile != "")
                if (!storage.DirectoryExists(pathToFile))
                    storage.CreateDirectory(pathToFile);

            if (isDeleteBeforeSerialize)
                if (storage.FileExists(pathToFile + "\\" + fileName + ".xml"))
                    storage.DeleteFile(pathToFile + "\\" + fileName + ".xml");

            stream = storage.OpenFile(pathToFile + "\\" + fileName + ".xml", FileMode.OpenOrCreate);
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(stream, obj);

                stream.Close();
                stream.Dispose();
            }
            catch
            {
                stream.Close();
                stream.Dispose();
                return false;
            }
            return true;

        }

        public T XmlDeserialize(string fileName, string pathToFile = "")
        {
            T obj;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (pathToFile != "")
                if (!storage.DirectoryExists(pathToFile))
                    storage.CreateDirectory(pathToFile);

            IsolatedStorageFileStream stream =
            storage.OpenFile(pathToFile + "\\" + fileName + ".xml", FileMode.OpenOrCreate);

            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                obj = (T)xml.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                return obj;
            }
            catch (Exception e)
            {
                e.ToString();
                stream.Close();
                stream.Dispose();
                return default(T);
            }
        }

        public bool XmlDCSerialize(T obj, string fileName, bool isDeleteBeforeSerialize, string pathToFile = "")
        {
            IsolatedStorageFileStream stream;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (pathToFile != "")
                if (!storage.DirectoryExists(pathToFile))
                    storage.CreateDirectory(pathToFile);

            if (isDeleteBeforeSerialize)
                if (storage.FileExists(pathToFile + "\\" + fileName + ".xml"))
                    storage.DeleteFile(pathToFile + "\\" + fileName + ".xml");

            stream = storage.OpenFile(pathToFile + "\\" + fileName + ".xml", FileMode.OpenOrCreate);
            try
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(stream, obj);
                stream.Close();
                stream.Dispose();
            }
            catch
            {
                stream.Close();
                stream.Dispose();
                return false;
            }
            return true;

        }

        public T XmlDCDeserialize(string fileName, string pathToFile = "")
        {
            T obj;
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (pathToFile != "")
                if (!storage.DirectoryExists(pathToFile))
                    storage.CreateDirectory(pathToFile);

            IsolatedStorageFileStream stream =
            storage.OpenFile(pathToFile + "\\" + fileName + ".xml", FileMode.OpenOrCreate);

            try
            {
                XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings());
                //var reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);

                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                obj = (T)ser.ReadObject(xmlReader, true);
                xmlReader.Close();
                stream.Close();
                stream.Dispose();
                return obj;
            }
            catch (Exception e)
            {
                e.ToString();
                stream.Close();
                stream.Dispose();
                return default(T);
            }
        }

    }
}


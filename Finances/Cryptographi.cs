using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO.IsolatedStorage;
using System.IO;
using IsolatedStorageDeserializator;

namespace Finances
{
    public class Cryptographi
    {
        public static bool isRightPassword(string str)
        {
            var ISD = new IsolatedStorageDeserializer<UserData>();
            UserData userData = ISD.XmlDeserialize("UserData");
            if (GetSHA256Hash(str) == userData.Password)
                return true;
            else return false;
        }

        public static void SaveNewPass(string str)
        {
            UserData userData = new UserData { Password =  GetSHA256Hash(str) };
            var ISD = new IsolatedStorageDeserializer<UserData>();
            ISD.XmlSerialize(userData, "UserData", true);
        }

        public static void DeletePass()
        {
            UserData userData = new UserData { Password = "" };
            var ISD = new IsolatedStorageDeserializer<UserData>();
            ISD.XmlSerialize(userData, "UserData", true);
        }

        public static bool IsSetPass()
        {
            var ISD = new IsolatedStorageDeserializer<UserData>();
            UserData userData = ISD.XmlDeserialize("UserData");
            try
            {
                if (userData.Password == "")
                    return false;
                else
                    return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static string GetSHA256Hash(string str)
        {
            byte[] data = StringToAscii(str);
            var result = new SHA256Managed().ComputeHash(data);
            string returnedString = BitConverter.ToString(result).Replace("-", "").ToLower();
            return returnedString;      
        }

        public static byte[] StringToAscii(string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }           
    }
}

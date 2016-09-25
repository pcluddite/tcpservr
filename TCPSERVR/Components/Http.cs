// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Tcpservr
{
    public class Http
    {
        private Request request;
        private Stream stream;
        private string getAddr;
        private string filePath = null;

        public Http(Stream nStream, string msg, string downloadableFile)
        {
            Request tmp;
            int space = msg.IndexOf(' ');
            if (space > 0 && Enum.TryParse(msg.Remove(space), out tmp)) {
                request = tmp;
            }
            else {
                request = Request.ERROR;
            }
            stream = nStream;
            getAddr = msg.Substring(space, msg.IndexOf("HTTP/1") - space - 1);
            filePath = downloadableFile;
        }

        public void SendWebpage()
        {
            byte[] data;
            switch (request) {
                case Request.GET:
                    data = AnswerGet(getAddr);
                    break;
                case Request.HEAD:
                    data = Encoding.UTF8.GetBytes(AnswerGetHead());
                    break;
                case Request.BREW:
                    data = Encoding.UTF8.GetBytes(RESPONSE_BREW);
                    break;
                default:
                    data = Encoding.UTF8.GetBytes(RESPONSE_NOTALLOWED);
                    break;
            }
            stream.Write(data, 0, data.Length);
        }

        private byte[] AnswerGet(string path)
        {
            if (!path.Trim().Equals("/")) {
                return Get(path);
            }

            if (string.IsNullOrEmpty(filePath)) {
                return Encoding.UTF8.GetBytes(RESPONSE_FORBIDDEN);
            }
            else if (File.Exists(filePath) &&
                (filePath.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))) {
                return Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n" +
                                              "Content-Type: text/html\r\n\r\n" + File.ReadAllText(filePath));
            }
            else {
                return Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n" +
                                              "Content-Type: text/html\r\n\r\n" +
                                              "<html><body>A file is available for download:<br/><a href=\"" + filePath + "\">Download Link</a></body></html>");
            }
        }



        private byte[] Get(string relativePath)
        {
            try {
                relativePath = relativePath.Trim();
                if (!relativePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)) {
                    return Encoding.UTF8.GetBytes(RESPONSE_NOTFOUND);
                }
                string path = Path.GetFullPath("." + relativePath.Replace("/", "\\"));
                if (path.EndsWith(".htm") || path.EndsWith(".html")) {
                    return Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n" +
                                                  "Content-Type: text/html\r\n\r\n" + File.ReadAllText(path));
                }
                else {
                    List<byte> data = new List<byte>();
                    data.AddRange(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n" +
                                                         "Content-Type: application/octet-stream\r\n" +
                                                         "Content-Transfer-Encoding: binary\r\n\r\n"));
                    data.AddRange(File.ReadAllBytes(path));
                    return data.ToArray();
                }
            }
            catch (Exception ex) {
                return Encoding.UTF8.GetBytes(RESPONSE_ERROR +
                                              "<html><body>500 Internal Server Error: " + ex.Message + "</body></html>");
            }
        }

        private string AnswerGetHead()
        {
            return Regex.Split(Encoding.UTF8.GetString(AnswerGet("/")), "\r\n\r\n")[0];
        }

        private enum Request
        {
            GET, HEAD, BREW, ERROR
        }

        private const string RESPONSE_NOTFOUND = "HTTP/1.1 404 Not Found\r\n" +
                                                 "Content-Type: text/html\r\n\r\n" +
                                                 "<html><body bgcolor=\"pink\">404 Not Found</body></html>";

        private const string RESPONSE_FORBIDDEN = "HTTP/1.1 403 Forbidden\r\n" +
                                                  "Content-Type: text/html\r\n\r\n" +
                                                  "<html><body bgcolor=\"pink\">403 Forbidden</body></html>";

        private const string RESPONSE_NOTALLOWED = "HTTP/1.1 405 Method Not Allowed\r\n" +
                                              "Content-Type: text/html\r\n\r\n" +
                                              "<html><body bgcolor=\"pink\">405 Method Not Allowed</body></html>";

        private const string RESPONSE_ERROR = "HTTP/1.1 500 Internal Server Error\r\n" +
                                              "Content-Type: text/html\r\n\r\n";

        private const string RESPONSE_BREW = "HTTP/1.1 418 I'm a teapot\r\n" +
                                             "Content-Type: text/html\r\n\r\n" +
                                             "<html><body bgcolor=\"pink\">418 I'm a teapot</body></html>";
    }
}
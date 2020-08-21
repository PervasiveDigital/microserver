﻿using System;
using System.Diagnostics;

using Bytewizer.TinyCLR.Http;
using Bytewizer.TinyCLR.Sockets;
using Bytewizer.TinyCLR.Http.Header;
using System.IO;
using System.Text;

namespace Bytewizer.TinyCLR.WebServer
{
    public class HttpResponse : Middleware
    {
        protected override void Invoke(HttpContext context, RequestDelegate next)
        {
            try
            {
                //DebugHeaders(context);

                string response = "<doctype !html><html><head><meta http-equiv='refresh' content='5'><title>Hello, world!</title>" +
                                  "<style>body { background-color: #111 } h1 { font-size:2cm; text-align: center; color: white;}</style></head>" +
                                  "<body><h1>" + DateTime.Now.Ticks.ToString() + "</h1></body></html>";

                if (context.Request.Path.Equals("/index.html"))
                {
                    context.Response.Write(response, "text/html; charset=UTF-8");
                }
                else
                {
                    context.Response.Write(string.Empty, "text/html; charset=UTF-8", StatusCodes.Status404NotFound, Encoding.UTF8);
                }

                next(context);
            }
            catch (Exception ex)
            {
                // try to manage all unhandled exceptions in the pipeline
                Debug.WriteLine($"Unhandled exception message: { ex.Message } StackTrace: {ex.StackTrace}");
            }
        }

        private void DebugHeaders(HttpContext context)
        {
            Debug.WriteLine(string.Empty);
            
            foreach (HeaderValue header in context.Request.Headers)
            {
                Debug.WriteLine($"{header.Key}={header.Value}");
            }
        }
    }
}
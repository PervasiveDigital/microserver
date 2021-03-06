﻿using System;
using System.IO;
using System.Diagnostics;

using GHIElectronics.TinyCLR.IO;

using Bytewizer.TinyCLR.Pipeline;

namespace Bytewizer.TinyCLR.Http
{
    /// <summary>
    /// Enables serving static files for a given request path.
    /// </summary>
    public class StaticFileMiddleware : Middleware
    {
        private readonly StaticFileOptions _options;
        private readonly IDriveProvider _driveProvider;
        private readonly IContentTypeProvider _contentTypeProvider;

        /// <summary>
        /// Initializes a default instance of the <see cref="StaticFileMiddleware"/> class.
        /// </summary>
        public StaticFileMiddleware()
            : this(new StaticFileOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileMiddleware"/> class.
        /// </summary>
        /// <param name="options">The <see cref="StaticFileOptions"/> used to configure the middleware.</param>
        public StaticFileMiddleware(StaticFileOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _driveProvider = _options.DriveProvider;
            _contentTypeProvider = _options.ContentTypeProvider ?? new DefaultContentTypeProvider();
        }

        /// <summary>
        /// Processes a request to determine if it matches a known file and if so serves it.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> that encapsulates all HTTP-specific information about an individual HTTP request.</param>
        protected override void Invoke(HttpContext context, RequestDelegate next)
        {
            var matchUrl = context.Request.Path;

            if (!string.IsNullOrEmpty(matchUrl))
            {
                if (!ValidateMethod(context))
                {
                    var method = context.Request.Method;
                    Debug.WriteLine($"The request method {method} are not supported");
                }
                else if (!ValidatePath(matchUrl, out var subPath))
                {
                    var path = context.Request.Path;
                    Debug.WriteLine($"The request path {path} does not match the path filter");
                }
                else if (!LookupContentType(_contentTypeProvider, _options, subPath, out var contentType))
                {
                    var path = context.Request.Path;
                    Debug.WriteLine($"The request path {path} does not match a supported file type");
                }
                else
                {
                    // If we get here we can try to serve the file
                    TryServeStaticFile(context, contentType, subPath);

                    return;
                }
            }

            next(context);
        }

        private void TryServeStaticFile(HttpContext context, string contentType, string subPath)
        {
            if (!LookupFileInfo(subPath, out var lastModified))
            {
                Debug.WriteLine($"The request path {subPath} does not match an existing file");
            }
            else
            {
                // If we get here we can try to serve the file
                ServeStaticFile(context, contentType, subPath, lastModified);
            }
        }

        private static readonly object _lock = new object();

        private void ServeStaticFile(HttpContext context, string contentType, string subPath, DateTime lastModified)
        {
            //TODO: Implement If-Ranged
            var modifiedSince = context.Request.Headers.IfModifiedSince;

            if (modifiedSince < lastModified)
            {
                var driveName = _driveProvider?.Name ?? string.Empty;
                var fullPath = $@"{driveName}{subPath}";
                var filename = Path.GetFileName(subPath);

                context.Response.Headers.LastModified = lastModified.ToString("R");
                context.Response.Headers.ContentDisposition = $"inline; filename={filename}";
                context.Response.ContentType = contentType;
                context.Response.StatusCode = StatusCodes.Status200OK;

                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                context.Response.ContentLength = stream.Length;
                if (context.Request.Method == HttpMethods.Get)
                {
                    context.Response.Body = stream;
                }
                else
                {
                    stream.Close();
                    stream.Dispose();
                }
            }

            context.Response.StatusCode = StatusCodes.Status304NotModified;
        }

        private static bool ValidateMethod(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Get ||
                context.Request.Method == HttpMethods.Head)
            {
                return true;
            }

            return false;
        }

        private static bool ValidatePath(string matchUrl, out string subPath)
        {
            subPath = matchUrl.Replace("/", Path.DirectorySeparatorChar.ToString()).Split('?')[0];

            if (string.IsNullOrEmpty(subPath))
            {
                return false;
            }

            return true;
        }

        private bool LookupFileInfo(string subPath, out DateTime lastModified)
        {
            var fileInfo = new FileInfo(subPath);

            if (fileInfo.Exists)
            {
                var last = fileInfo.LastAccessTimeUtc;
                lastModified = last.AddTicks(-(last.Ticks % TimeSpan.TicksPerSecond));

                return fileInfo.Exists;
            }

            lastModified = DateTime.MinValue;
            return fileInfo.Exists;
        }
        private static bool LookupContentType(
            IContentTypeProvider contentTypeProvider,
            StaticFileOptions options,
            string subPath,
            out string contentType)
        {
            if (contentTypeProvider.TryGetContentType(subPath, out contentType))
            {
                return true;
            }

            if (options.ServeUnknownFileTypes)
            {
                contentType = options.DefaultContentType;
                return true;
            }

            return false;
        }
    }
}
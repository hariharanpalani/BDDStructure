using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace Mvc.EmbeddedViewsDemo
{
    public class EmbeddedViewEngine : RazorViewEngine
    {
        private const string AssemblyName = "Mvc.Plugin";

        public EmbeddedViewEngine()
        {
            ParseViews();
        }
        private void ParseViews()
        {
            var assembly = Assembly.Load(AssemblyName);
            var manifestResourceNames = assembly.GetManifestResourceNames().Where(name => name.EndsWith(".cshtml") || name.EndsWith(".config"));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                var nameWithAssemblyName = manifestResourceName.Replace(AssemblyName + ".", "");
                var lastIndex = nameWithAssemblyName.LastIndexOf(".", StringComparison.Ordinal);
                var extension = nameWithAssemblyName.Substring(lastIndex + 1);
                nameWithAssemblyName = nameWithAssemblyName.Substring(0, lastIndex);
                lastIndex = nameWithAssemblyName.LastIndexOf('.');
                var fileName = nameWithAssemblyName.Substring(lastIndex + 1);
                nameWithAssemblyName = nameWithAssemblyName.Substring(0, lastIndex).Replace(".", "/");

                var rootPath = HttpContext.Current.Server.MapPath(@"~/" + nameWithAssemblyName);
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                using (var stream = assembly.GetManifestResourceStream(manifestResourceName))
                {
                    using (var file = new FileStream(rootPath + @"\" + fileName + "." + extension, FileMode.Create))
                    {
                        stream.CopyTo(file);
                    } 
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Runtime.Caching;

namespace Cuemon.Web
{
    /// <summary>
    /// This utility class is designed to make Multipurpose Internet Mail Extensions (MIME) related operations easier to work with.
    /// </summary>
    public static class MimeUtility
    {
        private static readonly IReadOnlyCollection<FileMapping> KnownMimeTypes = GetKnownMimeTypes();

        /// <summary>
        /// Gets a read-only collection of the most common MIME types registered.
        /// </summary>
        /// <value>The the most common MIME types registered.</value>
        public static IReadOnlyCollection<FileMapping> MimeTypes
        {
            get { return KnownMimeTypes; }
        }

        /// <summary>
        /// Converts the specified <paramref name="extensions"/> to its equivalent sequence of <see cref="FileMapping"/> object.
        /// </summary>
        /// <param name="extensions">The file extensions to parse against <see cref="MimeTypes"/>.</param>
        /// <returns>A sequence of <see cref="FileMapping"/> objects matching the specified <paramref name="extensions"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="extensions"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has a length of zero.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has at least one invalid value.
        /// </exception>
        public static IEnumerable<FileMapping> ParseFileExtensions(params string[] extensions)
        {
            return ParseFileExtensions(MimeTypes, extensions);
        }

        /// <summary>
        /// Converts the specified <paramref name="extensions"/> to its equivalent sequence of <see cref="FileMapping"/> object.
        /// </summary>
        /// <param name="extensions">The file extensions to parse against <see cref="MimeTypes"/>.</param>
        /// <returns>A sequence of <see cref="FileMapping"/> objects matching the specified <paramref name="extensions"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="extensions"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has a length of zero.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has at least one invalid value.
        /// </exception>
        public static IEnumerable<FileMapping> ParseFileExtensions(IEnumerable<string> extensions)
        {
            return ParseFileExtensions(EnumerableConverter.ToArray(extensions));
        }

        /// <summary>
        /// Converts the specified <paramref name="extensions"/> to its equivalent sequence of <see cref="FileMapping"/> object.
        /// </summary>
        /// <param name="mimeTypes">The MIME types to use as source for the <paramref name="extensions"/>.</param>
        /// <param name="extensions">The file extensions to parse against <paramref name="mimeTypes"/>.</param>
        /// <returns>A sequence of <see cref="FileMapping"/> objects matching the specified <paramref name="extensions"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="extensions"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has a length of zero.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has at least one invalid value.
        /// </exception>
        public static IEnumerable<FileMapping> ParseFileExtensions(IEnumerable<FileMapping> mimeTypes, params string[] extensions)
        {
            if (extensions == null) { throw new ArgumentNullException(nameof(extensions)); }
            if (extensions.Length == 0) { throw new ArgumentOutOfRangeException(nameof(extensions), "At least one file extension must be specified."); }
            if (!HasExtension(extensions)) { throw new ArgumentOutOfRangeException(nameof(extensions), "At least one file extension seems to be invalid."); }

            Doer<IEnumerable<FileMapping>, IList<string>, IEnumerable<FileMapping>> parseMimeTypeCore = CachingManager.Cache.Memoize<IEnumerable<FileMapping>, IList<string>, IEnumerable<FileMapping>>(ParseMimeTypeCore, TimeSpan.FromMinutes(20));
            return parseMimeTypeCore(mimeTypes, new List<string>(extensions));
        }

        private static IEnumerable<FileMapping> ParseMimeTypeCore(IEnumerable<FileMapping> mimeTypes, IList<string> extensions)
        {
            foreach (string extension in extensions)
            {
                foreach (FileMapping mimeType in mimeTypes)
                {
                    foreach (string fileExtension in mimeType.Extensions)
                    {
                        if (fileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            yield return mimeType;
                            break;
                        }
                    }
                }
            }
        }

        private static bool HasExtension(IEnumerable<string> extensions)
        {
            foreach (string extension in extensions)
            {
                if (!Path.HasExtension(extension)) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Converts the specified <paramref name="contentType"/> to its equivalent <see cref="FileMapping"/> object.
        /// </summary>
        /// <param name="contentType">The content-type of a file to parse against <see cref="MimeTypes"/>.</param>
        /// <returns>A <see cref="FileMapping"/> object equivalent to the specified <paramref name="contentType"/>.</returns>
        /// <exception cref="System.ArgumentNullException">contentType
        /// <paramref name="contentType"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="contentType"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="contentType"/> has zero or more than one forward slash (/).
        /// </exception>
        public static FileMapping ParseContentType(string contentType)
        {
            return ParseContentType(MimeTypes, contentType);
        }

        /// <summary>
        /// Converts the specified <paramref name="contentType"/> to its equivalent <see cref="FileMapping"/> object.
        /// </summary>
        /// <param name="mimeTypes">The MIME types to use as source for the <paramref name="contentType"/>.</param>
        /// <param name="contentType">The content-type of a file to parse against <paramref name="mimeTypes"/>.</param>
        /// <returns>A <see cref="FileMapping"/> object equivalent to the specified <paramref name="contentType"/>.</returns>
        /// <exception cref="System.ArgumentNullException">contentType
        /// <paramref name="contentType"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="contentType"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="contentType"/> has zero or more than one forward slash (/).
        /// </exception>
        public static FileMapping ParseContentType(IEnumerable<FileMapping> mimeTypes, string contentType)
        {
            if (contentType == null) { throw new ArgumentNullException(nameof(contentType)); }
            if (contentType.Length == 0) { throw new ArgumentEmptyException(nameof(contentType)); }
            if (StringUtility.Count(contentType, '/') != 1) { throw new ArgumentException("A content-type must have one forward slash character (/) as it is composed of a top-level media type followed by a subtype identifier, eg. text/plain.", nameof(contentType)); }

            Doer<IEnumerable<FileMapping>, string, FileMapping> parseContentTypeCore = CachingManager.Cache.Memoize<IEnumerable<FileMapping>, string, FileMapping>(ParseMimeTypeCore, TimeSpan.FromMinutes(20));
            return parseContentTypeCore(mimeTypes, contentType);
        }

        private static FileMapping ParseMimeTypeCore(IEnumerable<FileMapping> mimeTypes, string contentType)
        {
            int indexOfSemicolon = contentType.IndexOf(';');
            string tempContentType = indexOfSemicolon > 0 ? contentType.Substring(0, indexOfSemicolon).Trim() : contentType.Trim();
            foreach (FileMapping mimeType in mimeTypes)
            {
                if (mimeType.ContentType.Equals(tempContentType, StringComparison.OrdinalIgnoreCase)) { return mimeType; }
            }
            return null;
        }

        private static IReadOnlyCollection<FileMapping> GetKnownMimeTypes()
        {
            // todo: ask nblapi.net for a list of mime-types
            // otherwise - fallback

            List<FileMapping> mappings = new List<FileMapping>();
            mappings.Add(new FileMapping("text/h323", ".323"));
            mappings.Add(new FileMapping("application/octet-stream", ".*", ".aaf", ".aca", ".afm", ".asd", ".asi", ".bin", ".cab", ".chm", ".csv", ".cur", ".deploy", ".dsp", ".dwp", ".emz", ".eot", ".exe", ".fla", ".hhk", ".hhp", ".ics", ".inf", ".java", ".jpb", ".lpk", ".lzh", ".mdp", ".mix", ".msi", ".mso", ".ocx", ".pcx", ".pcz", ".pfb", ".pfm", ".prm", ".prx", ".psd", ".psm", ".psp", ".qxd", ".rar", ".sea", ".smi", ".snp", ".thn", ".toc", ".ttf", ".u32", ".xsn", ".xtp"));
            mappings.Add(new FileMapping("application/msaccess", ".accdb", ".accde", ".accdt"));
            mappings.Add(new FileMapping("application/internet-property-stream", ".acx"));
            mappings.Add(new FileMapping("application/postscript", ".ai", ".eps", ".ps"));
            mappings.Add(new FileMapping("application/xhtml+xml", ".xhtml", ".xhtm", ".xht"));
            mappings.Add(new FileMapping("application/xml", ".xml"));
            mappings.Add(new FileMapping("audio/x-aiff", ".aif"));
            mappings.Add(new FileMapping("audio/aiff", ".aifc", ".aiff"));
            mappings.Add(new FileMapping("application/x-ms-application", ".application"));
            mappings.Add(new FileMapping("image/x-jg", ".art"));
            mappings.Add(new FileMapping("video/x-ms-asf", ".asf", ".asr", ".asx", ".nsc"));
            mappings.Add(new FileMapping("text/plain", ".asm", ".bas", ".c", ".cnf", ".cpp", ".h", ".map", ".txt", ".vcs", ".xdr"));
            mappings.Add(new FileMapping("application/atom+xml", ".atom"));
            mappings.Add(new FileMapping("image/vnd.microsoft.icon", ".ico"));
            mappings.Add(new FileMapping("audio/basic", ".au", ".snd"));
            mappings.Add(new FileMapping("video/x-msvideo", ".avi"));
            mappings.Add(new FileMapping("application/olescript", ".axs"));
            mappings.Add(new FileMapping("application/x-bcpio", ".bcpio"));
            mappings.Add(new FileMapping("image/bmp", ".bmp", ".dib"));
            mappings.Add(new FileMapping("application/vnd.ms-office.calx", ".calx"));
            mappings.Add(new FileMapping("application/vnd.ms-pki.seccat", ".cat"));
            mappings.Add(new FileMapping("application/x-cdf", ".cdf"));
            mappings.Add(new FileMapping("application/x-java-applet", ".class"));
            mappings.Add(new FileMapping("application/x-msclip", ".clp"));
            mappings.Add(new FileMapping("image/x-cmx", ".cmx"));
            mappings.Add(new FileMapping("image/cis-cod", ".cod"));
            mappings.Add(new FileMapping("application/x-cpio", ".cpio"));
            mappings.Add(new FileMapping("application/x-mscardfile", ".crd"));
            mappings.Add(new FileMapping("application/pkix-crl", ".crl"));
            mappings.Add(new FileMapping("application/x-x509-ca-cert", ".crt", ".der"));
            mappings.Add(new FileMapping("application/x-csh", ".csh"));
            mappings.Add(new FileMapping("text/css", ".css", ".less"));
            mappings.Add(new FileMapping("application/json", ".json"));
            mappings.Add(new FileMapping("application/ld+json", ".jsonld"));
            mappings.Add(new FileMapping("application/manifest+json", ".json", ".webapp"));
            mappings.Add(new FileMapping("application/rdf+xml", ".rdf"));
            mappings.Add(new FileMapping("application/rss+xml", ".rss", ".xml"));
            mappings.Add(new FileMapping("application/schema+json", ".json", ".schema"));
            mappings.Add(new FileMapping("application/vnd.geo+json", ".json", ".geojson"));
            mappings.Add(new FileMapping("application/vnd.ms-fontobject", ".eot"));
            mappings.Add(new FileMapping("application/x-font-ttf", ".ttf"));
            mappings.Add(new FileMapping("application/x-web-app-manifest+json", ".json", ".webapp"));
            mappings.Add(new FileMapping("application/x-director", ".dcr", ".dir", ".dxr"));
            mappings.Add(new FileMapping("text/xml", ".disco", ".dll.config", ".dtd", ".exe.config", ".mno", ".vml", ".wsdl", ".xml", ".xsd", ".xsf", ".xsl", ".xslt"));
            mappings.Add(new FileMapping("application/x-msdownload", ".dll"));
            mappings.Add(new FileMapping("text/dlm", ".dlm"));
            mappings.Add(new FileMapping("application/msword", ".doc", ".dot"));
            mappings.Add(new FileMapping("application/vnd.ms-word.document.macroenabled.12", ".docm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx"));
            mappings.Add(new FileMapping("application/vnd.ms-word.template.macroenabled.12", ".dotm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.wordprocessingml.template", ".dotx"));
            mappings.Add(new FileMapping("application/x-dvi", ".dvi"));
            mappings.Add(new FileMapping("drawing/x-dwf", ".dwf"));
            mappings.Add(new FileMapping("message/rfc822", ".eml", ".mht", ".mhtml", ".nws"));
            mappings.Add(new FileMapping("text/x-setext", ".etx"));
            mappings.Add(new FileMapping("application/envoy", ".evy"));
            mappings.Add(new FileMapping("application/vnd.fdf", ".fdf"));
            mappings.Add(new FileMapping("application/fractals", ".fif"));
            mappings.Add(new FileMapping("x-world/x-vrml", ".flr", ".wrl", ".wrz", ".xaf", ".xof"));
            mappings.Add(new FileMapping("video/x-flv", ".flv"));
            mappings.Add(new FileMapping("image/gif", ".gif"));
            mappings.Add(new FileMapping("application/x-gtar", ".gtar"));
            mappings.Add(new FileMapping("application/x-gzip", ".gz"));
            mappings.Add(new FileMapping("application/x-hdf", ".hdf"));
            mappings.Add(new FileMapping("text/x-hdml", ".hdml"));
            mappings.Add(new FileMapping("application/x-oleobject", ".hhc"));
            mappings.Add(new FileMapping("application/winhlp", ".hlp"));
            mappings.Add(new FileMapping("application/mac-binhex40", ".hqx"));
            mappings.Add(new FileMapping("application/hta", ".hta"));
            mappings.Add(new FileMapping("text/x-component", ".htc"));
            mappings.Add(new FileMapping("text/html", ".htm", ".html", ".hxt"));
            mappings.Add(new FileMapping("text/webviewhtml", ".htt"));
            mappings.Add(new FileMapping("image/x-icon", ".ico"));
            mappings.Add(new FileMapping("image/ief", ".ief"));
            mappings.Add(new FileMapping("application/x-iphone", ".iii"));
            mappings.Add(new FileMapping("application/x-internet-signup", ".ins", ".isp"));
            mappings.Add(new FileMapping("video/x-ivf", ".ivf"));
            mappings.Add(new FileMapping("application/java-archive", ".jar"));
            mappings.Add(new FileMapping("application/liquidmotion", ".jck", ".jcz"));
            mappings.Add(new FileMapping("image/pjpeg", ".jfif"));
            mappings.Add(new FileMapping("image/jpeg", ".jpe", ".jpeg", ".jpg"));
            mappings.Add(new FileMapping("application/javascript", ".js"));
            mappings.Add(new FileMapping("application/x-javascript", ".js"));
            mappings.Add(new FileMapping("text/jscript", ".jsx"));
            mappings.Add(new FileMapping("application/x-latex", ".latex"));
            mappings.Add(new FileMapping("application/x-ms-reader", ".lit"));
            mappings.Add(new FileMapping("video/x-la-asf", ".lsf", ".lsx"));
            mappings.Add(new FileMapping("application/x-msmediaview", ".m13", ".m14", ".mvb"));
            mappings.Add(new FileMapping("video/mpeg", ".m1v", ".mp2", ".mpa", ".mpe", ".mpeg", ".mpg", ".mpv2"));
            mappings.Add(new FileMapping("audio/x-mpegurl", ".m3u"));
            mappings.Add(new FileMapping("application/x-troff-man", ".man"));
            mappings.Add(new FileMapping("application/x-ms-manifest", ".manifest"));
            mappings.Add(new FileMapping("application/x-msaccess", ".mdb"));
            mappings.Add(new FileMapping("application/x-troff-me", ".me"));
            mappings.Add(new FileMapping("audio/mid", ".mid", ".midi", ".rmi"));
            mappings.Add(new FileMapping("application/x-smaf", ".mmf"));
            mappings.Add(new FileMapping("application/x-msmoney", ".mny"));
            mappings.Add(new FileMapping("video/quicktime", ".mov", ".qt"));
            mappings.Add(new FileMapping("video/x-sgi-movie", ".movie"));
            mappings.Add(new FileMapping("audio/mpeg", ".mp3"));
            mappings.Add(new FileMapping("application/vnd.ms-project", ".mpp"));
            mappings.Add(new FileMapping("application/x-troff-ms", ".ms"));
            mappings.Add(new FileMapping("application/x-miva-compiled", ".mvc"));
            mappings.Add(new FileMapping("application/x-netcdf", ".nc"));
            mappings.Add(new FileMapping("application/oda", ".oda"));
            mappings.Add(new FileMapping("text/x-ms-odc", ".odc"));
            mappings.Add(new FileMapping("application/oleobject", ".ods"));
            mappings.Add(new FileMapping("application/onenote", ".one", ".onea", ".onetoc", ".onetoc2", ".onetmp", ".onepkg"));
            mappings.Add(new FileMapping("application/opensearchdescription+xml", ".osdx"));
            mappings.Add(new FileMapping("application/pkcs10", ".p10"));
            mappings.Add(new FileMapping("application/x-pkcs12", ".p12", ".pfx"));
            mappings.Add(new FileMapping("application/x-pkcs7-certificates", ".p7b", ".spc"));
            mappings.Add(new FileMapping("application/pkcs7-mime", ".p7c", ".p7m"));
            mappings.Add(new FileMapping("application/x-pkcs7-certreqresp", ".p7r"));
            mappings.Add(new FileMapping("application/pkcs7-signature", ".p7s"));
            mappings.Add(new FileMapping("image/x-portable-bitmap", ".pbm"));
            mappings.Add(new FileMapping("application/pdf", ".pdf"));
            mappings.Add(new FileMapping("image/x-portable-graymap", ".pgm"));
            mappings.Add(new FileMapping("application/vnd.ms-pki.pko", ".pko"));
            mappings.Add(new FileMapping("application/x-perfmon", ".pma", ".pmc", ".pml", ".pmr", ".pmw"));
            mappings.Add(new FileMapping("image/png", ".png", ".pnz"));
            mappings.Add(new FileMapping("image/x-portable-anymap", ".pnm"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint", ".pot", ".pps", ".ppt"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint.template.macroenabled.12", ".potm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.presentationml.template", ".potx"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint.addin.macroenabled.12", ".ppam"));
            mappings.Add(new FileMapping("image/x-portable-pixmap", ".ppm"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint.slideshow.macroenabled.12", ".ppsm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.presentationml.slideshow", ".ppsx"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint.presentation.macroenabled.12", ".pptm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx"));
            mappings.Add(new FileMapping("application/pics-rules", ".prf"));
            mappings.Add(new FileMapping("application/x-mspublisher", ".pub"));
            mappings.Add(new FileMapping("application/x-quicktimeplayer", ".qtl"));
            mappings.Add(new FileMapping("audio/x-pn-realaudio", ".ra", ".ram"));
            mappings.Add(new FileMapping("image/x-cmu-raster", ".ras"));
            mappings.Add(new FileMapping("image/vnd.rn-realflash", ".rf"));
            mappings.Add(new FileMapping("image/x-rgb", ".rgb"));
            mappings.Add(new FileMapping("application/vnd.rn-realmedia", ".rm"));
            mappings.Add(new FileMapping("application/x-troff", ".roff", ".t", ".tr"));
            mappings.Add(new FileMapping("audio/x-pn-realaudio-plugin", ".rpm"));
            mappings.Add(new FileMapping("application/rtf", ".rtf"));
            mappings.Add(new FileMapping("text/richtext", ".rtx"));
            mappings.Add(new FileMapping("application/x-msschedule", ".scd"));
            mappings.Add(new FileMapping("text/scriptlet", ".sct"));
            mappings.Add(new FileMapping("application/set-payment-initiation", ".setpay"));
            mappings.Add(new FileMapping("application/set-registration-initiation", ".setreg"));
            mappings.Add(new FileMapping("text/sgml", ".sgml"));
            mappings.Add(new FileMapping("application/x-sh", ".sh"));
            mappings.Add(new FileMapping("application/x-shar", ".shar"));
            mappings.Add(new FileMapping("application/x-stuffit", ".sit"));
            mappings.Add(new FileMapping("application/vnd.ms-powerpoint.slide.macroenabled.12", ".sldm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.presentationml.slide", ".sldx"));
            mappings.Add(new FileMapping("audio/x-smd", ".smd", ".smx", ".smz"));
            mappings.Add(new FileMapping("application/futuresplash", ".spl"));
            mappings.Add(new FileMapping("application/x-wais-source", ".src"));
            mappings.Add(new FileMapping("application/streamingmedia", ".ssm"));
            mappings.Add(new FileMapping("application/vnd.ms-pki.certstore", ".sst"));
            mappings.Add(new FileMapping("application/vnd.ms-pki.stl", ".stl"));
            mappings.Add(new FileMapping("image/svg+xml", ".svg", ".svgz"));
            mappings.Add(new FileMapping("application/x-sv4cpio", ".sv4cpio"));
            mappings.Add(new FileMapping("application/x-sv4crc", ".sv4crc"));
            mappings.Add(new FileMapping("application/x-shockwave-flash", ".swf"));
            mappings.Add(new FileMapping("application/x-tar", ".tar"));
            mappings.Add(new FileMapping("application/x-tcl", ".tcl"));
            mappings.Add(new FileMapping("application/x-tex", ".tex"));
            mappings.Add(new FileMapping("application/x-texinfo", ".texi", ".texinfo"));
            mappings.Add(new FileMapping("application/x-compressed", ".tgz"));
            mappings.Add(new FileMapping("application/vnd.ms-officetheme", ".thmx"));
            mappings.Add(new FileMapping("image/tiff", ".tif", ".tiff"));
            mappings.Add(new FileMapping("application/x-msterminal", ".trm"));
            mappings.Add(new FileMapping("text/tab-separated-values", ".tsv"));
            mappings.Add(new FileMapping("text/iuls", ".uls"));
            mappings.Add(new FileMapping("application/x-ustar", ".ustar"));
            mappings.Add(new FileMapping("text/vbscript", ".vbs"));
            mappings.Add(new FileMapping("text/x-vcard", ".vcf"));
            mappings.Add(new FileMapping("application/vnd.ms-visio.viewer", ".vdx"));
            mappings.Add(new FileMapping("application/vnd.visio", ".vsd", ".vss", ".vst", ".vsw", ".vsx", ".vtx"));
            mappings.Add(new FileMapping("application/x-ms-vsto", ".vsto"));
            mappings.Add(new FileMapping("audio/wav", ".wav"));
            mappings.Add(new FileMapping("audio/x-ms-wax", ".wax"));
            mappings.Add(new FileMapping("image/vnd.wap.wbmp", ".wbmp"));
            mappings.Add(new FileMapping("application/vnd.ms-works", ".wcm", ".wdb", ".wks", ".wps"));
            mappings.Add(new FileMapping("video/x-ms-wm", ".wm"));
            mappings.Add(new FileMapping("audio/x-ms-wma", ".wma"));
            mappings.Add(new FileMapping("application/x-ms-wmd", ".wmd"));
            mappings.Add(new FileMapping("application/x-msmetafile", ".wmf"));
            mappings.Add(new FileMapping("text/vnd.wap.wml", ".wml"));
            mappings.Add(new FileMapping("application/vnd.wap.wmlc", ".wmlc"));
            mappings.Add(new FileMapping("text/vnd.wap.wmlscript", ".wmls"));
            mappings.Add(new FileMapping("application/vnd.wap.wmlscriptc", ".wmlsc"));
            mappings.Add(new FileMapping("video/x-ms-wmp", ".wmp"));
            mappings.Add(new FileMapping("video/x-ms-wmv", ".wmv"));
            mappings.Add(new FileMapping("video/x-ms-wmx", ".wmx"));
            mappings.Add(new FileMapping("application/x-ms-wmz", ".wmz"));
            mappings.Add(new FileMapping("application/font-woff", ".woff"));
            mappings.Add(new FileMapping("application/x-mswrite", ".wri"));
            mappings.Add(new FileMapping("video/x-ms-wvx", ".wvx"));
            mappings.Add(new FileMapping("application/directx", ".x"));
            mappings.Add(new FileMapping("application/xaml+xml", ".xaml"));
            mappings.Add(new FileMapping("application/x-silverlight-app", ".xap"));
            mappings.Add(new FileMapping("application/x-ms-xbap", ".xbap"));
            mappings.Add(new FileMapping("image/x-xbitmap", ".xbm"));
            mappings.Add(new FileMapping("application/vnd.ms-excel", ".xla", ".xlc", ".xlm", ".xls", ".xlt", ".xlw"));
            mappings.Add(new FileMapping("application/vnd.ms-excel.addin.macroenabled.12", ".xlam"));
            mappings.Add(new FileMapping("application/vnd.ms-excel.sheet.binary.macroenabled.12", ".xlsb"));
            mappings.Add(new FileMapping("application/vnd.ms-excel.sheet.macroenabled.12", ".xlsm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx"));
            mappings.Add(new FileMapping("application/vnd.ms-excel.template.macroenabled.12", ".xltm"));
            mappings.Add(new FileMapping("application/vnd.openxmlformats-officedocument.spreadsheetml.template", ".xltx"));
            mappings.Add(new FileMapping("image/x-xpixmap", ".xpm"));
            mappings.Add(new FileMapping("application/vnd.ms-xpsdocument", ".xps"));
            mappings.Add(new FileMapping("image/x-xwindowdump", ".xwd"));
            mappings.Add(new FileMapping("application/x-compress", ".z"));
            mappings.Add(new FileMapping("application/x-zip-compressed", ".zip"));
            return new ReadOnlyCollection<FileMapping>(mappings);
        }
    }
}

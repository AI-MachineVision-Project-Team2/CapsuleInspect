using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CapsuleInspect
{
    public static class ImageSaveHelper
    {
        public enum Category
        {
            OK,
            NG_Scratch,
            NG_PrintDefect,
            NG_Crack,
            NG_Squeeze
        }

        public static void EnsureDirectories()
        {
            Directory.CreateDirectory(@"c:\model\ok\Image");
            Directory.CreateDirectory(@"c:\model\ng_scratch\Image");
            Directory.CreateDirectory(@"c:\model\ng_printdefect\Image");
            Directory.CreateDirectory(@"c:\model\ng_crack\Image");
            Directory.CreateDirectory(@"c:\model\ng_squeeze\Image");
        }

        public static string SavePng(OpenCvSharp.Mat img, Category category, string fileNameNoExt, bool useDateFolder = false)
        {
            if (img == null) throw new ArgumentNullException(nameof(img));
            EnsureDirectories();

            string dir = GetTargetDir(category);
            if (useDateFolder)
            {
                dir = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd"));
                Directory.CreateDirectory(dir);
            }

            string safe = MakeSafeFileName(fileNameNoExt);
            string pathNoExt = Path.Combine(dir, safe);
            string finalPath = GetNonConflictingPath(pathNoExt, ".png");

            img.SaveImage(finalPath);
            return finalPath;
        }

        private static string GetTargetDir(Category category)
        {
            switch (category)
            {
                case Category.OK:
                    return @"c:\model\ok\Image";
                case Category.NG_Scratch:
                    return @"c:\model\ng_scratch\Image";
                case Category.NG_PrintDefect:
                    return @"c:\model\ng_printdefect\Image";
                case Category.NG_Crack:
                    return @"c:\model\ng_crack\Image";
                case Category.NG_Squeeze:
                    return @"c:\model\ng_squeeze\Image";
                default:
                    return @"c:\model";
            }
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return string.IsNullOrWhiteSpace(name) ? "image" : name.Trim();
        }

        private static string GetNonConflictingPath(string pathNoExt, string extWithDot)
        {
            string candidate = pathNoExt + extWithDot;
            int idx = 1;
            while (File.Exists(candidate))
                candidate = $"{pathNoExt} ({idx++}){extWithDot}";
            return candidate;
        }
    }
}

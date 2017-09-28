using ImageMagick;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DocumentManager
{
    public class ImportCompletedEventArgs : EventArgs
    {
        public string imgTempFile { get; private set; }
        public List<string> pdfFileList  { get; private set; }
        public ImportCompletedEventArgs(string f, List<string> pdfList)
        {
            imgTempFile = f;
            if (pdfList != null)
                pdfFileList = new List<string>(pdfList);
        }
    }

    public class OverlayEventArgs : EventArgs
    {
        public DataRow dr { get; private set; }
        public float rotateDegrees { get; private set; }
        public OverlayEventArgs(DataRow r, float d)
        {
            this.dr = r;
            this.rotateDegrees = d;
        }
    }

    public class LicenseEventArgs : EventArgs
    {
        public string licenseTo { get; set; }
        public string companyName { get; set; }
        public string licenseKey { get; set; }
        public LicenseEventArgs(string l, string c, string k)
        {
            this.licenseTo = l;
            this.companyName = c;
            this.licenseKey = k;
        }
    }

    public class DocEvents
    {
        public event EventHandler<ImportCompletedEventArgs> ImportCompleted;
        public event EventHandler<OverlayEventArgs> OverlayCompleted;
        public event EventHandler<LicenseEventArgs> LicenseCompleted;

        public void ImportAsync(string[] files)
        {
            foreach (string s in files)
            {
                if (ImportCompleted != null)
                {
                    this.ImportCompleted(this, new ImportCompletedEventArgs(s,null));
                }
            }
        }

        public void ImportPDF(string[] files)
        {
            List<string> f = new List<string>();

            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(150, 150);
            
            //settings.Format = MagickFormat.Png;

            using (MagickImageCollection images = new MagickImageCollection())
            {
                images.Read(files[0], settings);
                int page = 1;
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                foreach (MagickImage image in images)
                {
                    string tempPath = Path.GetTempPath();
                    string tFile = Path.Combine(tempPath, "docManager"+ dateTime + "_" + page + ".jpeg");
                    image.Write(tFile);
                    f.Add(tFile);
                    page++;
                }
            }

            foreach (string s in f)
            {
                if (ImportCompleted != null)
                {
                    this.ImportCompleted(this, new ImportCompletedEventArgs(s,f));
                }
            }
        }

        public void RenderWithOverlay(DataRow r, float rotateDegrees)
        {
            this.OverlayCompleted(this, new OverlayEventArgs(r, rotateDegrees));
        }

        public void LicenseKeyValidate(string l, string c, string k)
        {
            this.LicenseCompleted(this, new LicenseEventArgs(l,c,k));
        }
    }

}

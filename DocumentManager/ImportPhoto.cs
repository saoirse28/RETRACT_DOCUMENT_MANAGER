using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DocumentManager
{
    public class ImportCompletedEventArgs : EventArgs
    {
        public string imgTempFile { get; private set; }
        public ImportCompletedEventArgs(string f)
        {
            imgTempFile = f;
        }
    }

    public class OverlayEventArgs : EventArgs
    {
        public DataRow dataRow64 { get; private set; }
        public float rotateDegrees { get; private set; }
        public OverlayEventArgs(DataRow r, float d)
        {
            this.dataRow64 = r;
            this.rotateDegrees = d;
        }
    }

    public class ImportPhoto
    {
        public event EventHandler<ImportCompletedEventArgs> ImportCompleted;
        public event EventHandler<OverlayEventArgs> OverlayCompleted;

        public void ImportAsync(string[] files)
        {
            foreach (string s in files)
            {
                if (ImportCompleted != null)
                {
                    this.ImportCompleted(this, new ImportCompletedEventArgs(s));
                }
            }
        }

        public void Base64WithOverlay(DataRow r, float rotateDegrees)
        {
            this.OverlayCompleted(this, new OverlayEventArgs(r, rotateDegrees));
        }
    }

}

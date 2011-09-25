using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Drawing
{
    public class Bitmap
    {
        public string FilenameForWPTextureResource
        {
            get
            {
                return _filename;
            }
        }
        internal bool gdihandleready = false;
        internal string _filename;
        public Bitmap(string filename)
        {
            _filename = filename;
        }
    }
}

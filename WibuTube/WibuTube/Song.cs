using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WibuTube
{
    public class Song : ISong
    {
        public string Tittle { get ; set ; }
        public string[] Performers { get; set; }
        public string Album { get; set; }
    }
}

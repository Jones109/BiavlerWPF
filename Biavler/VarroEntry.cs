using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biavler
{
    
    public class VarroEntry
    {
        private string _bistadeId;
        private DateTime _datoTid;
        private int _antalMider;
        private string _bemærkninger;

        public VarroEntry()
        {
            _datoTid=DateTime.Now;
        }

        public VarroEntry(string bistadeId, DateTime datoTid, int antalMider, string bemærkninger)
        {
            _bistadeId = bistadeId;
            _datoTid = datoTid;
            _antalMider = antalMider;
            _bemærkninger = bemærkninger;
        }


        public string BistadeId
        {
            get { return _bistadeId;}
            set { _bistadeId = value; }
        }
        public DateTime DatoTid
        {
            get { return _datoTid;}
            set { _datoTid = value;
                
            }
        }

        public int AntalMider
        {
            get { return _antalMider;}
            set { _antalMider = value; }
        }
        public string Bemærkninger
        {
            get { return _bemærkninger; }

            set { _bemærkninger = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOCISA.Models
{
    public class MesajExtended
    {
        public Mesaj Mesaj { get; set; }
        public Nomenclator TipMesaj { get; set; }
        public Utilizator Sender { get; set; }
        public Utilizator[] Receivers { get; set; }
        public Dosar Dosar { get; set; }
        public bool selected { get; set; }

        public MesajExtended() { }

        public MesajExtended(Mesaj m)
        {
            this.Mesaj = m;
            this.TipMesaj = (Nomenclator)m.GetTipMesaj().Result;
            this.Sender = (Utilizator)m.GetSender().Result;
            this.Receivers = (Utilizator[])m.GetReceivers().Result;
            this.Dosar = (Dosar)m.GetDosar().Result;
            this.selected = false;
        }

        public MesajExtended(Mesaj m, bool _selected)
        {
            this.Mesaj = m;
            this.TipMesaj = (Nomenclator)m.GetTipMesaj().Result;
            this.Sender = (Utilizator)m.GetSender().Result;
            this.Receivers = (Utilizator[])m.GetReceivers().Result;
            this.Dosar = (Dosar)m.GetDosar().Result;
            this.selected = _selected;
        }
    }
}

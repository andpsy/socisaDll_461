using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOCISA.Models
{
    public class DosarExtended
    {
        public Dosar Dosar { get; set; }
        public Asigurat AsiguratCasco { get; set; }
        public Asigurat AsiguratRca { get; set; }
        public SocietateAsigurare SocietateCasco { get; set; }
        public SocietateAsigurare SocietateRca { get; set; }
        public Auto AutoCasco { get; set; }
        public Auto AutoRca { get; set; }
        public Intervenient Intervenient { get; set; }
        public Nomenclator TipDosar { get; set; }
        public bool selected { get; set; }

        public DosarExtended() { }

        public DosarExtended(Dosar d)
        {
            this.Dosar = d;
            this.AsiguratCasco = (Asigurat)d.GetAsiguratCasco().Result;
            this.AsiguratRca = (Asigurat)d.GetAsiguratRca().Result;
            this.AutoCasco = (Auto)d.GetAutoCasco().Result;
            this.AutoRca = (Auto)d.GetAutoRca().Result;
            this.Intervenient = (Intervenient)d.GetIntervenient().Result;
            this.SocietateCasco = (SocietateAsigurare)d.GetSocietateCasco().Result;
            this.SocietateRca = (SocietateAsigurare)d.GetSocietateRca().Result;
            this.TipDosar = (Nomenclator)d.GetTipDosar().Result;
            this.selected = false;
        }

        public DosarExtended(Dosar d, bool _selected)
        {
            this.Dosar = d;
            this.AsiguratCasco = (Asigurat)d.GetAsiguratCasco().Result;
            this.AsiguratRca = (Asigurat)d.GetAsiguratRca().Result;
            this.AutoCasco = (Auto)d.GetAutoCasco().Result;
            this.AutoRca = (Auto)d.GetAutoRca().Result;
            this.Intervenient = (Intervenient)d.GetIntervenient().Result;
            this.SocietateCasco = (SocietateAsigurare)d.GetSocietateCasco().Result;
            this.SocietateRca = (SocietateAsigurare)d.GetSocietateRca().Result;
            this.TipDosar = (Nomenclator)d.GetTipDosar().Result;
            this.selected = _selected;
        }
    }
}

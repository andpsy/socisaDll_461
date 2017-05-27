using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOCISA.Models
{
    public class UtilizatorExtended
    {
        public Utilizator Utilizator { get; set; }
        public Nomenclator TipUtilizator { get; set; }
        public SocietateAsigurare SocietateAsigurare { get; set; }
        public bool selected { get; set; }

        public UtilizatorExtended() { }

        public UtilizatorExtended(Utilizator u)
        {
            this.Utilizator = u;
            this.SocietateAsigurare = (SocietateAsigurare)u.GetSocietatiAsigurare().Result;
            this.TipUtilizator = (Nomenclator)u.GetTipUtilizator().Result;
            this.selected = false;
        }

        public UtilizatorExtended(Utilizator u, bool _selected)
        {
            this.Utilizator = u;
            this.SocietateAsigurare = (SocietateAsigurare)u.GetSocietatiAsigurare().Result;
            this.TipUtilizator = (Nomenclator)u.GetTipUtilizator().Result;
            this.selected = _selected;
        }
    }
}

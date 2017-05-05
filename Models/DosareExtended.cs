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

    }
}

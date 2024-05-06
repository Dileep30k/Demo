using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Abstraction.ComplexModels
{
    public class SchemeModel
    {
        public long SchemeId { get; set; }
        public string SchemeName { get; set; }
        public string SchemeCode { get; set; }
        public List<string> Institutes { get; set; } = new List<string>();
        public string Duration { get; set; }
    }
}

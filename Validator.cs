using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SOCISA
{
    public class Validation
    {
        public string FieldName { get; set; }
        public string ValidationType { get; set; }
        public string ErrorCode { get; set; }
        public bool Active { get; set; }
    }

    public static class Validator
    {
        public static Dictionary<string, Validation[]> Validations
        {
            get
            {
                string vs = File.ReadAllText("Validations.json");
                return JsonConvert.DeserializeObject<Dictionary<string, Validation[]>>(vs);
            }
        }

        public static Validation[] GetTableValidations(string tableName)
        {
            return Validations[tableName];
        }
    }
}

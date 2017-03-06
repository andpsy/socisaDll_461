using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
                try
                {
                    string vs = File.ReadAllText(Path.Combine(CommonFunctions.GetSettingsFolder(), "Validations.json"));
                    return JsonConvert.DeserializeObject<Dictionary<string, Validation[]>>(vs);
                }
                catch { return null; }
            }
        }

        public static Validation[] GetTableValidations(string tableName)
        {
            try
            {
                return Validations[tableName];
            }catch { return null; }
        }

        public static response Validate(int authenticatedUserId, string connectionString, object obj, string tableName, out bool succes)
        {
            succes = false;
            response toReturn = new response(true, "", null, null, new List<Error>());
            Error err = new Error();
            try
            {
                Validation[] validations = Validator.GetTableValidations(tableName);
                if (validations != null && validations.Length > 0) // daca s-au citit validarile din fisier mergem pe fisier
                {
                    PropertyInfo[] pis = obj.GetType().GetProperties();
                    foreach (Validation v in validations)
                    {
                        if (v.Active)
                        {
                            foreach (PropertyInfo pi in pis)
                            {
                                if (v.FieldName.ToUpper() == pi.Name.ToUpper())
                                {
                                    switch (v.ValidationType)
                                    {
                                        case "Mandatory":
                                            if (pi.GetValue(obj) == null || pi.GetValue(obj).ToString().Trim() == "")
                                            {
                                                toReturn.Status = false;
                                                err = ErrorParser.ErrorMessage(v.ErrorCode);
                                                toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                                                toReturn.InsertedId = null;
                                                toReturn.Error.Add(err);
                                            }
                                            break;
                                        case "Confirmation":
                                            // ... TO DO ...
                                            break;
                                        case "Duplicate":
                                            try
                                            {
                                                Type typeOfThis = obj.GetType();
                                                Type propertyType = pi.GetValue(obj).GetType();
                                                //ConstructorInfo[] cis = typeOfThis.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                                ConstructorInfo ci = typeOfThis.GetConstructor(new Type[] { Type.GetType("System.Int32"), Type.GetType("System.String"), propertyType });

                                                if (ci != null && obj.GetType().GetProperty("ID").GetValue(obj) == null) // doar la insert verificam dublura
                                                {
                                                    //Dosar dj = new Dosar(authenticatedUserId, connectionString, pi.GetValue(this).ToString()); // trebuie sa existe constructorul pt. campul trimis ca parametru !!!
                                                    dynamic dj = Activator.CreateInstance(typeOfThis, new object[] { authenticatedUserId, connectionString, pi.GetValue(obj) });
                                                    if (dj != null && dj.ID != null)
                                                    {
                                                        toReturn.Status = false;
                                                        err = ErrorParser.ErrorMessage(v.ErrorCode);
                                                        toReturn.Message = string.Format("{0}{1};", toReturn.Message == null ? "" : toReturn.Message, err.ERROR_MESSAGE);
                                                        toReturn.InsertedId = null;
                                                        toReturn.Error.Add(err);
                                                    }
                                                }
                                            }
                                            catch { }
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    succes = true;
                }else { succes = false; }
            }catch { succes = false; }
            return toReturn;
        }
    }
}

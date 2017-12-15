using Castle.ActiveRecord;
using Newtonsoft.Json;
using System;

namespace ProjetoModeloDDD.Domain.Auxiliar
{
    public class ClasseModeloJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(ClasseModelo));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                existingValue = serializer.Deserialize(reader, objectType);
            }
            catch (Exception e)
            {
                if (reader.Value == null || reader.Value.ToString() == "")
                    reader.Read();
                else
                {
                    try
                    {
                        existingValue = ActiveRecordMediator.FindByPrimaryKey(objectType, Convert.ToInt32(reader.Value), false);
                    }
                    catch (Exception ex)
                    {
                        while (reader.Read() && reader.TokenType != JsonToken.Comment)
                        {
                        }
                    }
                }
            }

            if (existingValue != null)
            {
                if (serializer.PreserveReferencesHandling != PreserveReferencesHandling.Objects)
                {
                    object objTxt = existingValue.GetType().InvokeMember("Id", System.Reflection.BindingFlags.GetProperty, null, existingValue, null);
                    int id = objTxt != null ? Convert.ToInt32(objTxt) : 0;
                    if (id != 0)
                    {
                        existingValue = ActiveRecordMediator.FindByPrimaryKey(objectType, id, false);
                    }
                    else
                    {
                        existingValue = null;
                    }
                }
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if ((value == null) || ((value is ClasseModelo) && (((ClasseModelo)value).Id == null)))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue("");
                writer.WriteEndObject();
            }
            else
            {
                string propDesc = "Descricao";
                object[] atributos = value.GetType().GetCustomAttributes(typeof(System.ComponentModel.DefaultPropertyAttribute), false);

                if (atributos.Length >= 1)
                    propDesc = ((System.ComponentModel.DefaultPropertyAttribute)atributos[0]).Name;

                System.Reflection.PropertyInfo propNome = value.GetType().GetProperty(propDesc);
                object obj = propNome != null ? propNome.GetValue(value, null) : null;

                string objText = obj != null ? obj.ToString() : "";
                objText = objText.Replace("\t", "");
                string cPatch = writer.Path.Substring(writer.Path.LastIndexOf('.') + 1);

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                propNome = value.GetType().GetProperty("Id");

                object objTxt = value.GetType().InvokeMember("Id", System.Reflection.BindingFlags.GetProperty, null, value, null);
                string txt = objTxt != null ? objTxt.ToString() : "";
                writer.WriteValue(txt);
                writer.WriteEndObject();

                writer.WritePropertyName(cPatch + "_Id");
                object obj2 = value.GetType().InvokeMember("Id", System.Reflection.BindingFlags.GetProperty, null, value, null);
                string txt2 = obj2 != null ? obj2.ToString() : "";
                writer.WriteValue(txt2);
                writer.WritePropertyName(cPatch + "_" + propDesc);
                writer.WriteValue(objText);

            }
        }
    }

    public class CodClasseModeloJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int valor = 0;
            string cVal = (int)existingValue > 0 ? existingValue as string : reader.Value.ToString();
            int.TryParse(cVal, out valor);
            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if ((int)value == 0)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue("");
                writer.WriteEndObject();
            }
            else
            {
                string cPatch = writer.Path.Substring(writer.Path.LastIndexOf('.') + 1);

                writer.WriteStartObject();
                writer.WritePropertyName("Id");
                writer.WriteValue(value.ToString());
                writer.WriteEndObject();

            }
        }
    }


    public class NullDoubleJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal?) || objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            decimal valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null && ((decimal)existingValue != 0) ? Convert.ToString(existingValue) : reader.Value as string;
            }
            catch
            {
                //
            }
            if (!string.IsNullOrEmpty(cVal))
            {
                try
                {
                    valor = Decimal.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                }
                catch
                {
                    //
                }

            }
            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0,00");
            }
            else
            {
                writer.WriteValue(((decimal)value).ToString("N2", new System.Globalization.CultureInfo("pt-BR")).Replace(".", ""));
            }
        }
    }


    public class NullInt64JsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Int64?) || objectType == typeof(Int64);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Int64 valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null && ((Int64)existingValue != 0) ? existingValue as string : reader.Value as string;
                if (!string.IsNullOrEmpty(cVal))
                {
                    valor = Int64.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                }
            }
            catch
            {
            }
            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else
            {
                writer.WriteValue(((Int64)value).ToString());
            }
        }
    }

    public class NullIntJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int?) || objectType == typeof(int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int valor = 0;
            string cVal = existingValue != null ? existingValue as string : reader.Value as string;
            int.TryParse(cVal, out valor);
            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0,00");
            }
            else
            {
                writer.WriteValue(((int)value).ToString());
            }
        }
    }



    public class DoubleJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal?) || objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            decimal valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null ? existingValue as string : reader.Value as string;
            }
            catch
            {
            }
            if (!string.IsNullOrEmpty(cVal))
            {
                valor = Decimal.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
            }

            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else
            {
                writer.WriteValue(((decimal)value).ToString("N", new System.Globalization.CultureInfo("pt-BR")));
            }
        }

    }

    public class DoubleQtdeJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal?) || objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            decimal valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null ? existingValue as string : reader.Value as string;
            }
            catch
            {
            }
            if (!string.IsNullOrEmpty(cVal))
            {
                valor = Decimal.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
            }

            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else
            {
                writer.WriteValue(((decimal)value).ToString("N3", new System.Globalization.CultureInfo("pt-BR")));
            }
        }

    }

    public class DoubleN4JsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal?) || objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            decimal valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null ? existingValue as string : reader.Value as string;
            }
            catch
            {
            }
            if (!string.IsNullOrEmpty(cVal))
            {
                valor = Decimal.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
            }

            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else
            {
                writer.WriteValue(((decimal)value).ToString("N4", new System.Globalization.CultureInfo("pt-BR")));
            }
        }

    }

    public class DoubleN3JsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal?) || objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            decimal valor = 0;
            string cVal = "";
            try
            {
                cVal = existingValue != null ? existingValue as string : reader.Value as string;
            }
            catch
            {
            }
            if (!string.IsNullOrEmpty(cVal))
            {
                valor = Decimal.Parse(cVal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
            }

            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("0");
            }
            else
            {
                writer.WriteValue(((decimal)value).ToString("N3", new System.Globalization.CultureInfo("pt-BR")));
            }
        }

    }

    public class NullDateJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?) || objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null && (string)reader.Value == "")
                return null;
            DateTime valor;
            if (DateTime.TryParse((string)reader.Value, out valor))
                return valor;
            else
                return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime nullDate = new DateTime();
            if (value == null || (DateTime)value == nullDate)
            {
                writer.WriteValue("");
            }
            else
            {
                writer.WriteValue(((DateTime)value).ToShortDateString());
            }
        }
    }

    public class NullTimeSpanJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan?) || objectType == typeof(TimeSpan);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null && (string)reader.Value == "")
                return null;
            TimeSpan valor;
            TimeSpan.TryParse((string)reader.Value, out valor);
            return valor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeSpan nullDate = new TimeSpan();
            if (value == null || (TimeSpan)value == nullDate)
            {
                writer.WriteValue("");
            }
            else
            {
                writer.WriteValue(((TimeSpan)value).ToString().Substring(0, 5));
            }
        }
    }
}
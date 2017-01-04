using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace Symulator
{
    public enum packageSize
    {
        [Description("Brak opisu")]
        none,
        [Description("Mała")]
        small,
        [Description("Średnia")]
        medium,
        [Description("Duża")]
        big,

    };

    public static class Description
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }

    public class Package
    {
        int sortNum;      //id w symulatorze
        string id;       //jakieś id w systemie
        string recName;  //odbiorca
        string recAdress;
        string recZipCode;
        string recCity;
        Nullable<DateTime> recTimeFrom;
        Nullable<DateTime> recTimeTo;
        string recTelNum;
        packageSize size;

        public int SortNum
        {
            get
            {
                return sortNum;
            }

            set
            {
                sortNum = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string RecName
        {
            get
            {
                return recName;
            }

            set
            {
                recName = value;
            }
        }

        public string RecAdress
        {
            get
            {
                return recAdress;
            }

            set
            {
                recAdress = value;
            }
        }

        public string RecZipCode
        {
            get
            {
                return recZipCode;
            }

            set
            {
                recZipCode = value;
            }
        }

        public string RecCity
        {
            get
            {
                return recCity;
            }

            set
            {
                recCity = value;
            }
        }

        public DateTime? RecTimeFrom
        {
            get
            {
                return recTimeFrom;
            }

            set
            {
                recTimeFrom = value;
            }
        }

        public DateTime? RecTimeTo
        {
            get
            {
                return recTimeTo;
            }

            set
            {
                recTimeTo = value;
            }
        }

        public string RecTelNum
        {
            get
            {
                return recTelNum;
            }

            set
            {
                recTelNum = value;
            }
        }

        public packageSize Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public Package(int i, string id, string recName, string recAdress, string recZipCode, string recCity, DateTime? recTimeFrom, DateTime? recTimeTo, string recTelNum, packageSize size)
        {
            this.sortNum = i;
            this.id = id;
            this.recName = recName;
            this.recAdress = recAdress;
            this.recZipCode = recZipCode;
            this.recCity = recCity;
            this.recTimeFrom = recTimeFrom;
            this.recTimeTo = recTimeTo;
            this.recTelNum = recTelNum;
            this.size = size;

        }

        public Package(int sortNum, string id, string recName, string recAdress, string recZipCode, string recCity, string recTelNum, packageSize size)
        {
            this.sortNum = sortNum;
            this.id = id;
            this.recName = recName;
            this.recAdress = recAdress;
            this.recZipCode = recZipCode;
            this.recCity = recCity;
            this.recTimeFrom = null;
            this.recTimeTo = null;
            this.recTelNum = recTelNum;
            this.size = size;

        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + id.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Package objAsPackage = obj as Package;
            if (objAsPackage == null) return false;
            else return Equals(objAsPackage);
        }

        public bool Equals(Package other)
        {
            if (other == null) return false;
            if (this.id == other.id)
                return true;
            else {
                return false;
            }
        }
    }
}

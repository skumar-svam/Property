using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.NICServices
{
    //public abstract class Modificator
    public abstract class ParserModificators
    {
        protected Hashtable _parameters = new Hashtable();

        public Hashtable Parameters
        {
            get { return _parameters; }
        }

        public abstract void Apply(ref string Value, params string[] Parameters);
    }

    class NL2BR : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("\n", "<br>");
        }
    }

    class HTMLENCODE : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace("<", "&lt;");
            Value = Value.Replace(">", "&gt;");
        }
    }

    class UPPER : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToUpper();
        }
    }

    class LOWER : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToLower();
        }
    }

    class TRIM : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Trim();
        }
    }

    class TRIMEND : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimEnd();
        }
    }

    class TRIMSTART : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimStart();
        }
    }

    class DEFAULT : ParserModificators
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            if (Value == null || Value.Trim() == string.Empty)
                Value = Parameters[0];
        }
    }
}


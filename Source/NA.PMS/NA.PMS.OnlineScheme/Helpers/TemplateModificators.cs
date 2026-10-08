using System;
using System.Collections;
//using NA.PMS.Common.TemplateParser.Modificators;

namespace NA.PMS.OnlineScheme
{

    /// <summary>
    /// Abstract class for Modificators
    /// </summary>
    public abstract class TemplateModificator
    {
        protected Hashtable _parameters = new Hashtable();

        public Hashtable Parameters
        {
            get { return _parameters; }
        }

        public abstract void Apply(ref string Value, params string[] Parameters);
    }

    class NL2BR : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("\n", "<br>");
        }
    }

    class HTMLENCODE : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace("<", "&lt;");
            Value = Value.Replace(">", "&gt;");
        }
    }

    class UPPER : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToUpper();
        }
    }

    class LOWER : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToLower();
        }
    }

    class TRIM : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Trim();
        }
    }

    class TRIMEND : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimEnd();
        }
    }

    class TRIMSTART : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimStart();
        }
    }

    class DEFAULT : TemplateModificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            if (Value == null || Value.Trim() == string.Empty)
                Value = Parameters[0];
        }
    }
}

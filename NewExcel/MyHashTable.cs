using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
namespace NewExcel

{
    class MyHashTable
    {
        //public Dictionary<string, List<string>> references;
        private List<string> usedcells = new List<string>();
        public Hashtable values, formulas;
        public Hashtable Values
        {
            get
            {
                return values;
            }
        }
        public Hashtable Formulas
        {
            get
            {
                return formulas;
            }
        }
        static MyHashTable instance;
        private MyHashTable()
        {
            values = new Hashtable();
            formulas = new Hashtable();
        }
        public static MyHashTable GetInstance()
        {
            if (instance == null)
                instance = new MyHashTable();
            return instance;
        }
        public void AddFormula(string cell, string formula)
        {
            if(formulas.Contains(cell))
            {
                formulas[cell] = formula;
                return;
            }
            formulas.Add(cell, formula);
        }
        public string ShowFormula(string cell)
        {
            if (formulas.ContainsKey(cell))
            {
                return formulas[cell].ToString();
            }
            else return "";
        }
        public void AddValue(string cell, string value)
        {
            if (values.Contains(cell))
            {
                values[cell] = value;
                return;
            }
            values.Add(cell, value);
        }
        public void DeleteHash(string key)
        {
            formulas.Remove(key);
            values.Remove(key);
        }
        public void AddBoth(string cell, string formula, string value)
        {
            AddFormula(cell, formula);
            AddValue(cell, value);
           /* foreach(string item in r)
            {
                references[item].Add(cell);
            }*/
        }
        private void RecursionCheck(string cell)
        {
            if (usedcells.Contains(cell))
            {
                usedcells.Clear();
                throw new TokenEx();
                
            }
            else
                usedcells.Add(cell);
        }
        public void Re(string cell, ref bool isRecalculated, ToPolReverseNotationConverter converter)
        {
            isRecalculated = false;
            try
            {
                // RecursionCheck(cell);
                values[cell] = converter.Calculate(formulas[cell].ToString());
                usedcells.Add(cell);

            }
            catch (DivideByZeroException)
            {
                //MessageBox.Show("Gotcha bitch");
                values[cell] = "Error";
            }
            foreach (DictionaryEntry pair in formulas)
            {
                try
                {
                    if (pair.Value.ToString().Contains(cell) && !pair.Value.ToString().Contains("Error"))
                    {
                        values[pair.Key] = converter.Calculate(formulas[pair.Key].ToString());
                        ReCalculate(pair.Key.ToString(), ref isRecalculated, converter);
                    }
                }
                catch (DivideByZeroException)
                {
                    values[pair.Key] = "Error";
                    continue;
                }
            }


        }
        public void ReCalculate(string cell, ref bool isRecalculated, ToPolReverseNotationConverter converter)
        {
            isRecalculated = false;
            try
            {
                //RecursionCheck(cell);
                values[cell] = converter.Calculate(formulas[cell].ToString());
                usedcells.Add(cell);
               
            }
            catch(DivideByZeroException)
            {
                //MessageBox.Show("Gotcha bitch");
                values[cell] = "Error";
            }
            foreach (DictionaryEntry pair in formulas)
            {
                try
                {
                    if (pair.Value.ToString().Contains(cell) && !pair.Value.ToString().Contains("Error"))
                    {
                        values[pair.Key] = converter.Calculate(formulas[pair.Key].ToString());
                        ReCalculate(pair.Key.ToString(), ref isRecalculated, converter);
                    }
                }
                catch (DivideByZeroException)
                {
                    values[pair.Key] = "Error";
                    continue;
                }
            }


        }
        public void ReEvaluate(string cell, ToPolReverseNotationConverter converter)
        {
            List<string> keys = new List<string>();
            foreach (DictionaryEntry pair in formulas)
            {
                if (pair.Value.ToString().Contains(cell))
                {
                    keys.Add(pair.Key.ToString());
                } 
            }
            foreach(string k in keys)
            {
                formulas[k] = formulas[k].ToString().Replace(cell, "Error");
                values[k] = "Error";
            }
        }
    }
}

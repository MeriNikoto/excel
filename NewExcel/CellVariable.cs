using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NewExcel
{
    class CellVariable
    {
        public CellVariable()
        {

        }
        public string GetValue(string currCell)
        {
            MyHashTable curr = MyHashTable.GetInstance();
            var sd = curr.Values[currCell];
            string test;
            if (sd == null)
            {
                test = "0";
            }
            else
            {
                test = curr.Values[currCell].ToString();
            }
            return test;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewExcel
{
    class TokenEx : Exception
    {
        
    }
    public class ToPolReverseNotationConverter
    {
        List<string> res;
        Exception exc = new Exception();
        private List<string> standardOperators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/", "dec", "inc", "min", "max", "not", ">", "<" });
        Class26BasedSys f = new Class26BasedSys();
        public ToPolReverseNotationConverter()
        { }
        public bool IsConverting(string cell)
        {
            string f = "";
            string l = "";
            int k;
            try
            {
                f = cell.Split('.')[0];
                l = cell.Split('.')[1];
                for (int i = 0; i < f.Length; i++)
                {
                    if ((int)(f[i]) < 65 || (int)(f[i]) > 90)
                    {
                        throw new Exception("Yo, wtf is that, dude");
                    }
                }
                if (!Int32.TryParse(l, out k))
                {
                    throw new Exception("Yo, wtf is that, dude");
                }

                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
        private string Check( string s)
        {
            s = s.Trim(' ');
            return s;
        }
        private void Converting(string curr)
        {
            try
            {
                curr = Check(curr);
               // MessageBox.Show("|" + curr + "|");
                res = new List<string>();
                Stack<string> operatorStack = new Stack<string>();
                string[] CalcParts = new string[100];
                CalcParts = curr.Split(' ');
                int l = CalcParts.Length;
                for (int i = 0; i < l; i++)
                {
                    if (standardOperators.Contains(CalcParts[i]))
                    {
                        if (operatorStack.Count != 0 && !CalcParts[i].Equals("("))
                        {
                            if (CalcParts[i].Equals(")"))
                            {
                                string s = operatorStack.Pop();
                                while (s != "(")
                                {
                                    res.Add(s);
                                    s = operatorStack.Pop();
                                }
                            }
                            else if (GetPriority(CalcParts[i]) > GetPriority(operatorStack.Peek()))
                            {
                                operatorStack.Push(CalcParts[i]);
                            }
                            else if (operatorStack.Count > 0 && GetPriority(CalcParts[i]) <= GetPriority(operatorStack.Peek()))
                            {
                                res.Add(operatorStack.Pop());
                                operatorStack.Push(CalcParts[i]);
                            }
                        }
                        else operatorStack.Push(CalcParts[i]);
                    }
                    else
                    {
                        char[] arr = CalcParts[i].ToCharArray();
                        if (((int)(arr[0]) > 57 || 48 > (int)arr[0]) && arr[0] != '-')
                        {
                            if (IsConverting(CalcParts[i]))
                            {
              
                                CellVariable CellVar = new CellVariable();
                                CalcParts[i] = CellVar.GetValue(CalcParts[i]);
                                if(CalcParts[i] == "Error")
                                {
                                    throw new TokenEx();
                                }
                            }
                            else
                                throw new TokenEx();
                            
                        }
                        res.Add(CalcParts[i]);
                    }
                }

                while (operatorStack.Count != 0)
                {
                    res.Add(operatorStack.Pop());
                }
                res.ToArray();
            }
            catch( DivideByZeroException e) 
            {
                MessageBox.Show(e.Message);
            }
           
        }

        public int GetPriority(string checkedOper)
        {
            switch(checkedOper)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                case "dec":
                case "inc":
                case "max":
                case "min":
                case "not":
                    return 4;
                case "<":
                case ">":
                    return 5;
                default:
                    return 6;
            }
        }
 
        public string Calculate(string curr)
        {
            
                Converting(curr);
                Stack<string> stack = new Stack<string>();
                Queue<string> queue = new Queue<string>(res);
                string str = queue.Dequeue();
                while (queue.Count >= 0)
                {
                    if (!standardOperators.Contains(str))
                    {
                        stack.Push(str);
                        if (queue.Count == 0)
                        {
                            break;
                        }
                        str = queue.Dequeue();
                    }
                    else
                    {
                        double summ = 0;
                       
                            switch (str)
                            {
                                case "+":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        summ = a + b;
                                        break;
                                    }
                                case "-":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        summ = b - a;
                                        break;
                                    }
                                case "*":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        summ = a * b;
                                        break;
                                    }
                                case "/":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        if (a == 0)
                                            throw new DivideByZeroException();
                                        summ = b / a;
                                        break;
                                    }
                                case "dec":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        summ = a - 1;
                                        break;
                                    }
                                case "inc":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        summ = a + 1;
                                        break;
                                    }
                                case "min":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        summ = Math.Min(a, b);
                                        break;
                                    }
                                case "max":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        summ = Math.Max(a, b);
                                        break;
                                    }
                                case "not":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        if (a != 0)
                                            summ = 0;
                                        else
                                            summ = 1;
                                        break;
                                    }
                                case "<":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        if (b < a)
                                            summ = 1;
                                        else
                                            summ = 0;
                                        break;
                                    }
                                case ">":
                                    {
                                        double a = Convert.ToDouble(stack.Pop());
                                        double b = Convert.ToDouble(stack.Pop());
                                        if (b > a)
                                            summ = 1;
                                        else
                                            summ = 0;
                                        break;
                                    }
                            }
                       
                        stack.Push(summ.ToString());
                        if (queue.Count > 0)
                            str = queue.Dequeue();
                        else
                            break;
                    }
                }
                return stack.Pop();
            
        }
    }
}

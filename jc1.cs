using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace epsizizyS_sharp_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        JustCup jc  =new JustCup (), dbg = new JustCup();

        private void button1_Click(object sender, EventArgs e)
        {
            string os = richTextBox1.Text;
            richTextBox2.Text = jc.Parse(os);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // dbg.Parse("debug \"aaa\"");
            加载核心库CoreToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void 编译ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 加载核心库CoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("core.jc")) return;
            string s = File.ReadAllText("core.jc");
            richTextBox1.Text += s;
        }

        private void 运行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string os = richTextBox1.Text;
            richTextBox2.Text = jc.Parse(os);
        }
    }
    public class JustCup
    {
        List<string> h = new List<string>();
        
        public List<string> RemoveEmpty(List<string> h)
        {
            List<string> ret = new List<string>();
            foreach (var hi in h) if (hi.Length > 0)
                {
                    int st = 0, ed = hi.Length - 1;
                    ret.Add(hi);
                }
                    
            return ret;
        }

        public class Cup
        {
            public int hi;
            public List<string> d = new List<string> (), type = new List<string> ();
            public Cup()
            {
                hi = -1;
                //type.Add("void");
               // d.Add("");
            }
            public Cup(int hhi)
            {
                hi = hhi;
                //d.Add("");
                //type.Add("void");
            }
            public Cup(int hhi, string ss)
            {
                hi = hhi;
                d.Add(ss);
                type.Add("string");
            }
            public Cup(int hhi, int aa)
            {
                hi = hhi;
                d.Add(aa.ToString());
                type.Add("int");
            }
            public Cup(string aors, string ctyp)
            {
                hi = -1;
                d.Add(aors);
                type.Add(ctyp);
            }
            public Cup(int hhi, Cup c)
            {
                hi = hhi;
                d = new List<string>(c.d);
                type = new List<string>(c.type);
            }
            public string ParsedAsString()
            {
                string ret = "";
                for(int i = 0; i < d.Count; i++)
                {
                    //if (type[i] == "void") ;
                    if (type[i] == "string") ret += "(string)"+d[i];
                    if (type[i] == "int") ret += "(int)"+d[i];
                    if (i != d.Count - 1) ret += " ";
                }
                if (ret == "") ret = "(void)";
                return ret;
              //  return "_ERROR_UNRECOGNISABLE_TYPE_RETURNED_";
            }
            public void Add(Cup c) // addrange actually
            {
                hi = c.hi;
                d.AddRange(c.d);
                type.AddRange(c.type);
            }
            public void Add(string s)
            {
                d.Add(s);
                type.Add("string");
            }
        }
        Dictionary<string, Cup> V = new Dictionary<string, Cup>();
        Dictionary<string, int> funcHi = new Dictionary<string, int>();
        Dictionary<string, int> classHi = new Dictionary<string, int>();
        Dictionary<string, List<string>> funcParamNam = new Dictionary<string, List<string>>();
        List<Cup> paramStack = new List<Cup>();
        string bedStack = "";
        int[] intarr = new int[100000000];
        char[] chararr = new char[100000000];
        string[] stringarr = new string[1000000];

        /*
         * 1. 完善bed功能，包括begin、param、if等。
         */

        string prevBed(string s)
        {
            for(int i = s.Length-2; i >= 0; i--)
            {
                if (s[i] == '.')
                {
                    return s.Substring(0, i+1);
                }
            }
            return "";
        }
        public void Debug(object obj)
        {
            MessageBox.Show(obj.ToString());
        }
        Cup Run(int hi, int fbd)
        {
            string ch = h[hi]; // don't modify!!!
            //Debug("HI = " + hi + " CH " + ch);
            //Debug(ch);
            if (ch == "begin" || ch == "{" || ch == "seq")
            {
                ++hi;
                int bed = 1;
                for (; ; )
                {
                    if (h[hi] == "end" || h[hi] == "}" || h[hi] == "endseq")
                    {
                        --bed;
                    }
                   // if (h[hi] == "begin" || h[hi] == "{" || h[hi] == "seq") ++bed;
                    if (bed == 0) break;
                    hi = Run(hi, fbd).hi;
                    ++hi;
                }
            }
            if(ch == "(" || ch == "param" || ch == "[")
            {
                ++hi;
                Cup ret = new Cup();
                //Debug("start from " + hi);
                int bed = 1;
                for(; ;)
                {
                 //   Debug("HI = " + hi + " content = " + h[hi] + " len = " + h[hi].Length+ "bed = "+bed);
                    if (h[hi] == ")" || h[hi] == "endparam" || h[hi] == "]") --bed;
                   // if (h[hi] == "(" || h[hi] == "param" || h[hi] == "[") ++bed;
                    if (bed == 0) break;
                    Cup c = Run(hi, fbd);
                   // if (h[hi] == ")" || h[hi] == "endparam" || h[hi] == "]") --bed;
                    hi = c.hi + 1;
                    ret.Add(c);
                }
                ret.hi = hi;
                return ret;
            }
            if(ch == "if")
            {
                Cup p = Run(1 + hi, fbd);
                int pd;
                Cup ret;
                if(p.d.Count > 0 && p.type[0] == "int" && p.d[0] != "0")
                {
                    pd = 1;
                    Cup q = Run(1 + p.hi, fbd);
                    ret =  q;
                }
                else
                {
                    pd = 0;
                    Cup q = Run(1 + p.hi, 1);
                    ret = new Cup(q.hi);
                }
                if(h.Count > ret.hi+1 && h[ret.hi+1] == "else")
                {
                    if(pd == 1)
                    {
                        Cup q = Run(ret.hi + 2, 1);
                        ret.hi = q.hi;
                    }
                    else
                    {
                        Cup q = Run(ret.hi + 2, fbd);
                        ret = q;
                    }
                }
                return ret;
            }
            if(ch == "while")
            {
                Cup ret = new Cup();
                for(; ;)
                {
                    Cup p = Run(1 + hi, fbd);
                    if(p.d.Count > 0 && p.type[0] == "int" && p.d[0] != "0" && fbd == 0)
                    {
                        Cup q = Run(1 + p.hi, fbd);
                        ret.Add(q);
                    }
                    else
                    {
                        Cup q = Run(1 + p.hi, 1);
                        ret.hi = q.hi;
                        break;
                    }
                }
                return ret;
            }
            if (ch == "rep")
            {
                Cup ret = new Cup();
                Cup p = Run(1 + hi, fbd);
                int times = Convert.ToInt32(p.d[0]);
                if (fbd == 1) times = 0;
                while(times--  >0)
                {
                    Cup q = Run(1 + p.hi, fbd);
                    ret.Add(q);
                }
                Cup qq = Run(1 + p.hi, 1);
                ret.hi = qq.hi;
                return ret;
            }
            if (ch == "debug")
            {
                Cup ret = Run(hi + 1, fbd);
                hi = ret.hi;
                if (fbd == 0)
                {
                    MessageBox.Show(ret.ParsedAsString());
                }
            }
            if (ch.Length >= 2 && ch[0] == '\"' && ch.Last() == '\"')
            {
                return new Cup(hi, ch.Substring(1, ch.Length - 2));
            }
            if(ch == "str")
            {
                return new Cup(hi + 1, h[hi + 1]);
            }
            if(ch[0] >= '0' && ch[0] <= '9' || ch[0] == '-' && ch.Length > 1)
            {
                return new Cup(hi, Convert.ToInt32(ch));
            }
            if(ch == "dim" || ch == "var")
            {
                if(fbd == 0)
                {
                    V[bedStack+ h[hi + 1]] = new Cup();
                }
                return new Cup(hi + 1);
            }
            if(ch == "=" || ch == "set")
            {
                if(fbd == 0)
                {
                    string varnam =bedStack+ h[hi + 1];
                    Cup q = Run(hi + 2, 0);
                    V[varnam] = q;
                    return q;
                }
                else
                {
                    Cup q = Run(hi + 2, 1);
                    return q;
                }
            }
            string cbs = bedStack;
            for(; ;)
            {
                if (V.ContainsKey(cbs + ch)) return new Cup(hi, V[cbs + ch]);
                if (cbs == "") break;
                cbs = prevBed(cbs);
            }
            /*
            if(V.ContainsKey(bedStack+ch)) // this must be ahead of the next one
            {
                return new Cup(hi, V[bedStack+ ch]);
            }
            if(V.ContainsKey(ch))
            {
                return new Cup(hi, V[ch]);
            }*/
            if(ch == "+" || ch == "sum")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for(int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if(type == "int")
                {
                    int sum = 0;
                    for (int i = 0; i < c.d.Count; i++) sum += Convert.ToInt32(c.d[i]);
                    return new Cup(hi, sum);
                }
                if(type == "string")
                {
                    string s = "";
                    for (int i = 0; i < c.d.Count; i++) s += c.d[i];
                    return new Cup(hi, s);
                }
            }
            if (ch == "*" || ch == "prod")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = 1;
                    for (int i = 0; i < c.d.Count; i++) prod *= Convert.ToInt32(c.d[i]);
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "Lord, you wanna get the prod of some strings?");
                }
            }
            if (ch == "/" || ch == "divcalc")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = 1;
                    for (int i = 0; i < c.d.Count; i++)
                    {
                        if ((i & 1) == 0) prod *= Convert.ToInt32(c.d[i]);
                        else prod /= Convert.ToInt32(c.d[i]);
                    }
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "Lord, you wanna get the division of some strings?");
                }
            }
            if (ch == "^" || ch == "bitxor")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = 0;
                    for (int i = 0; i < c.d.Count; i++) prod ^= Convert.ToInt32(c.d[i]);
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "Lord, you wanna get the XOR of some strings?");
                }
            }
            if (ch == "&" || ch == "bitand")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = 0;
                    for (int i = 0; i < c.d.Count; i++) prod &= Convert.ToInt32(c.d[i]);
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "Lord, you wanna get the & of some strings?");
                }
            }
            if (ch == "|" || ch == "bitor")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = 2147483647;
                    for (int i = 0; i < c.d.Count; i++) prod |= Convert.ToInt32(c.d[i]);
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "Lord, you wanna get the | of some strings?");
                }
            }
            if (ch == "%" || ch == "mod")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int prod = Convert.ToInt32(c.d[0]);
                    for (int i =1; i < c.d.Count; i++) prod %= Convert.ToInt32(c.d[i]);
                    return new Cup(hi, prod);
                }
                if (type == "string")
                {
                    return new Cup(hi, "I wanna see some interesting pics.");
                }
            }
            if (ch == "-" || ch == "diff")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    int sum = 0; int coef = 1;
                    for (int i = 0; i < c.d.Count; i++)
                    {
                        sum += coef * Convert.ToInt32(c.d[i]);
                        coef *= -1;
                    }
                    return new Cup(hi, sum);
                }
                if (type == "string")
                {
                    string s = c.d[0];
                    for (int i = 1; i < c.d.Count; i++) s = s.Substring(0, Math.Max(0, s.Length - c.d[1].Length));
                    return new Cup(hi, s);
                }
            }
            if (ch == "<" || ch == "lower" || ch == "smaller" || ch == "shorter")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                string type = "int";
                for (int i = 0; i < c.d.Count; i++)
                {
                    if (c.type[i] == "string") type = "string";
                }
                if (type == "int")
                {
                    return new Cup(hi, Convert.ToInt32(c.d[0]) < Convert.ToInt32(c.d[1])?1:0);
                }
                if (type == "string")
                {
                    return new Cup(hi, c.d[0].Length < c.d[1].Length ? 1 : 0);
                }
            }
            if (ch == "==")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                if (fbd == 1) return c;
                if (c.type[0] != c.type[1]) return new Cup(hi, 0);
                if (c.d[0] != c.d[1]) return new Cup(hi, 0);
                return new Cup(hi, 1);
            }
            /*if(ch == "===")
            {
                Cup p =  Run(hi + 1, fbd);
                Cup q = Run(p.hi + 1, fbd);
                if (fbd == 1) return c;
            }*/
            if(ch == "strlen")
            {
                Cup c = Run(hi + 1, fbd); hi = c.hi;
                return new Cup(hi, c.d[0].Length);
            }
            if (ch == "def" || ch == "func")
            {
                string funcnam =bedStack+ h[hi + 1];
                funcParamNam[funcnam] = new List<string>();
                hi = hi + 3;
                for(; ;)
                {
                    if (h[hi] == ")" || h[hi] == "endparam") break;
                    funcParamNam[funcnam].Add(h[hi]);
                    ++hi;
                }
                //Cup param = Run(hi + 2, fbd);
                funcHi[funcnam] = hi + 1;
                Cup tmp = Run(hi + 1, 1);
                return new Cup(tmp.hi);
            }
            if(funcHi.ContainsKey(ch))
            {
                Cup p = (funcParamNam[ch].Count==0?new Cup (hi):Run(hi + 1, fbd));
                if (fbd == 1) return new Cup(p.hi);
                string obedstack = bedStack;
                bedStack += ch + ".";
                if(funcParamNam[ch].Count > 0 && funcParamNam[ch].Last() == "...")
                {
                    V[bedStack + funcParamNam[ch][0]] = p;
                }else for (int i = 0; i < funcParamNam[ch].Count; i++)
                {
                    V[bedStack+ funcParamNam[ch][i]] = new Cup(p.d[i], p.type[i]);
                }
                //paramStack.Add(p);
                Cup q = Run(funcHi[ch], fbd);
                bedStack = obedstack;
                //paramStack.RemoveAt(paramStack.Count - 1);
                return new Cup(p.hi, q);
            }
            if(ch == "class")
            {
                string classnam = bedStack + h[hi + 1];
                classHi[classnam] = hi + 2;
                Cup tmp = Run(hi + 2, 1);
                return new Cup(tmp.hi);
            }
            if(classHi.ContainsKey(ch))
            {
                string varnam = h[hi + 1];
                if (fbd == 1) return new Cup(hi + 1);
                string obs = bedStack;
                bedStack += h[hi + 1] + "."; // this is the difference
                Cup q = Run(classHi[ch], fbd);
                bedStack = obs;
                V[varnam] = q; // WOW
                return new Cup(hi + 1, q);
            }
            if(ch == "@" || ch == "at") // 【不推荐使用。】
            {
                Cup p = Run(hi + 1, fbd); // 为了彰显这不是普通的函数我们不使用一般的函数参数表示法
                Cup q = Run(p.hi + 1, fbd);
                if (fbd == 1) return new Cup(q.hi);
                Cup ret = new Cup(q.hi);
                ret.d.Add(q.d[Convert.ToInt32(p.d[0])]);
                ret.type.Add(q.type[Convert.ToInt32(p.d[0])]);
                return ret;
            }
            if(ch == "setintarr")
            {
                Cup p = Run(hi + 1, fbd);
                Cup q = Run(p.hi + 1, fbd);
                intarr[Convert.ToInt32(p.d[0])] = Convert.ToInt32(q.d[0]);
                return new Cup(q.hi);
            }
            if(ch == "getintarr") // 用已有的东西应该能写出好用的数组吧！
            {
                Cup p = Run(hi + 1, fbd);
                return new Cup(p.hi, intarr[Convert.ToInt32(p.d[0])]);
            }

            // 牛逼的字符串处理能力
            if(ch == "for")
            {
                string fornam = h[hi + 1];
                Cup p = Run(hi + 2, fbd);
                int finalhi = Run(p.hi + 1, 1).hi;
                if (fbd == 1) return new Cup(finalhi);
                else
                {
                    Cup ret = new Cup();
                    for (int i = 0; i < p.d.Count; i++)
                    {
                        V[bedStack + fornam] = new Cup(p.d[i], p.type[i]);
                        ret.Add(Run(p.hi + 1, fbd));
                        p.d[i] = V[bedStack + fornam].d[0]; // WOW
                        p.type[i] = V[bedStack + fornam].type[0];
                    }
                    return ret;
                }
            }
            if(ch == "rev" || ch == "reverse")
            {
                Cup p = Run(hi + 1, fbd);
                p.d.Reverse(); p.type.Reverse();
                return p;
            }
            if(ch == "range")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                Cup ret = new Cup();
                int st = ToInt(p.d[0]), ed = ToInt(p.d[1]), step;
                if (p.d.Count >= 3) step = ToInt(p.d[2]); else step = (st <= ed ? 1 : -1);
                for (int i = st; (st<=ed?i < ed:i>ed); i += step) ret.Add(new Cup(-1, i));
                return new Cup(p.hi, ret);
            }
            if(ch == "count" || ch == "length" || ch == "size")
            {
                Cup p = Run(hi + 1, fbd);
                return new Cup(p.hi, p.d.Count);
            }
            if(ch == "partial")
            {
                Cup p = Run(hi + 1, fbd);
                Cup q = Run(p.hi + 1, fbd);
                return new Cup(-1, "myjs999 please code on");
                // FROMHERE
            }
            if(ch == "in")
            {
                return Run(hi + 1, fbd);
            }
            if(ch == "getfiles")
            {
                Cup ad = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(ad.hi);
                List<string> rets = new List<string>(Directory.GetFiles(ad.d[0], ad.d[1]));
                Cup ret = new Cup();
                foreach(var s in rets)
                {
                    ret.Add(new Cup(-1, s));
                }
                return new Cup(ad.hi, ret);
            }
            if(ch == "getcurrentpath")
            {
                return new Cup(hi, Directory.GetCurrentDirectory());
            }
            if(ch == "copy")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                File.Copy(p.d[0], p.d[1]);
                return new Cup(p.hi + 1);
            }
            if(ch == "delete")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                File.Delete(p.d[0]);
                return new Cup(p.hi + 1);
            }
            if(ch == "exist")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                return new Cup(p.hi + 1, File.Exists(p.d[0])?1:0);
            }
            if(ch == "randstring")
            {
                return new Cup(hi, randstring());
            }
            if(ch == "rename")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                if (File.Exists(p.d[1])) p.d[1]+= randstring();
                File.Copy(p.d[0], p.d[1]);
                File.Delete(p.d[0]);
                return new Cup(p.hi + 1);
            }
            if(ch == "print")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                foreach (var s in p.d) parseRet += s+" ";
                if(parseRet.Length > 0) parseRet= parseRet.Remove(parseRet.Length - 1);
                return new Cup(p.hi + 1);
            }
            if(ch == "split")
            {
                Cup p = Run(hi + 1, fbd);
                if (fbd == 1) return new Cup(p.hi);
                Cup ret = new Cup(p.hi);
                string s = p.d[0];
                int st = 0;
                for(int i = 0; i < s.Length-p.d[1].Length+1; i++)
                {
                    if(s.Substring(i, p.d[1].Length) == p.d[1])
                    {
                        ret.Add(s.Substring(st, i - st));
                        ret.Add(p.d[1]);
                        st = i + p.d[1].Length;
                        i += p.d[1].Length;
                    }
                }
                ret.Add(s.Substring(st, s.Length - st));
                ret.d = RemoveEmpty(ret.d);
                /*
                char[] spliter = p.d[1].ToCharArray();
                List<string> tmp =  p.d[0].Split(spliter).ToList();
                Cup ret = new Cup(p.hi);
                foreach (var e in tmp) ret.Add(new Cup(p.hi, e));
                */
                return ret;
            }
            if(ch == "inttochar")
            {
                Cup p = Run(hi + 1, fbd);
                char c = (char)ToInt(p.d[0]);
                string s = "";
                s += c;
                return new Cup(p.hi, s);
            }
            return new Cup(hi);
        }
        public string randstring()
        {
            Random rand = new Random();
            string ret = "";
            for (int i = 0; i < 16; i++) ret += (char)rand.Next('a', 'z' + 1);
            return ret;
        }
        public int ToInt(string s)
        {
            return Convert.ToInt32(s);
        }
        string parseRet = "";
        public string Parse(string os)
        {
            //string core = "def getcurfiles() getfiles(getcurpath,\"*\")\r\n";
            char[] s = new char[100000]; int sn = 0;
            List<char> dict = new List<char> {'(',')','{','}','[',']','@'/*,'+','-','*','/','&','^','%','!','<','>','=','|' */};
            for(int i = 0; i  < os.Length; i++)
            {
                if (dict.Contains(os[i]))
                {
                    s[sn++] = ' ';
                    s[sn++] = os[i];
                    s[sn++] = ' ';
                }
                else if (os[i] == ',') s[sn++] = ' ';
                else s[sn++] = os[i];
            }
            //Debug("SN = " + sn);
            s[sn++] = ' ';
           s[sn ++] = '?';
           s[sn ++] = ' ';
           // for (int i = 0; i < 15; i++) Debug(s[i]);
           // Debug(s);
            string ss = new string(s);
          //  Debug(ss);
           // Debug(ss.Length);
            h = new List<string>(ss.Split(' ', '\r', '\n', '\t'));
            h = RemoveEmpty(h);
            List<string> hh = h;
            hh.Add("end");
            h = new List<string> { "begin" };
            h.AddRange(hh);
            //foreach (var hs in h) Debug(hs);
            // try
            // {
            parseRet = "";
                Run(0, 0);
            // }
            // catch
            // {

            // }
            return parseRet;
        }
    }
}

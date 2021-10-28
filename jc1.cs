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
        public void Debug(object obj)
        {
            MessageBox.Show(obj.ToString());
        }
        int hideForm1 = 0;
        public Form1(string[] args)
        {
            InitializeComponent();
            加载核心库CoreToolStripMenuItem_Click(new object(), new EventArgs());

            if (args.Count() > 0)
            {
                richTextBox3.Text = File.ReadAllText(args[0]);
                this.Hide();
                运行ToolStripMenuItem_Click(new object(), new EventArgs());
                hideForm1 = 1;
               Dispose();
            }
        }

        JustCup jc  =new JustCup (), dbg = new JustCup();
        public string GetWindowInput()
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            return f3.ans;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            运行ToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // dbg.Parse("debug \"aaa\"");
            //visualForm.Show();
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
        public Form2 visualForm = new Form2();
        private void 可视化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 先搞单窗口吧。 2021.9.11
            //Form2 f2 = new Form2();
            visualForm.Show();
        }

        private void 更新说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debug("将原来的默认array改为defarr。\r\n" +
                "增加了windows库。\r\n" +
                "array也等同于defarr。数组元素未定义由报错改为了返回void。\r\n" +
                "{}可以用return了（只是指定返回值，不会结束{}，多个会累在一起）（这个有问题。不要用。）\r\n" +
                "被赋值的变量现在优先考虑已经定义的（可能在外层），没定义的话再在最内层定义。\r\n" +
                "增加了eqlvar和deqlvar\r\n" +
                "增加了传引用参数（用了eqlvar实际上是）（但现在还不能在复合传引用函数里eqlvar。这是eqlvar的问题）（当然，一切的目的都是为了传array和class）\r\n" +
                "b.set(&b, &a) 这已经是完全体了！！");
        }

        private void 加载Windows库WindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("windows.jc")) return;
            string s = File.ReadAllText("windows.jc");
            richTextBox1.Text += s;
        }

        private void 运行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string os = richTextBox1.Text;

            if (visualForm.showForm) {
                os += "show_window [" + visualForm.windowWidth + " " + visualForm.windowHeight + "]\n";
            }

            os += richTextBox3.Text; // 主义顺序！ 2021.9.11 3:09

            richTextBox2.Text = jc.Parse(os);
        }
    }
    public class JustCup
    {
        List<string> h = new List<string>();
        public string GetWindowInput()
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            return f3.ans;
        }
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
            public string acc = ""; // accessory
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
        Dictionary<string, int> arrayDi = new Dictionary<string, int>();
        Dictionary<string, List<int>> arrayShape = new Dictionary<string, List<int>>();
        Dictionary<string, int> funcHi = new Dictionary<string, int>();
        Dictionary<string, int> classHi = new Dictionary<string, int>();
        Dictionary<string, List<string>> funcParamNam = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> eqlvar = new Dictionary<string, List<string>>();
        List<Cup> paramStack = new List<Cup>();
        string bedStack = "";
        int[] intarr = new int[100000000];
        char[] chararr = new char[100000000];
        string[] stringarr = new string[1000000];
        Cup[] arrayData = new Cup[100000000];
        int arrayDataUsedCount = 0;
        Form4 f4;

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
        Cup Run(int hi, int fbd, Cup wit=null)
        {

            string ch = h[hi]; // don't modify!!!
                               // Debug("HI = " + hi + " CH " + ch);
                               // Debug(ch);
            try
            {


                if (ch == "begin" || ch == "{" || ch == "seq")
                {
                    ++hi;
                    int bed = 1;
                    Cup ret = new Cup();
                    for (; ; )
                    {
                        if (h[hi] == "end" || h[hi] == "}" || h[hi] == "endseq")
                        {
                            --bed;
                        }
                        // if (h[hi] == "begin" || h[hi] == "{" || h[hi] == "seq") ++bed;
                        if (bed == 0) break;
                        Cup c = Run(hi, fbd);
                        hi = c.hi + 1;
                        if(c.acc == "return")
                        {
                            c.acc = "";
                            ret.Add(c);
                        }
                    }
                    ret.hi = hi;
                    return ret;
                }
                if (ch == "return")
                {
                    Cup p = Run(hi + 1, fbd);
                    p.acc = "return";
                    return p;
                }
                if (ch == "(" || ch == "param" || ch == "[")
                {
                    ++hi;
                    Cup ret = new Cup();
                    //Debug("start from " + hi);
                    int bed = 1;
                    for (; ; )
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
                if (ch == "if")
                {
                    Cup p = Run(1 + hi, fbd);
                    int pd;
                    Cup ret;
                    if (p.d.Count > 0 && p.type[0] == "int" && p.d[0] != "0")
                    {
                        pd = 1;
                        Cup q = Run(1 + p.hi, fbd);
                        ret = q;
                    }
                    else
                    {
                        pd = 0;
                        Cup q = Run(1 + p.hi, 1);
                        ret = new Cup(q.hi);
                    }
                    if (h.Count > ret.hi + 1 && h[ret.hi + 1] == "else")
                    {
                        if (pd == 1)
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
                if (ch == "while")
                {
                    Cup ret = new Cup();
                    for (; ; )
                    {
                        Cup p = Run(1 + hi, fbd);
                        if (p.d.Count > 0 && p.type[0] == "int" && p.d[0] != "0" && fbd == 0)
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
                    while (times-- > 0)
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
                if (ch == "str")
                {
                    return new Cup(hi + 1, h[hi + 1]);
                }
                if (ch[0] >= '0' && ch[0] <= '9' || ch[0] == '-' && ch.Length > 1)
                {
                    return new Cup(hi, Convert.ToInt32(ch));
                }
                if (ch == "dim" || ch == "var")
                {
                    if (fbd == 0)
                    {
                        V[bedStack + h[hi + 1]] = new Cup();
                    }
                    return new Cup(hi + 1);
                }
                if(ch == "eqlvar") // 2021.10.25 for 3.0
                {
                    if(fbd == 0)
                    {
                        string va =bedStack+ h[hi + 1], vb =bedStack+ h[hi + 2];
                        if (!eqlvar.ContainsKey(va)) eqlvar[va] = new List<string>();
                        if (!eqlvar.ContainsKey(vb)) eqlvar[vb] = new List<string>();
                        eqlvar[va].Add(vb);
                        eqlvar[vb].Add(va);
                    }
                    return new Cup(hi + 2);
                }
                if(ch == "deqlvar")
                {
                    if (fbd == 0)
                    {
                        string va =bedStack+ h[hi + 1], vb =bedStack+ h[hi + 2];
                        while (eqlvar.ContainsKey(va) && eqlvar[va].Contains(vb))
                        {
                            eqlvar[va].Remove(vb);
                            eqlvar[vb].Remove(va);
                        }
                    }
                    return new Cup(hi + 2);
                }
                if (ch == "defarr" || ch == "array")
                {
                    string arrayNam = bedStack + h[hi + 1];
                    Cup shape = Run(hi + 2, fbd);
                    int size = 1;
                    for (int i = 0; i < shape.d.Count; i++) size *= ToInt(shape.d[i]);
                    if (fbd == 0)
                    {
                        arrayDi[arrayNam] = arrayDataUsedCount;
                        arrayDataUsedCount += size;
                        arrayShape[arrayNam] = new List<int>();
                        for (int i = 0; i < shape.d.Count; i++)
                        {
                            arrayShape[arrayNam].Add(ToInt(shape.d[i]));
                        }
                    }
                    return new Cup(shape.hi);
                    // array d[100,100]
                }
                if (ch == "=" || ch == "set")
                {
                    if (fbd == 0)
                    {// 10.25 23:55 lvn可能含句号！！！
                        //swap.a.x或swap.a有定义：相当于swap.a.x有定义
                        //a.x或a有定义：相当于a.x有定义
                        //我想取消对全局变量的指涉了，至少是今天。
                        string allstr = bedStack + h[hi + 1];
                        string cstr = allstr;
                        //string curBedStack = bedStack;// prevBed(allstr);
                        //string lastVarNam = h[hi + 1];// allstr.Substring(curBedStack.Length);
                        //Debug(allstr + " -> " + curBedStack + " + " + lastVarNam);
                        string varnam = "";
                        /*
                        for (; ; )
                        {
                            if (arrayDi.ContainsKey(curBedStack + lastVarNam))
                            {
                                varnam = curBedStack + lastVarNam;
                                break;
                            }
                            if (V.ContainsKey(curBedStack + lastVarNam))
                            {
                                varnam = curBedStack + lastVarNam;
                                Debug(varnam + " found");
                                break;
                            }
                            if (curBedStack == "") break;
                            Debug(curBedStack + lastVarNam + " not found, prevbed.");
                            curBedStack = prevBed(curBedStack);
                        }*/
                        for (; ; )
                        {
                            //Debug("CSTR " + cstr);
                            if (arrayDi.ContainsKey(cstr))
                            {
                                varnam = allstr;
                                break;
                            }
                            if (V.ContainsKey(cstr))
                            {
                                varnam = allstr;
                                //Debug(varnam + " found");
                                break;
                            }
                            if (!cstr.Contains(".")) break;
                            //Debug(curBedStack + lastVarNam + " not found, prevbed.");
                            cstr = prevBed(cstr);
                            cstr = cstr.Remove(cstr.Length - 1);
                        }
                            if (varnam == "")
                        {
                            //Debug("Error: var " + lastVarNam + " no defined!");
                            varnam = allstr; // new var(can't be array)
                        }
                        List<string> eqlvarnams = new List<string>();
                        eqlvarnams.Add(varnam);
                        string curvn = varnam, lstnam = "";
                        for(; ;)
                        {
                          //  Debug("CURVN" + curvn);
                            if (eqlvar.ContainsKey(curvn))
                            {
                               // Debug("YEshave " + curvn);
                                foreach (string v in eqlvar[curvn]) eqlvarnams.Add(v+lstnam);
                                break;
                            }
                            if (!curvn.Contains(".")) break;
                            string newstr = prevBed(curvn);
                            newstr = newstr.Substring(0, newstr.Length - 1);
                            lstnam = curvn.Substring(newstr.Length, curvn.Length - newstr.Length) + lstnam;
                            curvn = newstr; // 10.25 23:31 不过是寥寥几句话的事。有什么难的？
                        }
                       // Debug("finhave " + curvn);
                        if (arrayDi.ContainsKey(varnam))
                        {
                            Cup shape = Run(hi + 2, 0);
                            Cup q = Run(shape.hi + 1, 0);
                            foreach (string cvarnam in eqlvarnams)
                            {
                                if (shape.d.Count == 0)
                                {
                                    arrayDi[cvarnam] = ToInt(q.d[0]);
                                }
                                else
                                {
                                    int pos = 0;
                                    for (int i = 0; i < shape.d.Count; i++)
                                    {
                                        pos += ToInt(shape.d[i]);
                                        if (i != shape.d.Count - 1) pos *= arrayShape[varnam][i + 1];
                                    }
                                    pos += arrayDi[cvarnam];
                                    arrayData[pos] = q;
                                }
                            }
                            return q;
                        }
                        else
                        {
                            Cup q = Run(hi + 2, 0);
                           // Debug("I'm setting " + varnam + " to " + q.d[0]);
                            foreach (string cvarnam in eqlvarnams)
                                V[cvarnam] = q;
                            return q;
                        }
                    }
                    else
                    {
                        Cup q = Run(hi + 2, 1);
                        return q;
                    }
                }
                if(ch == "debugmark")
                {
                    Debug("debugmark at " + hi + " says " + h[hi + 1]);
                    return new Cup(hi + 1);
                }
                string cbs = bedStack;
                if (funcHi.ContainsKey(ch))
                {
                    //if (ch == "a") Debug("considering ch " + ch);

                    Cup p = (funcParamNam[ch].Count == 0 ? new Cup(hi) : Run(hi + 1, fbd));
                    if (fbd == 1) return new Cup(p.hi);
                    string obedstack = bedStack;
                    bedStack += ch + ".";
                    if (funcParamNam[ch].Count > 0 && funcParamNam[ch].Last() == "...")
                    {
                        V[bedStack + funcParamNam[ch][0]] = p;
                    }
                    else for (int i = 0; i < funcParamNam[ch].Count; i++)
                        {
                            if (funcParamNam[ch][i][0] == '&')
                            {
                                //Debug("&" + funcParamNam[ch][i]);
                                string cf = funcParamNam[ch][i];
                                cf = cf.Substring(1, cf.Length - 1);
                                string va = obedstack + p.d[i]; // varnam in outer space
                                string vb = bedStack + cf; // varnam in function
                                if (!eqlvar.ContainsKey(va)) eqlvar[va] = new List<string>();
                                if (!eqlvar.ContainsKey(vb)) eqlvar[vb] = new List<string>();
                                eqlvar[va].Add(vb);
                                eqlvar[vb].Add(va);
                                if (V.ContainsKey(va)) // class可能没有值
                                    V[vb] = new Cup(-1, V[va]); // 至少得穿一次参数呀！
                                else
                                    V[vb] = new Cup(-1); // 不然一会儿以为这个变量没定义
                               // Debug("EQLVAR " + va + " " + vb);
                            }
                            else
                            {
                                V[bedStack + funcParamNam[ch][i]] = new Cup(p.d[i], p.type[i]);
                            }
                        }
                    //paramStack.Add(p);
                    //Debug("run func " + ch);
                    Cup q = Run(funcHi[ch], fbd);
                    for (int i = 0; i < funcParamNam[ch].Count; i++)
                    {
                        if (funcParamNam[ch][i][0] == '&')
                        {
                            string cf = funcParamNam[ch][i];
                            cf = cf.Substring(1, cf.Length - 1);
                            string va = obedstack + p.d[i]; // varnam in outer space
                            string vb = bedStack + cf; // varnam in function
                            while (eqlvar.ContainsKey(va) && eqlvar[va].Contains(vb))
                            {
                                eqlvar[va].Remove(vb);
                                eqlvar[vb].Remove(va);
                            }
                        }
                    }
                    bedStack = obedstack;
                    //paramStack.RemoveAt(paramStack.Count - 1);
                    return new Cup(p.hi, q);
                }
                if(ch[0] == '&')
                {
                    return new Cup(hi, ch.Substring(1, ch.Length - 1));
                }
                string vallstr = bedStack + ch;
                string vcstr = vallstr;
                string vclst = "";
                for(; ;) // 全局变量さようなら
                {
                    List<string> eqls = new List<string>();
                    eqls.Add(vcstr);
                    if (eqlvar.ContainsKey(vcstr)) foreach(string eqlpre in eqlvar[vcstr])
                    {
                        eqls.Add(eqlpre);
                    }
                    foreach (string eqlpre in eqls)
                    {
                        string callstr = eqlpre + vclst;
                        //Debug("try callstr " + callstr);
                        if (arrayDi.ContainsKey(callstr))
                        {
                            Cup shape = Run(hi + 1, fbd);
                            if (shape.d.Count == 0) return new Cup(shape.hi, arrayDi[callstr]);
                            else
                            {

                                int pos = 0;
                                for (int i = 0; i < shape.d.Count; i++)
                                {
                                    pos += ToInt(shape.d[i]);
                                    if (i != shape.d.Count - 1) pos *= arrayShape[callstr][i + 1];
                                }

                                pos += arrayDi[callstr];
                                if (arrayData[pos] == null)
                                    return new Cup(shape.hi, new Cup(-1));
                                return new Cup(shape.hi, arrayData[pos]);
                            }
                        }
                        if (V.ContainsKey(callstr))
                        {
                            //Debug("V have " + vcstr+" so return "+vallstr+" as "+V[vallstr]);
                            return new Cup(hi, V[callstr]);
                        }
                    }
                    

                    if (!vcstr.Contains(".")) break;
                    vclst = vcstr + vclst;
                    vcstr = prevBed(vcstr);
                    vcstr = vcstr.Remove(vcstr.Length - 1);
                    vclst = vclst.Substring(vcstr.Length);
                }
                /*
                for (; ; )
                {
                    //if (ch == "asd") Debug("considering ch " + ch);
                    
                    
                    if (arrayDi.ContainsKey(cbs + ch))
                    {
                        //Debug("array detected");
                        //if (ch == "asd") Debug(1);
                        Cup shape = Run(hi + 1, fbd);
                        if (shape.d.Count == 0) return new Cup(shape.hi, arrayDi[cbs + ch]);
                        else
                        {
                            
                            int pos = 0;
                            for (int i = 0; i < shape.d.Count; i++)
                            {
                                pos += ToInt(shape.d[i]);
                                if (i != shape.d.Count - 1) pos *= arrayShape[cbs + ch][i + 1];
                            }
                            
                            pos += arrayDi[cbs + ch];
                            if(arrayData[pos] == null)
                            {
                               // Debug("Error: array element not defined");
                                return new Cup(shape.hi, new Cup(-1));
                            }
                           // Debug("POS = " + pos);
                            //if (ch == "asd") Debug(shape.d[0]);
                            //if (ch == "asd") Debug("return la "+hi);
                            return new Cup(shape.hi, arrayData[pos]);
                        }
                        
                    }
                   // if (ch == "asd") Debug("fuck2");
                    if (V.ContainsKey(cbs + ch)) return new Cup(hi, V[cbs + ch]);
                    if (cbs == "") break;
                    cbs = prevBed(cbs);
                    //if (ch == "asd") Debug("fuck3");
                }
                */
                /*
                if(V.ContainsKey(bedStack+ch)) // this must be ahead of the next one
                {
                    return new Cup(hi, V[bedStack+ ch]);
                }
                if(V.ContainsKey(ch))
                {
                    return new Cup(hi, V[ch]);
                }*/
                if (ch == "+" || ch == "sum")
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
                        int sum = 0;
                        for (int i = 0; i < c.d.Count; i++) sum += Convert.ToInt32(c.d[i]);
                        return new Cup(hi, sum);
                    }
                    if (type == "string")
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
                        return new Cup(hi, "Lord, you wanna get the prod of some strings?");// 21.10.25 I really checked this HAHAHA!!!
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
                        for (int i = 1; i < c.d.Count; i++) prod %= Convert.ToInt32(c.d[i]);
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
                        return new Cup(hi, Convert.ToInt32(c.d[0]) < Convert.ToInt32(c.d[1]) ? 1 : 0);
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
                if (ch == "strlen")
                {
                    Cup c = Run(hi + 1, fbd); hi = c.hi;
                    return new Cup(hi, c.d[0].Length);
                }
                if (ch == "def" || ch == "func")
                {
                    string funcnam = "";
                    if (fbd == 0 && (h[hi + 1] == "name" || h[hi + 1] == "class" || h[hi+1] == "classname")) // 2021.9.12 23:33
                    {
                        funcnam = bedStack.Substring(0, bedStack.Length - 1);
                       // Debug("FUNCNAM = " + funcnam);
                    }
                    else
                    {
                        funcnam = bedStack + h[hi + 1];
                    }
                    funcParamNam[funcnam] = new List<string>();
                    hi = hi + 3;
                    for (; ; )
                    {
                        if (h[hi] == ")" || h[hi] == "endparam") break;
                        funcParamNam[funcnam].Add(h[hi]);
                        ++hi;
                    }
                    //Cup param = Run(hi + 2, fbd);
                    funcHi[funcnam] = hi + 1;
                    //if (funcnam == "a") Debug("funchi[a]=" + funcHi[funcnam]);
                    Cup tmp = Run(hi + 1, 1);
                    return new Cup(tmp.hi);
                }
                
                
               /* catch
                {
                    Debug(ch + "函数传参时出错!");
                }*/
                if (ch == "class")
                {
                    string classnam = bedStack + h[hi + 1];
                    classHi[classnam] = hi + 2;
                    Cup tmp = Run(hi + 2, 1);
                    return new Cup(tmp.hi);
                }
                
                if (classHi.ContainsKey(ch))
                {
                    
                    string varnam = h[hi + 1];
                    if (fbd == 1) return new Cup(hi + 1);
                    string obs = bedStack;
                    bedStack += h[hi + 1] + "."; // this is the difference
                    //Debug("start define class");
                    Cup q = Run(classHi[ch], fbd);
                    //Debug("end define class");
                    bedStack = obs;
                    V[varnam] = q; // WOW
                    return new Cup(hi + 1, q);
                }
                if (ch == "@" || ch == "at") // 【不推荐使用。】
                {
                    Cup p = Run(hi + 1, fbd); // 为了彰显这不是普通的函数我们不使用一般的函数参数表示法
                    Cup q = Run(p.hi + 1, fbd);
                    if (fbd == 1) return new Cup(q.hi);
                    Cup ret = new Cup(q.hi);
                    ret.d.Add(q.d[Convert.ToInt32(p.d[0])]);
                    ret.type.Add(q.type[Convert.ToInt32(p.d[0])]);
                    return ret;
                }
                if (ch == "setintarr")
                {
                    Cup p = Run(hi + 1, fbd);
                    Cup q = Run(p.hi + 1, fbd);
                    if (fbd == 1) return new Cup(q.hi);
                    intarr[Convert.ToInt32(p.d[0])] = Convert.ToInt32(q.d[0]);
                    return new Cup(q.hi);
                }
                // lead array(i)  getintarr i 
                // class def array(i) getintarr i
                // array a
                // print a[1]
                // or like this
                // class array {
                //    namedef (i) getintarr i
                //    querydef
                //    def name(i) getintarr i
                //    =def (i) (v) setintarr i v
                //    modifydef
                //    def =name(i) (v) setintarr i v

                //   def name() {} 
                //   def =name() (v) {}
                // }
                if (ch == "getintarr") // 用已有的东西应该能写出好用的数组吧！
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    return new Cup(p.hi, intarr[Convert.ToInt32(p.d[0])]);
                }
                if (ch == "int")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    for (int i = 0; i < p.d.Count; i++)
                    {
                        if (p.type[i] == "string")
                        {
                            p.type[i] = "int";
                        }
                    }
                    return p;
                }
                // 牛逼的字符串处理能力
                if (ch == "for")
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
                if (ch == "rev" || ch == "reverse")
                {
                    Cup p = Run(hi + 1, fbd);
                    p.d.Reverse(); p.type.Reverse();
                    return p;
                }
                if (ch == "range")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    Cup ret = new Cup();
                    int st = ToInt(p.d[0]), ed = ToInt(p.d[1]), step;
                    if (p.d.Count >= 3) step = ToInt(p.d[2]); else step = (st <= ed ? 1 : -1);
                    for (int i = st; (st <= ed ? i < ed : i > ed); i += step) ret.Add(new Cup(-1, i));
                    return new Cup(p.hi, ret);
                }
                if (ch == "count" || ch == "length" || ch == "size")
                {
                    Cup p = Run(hi + 1, fbd);
                    return new Cup(p.hi, p.d.Count);
                }
                if (ch == "partial")
                {
                    Cup p = Run(hi + 1, fbd);
                    Cup q = Run(p.hi + 1, fbd);
                    return new Cup(-1, "myjs999 please code on");
                    // FROMHERE
                }
                /*if(ch == "in")
                {
                    return Run(hi + 1, fbd);
                }*/
                if (ch == "getfiles")
                {
                    Cup ad = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(ad.hi);
                    List<string> rets = new List<string>(Directory.GetFiles(ad.d[0], ad.d[1]));
                    Cup ret = new Cup();
                    foreach (var s in rets)
                    {
                        ret.Add(new Cup(-1, s));
                    }
                    return new Cup(ad.hi, ret);
                }
                if (ch == "getcurrentpath")
                {
                    return new Cup(hi, Directory.GetCurrentDirectory());
                }
                if (ch == "copy")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    File.Copy(p.d[0], p.d[1]);
                    return new Cup(p.hi);
                }
                if (ch == "delete")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    File.Delete(p.d[0]);
                    return new Cup(p.hi);
                }
                if (ch == "exist")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    return new Cup(p.hi, File.Exists(p.d[0]) ? 1 : 0);
                }
                if(ch == "write")
                {
                    Cup ad = Run(hi + 1, fbd);
                    Cup p = Run(ad.hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    string parseRet = "";
                    foreach (var s in p.d) parseRet += s + " ";
                    if (parseRet.Length > 0) parseRet = parseRet.Remove(parseRet.Length - 1);
                    File.WriteAllText(ad.d[0], parseRet);
                    return new Cup(p.hi);
                }
                if(ch == "read")
                {
                    Cup ad = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(ad.hi);
                    return new Cup(ad.hi, File.ReadAllText(ad.d[0]));
                }
                if (ch == "randstring")
                {
                    return new Cup(hi, randstring());
                }
                if (ch == "rename")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    if (File.Exists(p.d[1])) p.d[1] += randstring();
                    File.Copy(p.d[0], p.d[1]);
                    File.Delete(p.d[0]);
                    return new Cup(p.hi);
                }
                if (ch == "print")
                {
                    
                    Cup p = Run(hi + 1, fbd);
                    //Debug("I ran to here la "+hi);
                    if (fbd == 1) return new Cup(p.hi);
                    foreach (var s in p.d) parseRet += s + " ";
                    if (parseRet.Length > 0) parseRet = parseRet.Remove(parseRet.Length - 1);
                    return new Cup(p.hi);
                }
                if (ch == "split")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    Cup ret = new Cup(p.hi);
                    string s = p.d[0];
                    int st = 0;
                    for (int i = 0; i < s.Length - p.d[1].Length + 1; i++)
                    {
                        if (s.Substring(i, p.d[1].Length) == p.d[1])
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
                if (ch == "inttochar")
                {
                    Cup p = Run(hi + 1, fbd);
                    char c = (char)ToInt(p.d[0]);
                    string s = "";
                    s += c;
                    return new Cup(p.hi, s);
                }
                if (ch == "input")
                {
                    if (fbd == 1) return new Cup(hi);
                    return new Cup(hi, GetWindowInput());
                }
                if (ch == "show_window")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                //    foreach(var l in showLabelList)
               //     {
               //         l.Location = new Point(ToInt(V[l.Name + ".x"].d[0]), ToInt(V[l.Name + ".y"].d[0]));
              //          l.Text = V[l.Name + ".text"].d[0];
             //       }
                    f4 = new Form4();
                    //foreach (var l in showLabelList)
                   // {
                   //     f4.Controls.Add(l);
                 //   }
                    
                    f4.ShowDialog();
                    if (p.d.Count >= 2)
                        f4.Size = new Size(ToInt(p.d[0]), ToInt(p.d[1]));
                    else;
                    return new Cup(p.hi, "window built successfully");
                }
                if(ch == "rand")
                {
                    Cup p = Run(hi + 1, fbd);
                    if (fbd == 1) return new Cup(p.hi);
                    Random rand = new Random();
                    return new Cup(p.hi, rand.Next(ToInt(p.d[0])));
                }
                if(ch == "label")
                {
                    if (fbd == 1) return new Cup(hi+1);
                    Label l = new Label();
                    l.Name = h[hi + 1];
                    //showLabelList.Add(l);
                    V[l.Name + ".x"] = new Cup(-1, "0") ;
                    V[l.Name + ".y"] = new Cup(-1, "0");
                    V[l.Name + ".text"] = new Cup(-1, "undefined");
                    return new Cup (hi+1);
                }
                if(ch == "show_label")
                {

                }
                if(ch == "with")
                {
                    return new Cup(hi);
                }
                //return new Cup(hi, ch);
               
            }
            catch(Exception e)
            {
                Debug("运行句柄时出错。句柄名称："+ch+" 位置："+hi+" FBD="+fbd+"\r\n错误信息："+e.Message);
            }
            return new Cup(hi);
        }
        public List<Label> showLabelList = new List<Label>();
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

            V.Clear();
            arrayDi.Clear();
            arrayShape.Clear();
            funcHi.Clear();
            classHi.Clear();
            funcParamNam.Clear();
            paramStack.Clear();
            eqlvar.Clear();

            showLabelList.Clear();

            bedStack = "";
            //arrayData
            arrayDataUsedCount = 0;
            try
            {
                Run(0, 0);
            }
            catch
            {
               // Exception e = new Exception ();
                Debug("运行时发生了错误.");
            }

               
            // }
            // catch
            // {

            // }
            return parseRet;
        }
    }
}

using FeiSharpStudio.ClassInstance;
using FeiSharpStudio.UUID;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace FeiSharpStudio
{
    public class Parser
    {
        private Stopwatch Stopwatch { get; set; }
        private List<Token> _tokens;
        private int _current;
        public Dictionary<string, object> _variables = new();
        public Dictionary<string, FunctionInfo> _functions = new();
        public event EventHandler<OutputEventArgs> OutputEvent;
        public Dictionary<string, object> _results = new();
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
            _variables.Add("true", true);
            _variables.Add("false", false);
        }
        protected virtual void OnOutputEvent(OutputEventArgs e)
        {
            EventHandler<OutputEventArgs> handler = OutputEvent;
            handler?.Invoke(this, e);
        }
        public void ParseStatements(string funcName = "")
        {
            do
            {
                if (MatchKeyword(TokenKeywords._var))
                {
                    ParseVariableDeclaration();
                }
                else if (MatchKeyword(TokenKeywords.print))
                {
                    PrintStmt printStmt = ParsePrintStatement();
                    EvaluatePrintStmt(printStmt);
                }
                else if (MatchKeyword(TokenKeywords.init))
                {
                    ParseInitStatement();
                }
                else if (MatchKeyword(TokenKeywords.set))
                {
                    ParseSetStatement();
                }
                else if (MatchKeyword(TokenKeywords.run))
                {
                    ParseRunStatement();
                }
                else if (MatchKeyword(TokenKeywords.export))
                {
                    ParseExportStatement();
                }
                else if (MatchKeyword(TokenKeywords.start))
                {
                    ParseStartStatement();
                }
                else if (MatchKeyword(TokenKeywords.stop))
                {
                    ParseStopStatement();
                }
                else if (MatchKeyword(TokenKeywords.wait))
                {
                    ParseWaitStatement();
                }
                else if (MatchKeyword(TokenKeywords.watchstart))
                {
                    ParseWatchStartStatement();
                }
                else if (MatchKeyword(TokenKeywords.watchend))
                {
                    ParseWatchEndStatement();
                }
                else if (MatchKeyword(TokenKeywords.abe))
                {
                    ParseABEStatement();
                }
                else if (MatchKeyword(TokenKeywords.helper))
                {
                    ParseHelperStatement();
                }
                else if (MatchKeyword(TokenKeywords._if))
                {
                    ParseIfStatement();
                }
                else if (MatchKeyword(TokenKeywords._while))
                {
                    ParseWhileStatement();
                }
                else if (MatchKeyword(TokenKeywords.func))
                {
                    ParseFunctionStatement();
                }
                else if (MatchKeyword(TokenKeywords.dowhile))
                {
                    ParseDowhileStatement();
                }
                else if (MatchKeyword(TokenKeywords._throw))
                {
                    ParseThrowStatement();
                }
                else if (MatchKeyword(TokenKeywords._return))
                {
                    ParseReturnStatement(funcName);
                }
                else if (MatchKeyword(TokenKeywords.gethtml))
                {
                    ParseGetHtmlStatement();
                }
                else if (MatchKeyword(TokenKeywords.getVarsFromJsonFilePath))
                {
                    ParseGetJsonFilePathStatement();
                }
                else if (MatchKeyword(TokenKeywords.readonlyclass))
                {
                    ParseClassStatement();
                }
                else if (MatchKeyword(TokenKeywords.invoke))
                {
                    ParseInvokeStatement();
                }
                else if (MatchKeyword(TokenKeywords.read))
                {
                    ParseReadStatement();
                }
                else if (MatchKeyword(TokenKeywords.import))
                {
                    ParseImportStatement();
                }
                else if (MatchKeyword(TokenKeywords.annotation))
                {
                    ParseAnnotationStatement();
                }
                else if (MatchKeyword(TokenKeywords.define))
                {
                    ParseDefineStatement();
                }
                else if (MatchKeyword(TokenKeywords.readline))
                {
                    ParseReadLineStatement();
                }
                else if(MatchKeyword(TokenKeywords.readkey))
                {
                    ParseReadKeyStatement();
                }
                else if (MatchKeyword(TokenKeywords.ctype))
                {
                    ParseCTypeStatement();
                }
                else if (MatchKeyword(TokenKeywords.cstr))
                {
                    ParseCStRStatement();
                }
                else if (MatchKeyword(TokenKeywords._astextbox))
                {
                    ParseAstextboxStatement();
                }
                else if(MatchKeyword(TokenKeywords.createData))
                {
                    ParseCreateDataStatement();
                }
                else if (MatchKeyword(TokenKeywords.addData))
                {
                    ParseAddDataStatement();
                }
                else if (MatchKeyword(TokenKeywords.delData))
                {
                    ParseDelDataStatement();
                }
                else if (MatchKeyword(TokenKeywords.replaceData))
                {
                    ParseReplaceData();
                }
                else if (MatchKeyword(TokenKeywords.getData))
                {
                    ParseGetData();
                }
                else if (MatchKeyword(TokenKeywords.saveDataChanges))
                {
                    ParseSaveDataChange();
                }
                else if (MatchKeyword(TokenKeywords.invokeData))
                {
                    ParseInvokeData();
                }
                else if (MatchFunction(Peek().Value))
                {
                    RunFunction(Peek().Value);
                }
                else
                {
                    if(_classInfos.ContainsKey(Peek().Value))
                    Runclass(Peek().Value);
                    else
                    {
                        Advance();
                    }
                }
            } while (!IsAtEnd());
        }
        bool isfileassembly = false;
        bool isjsonassembly = false;
        bool isnetassembly = false;
        Dictionary<string,string> modals = new Dictionary<string,string>();
        private void ParseReplaceData()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            object value = EvaluateExpression(ParseExpression());
            var vari = _variables[name].ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            vari = vari.Replace(value.ToString()+",", EvaluateExpression(ParseExpression()).ToString()+",");
            _variables[name] = vari;
            Advance();
            Advance();
        }
        private void ParseSaveDataChange()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string path = EvaluateExpression(ParseExpression()).ToString();
            File.WriteAllText(path, _variables[name].ToString());
            Advance();
            Advance();
        }
        private void ParseInvokeData()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string path = EvaluateExpression(ParseExpression()).ToString();
            if(Advance().Value != "as") throw new Exception("Expected 'as' keyword");
            string name = EvaluateExpression(ParseExpression()).ToString();
            _variables.Add(name, File.ReadAllText(path));
            Advance();
            Advance();
        }
        private void ParseGetData()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string varname = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            int index = int.Parse(EvaluateExpression(ParseExpression()).ToString());
            var datas = _variables[name].ToString().Split('{')[1].Split("}")[0].Split(',');
            for (int i1 = 0; i1 < datas.Length; i1++) {
                if(i1 == index)
                {
                    _variables.Add(varname, datas[i1]);
                }
            }
            Advance();
            Advance();
        }
        private void ParseCreateDataStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            _variables.Add(name, "{}");
            Advance();
            Advance();
        }
        private void ParseAddDataStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            object value = EvaluateExpression(ParseExpression());
            var vari = _variables[name].ToString();
            vari = vari.Insert(vari.Length - 1, value.ToString()+",");
            _variables[name] = vari;
            Advance();
            Advance();
        }
        private void ParseDelDataStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            object value = EvaluateExpression(ParseExpression());
            var vari = _variables[name].ToString();
            vari = vari.Replace(value.ToString() + ",", "");
            _variables[name] = vari;
            Advance();
            Advance();
        }
        private void ParseAstextboxStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string varname = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string endValue = EvaluateExpression(ParseExpression()).ToString();
            string alltext = "";
            string readlinetxt = "";
            readlinetxt = Console.ReadLine();
            while (readlinetxt != endValue)
            {
                readlinetxt = Console.ReadLine();
                alltext += readlinetxt;
            }
            _variables.Add(varname, alltext);
            Advance();
            Advance();
        }
        private void ParseCStRStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            object convertItem = EvaluateExpression(ParseExpression());
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string varname = EvaluateExpression(ParseExpression()).ToString();
            _variables.Add(varname, convertItem.ToString());
            Advance();
Advance();
        }
        private void ParseCTypeStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            object convertItem = EvaluateExpression(ParseExpression());
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string type = EvaluateExpression(ParseExpression()).ToString();
            Type typeT = null;
            if (type == "system.integer")
            {
                typeT = typeof(int);
            }
            else if(type == "system.string")
            {
                typeT = typeof(string);
            }
            else if(type == "system.boolean")
            {
                typeT = typeof(bool);
            }
            object convertedItem = Convert.ChangeType(convertItem, typeT != null ? typeT : typeof(string));
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string varname = EvaluateExpression(ParseExpression()).ToString();
            if (convertedItem is int)
            {
                _variables.Add(varname, Convert.ToInt32(convertedItem));
            }
            else if (convertedItem is string) { 
                _variables.Add(varname, convertedItem.ToString());
            }
            else
            {
                _variables.Add(varname, bool.Parse(convertedItem.ToString()));
            }
            Advance();
Advance();
        }
        private void ParseReadLineStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if(name == "_")
            {
                Console.ReadLine();
            }
            else
            {
                _variables.Add(name,Console.ReadLine());
            }
            Advance();
Advance();
        }
        private void ParseReadKeyStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string name = EvaluateExpression(ParseExpression()).ToString();
            if (name == "_")
            {
                Console.ReadKey();
            }
            else
            {
                _variables
                .Add(name, Console.ReadKey().KeyChar.ToString());
            }
            Console.WriteLine();
            Advance();
Advance();
        }
        private void ParseDefineStatement()
        {
            string context = EvaluateExpression(ParseExpression()).ToString();
            if(context == "modal")
            {
                try
                {
                    string modalName = EvaluateExpression(ParseExpression()).ToString();
                    string modalSet = EvaluateExpression(ParseExpression()).ToString();
                    modals.Add(modalName, modalSet);
                    _variables.Add(modalName, modalSet);
                }
                catch
                {
                    OnOutputEvent(new("Enter STRING_OBJ('modalName' or 'modalSet') is not valid."));
                }
            }
            else if(context == "edit")
            {
                try
                {
                    string id = EvaluateExpression(ParseExpression()).ToString();
                    string value = EvaluateExpression(ParseExpression()).ToString();
                    modals[id] = value;
                    _variables[id] = value;
                }
                catch
                {
                    OnOutputEvent(new("Enter STRING_OBJ('id' or 'value') is not valid."));
                }
            }
            else if(context == "view")
            {
                if(modals.Count == 0)
                {
                    Console.WriteLine("MODALS_OBJS: It is empty.");
                }
                else
                {
                    foreach (var item in modals)
                    {
                        OnOutputEvent(new("[" + item.Key + ":" + item.Value + "]" + "\r\n"));
                    }
                    OnOutputEvent(new(modals.Count+" modals in MODALS_OBJS."));
                }
            }
            else
            {
                OnOutputEvent(new(context+": It is not a correct DEFINE_OBJ.DEFiNE_OBJ is must be 'modal', 'view' or 'edit'."));
                Advance();
            }
Advance();
        }
        private void ParseAnnotationStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string context = EvaluateExpression(ParseExpression()).ToString();
            Debug.WriteLine("code annotation:" + context);
            Advance();
Advance();
        }
        private void ParseImportStatement()
        {
            string assembly = EvaluateExpression(ParseExpression()).ToString();
            if (assembly == "FeiSharp.IO") { 
                isfileassembly = true;
            }
            else if(assembly == "FeiSharp.Json")
            {
                isjsonassembly = true;
            }
            else if (assembly == "FeiSharp.Net.Http")
            {
                isnetassembly = true;
            }
            Advance();
        }
        private void ParseReadStatement()
        {
            if (isfileassembly)
            {
                if (!MatchPunctuation("(")) throw new Exception("Expected '('");
                string varname = EvaluateExpression(ParseExpression()).ToString();
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                string path = EvaluateExpression(ParseExpression()).ToString();
                Advance();
                try
                {
                    _variables.Add(varname, File.ReadAllText(path));
                }
                catch
                {
                    _variables[varname] = File.ReadAllText(path);
                }
    Advance();
            }
            else
            {
                throw new Exception("the text \"read\" is not a context var function or readonlyclass name.(are you missing import assembly:FeiSharp.IO?)");
            }
        }
        private KeyValuePair<string,bool> Runclass(string name)
        {
            ClassInfo classInfo = _classInfos[name];
            string funcorvarname = "";
            bool isFunc = default;
            try {
                foreach (var item in classInfo._Vars)
                {
                    _variables.Add(item.Key, item.Value);
                }
            }
            catch
            {
                goto Parse;
            }
        Parse:
            Advance();
            if (classInfo != null) {
                if (Peek().Value == ".") {
                    Advance();
                    if (classInfo._FunctionInfo.ContainsKey(Peek().Value))
                    {
                        _functions.Add(Peek().Value, classInfo._FunctionInfo[Peek().Value]);
                        funcorvarname = Peek().Value;
                        isFunc = true;
                        RunFunction(Peek().Value);
                        Advance();
                    }
                    else if (classInfo._Vars.ContainsKey(Peek().Value))
                    {
                        funcorvarname = Peek().Value;
                        isFunc = false;
                }
                    }
            }
            return new(funcorvarname,isFunc);
        }
        private void ParseClassStatement()
        {
            string className = Peek().Value;
            Advance();
            List<Token> tokens = new List<Token>();
            int indexC = 0;
            for (int i = _current + 1; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "}")
                {
                    indexC = i;
                    Advance();
                    break;
                }
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "{")
                {
                    Advance();
                    continue;
                }
                tokens.Add(_tokens[i]);
                Advance();
            }
            Advance();
            ClassInfo classInfo;
            Parser parser = new(tokens);
            parser.OutputEvent += (s, e) => Console.WriteLine();
            var @return = parser.Run(tokens, new(), 0);
            classInfo = new(@return.Value,@return.Key,className);
            _classInfos.Add(className, classInfo);
        }
        internal Dictionary<string,ClassInfo> _classInfos = new Dictionary<string,ClassInfo>();
        private void ParseInvokeStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string uuid = EvaluateExpression(ParseExpression()).ToString();
            if(uuid == UUIDData.AndUUID)
            {
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                string varname = EvaluateExpression(ParseExpression()).ToString();
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                bool bool1 = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                bool bool2 = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
                try
                {
                    _variables.Add(varname, (object)(bool1 && bool2));
                }
                catch
                {
                    _variables[varname] = (object)(bool1 && bool2);
                }
                Advance();
    Advance();
            }
            else if(uuid == UUIDData.OrUUID)
            {
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                string varname = EvaluateExpression(ParseExpression()).ToString();
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                bool bool1 = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                bool bool2 = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
                try
                {
                    _variables.Add(varname, (object)(bool1 || bool2));
                }
                catch
                {
                    _variables[varname] = (object)(bool1 || bool2);
                }
                Advance();
    Advance();
            }
            else if (uuid == UUIDData.NotUUID)
            {
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                string varname = EvaluateExpression(ParseExpression()).ToString();
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                bool bool1 = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
                try
                {
                    _variables.Add(varname, (object)(!bool1));
                }
                catch
                {
                    _variables[varname] = (object)(!bool1);
                }
                Advance();
    Advance();
            }
        }
        private void ParseGetJsonFilePathStatement()
        {
            if (isjsonassembly)
            {
                if (!MatchPunctuation("(")) throw new Exception("Expected '('");
                string a = File.ReadAllText(EvaluateExpression(ParseExpression()).ToString());
                Console.WriteLine(a);
                Advance();
    Advance();
            }
            else
            {
                throw new Exception("the text \"getJsonFromFilePath\" is not a context var function or readonlyclass name.(are you missing import assembly:json?)");
            }
        }
        private void ParseGetHtmlStatement()
        {
            if (isnetassembly)
            {
                if (!MatchPunctuation("(")) throw new Exception("Expected '('");
                string content = "";
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(EvaluateExpression(ParseExpression()).ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    content = response.Content.ReadAsStringAsync().Result;
                }
                if (!MatchPunctuation(",")) throw new Exception("Expected ','");
                string a = EvaluateExpression(ParseExpression()).ToString();
                try
                {
                    _variables.Add(a, content);
                }
                catch
                {
                    _variables[a] = content;
                }
                Advance();
    Advance();
            }
            else
            {
                throw new Exception("the text \"gethtml\" is not a context var function or readonlyclass name.(are you missing import assembly:net?)");
            }
        }
        private void ParseReturnStatement(string funcName)
        {
            _results.Add(funcName, EvaluateExpression(ParseExpression()));
            _variables.Add($"{funcName}:return", _results[funcName]);
        }

        private void ParseThrowStatement()
        {
            string msg = EvaluateExpression(ParseExpression()).ToString();
            Console.WriteLine("Application: FeiSharp.Exception went throw at throw ststement, "+msg+" Maybe this application's some value is invalid or some args is invalid.");
            Console.Write("Do you want to continue and skip it?(y/n)");
            var a = Console.ReadKey();
            Console.WriteLine();
            if(a.Key == ConsoleKey.N)
            {
                throw new Exception("This application is stop......");
            }
            Advance();
        }
        private void ParseDowhileStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            int current = _current;
            string b = EvaluateExpression(ParseExpression()).ToString();
            bool a = bool.Parse(b);
            Advance();
            List<Token> tokens = new List<Token>();
            int indexC = 0;
            for (int i = _current + 1; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "}")
                {
                    indexC = i;
                    break;
                }
                tokens.Add(_tokens[i]);
            }
            do
            {
                _variables = Run(tokens, _variables);
                _current = current;
                a = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
            } while (a);
            _current = indexC;
        }
        private void RunFunction(string funcName)
        {
            FunctionInfo functionInfo = _functions[funcName];
            List<object> actualParameters = new();
            Advance();
            while (Peek().Value != ")" && Peek().Value != ";")
            {
                if (Peek().Value == "," || Peek().Value == "(")
                {
                    Advance();
                    continue;
                }
                else
                {
                    actualParameters.Add(EvaluateExpression(ParseExpression()));
                    Advance();
                }
            }
            for (int i = 0; i < functionInfo.Parameter.Count; i++)
            {
                try
                {
                    _variables.Add(functionInfo.Parameter[i], actualParameters[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception("Parameters is not correct.");
                }
            }
            _variables = Run(functionInfo.FunctionBody, _variables, funcName);
        }
        private void RunFunction(string funcName,List<Token> tokens,List<string> args)
        {
            FunctionInfo functionInfo = new(funcName,args,tokens);
            List<object> actualParameters = new();
            Advance();
            while (Peek().Value != ")" && Peek().Value != ";")
            {
                if (Peek().Value == "," || Peek().Value == "(")
                {
                    Advance();
                    continue;
                }
                else
                {
                    actualParameters.Add(EvaluateExpression(ParseExpression()));
                    Advance();
                }
            }
            for (int i = 0; i < functionInfo.Parameter.Count; i++)
            {
                try
                {
                    _variables.Add(functionInfo.Parameter[i], actualParameters[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception("Parameters is not correct.");
                }
            }
            _variables = Run(functionInfo.FunctionBody, _variables, funcName);
        }
        private bool MatchFunction(string funcName)
        {
            return _functions.ContainsKey(funcName);
        }
        private void ParseFunctionStatement()
        {
            FunctionInfo functionInfo;
            string name = "";
            name = Peek().Value;
            List<string> parameters = [];
            Advance();
            while (Peek().Value != ")")
            {
                if (Peek().Value == "," || Peek().Value == "(")
                {
                    Advance();
                    continue;
                }
                else
                {
                    parameters.Add(Peek().Value);
                    Advance();
                }
            }
            Advance();
            List<Token> tokens = ParseTokens();
            functionInfo = new(name, parameters, tokens);
            _functions.Add(name, functionInfo);
            Advance();
            Advance();
        }
        private List<Token> ParseTokens()
        {
            List<Token> tokens = new List<Token>();
            int indexC = 0;
            for (int i = _current + 1; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "]")
                {
                    indexC = i;
                    break;
                }
                tokens.Add(_tokens[i]);
                Advance();
            }
            return tokens;
        }
        private void ParseWhileStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            int current = _current;
            string b = EvaluateExpression(ParseExpression()).ToString();
            bool a = bool.Parse(b);
            Advance();
            Advance();
            List<Token> tokens = new List<Token>();
            int indexC = 0;
            for (int i = _current; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "}")
                {
                    indexC = i;
                    Advance();
                    break;
                }
                tokens.Add(_tokens[i]);
                Advance();
            }
            Advance();
            while (a)
            {
                _variables = Run(tokens, _variables);
                _current = current;
                a = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
            }
            _current = indexC;
        }
        private void ParseIfStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            int current = _current;
            string b = EvaluateExpression(ParseExpression()).ToString();
            bool a = bool.Parse(b);
            Advance();
            List<Token> tokens = new List<Token>();
            int indexC = 0;
            for (int i = _current + 1; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "]")
                {
                    indexC = i;
                    break;
                }
                if (_tokens[i].Type == TokenTypes.Punctuation && _tokens[i].Value == "[")
                {
                    continue;
                }
                tokens.Add(_tokens[i]);
            }
            Advance();
            if (a)
            {
                _variables = Run(tokens, _variables);
                _current = current;
                a = bool.Parse(EvaluateExpression(ParseExpression()).ToString());
            }
            _current = indexC;
        }
        private void ParseHelperStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string a = EvaluateExpression(ParseExpression()).ToString();
            if (a.Equals("syntax", StringComparison.OrdinalIgnoreCase))
            {
                OnOutputEvent(new OutputEventArgs("Syntax:\r\n1.keyword+(args);\r\nInvoke keyword with args.\r\nWarning: If keyword hasn't args,\r\nuse keyword+;\r\n2.Define var.\r\n(1)define:\r\ninit(varname,Type); Or var varname = value;\r\n(2)assignment:\r\nset(varname,value);\r\n3.Keywords Table.\r\n________________________________________________\r\n|keyword   |  args   |  do somwthings           |\r\n|paint        text     print the text           |\r\n|watchstart  varname   start stopwatch.         |\r\n|watchend     null     stop stopwatch           |\r\n|init    varname,Type  init var.                |\r\n|set    varname,value  set var.                 |\r\n|...          ....     ............             |\r\n|_______________________________________________|"));
            }
            else if (a.Equals("github", StringComparison.OrdinalIgnoreCase))
            {
                OnOutputEvent(new("https://github.com/Mars-FeiFei/FeiSharp-Terminal/tree/main\r\n"));
            }
            else
            {
                throw new Exception("Invalid string for \"helper\" keyword: " + a);
            }
            Advance();
Advance();
        }
        private void ParseABEStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            string a = EvaluateExpression(ParseExpression()).ToString();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            double b = double.Parse(EvaluateExpression(ParseExpression()).ToString());
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            double c = double.Parse(EvaluateExpression(ParseExpression()).ToString());
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            double d = double.Parse(EvaluateExpression(ParseExpression()).ToString());
            Advance();
Advance();
            try
            {
                _variables.Add(a, (b + c + d) / 3);
            }
            catch
            {
                _variables[a] = (b + c + d) / 3;
            }
        }
        private void ParseWatchEndStatement()
        {
            Stopwatch.Stop();
            try
            {
                _variables.Add(name, Stopwatch.Elapsed.TotalSeconds);
            }
            catch
            {
                _variables[name] = Stopwatch.Elapsed.TotalSeconds;
            }
Advance();
        }
        string name = "";
        private void ParseWatchStartStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Stopwatch = Stopwatch.StartNew();
            name = EvaluateExpression(ParseExpression()).ToString();
            Advance();
Advance();
        }
        private void ParseWaitStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Thread.Sleep(int.Parse(EvaluateExpression(ParseExpression()).ToString()) * 1000);
            Advance();
Advance();
        }
        private void ParseStopStatement()
        {
Advance();
            OnOutputEvent(new OutputEventArgs("Application is stop..."));
            foreach (var item in _variables)
            {
                OnOutputEvent(new OutputEventArgs($"var {item.Key} = {item.Value} : {item.Value.GetType()}"));
            }
            OnOutputEvent(new OutputEventArgs($"{_variables.Count}" + " items of vars."));
            //Console Edition
            Console.WriteLine("Enter any key to continue......");
            Console.ReadKey();
            Console.WriteLine();
        }
        private void ParseStartStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Expr b = ParseExpression();
            string a = (string)EvaluateExpression(b);
            Process.Start(a);
            Advance();
Advance();
        }
        private void ParseExportStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Expr b = ParseExpression();
            string a = (string)EvaluateExpression(b);
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            Expr b1 = ParseExpression();
            string a1 = (string)EvaluateExpression(b1);
            File.AppendAllText(a, a1);
            Advance();
Advance();
        }
        private void ParseRunStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Expr b = ParseExpression();
            string a = (string)EvaluateExpression(b);
            Run(File.ReadAllText(a));
            Advance();
Advance();
        }
        internal void Run(string code)
        {
            string sourceCode = code;
            Lexer lexer = new(sourceCode);
            List<Token> tokens = [];
            Token token;
            do
            {
                token = lexer.NextToken();
                tokens.Add(token);
            } while (token.Type != TokenTypes.EndOfFile);

            Parser parser = new(tokens);
            parser._functions = _functions;
            try
            {
                parser.ParseStatements();
            }
            catch (Exception ex)
            {
                OnOutputEvent(new OutputEventArgs("Parsing error: " + ex.Message));
            }
            return;
        }
        internal Dictionary<string, object> Run(IEnumerable<Token> tokens, Dictionary<string, object> _vars)
        {
            List<Token> _tokens = new(tokens);
            Parser parser = new(_tokens);
            parser.OutputEvent = this.OutputEvent;
            parser._variables = _vars;
            try
            {
                parser.ParseStatements();
            }
            catch (Exception ex)
            {
                OnOutputEvent(new OutputEventArgs("Parsing error: " + ex.Message));
            }
            return parser._variables;
        }
        internal KeyValuePair<Dictionary<string,object>, Dictionary<string, FunctionInfo>> Run(IEnumerable<Token> tokens, Dictionary<string, object> _vars, int op = 0)
        {
            List<Token> _tokens = new(tokens);
            Parser parser = new(_tokens);
            parser.OutputEvent = this.OutputEvent;
            parser._variables = _vars;
            try
            {
                parser.ParseStatements();
            }
            catch (Exception ex)
            {
                OnOutputEvent(new OutputEventArgs("Parsing error: " + ex.Message));
            }
            KeyValuePair<Dictionary<string, object>, Dictionary<string,FunctionInfo>> result = new(parser._variables,parser._functions);
            return result;
        }
        internal Dictionary<string, object> Run(IEnumerable<Token> tokens, Dictionary<string, object> _vars, string funcName)
        {
            List<Token> _tokens = new(tokens);
            Parser parser = new(_tokens);
            parser.OutputEvent = this.OutputEvent;
            parser._variables = _vars;
            try
            {
                parser.ParseStatements(funcName);
            }
            catch (Exception ex)
            {
                OnOutputEvent(new OutputEventArgs("Parsing error: " + ex.Message));
            }
            return parser._variables;
        }
        internal Dictionary<string, object> Run(string code, int a)
        {
            string sourceCode = code;
            Lexer lexer = new(sourceCode);
            List<Token> tokens = [];
            Token token;
            do
            {
                token = lexer.NextToken();
                tokens.Add(token);
            } while (token.Type != TokenTypes.EndOfFile);

            Parser parser = new(tokens);
            parser.OutputEvent = this.OutputEvent;
            try
            {
                parser.ParseStatements();
            }
            catch (Exception ex)
            {
                OnOutputEvent(new OutputEventArgs("Parsing error: " + ex.Message));
            }
            return parser._variables;
        }
        private void ParseSetStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Expr expr = GetVar();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            string name = EvaluateExpression(ParseExpression()).ToString();
            Expr expr2 = new VarExpr(name);
            Advance();
Advance();
            _variables[((VarExpr)expr).Name] = ((VarExpr)expr2).Name;
        }
        private void ParseInitStatement()
        {
            if (!MatchPunctuation("(")) throw new Exception("Expected '('");
            Expr expr = GetVar();
            if (!MatchPunctuation(",")) throw new Exception("Expected ','");
            Expr expr2 = GetType();
            Advance();
Advance();
            _variables.Add(((VarExpr)expr).Name, InitValue(((VarExpr)expr2).Name));
        }
        private object InitValue(string type)
        {
            Type t = Type.GetType("System." + type);
            return Activator.CreateInstance(t);
        }
        private Expr GetVar()
        {
            if (MatchToken(TokenTypes.Identifier))
            {
                return new VarExpr(Previous().Value);
            }
            return null;
        }
        private Expr GetType()
        {
            if (MatchToken(TokenTypes.Type))
            {
                return new VarExpr(Previous().Value);
            }
            return null;
        }
        private void ParseVariableDeclaration()
        {
            if (!MatchToken(TokenTypes.Identifier, out string varName))
            {
                throw new Exception("Expected variable name");
            }

            if (!MatchToken(TokenTypes.Operator, out string op) || op != "=")
            {
                throw new Exception("Expected '=' after variable name");
            }
            
            if(Peek().Value == "new" && Peek().Type == TokenTypes.Identifier)
            {
                Advance();
                foreach(var item in _classInfos)
                {
                    string classname = Peek().Value;
                    if(classname == item.Key)
                    {
                        Advance();
                        List<string> parameters = new List<string>();
                        while (Peek().Value != ")")
                        {
                            if (Peek().Value == "," || Peek().Value == "(")
                            {
                                Advance();
                                continue;
                            }
                            else
                            {
                                parameters.Add(Peek().Value);
                                Advance();
                            }
                        }
                        List<Token> tokens = new List<Token>();
                        Lexer lexer;
                        if(parameters.Count == 0)
                        {
                            lexer = new($"{classname}.init;");
                        }
                        else
                        {
                            string a = "";
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                string? item1 = parameters[i];
                                if(i == parameters.Count - 1)
                                {
                                    a += item1;
                                }
                                else
                                {
                                    a += item1 + ",";
                                }
                            }
                            lexer = new($"{classname}.init({a});");
                        }
                        Token token;
                        do
                        {
                            token = lexer.NextToken();
                            tokens.Add(token);
                        }
                        while (token.Type != TokenTypes.EndOfFile);
                        Parser parser = new(tokens);
                        parser._classInfos = _classInfos;
                        parser.ParseStatements();
                        Dictionary<string,object> _vars = new Dictionary<string,object>();
                        foreach(var item2 in parser._classInfos[classname]._Vars)
                        {
                            _vars.Add(item2.Key, item2.Value);
                        }
                        _classInfos.Add(varName,new(item.Value._FunctionInfo,_vars,varName));
                        break;
                    }
                }
                Advance();
                Advance();
            }
            else
            {
                Expr expr = ParseExpression();
                if (!MatchPunctuation(";"))
                {
                    throw new Exception("Expected ';' after variable declaration");
                }

                object value = EvaluateExpression(expr);
                _variables[varName] = value;
            }
        }

        private PrintStmt ParsePrintStatement()
        {
            if (!MatchPunctuation("("))
            {
                _current--;
                string text = EvaluateExpression(ParseExpression()).ToString();
                if (Peek().Value == "as")
                {
                    Advance();
                    string path = EvaluateExpression(ParseExpression()).ToString();
                    Advance();
                    File.WriteAllText(path, text);
                }
                else
                {
                    OnOutputEvent(new("No 'as' keyword."));
                }
            }
            Expr expr = ParseExpression();
            Advance();
            Advance();
            return new PrintStmt(expr);
        }

        private Expr ParseExpression()
        {
            Expr expr = ParsePrimary();
            while (MatchOperator("+", "-", "*", "/", "|", "^", "<", ">", "=", "!","|","&", "$"))
            {
                string op = Previous().Value;
                Expr right = ParsePrimary();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        private Expr ParsePrimary()
        {
            if (MatchToken(TokenTypes.Number))
            {
                return new ValueExpr(double.Parse(Previous().Value));
            }
            else if (MatchToken(TokenTypes.String))
            {
                return new StringExpr(Previous().Value);
            }
            else if (MatchToken(TokenTypes.Identifier))
            {
                string varName = Previous().Value;
                if (_variables.TryGetValue(varName, out object value))
                {
                    return new ValueExpr(value);
                }
                else if(_functions.ContainsKey(varName))
                {
                    RunFunction(varName);
                    return new ValueExpr(_variables[$"{varName}:return"]);
                }
                else
                {
                    var a = Runclass(varName);
                    if (a.Value)
                    {
                        return new ValueExpr(_variables[$"{a.Key}:return"]);
                    }
                    else {
                        return new ValueExpr(_variables[a.Key]);
                    }
                }
                throw new Exception($"Undefined variable: {varName}");
            }
            else if (MatchPunctuation("("))
            {
                Expr expr = ParseExpression();
                if (!MatchPunctuation(")"))
                {
                    throw new Exception("Expected ')' after expression");
                }
                return expr;
            }
            else if (MatchKeyword("true"))
            {
                string varName = Previous().Value;
                if (_variables.TryGetValue(varName, out object value))
                {
                    return new ValueExpr(value);
                }

                throw new Exception($"Undefined variable: {varName}");
            }
            else if (MatchKeyword("false"))
            {
                string varName = Previous().Value;
                if (_variables.TryGetValue(varName, out object value))
                {
                    return new ValueExpr(value);
                }

                throw new Exception($"Undefined variable: {varName}");
            }
            else
            {
                if(Previous().Type == TokenTypes.Keyword && Previous().Value == "true")
                {
                    _current--;
                    return new ValueExpr(true);
                }
                else if (Previous().Type == TokenTypes.Keyword && Previous().Value == "false")
                {
                    _current--;
                    return new ValueExpr(true);
                }
                else if (MatchPreviousToken(TokenTypes.Number))
                {
                    return new ValueExpr(double.Parse(Previous().Value));
                }
                else if (MatchPreviousToken(TokenTypes.String))
                {
                    return new StringExpr(Previous().Value);
                }
                else if (MatchPreviousToken(TokenTypes.Identifier))
                {
                    string varName = Previous().Value;
                    if (_variables.TryGetValue(varName, out object value))
                    {
                        return new ValueExpr(value);
                    }
                    else if (_functions.ContainsKey(varName))
                    {
                        RunFunction(varName);
                        return new ValueExpr(_variables[$"{varName}:return"]);
                    }
                    else
                    {
                        var a = Runclass(varName);
                        if (a.Value)
                        {
                            return new ValueExpr(_variables[$"{a.Key}:return"]);
                        }
                        else
                        {
                            return new ValueExpr(_variables[a.Key]);
                        }
                    }
                    throw new Exception($"Undefined variable: {varName}");
                }
                else if (MatchPunctuation("("))
                {
                    Expr expr = ParseExpression();
                    if (!MatchPunctuation(")"))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    return expr;
                }
            }
            throw new Exception("Unvalid token: " + Peek().Value);
        }
        private bool MatchPreviousToken(params TokenTypes[] types)
        {
            foreach (var type in types)
            {
                if (PreviousCheck(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        private bool PreviousCheck(TokenTypes type)
        {
            return !IsAtEnd() && Previous().Type == type;
        }
        private bool MatchToken(params TokenTypes[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool MatchToken(TokenTypes type, out string value)
        {
            if (Check(type))
            {
                value = Peek().Value;
                Advance();
                return true;
            }
            value = null;
            return false;
        }
        private bool MatchKeyword(string keyword)
        {
            if (Check(TokenTypes.Keyword) && Peek().Value == keyword)
            {
                Advance();
                return true;
            }
            return false;
        }
        private bool MatchPunctuation(string punctuation)
        {
            if (Check(TokenTypes.Punctuation) && Peek().Value == punctuation)
            {
                Advance();
                return true;
            }
            else {
                Advance();
                if (Check(TokenTypes.Punctuation) && Peek().Value == punctuation)
                {
                    return true;
                }
            }
            return false;
        }
        private bool MatchOperator(params string[] operators)
        {
            if (Check(TokenTypes.Operator) && operators.Contains(Peek().Value))
            {
                Advance();
                return true;
            }
            return false;
        }
        private bool Check(TokenTypes type)
        {
            return !IsAtEnd() && Peek().Type == type;
        }
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }
        private bool IsAtEnd()
        {
            return _current >= _tokens.Count;
        }
        private Token Peek()
        {
            if (IsAtEnd()) throw new InvalidOperationException("No more tokens available.");
            return _tokens[_current];
        }
        private Token Previous()
        {
            if (_current == 0) throw new InvalidOperationException("No previous token available.");
            return _tokens[_current - 1];
        }
        private void EvaluatePrintStmt(PrintStmt stmt)
        {
            string text = EvaluateExpression(stmt.Expression).ToString();
            OnOutputEvent(new OutputEventArgs(text));
        }
        private object EvaluateExpression(Expr expr)
        {
            switch (expr)
            {
                case ValueExpr numExpr:
                    if (double.TryParse(numExpr.Value.ToString(), out double a))
                    {
                        return double.Parse(numExpr.Value.ToString());
                    }
                    else
                    {
                        if(numExpr.Value is string)
                        {
                            return numExpr.Value.ToString();
                        }
                        else
                        {
                            return bool.Parse(numExpr.Value.ToString());
                        }
                    }

                case BinaryExpr binExpr:
                    object left = EvaluateExpression(binExpr.Left);
                    object right = EvaluateExpression(binExpr.Right);
                    if (left is string && right is string)
                    {
                        return binExpr.Operator switch
                        {
                            "+" => (string)left + (string)right,
                            "-" => Regex.Replace((string)left, Regex.Escape((string)right), ""),
                            "=" => (string)left == (string)right,
                            "!" => (string)left != (string)right,
                            _ => throw new Exception("Unexpected operator: " + binExpr.Operator)
                        };
                    }
                    else if (right is bool && left is bool)
                    {
                        return binExpr.Operator switch
                        {
                            "&" => (bool)left && (bool)right,
                            "|" => (bool)left || (bool)right,
                            "!" => (bool)left != (bool)right,
                            _ => throw new Exception("Unexpected operator: " + binExpr.Operator)
                        };
                    }
                    left = Convert.ToDouble(left.ToString());
                    right = Convert.ToDouble(right.ToString());
                    return binExpr.Operator switch
                    {
                        "+" => (double)left + (double)right,
                        "-" => (double)left - (double)right,
                        "*" => (double)left * (double)right,
                        "/" => (double)left / (double)right,
                        "^" => Math.Pow((double)left, (double)right),
                        "|" => Math.Pow((double)left, 1 / (double)right),
                        ">" => (double)left > (double)right,
                        "<" => (double)left < (double)right,
                        "=" => (double)left == (double)right,
                        "!" => (double)left != (double)right,
                        "$" => 1 / (double)left,
                        _ => throw new Exception("Unexpected operator: " + binExpr.Operator)
                    };
                case StringExpr stringExpr:
                    return stringExpr.Value;
                default:
                    throw new Exception("Unexpected expression type");
            }
            string RepeatZeros(int a)
            {
                string result = "";
                for (int i = 0; i < a; i++)
                {
                    result += "0";
                }
                return result;
            }
        }
    }
}

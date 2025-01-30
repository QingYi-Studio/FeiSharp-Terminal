using System.Runtime.CompilerServices;

namespace FeiSharpStudio
{
    
    public class Lexer
    {
        private readonly string _source;
        private int _index;

        public Lexer(string source)
        {
            _source = source;
            _index = 0;
        }

        public Token NextToken()
        {
            while (_index < _source.Length)
            {
                char current = _source[_index];
                if (char.IsWhiteSpace(current))
                {
                    _index++;
                    continue;
                }
                if (current == '&') { _index++; return new Token(TokenTypes.Operator, "&"); }
                if (current == '|') { _index++; return new Token(TokenTypes.Operator, "|"); }
                if (current == '.') { _index++; return new Token(TokenTypes.Punctuation, "."); }
                if (current == ']') { _index++; return new Token(TokenTypes.Punctuation, "]"); }
                if (current == '[') { _index++; return new Token(TokenTypes.Punctuation, "["); }
                if (current == '!') { _index++; return new Token(TokenTypes.Operator, "!"); }
                if (current == '}') { _index++; return new Token(TokenTypes.Punctuation, "}"); }
                if (current == '{') { _index++; return new Token(TokenTypes.Punctuation, "{"); }
                if (current == '<') { _index++; return new Token(TokenTypes.Operator, "<"); }
                if (current == '>') { _index++; return new Token(TokenTypes.Operator, ">"); }
                if (current == '=') { _index++; return new Token(TokenTypes.Operator, "="); }
                if (current == '|') { _index++; return new Token(TokenTypes.Operator, "|"); }
                if (current == '^') { _index++; return new Token(TokenTypes.Operator, "^"); }
                if (current == '/') { _index++; return new Token(TokenTypes.Operator, "/"); }
                if (current == '*') { _index++; return new Token(TokenTypes.Operator, "*"); }
                if (current == '-') { _index++; return new Token(TokenTypes.Operator, "-"); }
                if (current == ',') { _index++; return new Token(TokenTypes.Punctuation, ","); }
                if (current == '+') { _index++; return new Token(TokenTypes.Operator, "+"); }
                if (current == '-') { _index++; return new Token(TokenTypes.Operator, "-"); }
                if (current == '=') { _index++; return new Token(TokenTypes.Operator, "="); } 
                if (current == ';') { _index++; return new Token(TokenTypes.Punctuation, ";"); }
                if (current == '(') { _index++; return new Token(TokenTypes.Punctuation, "("); }
                if (current == ')') { _index++; return new Token(TokenTypes.Punctuation, ")"); }
                if (current == '$') { _index++; return new Token(TokenTypes.Operator, "$"); }
                if (current == '"')
                {
                    int start = ++_index;

                    while (_index < _source.Length && _source[_index] != '"')
                    {
                        _index++;
                    }
                    return new Token(TokenTypes.String, _source[start.._index++]);
                }
                if (char.IsDigit(current))
                {
                    int start = _index;
                    while (_index < _source.Length && (char.IsDigit(_source[_index]) || _source[_index] == '.')) _index++;
                    return new Token(TokenTypes.Number, _source[start.._index]);
                }

                if (char.IsLetter(current))
                {
                    int start = _index;
                    while (_index < _source.Length && char.IsLetter(_source[_index])) _index++;
                    string value = _source.Substring(start, _index - start);
                    if (value == TokenKeywords._var) return new Token(TokenTypes.Keyword, "var");
                    else if (value == TokenKeywords.gethtml) return new Token(TokenTypes.Keyword, "gethtml");
                    else if (value == TokenKeywords.print) return new Token(TokenTypes.Keyword, "print");
                    else if (value == TokenKeywords.init) return new Token(TokenTypes.Keyword, "init");
                    else if (value == TokenKeywords.set) return new Token(TokenTypes.Keyword, "set");
                    else if (value == TokenKeywords.run) return new Token(TokenTypes.Keyword, "run");
                    else if (value == TokenKeywords.export) return new Token(TokenTypes.Keyword, "export");
                    else if (value == TokenKeywords.start) return new Token(TokenTypes.Keyword, "start");
                    else if (value == TokenKeywords.stop) return new Token(TokenTypes.Keyword, "stop");
                    else if (value == TokenKeywords.wait) return new Token(TokenTypes.Keyword, "wait");
                    else if (value == TokenKeywords.watchstart) return new Token(TokenTypes.Keyword, "watchstart");
                    else if (value == TokenKeywords.watchend) return new Token(TokenTypes.Keyword, "watchend");
                    else if (value == TokenKeywords.abe) return new Token(TokenTypes.Keyword, "abe");
                    else if (value == TokenKeywords.Double) return new Token(TokenTypes.Type, "Double");
                    else if (value == TokenKeywords.helper) return new Token(TokenTypes.Keyword, "helper");
                    else if (value == TokenKeywords._true) return new Token(TokenTypes.Keyword, "true");
                    else if (value == TokenKeywords._false) return new Token(TokenTypes.Keyword, "false");
                    else if (value == TokenKeywords._if) return new Token(TokenTypes.Keyword, "if");
                    else if (value == TokenKeywords._while) return new Token(TokenTypes.Keyword, "while");
                    else if (value == TokenKeywords.dowhile) return new Token(TokenTypes.Keyword, "dowhile");
                    else if (value == TokenKeywords._throw) return new Token(TokenTypes.Keyword, "throw");
                    else if (value == TokenKeywords._return) return new Token(TokenTypes.Keyword, "return");
                    else if (value == TokenKeywords.getVarsFromJsonFilePath) return new Token(TokenTypes.Keyword, "getVarsFromJsonFilePath");
                    else if (value == TokenKeywords.readonlyclass) return new Token(TokenTypes.Keyword, "readonlyclass");
                    else if (value == TokenKeywords.invoke) return new Token(TokenTypes.Keyword, "invoke");
                    else if (value == TokenKeywords.read) return new Token(TokenTypes.Keyword, "read");
                    else if (value == TokenKeywords.import) return new Token(TokenTypes.Keyword, "import");
                    else if (value == TokenKeywords.annotation) return new Token(TokenTypes.Keyword, "annotation");
                    else if (value == TokenKeywords.define) return new Token(TokenTypes.Keyword, "define");
                    else if (value == TokenKeywords.readline) return new Token(TokenTypes.Keyword, "readline");
                    else if (value == TokenKeywords.readkey) return new Token(TokenTypes.Keyword, "readkey");
                    else if (value == TokenKeywords.ctype) return new Token(TokenTypes.Keyword, "ctype");
                    else if (value == TokenKeywords.cstr) return new Token(TokenTypes.Keyword, "cstr");
                    else if (value == TokenKeywords._astextbox) return new Token(TokenTypes.Keyword, "astextbox");
                    else if (value == TokenKeywords.createData) return new Token(TokenTypes.Keyword, "createData");
                    else if (value == TokenKeywords.addBundingData) return new Token(TokenTypes.Keyword, "addBundingData");
                    else if (value == TokenKeywords.addData) return new Token(TokenTypes.Keyword, "addData");
                    else if (value == TokenKeywords.delData) return new Token(TokenTypes.Keyword, "delData");
                    else if (value == TokenKeywords.replaceData) return new Token(TokenTypes.Keyword, "replaceData");
                    else if (value == TokenKeywords.getData) return new Token(TokenTypes.Keyword, "getData");
                    else if (value == TokenKeywords.saveDataChanges) return new Token(TokenTypes.Keyword, "saveDataChanges");
                    else if (value == TokenKeywords.invokeData) return new Token(TokenTypes.Keyword, "invokeData");
                    else if (value == TokenKeywords.func) { 
                        return new Token(TokenTypes.Keyword, "func"); 
                    }
                    else return new Token (TokenTypes.Identifier, value);
                }
                
                throw new Exception("Unexpected character: " + current);
            }
            return new Token(TokenTypes.EndOfFile, "");
        }
    }
}

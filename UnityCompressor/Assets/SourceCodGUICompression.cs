using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

public class SourceCodGUICompression : MonoBehaviour {



    public string pathFileLoad = "";
    public string pathFileSave = "";
    string originText = "";
    string lastOriginText = "";
    string convertedText = "";
    string lengthText = "";
    string protositionOfWordToRemoveText="";
    string saveZone = "";
    string lastCompressedName = "";
    int minSize=5;
    int minRepeat=3;
    bool withLineReturn=true;



    public List<string> wordUsedOrIgnored = new List<string>();
    public List<string> wordProposedToBeConverted = new List<string>();
    Dictionary<string, string> wordToRemove = new Dictionary<string, string>();
    public void Start() {

        foreach (string w in reservedWord_CSharp) 
        {
            if( ! reservedWords.ContainsKey(w))
            reservedWords.Add(w,w);
        }

        minRepeat = PlayerPrefs.GetInt("Repeat");
        minSize = PlayerPrefs.GetInt("Size");
        saveZone = PlayerPrefs.GetString("Save");
        withLineReturn = PlayerPrefs.GetInt("WithReturn")==1?true:false;
        if(string.IsNullOrEmpty(pathFileLoad))
        pathFileLoad = PlayerPrefs.GetString("PathLoad");
        if(string.IsNullOrEmpty(pathFileSave))
        pathFileSave = PlayerPrefs.GetString("PathSave");
    
    }
    void OnDestroy() 
    {
        PlayerPrefs.SetInt("Repeat", minRepeat);
        PlayerPrefs.SetInt("Size", minSize);
        PlayerPrefs.SetString("Save", saveZone);
        PlayerPrefs.SetInt("WithReturn", withLineReturn?1:0);
        pathFileLoad = PlayerPrefs.GetString("PathLoad");
        pathFileSave = PlayerPrefs.GetString("PathSave");
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Repeat", minRepeat);
        PlayerPrefs.SetInt("Size", minSize);
        PlayerPrefs.SetString("Save", saveZone);
        pathFileLoad = PlayerPrefs.GetString("PathLoad");
        pathFileSave = PlayerPrefs.GetString("PathSave");
    }
    public void OnGUI() 
    {


        if (GUI.Button(new Rect(0, 0, 55, 20), "Import"))
        {
            originText = "";
            convertedText = "";
            wordUsedOrIgnored.Clear();
            wordProposedToBeConverted.Clear();
            TryToImport(pathFileLoad, ref originText);
            convertedText = ConvertText(originText);
            lastOriginText = originText;
            TryToExport(pathFileSave, ref convertedText);
        }
        pathFileLoad = GUI.TextArea(new Rect(60, 0, Screen.width - 60, 20), pathFileLoad);

        GUI.Button(new Rect(0, 25, 55, 20), "Save At");
      
        pathFileSave = GUI.TextArea(new Rect(60, 25, Screen.width - 60, 20), pathFileSave);
        

        originText = GUI.TextArea(new Rect(0, 45, Screen.width, 70), originText);

        convertedText = GUI.TextArea(new Rect(0, 150, Screen.width, 100), convertedText);
        float pourcentConverted = originText.Length == 0 ? 0 : -(int)((1f - ((float)convertedText.Length / (float)originText.Length))*100f);
        GUI.TextArea(new Rect(20, 125, Screen.width - 50, 20), originText.Length + " -> " + convertedText.Length + " ( " + pourcentConverted + " %)");
        if (wordUsedOrIgnored.Count>0)
            GUI.TextArea(new Rect(20, 260, Screen.width - 50, 20), GetConvertedKeyStringList());

        GUI.Label(new Rect(20, 280, 60, 20), "Min Size");
        int.TryParse(GUI.TextArea(new Rect(80, 280, 20, 20), "" + minSize), out minSize);
        GUI.Label(new Rect(120, 280, 60, 20), "Min repeat");
        int.TryParse(GUI.TextArea(new Rect(180, 280, 20, 20), "" + minRepeat), out minRepeat);
        GUI.Label(new Rect(225, 280, 100, 20), "With Line Return");
        withLineReturn = GUI.Toggle(new Rect(330, 280, 20, 20), withLineReturn, "");
        GUI.Label(new Rect(355, 280, 100, 20), "Last name");
        GUI.Label(new Rect(430, 280, 100, 20), lastCompressedName);

        GUI.Label(new Rect(3, 305, 80, 20), "Save Zone");
        saveZone = GUI.TextArea(new Rect(80, 305, Screen.width - 50 - 80, 20), saveZone);
        protositionOfWordToRemoveText = GUI.TextArea(new Rect(5, 340, Screen.width - 5, 1000), protositionOfWordToRemoveText);

    }

    private string GetConvertedKeyStringList()
    {
        string list = "";
        foreach (string w in wordUsedOrIgnored)
            list += " " + w;
        return list;
    }

    private void TryToExport(string pathFile, ref string originText)
    {
        if (string.IsNullOrEmpty(pathFile)) return;
        StreamWriter streamWriter = new StreamWriter(pathFile);
        streamWriter.Write(originText);
        streamWriter.Close();
    }

    private void TryToImport(string pathFile, ref string originText)
    {
        if (string.IsNullOrEmpty(pathFile)) return;
        StringBuilder sb = new StringBuilder();
        StreamReader streamReader = new StreamReader(pathFile);
        string line="";
        do
        {   line = streamReader.ReadLine();
        if (line != null)
        {
            if (line.Contains("#region") || line.Contains("#endregion") || line.Contains("///")) { line = "\n\r"; continue; }
            if (line.Contains("//R:")) 
            {
                AddReplaceTaggedName(line);
                continue;
            }
       
                sb.Append(line+"\n\r");
                //Debug.Log(line);
            }
        } while (line!=null);
        originText = sb.ToString() ;
            streamReader.Close();
    }
           

    public string RemoveReturnToLine(string value) 
    {
        value = value.Replace('\r',' ');
        value = value.Replace('\n',' ');
        return value;
    }


    public string ConvertText(string toCompress)
    {
        wordUsedOrIgnored.Clear();
        protositionOfWordToRemoveText = "//R: ";
        
        string origin = toCompress;
        string result = origin;

        result = StripComments(result);

        wordProposedToBeConverted = PropositionOfKeywordToCompress(result, minRepeat, minSize);

        foreach (string w in wordProposedToBeConverted)
            protositionOfWordToRemoveText += "  "+w;
        if (! withLineReturn)
        result = RemoveReturnToLine(result);
        //replace private by space
        result = result.Replace("private", " ");
        result = ConvertNameToId(result, wordUsedOrIgnored);

        for (int i = 0; i < 5; i++)
        {
            result = result.Replace("  ", " ");
        }

        return result;
    }

    private string ConvertNameToId(string result, List<string> convertedKey)
    {
        string [] keysToConvert = new string[wordToRemove.Keys.Count];
        wordToRemove.Keys.CopyTo(keysToConvert, 0);
        string[] replaceBy = GetReplacementTable(keysToConvert.Length);
        for (int i = 0; i < keysToConvert.Length; i++)
        {
            convertedKey.Add(keysToConvert[i]);
            result = result.Replace(keysToConvert[i], replaceBy[i]);
            
            lastCompressedName = replaceBy[i];

        }
        return result;
    }

     List<string> PropositionOfKeywordToCompress(string input, int repeatMoreThen =4, int wordMinLength=3)
    {
        Dictionary<string, int> propositionRegister = new Dictionary<string, int>();
        MatchCollection matches = Regex.Matches(input, @"\b[\w]*");
        List<string> result = new List<string>();
        var words = from m in matches.Cast<Match>()
                    where !string.IsNullOrEmpty(m.Value)
                    select m.ToString();
        string [] wordFoundInCode = (string []) words.ToArray();

         //Record word in code with value 
        foreach (string w in wordFoundInCode)
        { 
            if (!propositionRegister.ContainsKey(w))
                propositionRegister.Add(w, 1);
            else
                propositionRegister[w] = propositionRegister[w]+1;
        }
        foreach (string w in propositionRegister.Keys)
        {
            if (w.Length >= wordMinLength && propositionRegister[w] > repeatMoreThen)
            {
                if (!reservedWords.ContainsKey(w) && ! wordUsedOrIgnored.Contains(w)& ! wordToRemove.ContainsKey(w))
                {
                    result.Add(w);
                    Debug.Log(w);
                }
            }
        }
        return result;
    }


    
    private void AddReplaceTaggedName(string line )
     {

         string start = "//R:";

        char spliteChar =' ';

        int from = line.IndexOf(start) + start.Length;
        int to = line.Length;


        if (from < 0 || to < 0) return;
        string replaceLineInfo = line.Substring(from, to - from);
            if (replaceLineInfo == null || replaceLineInfo.Length <= 0)
                return ;
            string[] wordToReplace = replaceLineInfo.Split(spliteChar);
            for (int i = 0; i < wordToReplace.Length; i++)
            {
                if (!string.IsNullOrEmpty(wordToReplace[i]) && !wordToRemove.ContainsKey(wordToReplace[i]))
                {

                    wordToRemove.Add(wordToReplace[i], wordToReplace[i]);
                }
            }
        
    }

    private string[] GetReplacementTable(int size)
    {
        string[] word = new string[size];
        for (int i = 0; i < word.Length; i++)
        {
            word[i] = "_" + i;
            
        }
        return word;
    }

    static string StripComments(string code)
    {
//        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        var blockComments = @"/\*(.*?)\*/";
        var lineComments = @"//(.*?)\r?\n";
        code = Regex.Replace(code, lineComments, "  ");
        return Regex.Replace(code, blockComments, "  ");
    }

    Dictionary<string, string> reservedWords = new Dictionary<string, string>();
    private string[] reservedWord_CSharp = new string[]
    {
    
     "abstract",
"as",
"base",
"bool",
"break",
"byte",

"case",
"catch",
"char",
"checked",
"class",
"const",
"continue",
"decimal",
"default",
"delegate",
"do",
"double",
"else",
"enum",
"event",
"explicit",
"extern",

"false",
"finally",
"fixed",

"float",
"for",
"foreach",
"goto",
"if",

"implicit",
"in",
"int",

"interface",
"internal",
"is",
"lock",
"long",
"namespace",
"new",
"null",
"object",
"operator",
"out",
"out",
"override",
"params",
"private",
"protected",
"public",
"readonly",
"ref",
"return",

"sbyte",
"sealed",
"short",
"sizeof",
"stackalloc",
"static",
"string",
"struct",
"switch",
"this",
"throw",
"true",

"try",
"typeof",
"uint",
"ulong",
"unchecked",
"unsafe",
"ushort","using","virtual","void","volatile","while","add","alias","ascending","async","await","descending","dynamic","from","get","global","group","into","join","let","orderby","partial","partial","remove","select","set","value","var","where (generic type constraint)","where (query clause)","yield "};

}

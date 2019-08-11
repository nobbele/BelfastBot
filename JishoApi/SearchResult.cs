using System.Collections.Generic;

namespace JishoApi
{
    public struct SearchResult
    {
        public string Word;
        public KeyValuePair<string, string>[] Japanese;
        public EnglishDefinition[] English;
    }
}
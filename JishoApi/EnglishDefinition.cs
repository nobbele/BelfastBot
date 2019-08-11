namespace JishoApi
{
    public struct EnglishDefinition
    {
        public string[] English;
        public string[] Info;

        public EnglishDefinition(string[] english, string[] info)
        {
            English = english;
            Info = info;
        }
    }
}
using System;

[Serializable]
public class Language
{
    public Languages currentLanguage;

    public enum Languages
    {
        NL,
        EN
    }

    public Language(string language)
    {
        if (language == "Dutch")
        {
            currentLanguage = Languages.NL;
        }
        else
        {
            currentLanguage = Languages.EN;
        }
    }

    public string ReturnLanguage()
    {
        return currentLanguage.ToString();
    }
}
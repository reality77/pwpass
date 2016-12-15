using System.Collections.Generic;
using PwPass.WordDictionary;

namespace PwPass.PasswordManagement
{
    public class PasswordGenerator
    {
        static WordDictionaryAccessorBase s_dicWords;

        static PasswordGenerator()
        {
            s_dicWords = new WordFileDictionaryAccessor()
            {
                DictionaryPath = "dic.txt",
            };

            s_dicWords.InitializeWords();
        }

        public PasswordGenerator()
        {
        }
        
        public string Generate(int numberOfPickedwords = 5, int minDigits = 0, int minSpecialChars = 0)
        {
            var usedWords = new List<string>();
            string password = null; 

            for(int i = 0; i < numberOfPickedwords; i++)
            {
                string word = null;
                
                do
                {
                    word = s_dicWords.GetRandomWord();
                }
                while(usedWords.Contains(word));

                if(password == null)
                    password = string.Empty;
                else
                    password += ' ';
                      
                password += word;
            }

            // TODO minDigits

            // TODO minSpecialChars

            return password;
        }
    }
}
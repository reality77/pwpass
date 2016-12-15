
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PwPass.WordDictionary
{
    public class WordFileDictionaryAccessor : WordDictionaryAccessorBase
    {
        public string DictionaryPath { get; set; } 
        
        List<string> _words;
        int _wordsCount = 0;
        Random _rand;

        public WordFileDictionaryAccessor()
        {
            _rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            for(int i = 0; i< _rand.Next(0, DateTime.Now.Minute * 20 + 5); i++)
                _rand.Next();
        }

        public override void InitializeWords()
        {
            _words = new List<string>();

            using(var file = new FileStream(this.DictionaryPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(file);
                while(!reader.EndOfStream)
                    _words.Add(reader.ReadLine());
            }

            _wordsCount = _words.Count();
        }

        public override string GetRandomWord()
        {
            return _words[_rand.Next(0, _wordsCount - 1)]; 
        }
    }
}
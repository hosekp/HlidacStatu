﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FullTextSearch
{
    public class Index<T>
    {
        public SortedList<string, Token<T>> SortedTokens { get; private set; } = new SortedList<string, Token<T>>();
        public List<Sentence<T>> Sentences { get; private set; } = new List<Sentence<T>>();

        private readonly ITokenizer _tokenizer;

        private readonly Options _options;

        public Index(IEnumerable<T> inputObjects)
        {
            _tokenizer = Tokenizer.DefaultTokenizer();
            _options = Options.DefaultOptions();
            BuildIndex(inputObjects);
        }

        public Index(IEnumerable<T> inputObjects, ITokenizer tokenizer, Options options)
        {
            _tokenizer = tokenizer;
            _options = options;
            BuildIndex(inputObjects);
        }

        private void BuildIndex(IEnumerable<T> inputObjects)
        {
            foreach (T inputObject in inputObjects)
            {
                var sentence = new Sentence<T>(inputObject, _tokenizer);
                Sentences.Add(sentence);

                AddTokens(sentence.Tokens);
            }
        }

        private void AddTokens(List<Token<T>> tokens)
        {
            for(int i = 0; i < tokens.Count(); i++)
            {
                if(SortedTokens.TryGetValue(tokens[i].Word, out Token<T> olderToken))
                {
                    olderToken.MergeWith(tokens[i]);
                }
                else
                {
                    SortedTokens.Add(tokens[i].Word, tokens[i]);
                }
            }
        }

        public IEnumerable<Result<T>> Search(string query, int count)
        {
            var tokenizedQuery = _tokenizer.Tokenize(query);

            List<ScoredSentence<T>> results = new List<ScoredSentence<T>>();
            foreach(string queryToken in tokenizedQuery)
            {
                
                // tokeny, které odpovídají query
                var filteredTokens = SortedTokens
                    .Where(t => t.Key.StartsWith(queryToken))
                    .Select(x => x.Value);


                foreach(var token in filteredTokens)
                {
                    var scoredSentences = ScoreToken(token, queryToken);
                    results.AddRange(scoredSentences);
                }
            }

            var summedResults = results
                .GroupBy(r => r.Sentence,
                    (sentence, result) => new ScoredSentence<T>(sentence, result.Sum(x => x.Score)))
                .ToList();

            // přidat score za nejdelší řetězec
            foreach(ScoredSentence<T> result in summedResults)
            {
                result.Score += ScoreSentence(result.Sentence, tokenizedQuery); 
            }

            return summedResults
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => new Result<T>()
                {
                    Original = x.Sentence.Original,
                    Score = x.Score
                });
                
        }

        // Neověřuju na začátku, jestli jsou stejné
        // předpokládám, že sem už stejné texty lezou - asi chybně
        private List<ScoredSentence<T>> ScoreToken(Token<T> token, string queryToken)
        {
            double basicScore = queryToken.Length;
            
            // bonus for whole word
            if (_options.WholeWordBonusMultiplier.HasValue 
                && queryToken.Length == token.Word.Length)
            {
                basicScore *= _options.WholeWordBonusMultiplier.Value;
            }

            return token.Sentences.Select(s => new ScoredSentence<T>(s, basicScore)).ToList();
        }

        private Double ScoreSentence(Sentence<T> sentence, string[] tokenizedQuery)
        {
            if (tokenizedQuery.Length == 0)
                return 0;

            double score = 0;

            int tokenPosition = 0;
            // bonus for first three words
            if (_options.FirstWordsBonus != null)
            {
                for (int wordPosition = 0; wordPosition < _options.FirstWordsBonus.BonusWordsCount; wordPosition++)
                {
                    if (wordPosition >= sentence.Tokens.Count
                        || tokenPosition >= tokenizedQuery.Length)
                        break;
                    
                    string queryToken = tokenizedQuery[tokenPosition];
                    if (sentence.Tokens[wordPosition].Word.StartsWith(queryToken))
                    {
                        score += queryToken.Length 
                            * (_options.FirstWordsBonus.MaxBonusMultiplier - 
                                (_options.FirstWordsBonus.BonusMultiplierDegradation * wordPosition));

                        tokenPosition++;
                    }
                }
            }

            //bonus for longest word chain
            tokenPosition = 0;
            double chainScore = 0;
            // bonus for first three words
            if (_options.ChainBonusMultiplier.HasValue)
            {
                for (int wordPosition = 0; wordPosition < sentence.Tokens.Count; wordPosition++)
                {
                    if (tokenPosition >= tokenizedQuery.Length)
                        break;
                    
                    string queryToken = tokenizedQuery[tokenPosition];
                    if (sentence.Tokens[wordPosition].Word.StartsWith(queryToken))
                    {
                        chainScore += queryToken.Length;
                        tokenPosition++;
                    }
                    else if (chainScore > 0) // after previous match there is no other match
                        break;
                }

                if (chainScore > 1)
                    score += chainScore * _options.ChainBonusMultiplier.Value; //todo: put it to options
            }

            

            // Query == sentence
            // toto téměř nikdy nenastane! sentence je potřeba rozdělit do chlívků
            // podle parametrů, ze kterých se pomocí reflexe vytvořili věty
            // pak je potřeba vyhledávat pouze v těchto chlívcích!
            if (sentence.Text == string.Join(" ", tokenizedQuery))
            {
                return score + _options.ExactMatchBonus ?? 0;
            }

            // sentence starts with query without its last word
            // toto téměř nikdy nenastane! sentence je potřeba rozdělit do chlívků
            // podle parametrů, ze kterých se pomocí reflexe vytvořili věty
            // pak je potřeba vyhledávat pouze v těchto chlívcích!
            if (tokenizedQuery.Length > 2) // 3+ words
            {
                string shorterQuery = string.Join(" ", tokenizedQuery.Take(tokenizedQuery.Length - 1));
                if (sentence.Text.StartsWith(shorterQuery) )
                {
                    return score + _options.AlmostExactMatchBonus ?? 0;
                }

            }

            // Má smysl scorovat nejdelší shodný substring? Zatím si myslím, že asi ne,
            // protože by to mohlo zamíchat pořadím. Navíc takový výpočet není levný
            // a pro velký počet dokumentů by to mohlo znamenat pomalé hledání.

            return score;
        }

    }
}

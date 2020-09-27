﻿using Devmasters;

using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public class SubjectNameCache
    {
        public SubjectNameCache(string name, string ico)
        {
            Name = name;
            Ico = ico;
            Tokens = Tokenize($"{name} {ico}");
        }

        public static Devmasters.Cache.File.FileCache<Dictionary<string,SubjectNameCache>> CachedCompanies = 
            new Devmasters.Cache.File.FileCache<Dictionary<string, SubjectNameCache>>(
                Lib.Init.WebAppDataPath, TimeSpan.Zero, "KIndexCompanies",
                    (o) =>
                    {
                        return ListCompanies();
                    });

        private static Dictionary<string,SubjectNameCache> ListCompanies()
        {
            Dictionary<string, SubjectNameCache> companies = new Dictionary<string, SubjectNameCache>();
            foreach(var kindexRecord in KIndex.YieldExistingKindexes())
            {
                companies.Add(kindexRecord.Ico, new SubjectNameCache(kindexRecord.Jmeno, kindexRecord.Ico));
            }

            return companies;
        }

        public static Dictionary<string, SubjectNameCache> GetCompanies()
        {
            return CachedCompanies.Get();
        }

        public static IEnumerable<SubjectNameCache> FullTextSearch(string search, int take = 50)
        {
            if (string.IsNullOrEmpty(search))
                return new List<SubjectNameCache>();
            
            IEnumerable<SubjectNameCache> fullSearchNames = GetCompanies().Values
                .Where(c => c.Name.ToLower().StartsWith(search.ToLower()))
                .Take(take);

            IEnumerable<SubjectNameCache> totalResult = fullSearchNames;
            if (totalResult.Count() >= take)
                return totalResult;

            var fullSearchIcos = GetCompanies()
                .Where(c => c.Key.StartsWith(search))
                .Select(c => c.Value)
                .Take(take);
            totalResult = totalResult.Union(fullSearchIcos).Take(take);
            if (totalResult.Count() >= take)
                return totalResult;

            var tokenizedSearchInput = Tokenize(search);

            var tokenSearchCount = GetCompanies().Values
                .Select(c => new
                {
                    hits = tokenizedSearchInput.Sum(i => c.Tokens.Any(tkn => tkn.StartsWith(i)) ? i.Length : 0),
                    subject = c
                })
                .Where(x => x.hits > 0)
                .OrderByDescending(x => x.hits)
                .Select(x => x.subject)
                .Take(take);
            totalResult = totalResult.Union(tokenSearchCount).Take(take);

            return totalResult;
        }


        public string Name { get; set; }
        public string Ico { get; set; }
        public string[] Tokens { get; set; }
        
        private static string[] Tokenize(string input)
        {
            return input.ToLower().KeepLettersNumbersAndSpace().RemoveAccents().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        
    }
}

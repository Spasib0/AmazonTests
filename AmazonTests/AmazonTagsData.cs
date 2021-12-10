using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmazonTests
{
    class AmazonTagsData
    {
        private List<S3Object> _bucketItems = new List<S3Object>();
        private string[] _statisticsProducts;
        private string[] _valideLanguages;
        private readonly string[] VALIDE_TAG_KEYS = new string[] { PRODUCT, OS, LANGUAGE, LANGUAGES, VERSION, MASTER, WHATS_NEW, TITLE };
        private readonly string[] VALIDE_OS = new string[] { "Windows", "Windows x64", "Windows x32" };
        private readonly string[] NAUROBO_FIRMWARES = new string[] { "robots" }; //Прошивки для NAUROBO (на них есть ссылки в README)
        private IEnumerable<string> ValideProducts => _statisticsProducts.Concat(NAUROBO_FIRMWARES);

        private const string PRODUCT = "product";
        private const string OS = "os";
        private const string LANGUAGE = "language";
        private const string LANGUAGES = "languages";
        private const string VERSION = "version";
        private const string MASTER = "master";
        private const string TITLE = "title";
        private const string WHATS_NEW = "whats-new";

        public bool Test()
        {
            Setup();

            return BucketHasItems() & AllItemsHasTags()
                                    & AllTagKeysValide()
                                    & AllProductTagsValide()
                                    & AllOsTagsValide()
                                    & AllLanguageAndLanguagesTagsValide()
                                    & AllVersionTagsIsValide()
                                    & AllMasterTagsIsValide();

        }

        private bool BucketHasItems()
        {
            Print($"***BucketHasItems*** \n\t{string.Join("\n\t", _bucketItems.Select(item => item.Key))}");

            return _bucketItems.Count > 0;
        }

        private bool AllItemsHasTags()
        {
            Print($"\n***AllItemsHasTags***");

            return _bucketItems.TrueForAll(item => item.Tags.Count > 0);
        }

        private bool AllTagKeysValide()
        {
            Print($"\n***AllTagKeysValide***");
            bool passed = true;

            foreach(var item in _bucketItems)
            {
                var info = $"\t{item.Key} has not valide tags:\n";
                var invalidTags = "";

                foreach(var tag in item.Tags)
                {
                    if (!VALIDE_TAG_KEYS.Contains(tag.Key))
                    {
                        invalidTags += $"\t\t{tag.Key}\n";
                    }
                }

                if (!string.IsNullOrEmpty(invalidTags))
                {
                    passed = false;
                    Print(info + invalidTags);
                }
            }

            return passed;
        }

        private bool AllProductTagsValide()
        {
            Print($"\n***AllProductTagsValide***");

            return CheckTags("product", (string product) => ValideProducts.Contains(product));
        }

        private bool AllOsTagsValide()
        {
            Print($"\n***AllOsTagsValide***");

            return CheckTags("os", (string os) => VALIDE_OS.Contains(os));
        }

        private bool AllLanguageAndLanguagesTagsValide()
        {
            Print($"\n***AllLanguageAndLanguagesTagsValide***");

            return CheckTags("language", (string language) => _valideLanguages.Contains(language))
                 & CheckTags("languages", (string languages) => languages.Split(" ").All(lang => _valideLanguages.Contains(lang)), true);
        }

        private bool AllVersionTagsIsValide()
        {
            Print($"\n***AllVersionTagsIsValide***");

            return CheckTags("version", (string version) => int.TryParse(version, out int _));
        }

        private bool AllMasterTagsIsValide()
        {
            Print($"\n***AllMasterTagsIsValide***");

            return CheckTags("master", (string master) => bool.TryParse(master, out bool _), true);
        }

        private void Setup()
        {
            _bucketItems = S3UpdateBucketReader.ListBucket();
            _statisticsProducts = Enum.GetNames(typeof(StatisticsProduct));
            _valideLanguages = Enum.GetNames(typeof(Language));
        }

        private bool CheckTags(string tagName, Func<string, bool> condition, bool ignoreMissingTag = false)
        {
            return _bucketItems.Where(item => item.Tags.TryGetValue(tagName, out string tagValue)
                                         ? Print($"\tNot correct \"{tagName}\" value - {item.Key}", condition(tagValue))
                                         : Print($"\tNot exist \"{tagName}\" tag - {item.Key}", ignoreMissingTag)).Count() == _bucketItems.Count();
        }

        private bool Print(string str, bool condition = false)
        {
            if (!condition)
                Console.WriteLine(str);

            return condition;
        }

        private enum StatisticsProduct
        {
            school,
            cards_app,
            cards_app_school,
            logopedia,
            school_japan,
            robot_key,
            vna_labs,
            expedition_magnet,
            school_demo
        }

        private enum Language
        {
            Russian,
            English,
            French,
            Japanese,
            German,
            Korean,
            Hirogana,
            Porto
        }
    }
}

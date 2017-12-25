using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CS600FinalProject
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Base url from which all other pages will be downloaded
                const string baseUrl = "https://www.stevens.edu";

                TrieProcessor _processor = new TrieProcessor();

                //Initial page from which other pages will be loaded
                string _startUrl = "/news/stevens-names-dr-jean-zu-dean-schaefer-school-engineering-science";

                //to keep track of url with it's index
                var _dicWebsiteMap = new Dictionary<int, string>();


                Console.WriteLine("Started reading webpages");
                int _index = 1;
                string _urlToRead = $"{baseUrl}{_startUrl}";
                _dicWebsiteMap.Add(_index, _urlToRead);
                Console.WriteLine($"{_index}, {_urlToRead}");

                //read first page
                var s = _processor.ReadWebPage(_index, _urlToRead);

                var doc = new HtmlDocument();
                doc.LoadHtml(s.rawHtml);
                //Get all anchor tags and it's href property for url's of next pages.
                var _urlCollection = (from h in
                            ((from a in doc.DocumentNode.SelectNodes("//a")
                              select a.Attributes["href"].Value))
                                      where h.StartsWith("/") && !h.Contains("#") && h.Length > 1
                                      select h).Distinct().Take(5).ToList();

                var _firstPageResult = _processor.StripHtmlTags(s.docIndex, s.rawHtml);

                var _lstFinalHtmls = new Dictionary<int, string>() { { _firstPageResult.docIndex, _firstPageResult.processedHtml } };

                //Start reading pages
                foreach (var url in _urlCollection)
                {
                    _urlToRead = $"{baseUrl}{url}";
                    _index++;
                    _dicWebsiteMap.Add(_index, _urlToRead);
                    Console.WriteLine($"{_index}, {_urlToRead}");
                    var readResult = _processor.ReadWebPage(_index, _urlToRead);
                    var stripResult = _processor.StripHtmlTags(readResult.docIndex, readResult.rawHtml);
                    _lstFinalHtmls.Add(stripResult.docIndex, stripResult.processedHtml);
                }

                Console.WriteLine($"Finished reading webpages. {Environment.NewLine}");

                Console.WriteLine("Creating trie from the contents of the webpages");
                _processor.CreateTrie(_lstFinalHtmls);
                Console.WriteLine($"Creation of trie completed {Environment.NewLine}");

                string _strUserInput = string.Empty;
                do
                {
                    Console.WriteLine($"Enter input to search or just press 'Enter' key to exit");
                    _strUserInput = Console.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(_strUserInput))
                    {
                        bool _bNotFound = false;
                        var _lstIntersection = new List<int>();
                        foreach (var item in _strUserInput.Split(' '))
                        {
                            var _lstDocRef = _processor.SearchWord(item.Trim().ToLower());
                            if (_lstDocRef != null && _lstDocRef.Count > 0)
                            {
                                if (_lstIntersection.Count.Equals(0))
                                    _lstIntersection = _lstDocRef.Keys.ToList();
                                else
                                    _lstIntersection = _lstIntersection.Intersect(_lstDocRef.Keys.ToList()).ToList();

                                Console.WriteLine($"Word '{item}' is found in the pages as below based on frequency in descending order");
                                foreach (var docRef in _lstDocRef)
                                    Console.WriteLine($"Url: {_dicWebsiteMap[docRef.Key]} with frequency: {docRef.Value}");
                                Console.WriteLine();
                            }
                            else
                            {
                                _bNotFound = true;
                                break;
                            }
                        }

                        if (!_bNotFound && _lstIntersection.Count > 0)
                        {
                            Console.WriteLine($"The complete search input '{_strUserInput}' is found in the pages as below");
                            foreach (var item in _lstIntersection)
                                Console.WriteLine($"Url: {_dicWebsiteMap[item]}");
                        }
                        else
                            _bNotFound = true;

                        if (_bNotFound)
                            Console.WriteLine($"'{_strUserInput}' not found");

                        Console.WriteLine();
                    }
                }
                while (_strUserInput != string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Good bye!!!");
        }
    }

    public class TrieProcessor
    {
        Regex _flattenNames = new Regex(@"[^a-zA-Z ]+", RegexOptions.Compiled);

        ITrie _trie = null;

        public int WordCountinTrie { get { return _trie != null ? _trie.TotalWords : 0; } }

        /// <summary>
        /// Function to read the webpage content from the internet
        /// </summary>
        /// <param name="docIndex"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public (int docIndex, string rawHtml) ReadWebPage(int docIndex, string url)
        {
            string _strRtnVal = string.Empty;
            try
            {
                var _request = HttpWebRequest.Create(url) as HttpWebRequest;

                using (var _response = _request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader _reader = new StreamReader(_response.GetResponseStream()))
                    {
                        _strRtnVal = _reader.ReadToEnd();
                        int _start = _strRtnVal.IndexOf("<body ");
                        int _end = _strRtnVal.IndexOf("</body>");
                        _strRtnVal = _strRtnVal.Substring(_start, (_end - _start + 7));
                        Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
                        _strRtnVal = rRemScript.Replace(_strRtnVal, "");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"An exception occurred for url {url}. Exception details {ex}");
            }

            return (docIndex, _strRtnVal.Trim());
        }

        /// <summary>
        /// Function to remove all the possible html tags from the body content
        /// </summary>
        /// <param name="docIndex"></param>
        /// <param name="strInHtml"></param>
        /// <returns></returns>
        public (int docIndex, string processedHtml) StripHtmlTags(int docIndex, string strInHtml)
        {
            string _strRtnVal = string.Empty;
            const string HTML_TAG_PATTERN = "<.*?>";
            _strRtnVal = Regex.Replace(strInHtml, HTML_TAG_PATTERN, string.Empty);
            _strRtnVal = Regex.Replace(_strRtnVal, @"\t|\n|\r|&nbsp;", "");
            _strRtnVal = _flattenNames.Replace(_strRtnVal, string.Empty);
            return (docIndex, _strRtnVal.Trim());
        }

        /// <summary>
        /// Creates the trie from flattend body contents. Stores the frequency of the word in the particular document by it's id
        /// </summary>
        /// <param name="webResponses"></param>
        public void CreateTrie(Dictionary<int, string> webResponses)
        {
            try
            {
                if (_trie == null)
                    _trie = TrieFactory.CreateTrie();

                foreach (var _webString in webResponses)
                {
                    List<string> _lstSorted = _webString.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    _lstSorted.Sort();

                    var numberOfTestcasesWithDuplicates = (from word in _lstSorted
                                                           where !Constants._lstExclusion.Contains(word.ToLower())
                                                           select word.ToLower())
                                                           .GroupBy(x => x)
                                                           .ToDictionary(x => x.First(), x => x.Count());

                    foreach (var item in numberOfTestcasesWithDuplicates)
                    {
                        var _trieNode = _trie.ContainsWord(item.Key.Trim());
                        if (_trieNode != null)
                        {
                            _trieNode.DocReferences[_webString.Key] = item.Value;
                        }
                        else
                        {
                            _trie.AddWord(item.Key.Trim(), new Dictionary<int, int>() { { _webString.Key, item.Value } });
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"An exception occurred while creating trie {ex}");
            }
        }

        /// <summary>
        /// Function to do searching in the trie and to show the result to user based on descending frequency of occurrence
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Dictionary<int, int> SearchWord(string word)
        {
            Dictionary<int, int> _lstRtnVal = null;
            var _trieNode = _trie.ContainsWord(word.Trim());
            if (_trieNode != null)
            {
                _lstRtnVal = _trieNode.DocReferences
                                .OrderByDescending(x => x.Value)
                               .ToDictionary(x => x.Key, x => x.Value);
            }
            return _lstRtnVal;
        }
    }
}

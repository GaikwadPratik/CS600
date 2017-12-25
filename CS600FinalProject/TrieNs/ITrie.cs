using System.Collections.Generic;

namespace CS600FinalProject
{
    #region interface ITrie
    // The interface to be implemented by Trie class
    public interface ITrie
    {
        /// <summary>
        /// Returns the total word Count in the trie structure
        /// </summary>
        /// <returns>Count of the words in the trie</returns>
        int TotalWords { get; }

        /// <summary>
        /// To a new word in Trie collection
        /// </summary>
        /// <param name="word" type="string"></param>
        void AddWord(string word, Dictionary<int, int> dicDocuFrequcy);

        /// <summary>
        /// To remove a particular word from Trie
        /// </summary>
        /// <param name="word" type="string"></param>
        void RemoveWord(string word);

        /// <summary>
        /// To remove all the words by a particular prefix
        /// </summary>
        /// <param name="prefix" type="string"></param>
        void RemovePrefix(string prefix);

        /// <summary>
        /// Returns all the words present in the trie
        /// </summary>
        /// <returns>All list of words in the trie</returns>
        List<string> GetAllWords();

        /// <summary>
        /// Returns all the words for a praticular prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>List of words starting with a particular prefix</returns>
        List<string> GetWordsByPrefix(string prefix);

        /// <summary>
        /// Returns true if the word is present in the Trie.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Boolean value indicating whether a word is present in the trie or not</returns>
        TrieNode ContainsWord(string word);

        /// <summary>
        /// Remove all the words from the Trie.
        /// </summary>
        void Clear();
    }
    #endregion interface ITrie
}
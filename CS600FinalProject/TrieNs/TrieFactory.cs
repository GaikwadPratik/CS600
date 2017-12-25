using System.Collections.Generic;

namespace CS600FinalProject
{
    #region TrieFactory class
    /// <summary>
    /// Class to create a instance Trie datastructure or node for the trie datastructure
    /// </summary>
    class TrieFactory
    {
        #region Static Methods
        /// <summary>
        /// To create an instance of Trie object
        /// </summary>
        /// <returns>ITrie instance of the trie separately for each invocation</returns>
        public static ITrie CreateTrie()
        {
            return new Trie(CreateTrieNode(" "));
        }

        /// <summary>
        /// To create the node as per Trie structure configuration
        /// </summary>
        /// <param name="character"></param>
        /// <returns>Instance of the node of the Trie structure</returns>
        internal static TrieNode CreateTrieNode(string character)
        {
            return new TrieNode(character, new Dictionary<string, TrieNode>(), 0);
        }
        #endregion Static M
    }
    #endregion Trie class
}
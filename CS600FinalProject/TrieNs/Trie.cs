using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS600FinalProject
{
    #region class Trie
    /// <summary>
    /// Class representation of the Trie datastructure
    /// </summary>
    class Trie : ITrie
    {
        #region  members
        private TrieNode _rootTrieNode = null;

        private int _wordIndex = 0;
        #endregion

        #region  constructor
        internal Trie(TrieNode rootTrieNode)
        {
            _rootTrieNode = rootTrieNode;
        }
        #endregion

        #region  Properties
        /// <summary>
        /// Total number words present in the trie structure
        /// </summary>
        /// <returns></returns>
        public int TotalWords
        {
            get
            {
                int _nRtnVal = 0;
                GetCount(_rootTrieNode, _nRtnVal);
                return _nRtnVal;
            }
        }
        #endregion Properties

        #region Public methods
        /// <summary>
        /// Add a word to structure
        /// </summary>
        /// <param name="word">String representation of the word to be added in the trie</param>
        public void AddWord(string word, Dictionary<int, int> dicDocuFrequcy) => AddWord(_rootTrieNode, word, dicDocuFrequcy);

        /// <summary>
        /// Remove all the nodes from the trie structure
        /// </summary>
        public void Clear() => _rootTrieNode.Clear();

        /// <summary>
        /// To check if word is present in the datastructre
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Trie node which represents word</returns>
        public TrieNode ContainsWord(string word)
        {           
            return GetNode(word);            
        }

        /// <summary>
        /// To get all the words formed by the current trie structure
        /// </summary>
        /// <returns>List of the strings containing the words</returns>
        public List<string> GetAllWords()
        {
            List<string> _lstWords = new List<string>();
            StringBuilder _sb = new StringBuilder();
            GetWords(_rootTrieNode, _lstWords, _sb);
            if (_sb != null)
                _sb = null;
            return _lstWords;
        }

        /// <summary>
        /// To retrieve words for a particular prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>List of words formed by a particular prefix</returns>
        public List<string> GetWordsByPrefix(string prefix)
        {
            List<string> _lstRtnVal = new List<string>();
            TrieNode _node = GetNode(prefix);
            if (_node != null)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.Append(prefix);
                GetWords(_node, _lstRtnVal, _sb);
                if (_sb != null)
                    _sb = null;
            }
            return _lstRtnVal;
        }

        /// <summary>
        /// Remove list of words formed from a particular prefix from the Trie datastructure
        /// </summary>
        /// <param name="prefix">prefix which needs to be removed from the structure</param>
        public void RemovePrefix(string prefix)
        {
            List<TrieNode> _lstNodes = GetTrieNodes(prefix, false);
            if (_lstNodes.Count > 0)
            {
                TrieNode _lastNode = _lstNodes.Last();
                if (_lastNode != null)
                    _lastNode.Clear();
            }
        }

        /// <summary>
        /// Rmove the particular word from the trie
        /// </summary>
        /// <param name="word">Word to be removed from the trie</param>
        public void RemoveWord(string word)
        {
            List<TrieNode> _lstNodes = GetTrieNodes(word, false);
            if (_lstNodes.Count > 0)
                RemoveWord(_lstNodes);
        }
        #endregion public methods

        #region Private methods
        /// <summary>
        /// Add a particular word to node
        /// </summary>
        /// <param name="node">Node to which the new nodes to apped as child</param>
        /// <param name="word">Word to be converted in the form of trie nodes</param>
        private void AddWord(TrieNode node, string word, Dictionary<int, int> dicDocuFrequcy)
        {            
            for (int _index = 0, len = word.Length; _index < len; _index++)
            {
                string _characterToAdd = word[_index].ToString();
                TrieNode _childNode = node.GetChild(_characterToAdd);
                if (_childNode == null)
                {
                    _wordIndex++;
                    _childNode = TrieFactory.CreateTrieNode(_characterToAdd);
                    _childNode.WordIndex = _wordIndex;
                    _childNode.DocReferences = dicDocuFrequcy;
                    node.SetChild(_childNode);
                }
                node = _childNode;
            }
            node.WordCount++;
        }

        /// <summary>
        /// Get the list of words formed from  a particular node recursively
        /// </summary>
        /// <param name="node">Node from which the word formation needs to be evaluated</param>
        /// <param name="lstWords">Return parameter</param>
        /// <param name="sb"></param>
        private void GetWords(TrieNode node, List<string> lstWords, StringBuilder sb)
        {
            if (node.WordCount > 0)
            {
                lstWords.Add(sb.ToString());
            }
            foreach (TrieNode child in node.ChildNodes.Values)
            {
                sb.Append(child.Character);
                GetWords(child, lstWords, sb);
                sb.Length--;
            }
        }

        /// <summary>
        /// To cound total nodes recursively till last node for a particular node
        /// </summary>
        /// <param name="node">Node for which count needs to be calculated</param>
        /// <param name="count">Count of the nodes returns</param>
        private void GetCount(TrieNode node, int count)
        {
            if (node.WordCount > 0)
            {
                count += node.WordCount;
            }
            foreach (TrieNode child in node.ChildNodes.Values)
            {
                GetCount(child, count);
            }
        }

        /// <summary>
        /// To get the node representation of the word
        /// </summary>
        /// <param name="word"></param>
        /// <returns>node presentation of the word</returns>
        private TrieNode GetNode(string word)
        {
            TrieNode _rtnVal = _rootTrieNode;
            foreach (var w in word)
            {
                _rtnVal = _rtnVal.GetChild(w.ToString());
                if (_rtnVal == null)
                    break;
            }
            return _rtnVal;
        }

        /// <summary>
        /// Get stack of trieNodes for given character.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="isWord">Optioanl parameter indicating if word is to be checked</param>
        /// <returns>list of child nodes starting with given character</returns>

        private List<TrieNode> GetTrieNodes(string s, bool isWord = true)
        {
            var nodes = new List<TrieNode>();
            var trieNode = _rootTrieNode;
            nodes.Add(trieNode);
            foreach (var c in s)
            {
                trieNode = trieNode.GetChild(c.ToString());
                if (trieNode == null)
                {
                    nodes.Clear();
                    break;
                }
                nodes.Add(trieNode);
            }

            if (isWord)
            {
                if (trieNode == null || trieNode.WordCount.Equals(0))
                {
                    Console.WriteLine($"{s} does not exist in trie.");
                }
            }
            return nodes;
        }

        /// <summary>
        /// Removes the word from the trie by clearing the child nodes. For the safty the removal process starts from end of the word
        /// </summary>
        /// <param name="lstNodes">List of the nodes representating the word</param>
        private void RemoveWord(List<TrieNode> lstNodes)
        {
            TrieNode _lastNode = lstNodes.Last();
            if (_lastNode != null)
                _lastNode.WordCount = 0;
            while (lstNodes.Count > 1)
            {
                lstNodes.Remove(_lastNode);
                TrieNode _secondLastNode = lstNodes.Last();
                if (_lastNode.WordCount > 0 || _lastNode.GetChildren().Any())
                {
                    break;
                }
                _secondLastNode.RemoveChild(_lastNode.Character);
                _lastNode = _secondLastNode;
            }
        }
        #endregion Private methods
    }
    #endregion class Trie
}
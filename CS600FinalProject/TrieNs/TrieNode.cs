using System.Collections.Generic;

namespace CS600FinalProject
{
    #region TrieNode class
    /// <summary>
    /// Class representation of the node for Trie datastructure. Uses Dictionary for faster retrival.
    /// </summary>
    public class TrieNode
    {
        #region Private members
        private string _character = string.Empty;

        private IDictionary<string, TrieNode> _childNodes = null;

        private int _wordCount = 0;

        private int _wordIndex = 0;

        private IDictionary<int, int> _dicDocFrequency = new Dictionary<int, int>();
        #endregion Private members

        #region  Properties
        /// <summary>
        /// Character to be stored in Trie node
        /// </summary>
        internal string Character
        {
            get
            {
                return _character;
            }
            private set
            {
                _character = value;
            }
        }

        /// <summary>
        /// Child characters of the words
        /// </summary>
        public IDictionary<string, TrieNode> ChildNodes
        {
            get
            {
                return _childNodes;
            }
            set
            {
                _childNodes = value;
            }
        }

        /// <summary>
        /// Total number of words in the node
        /// </summary>
        public int WordCount
        {
            get
            {
                return _wordCount;
            }
            set
            {
                _wordCount = value;
            }
        }

        /// <summary>
        /// Index of the word in the trie structure
        /// </summary>
        public int WordIndex
        {
            set
            {
                _wordIndex = value;
            }
            get
            {
                return _wordIndex;
            }
        }

        /// <summary>
        /// Dictionary containing document number from website map and frequency of occurrence in that document
        /// </summary>
        public IDictionary<int, int> DocReferences
        {
            get
            {
                return _dicDocFrequency;
            }
            set
            {
                _dicDocFrequency = value;
            }
        }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// constructor to be invoked by the Factory class.
        /// </summary>
        /// <param name="character">Character for which node to be created</param>
        /// <param name="children">Any descendent characters of the word to append to this node</param>
        /// <param name="wordCount">Total number of words can be formed from the node</param>
        internal TrieNode(string character, IDictionary<string, TrieNode> children, int wordCount)
        {
            _character = character;
            _childNodes = children;
            _wordCount = wordCount;
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Overriding default ToString method to convert to string
        /// </summary>
        /// <returns>String representation of the character stored in the ndoe</returns>
        public override string ToString() => Character.ToString();

        /// <summary>
        /// Overridden default method for comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean value indicating if two nodes are equal based on the character stored in the node(case sensetive)</returns>
        public override bool Equals(object obj)
        {
            TrieNode _self = obj as TrieNode;
            return obj != null
                && (obj) != null
                && _self.Character.Equals(Character);
        }

        public override int GetHashCode() => Character.GetHashCode();

        internal IEnumerable<TrieNode> GetChildren() => _childNodes.Values;

        /// <summary>
        /// To retrieve the child node of the current node for a particular character
        /// </summary>
        /// <param name="character"></param>
        /// <returns>Child node containing the character</returns>
        internal TrieNode GetChild(string character)
        {
            _childNodes.TryGetValue(character, out TrieNode _rtnVal);
            return _rtnVal;
        }

        /// <summary>
        /// To modify the child node character for a particular node
        /// </summary>
        /// <param name="node"></param>
        internal void SetChild(TrieNode node) => _childNodes[node.Character] = node;

        /// <summary>
        /// To remove the child node based on the character
        /// </summary>
        /// <param name="character"></param>
        internal void RemoveChild(string character) => _childNodes.Remove(character);

        /// <summary>
        /// To remove the entire descendents for the node
        /// </summary>
        internal void Clear()
        {
            _wordCount = 0;
            _childNodes.Clear();
        }
        #endregion Methods
    }
    #endregion TrieNode class
}
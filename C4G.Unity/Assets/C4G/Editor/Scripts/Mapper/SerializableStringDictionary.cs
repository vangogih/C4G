using System;
using System.Collections.Generic;
using UnityEngine;

namespace C4G.Editor
{
    /// <summary>
    /// Editor-only serializable string-to-string dictionary with enhanced features.
    /// Optimized for editor use cases with better debugging and validation.
    /// </summary>
    [Serializable]
    public class SerializableStringDictionary : Dictionary<string, string>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<string> _values = new List<string>();

        [SerializeField, HideInInspector] private bool _allowEmptyKeys = false;
        [SerializeField, HideInInspector] private bool _trimWhitespace = true;

        /// <summary>
        /// Whether to allow empty or null keys (default: false)
        /// </summary>
        public bool AllowEmptyKeys
        {
            get => _allowEmptyKeys;
            set => _allowEmptyKeys = value;
        }

        /// <summary>
        /// Whether to automatically trim whitespace from keys and values (default: true)
        /// </summary>
        public bool TrimWhitespace
        {
            get => _trimWhitespace;
            set => _trimWhitespace = value;
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in this)
            {
                string key = _trimWhitespace ? kvp.Key?.Trim() : kvp.Key;
                string value = _trimWhitespace ? kvp.Value?.Trim() : kvp.Value;

                _keys.Add(key ?? string.Empty);
                _values.Add(value ?? string.Empty);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            if (_keys == null || _values == null)
            {
                InitializeLists();
                return;
            }

            if (_keys.Count != _values.Count)
            {
#if UNITY_EDITOR
                Debug.LogError($"[SerializableStringDictionary] Key/Value count mismatch: " +
                               $"{_keys.Count} keys vs {_values.Count} values. Data will be truncated.");
#endif
                int minCount = Mathf.Min(_keys.Count, _values.Count);
                if (_keys.Count > minCount) _keys.RemoveRange(minCount, _keys.Count - minCount);
                if (_values.Count > minCount) _values.RemoveRange(minCount, _values.Count - minCount);
            }

            var duplicateKeys = new HashSet<string>();

            for (int i = 0; i < _keys.Count; i++)
            {
                string key = _keys[i];
                string value = _values[i];

                // Apply trimming if enabled
                if (_trimWhitespace)
                {
                    key = key?.Trim() ?? string.Empty;
                    value = value?.Trim() ?? string.Empty;
                }

                // Validate key
                if (!_allowEmptyKeys && string.IsNullOrEmpty(key))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[SerializableStringDictionary] Skipping empty key at index {i}");
#endif
                    continue;
                }

                // Handle duplicates
                if (ContainsKey(key))
                {
                    duplicateKeys.Add(key);
                    this[key] = value; // Overwrite with last occurrence
                }
                else
                {
                    this[key] = value;
                }
            }

#if UNITY_EDITOR
            if (duplicateKeys.Count > 0)
            {
                Debug.LogWarning($"[SerializableStringDictionary] Found {duplicateKeys.Count} duplicate keys: " +
                                 string.Join(", ", duplicateKeys));
            }
#endif
        }

        /// <summary>
        /// Adds or updates a key-value pair with optional validation
        /// </summary>
        public bool TryAdd(string key, string value)
        {
            if (_trimWhitespace)
            {
                key = key?.Trim();
                value = value?.Trim();
            }

            if (!_allowEmptyKeys && string.IsNullOrEmpty(key))
            {
#if UNITY_EDITOR
                Debug.LogWarning("[SerializableStringDictionary] Cannot add empty key");
#endif
                return false;
            }

            this[key] = value ?? string.Empty;
            return true;
        }

        /// <summary>
        /// Gets all keys as a new list (safe for iteration)
        /// </summary>
        public List<string> GetKeysList()
        {
            return new List<string>(Keys);
        }

        /// <summary>
        /// Gets all values as a new list (safe for iteration)
        /// </summary>
        public List<string> GetValuesList()
        {
            return new List<string>(Values);
        }

        /// <summary>
        /// Validates the dictionary and reports issues
        /// </summary>
        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            foreach (var key in Keys)
            {
                if (string.IsNullOrEmpty(key))
                    result.EmptyKeys++;
                else if (string.IsNullOrWhiteSpace(key))
                    result.WhitespaceOnlyKeys++;
            }

            return result;
        }

        private void InitializeLists()
        {
            _keys = new List<string>();
            _values = new List<string>();
        }

        /// <summary>
        /// Validation result for dictionary health checks
        /// </summary>
        public struct ValidationResult
        {
            public int EmptyKeys;
            public int WhitespaceOnlyKeys;

            public bool IsValid => EmptyKeys == 0 && WhitespaceOnlyKeys == 0;

            public override string ToString()
            {
                if (IsValid) return "Dictionary is valid";

                var issues = new List<string>();
                if (EmptyKeys > 0) issues.Add($"{EmptyKeys} empty keys");
                if (WhitespaceOnlyKeys > 0) issues.Add($"{WhitespaceOnlyKeys} whitespace-only keys");

                return $"Issues found: {string.Join(", ", issues)}";
            }
        }
    }
}
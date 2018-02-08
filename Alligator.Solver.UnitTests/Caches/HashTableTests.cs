using Alligator.Solver.Caches;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Alligator.Solver.UnitTests.Caches
{
    [TestClass]
    public class HashTableTests
    {
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void Hashtable_ctor_throws_exception_if_length_param_is_negative()
        {
            new HashTable<string>(-1, 0, (x, y) => false);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void Hashtable_ctor_throws_exception_if_length_param_is_zero()
        {
            new HashTable<string>(0, 0, (x, y) => false);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void Hashtable_ctor_throws_exception_if_retrylimit_param_is_negative()
        {
            new HashTable<string>(4, -1, (x, y) => false);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Hashtable_ctor_throws_exception_if_isreplaceable_param_is_null()
        {
            new HashTable<string>(4, 0, null);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_true_if_target_position_is_empty()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            bool added = hashTable.TryAdd(2, "2");
            Assert.IsTrue(added);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_true_if_target_position_is_not_empty_but_always_replace()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => true);
            hashTable.TryAdd(2, "2");
            bool addedDuplicated = hashTable.TryAdd(2, "2");
            Assert.IsTrue(addedDuplicated);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_false_if_target_position_is_not_empty_and_never_replace()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            hashTable.TryAdd(2, "2");
            bool addedDuplicated = hashTable.TryAdd(2, "2");
            Assert.IsFalse(addedDuplicated);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_true_if_retry_allowed_and_second_target_position_is_empty()
        {
            var hashTable = new HashTable<string>(4, 1, (x, y) => false);
            hashTable.TryAdd(2, "2");
            bool addedDuplicated = hashTable.TryAdd(2, "2");
            Assert.IsTrue(addedDuplicated);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_false_if_retry_allowed_but_all_target_positions_are_not_empty()
        {
            var hashTable = new HashTable<string>(4, 1, (x, y) => false);
            hashTable.TryAdd(2, "2");
            hashTable.TryAdd(3, "3");
            bool addedDuplicated = hashTable.TryAdd(2, "2");
            Assert.IsFalse(addedDuplicated);
        }

        [TestMethod]
        public void Hashtable_tryadd_returns_true_for_non_empty_last_position_if_retry_allowed_and_first_position_is_empty()
        {
            var hashTable = new HashTable<string>(4, 1, (x, y) => false);
            hashTable.TryAdd(3, "3");
            bool addedDuplicated = hashTable.TryAdd(3, "3");
            Assert.IsTrue(addedDuplicated);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_false_if_target_position_is_empty()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            string value;
            bool found = hashTable.TryGetValue(2, out value);
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_does_not_fill_value_if_target_position_is_empty()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            string value;
            bool found = hashTable.TryGetValue(2, out value);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_true_if_target_position_contains_matched_item()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            hashTable.TryAdd(3, "3");
            string value;
            bool found = hashTable.TryGetValue(3, out value);
            Assert.IsTrue(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_fills_value_if_target_position_contains_matched_item()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            hashTable.TryAdd(3, "3");
            string value;
            bool found = hashTable.TryGetValue(3, out value);
            Assert.AreEqual("3", value);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_false_if_target_position_has_only_index_collision()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            hashTable.TryAdd(2, "2");
            string value;
            bool found = hashTable.TryGetValue(6, out value);
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_does_not_fill_value_if_target_position_has_only_index_collision()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            hashTable.TryAdd(2, "2");
            string value;
            bool found = hashTable.TryGetValue(6, out value);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_true_if_retry_allowed_and_second_target_position_contains_matched_item()
        {
            var hashTable = new HashTable<string>(4, 1, (x, y) => false);
            hashTable.TryAdd(2, "2");
            hashTable.TryAdd(6, "6");
            hashTable.TryAdd(3, "3");
            string value;
            bool found = hashTable.TryGetValue(3, out value);
            Assert.IsTrue(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_fills_value_if_retry_allowed_and_second_target_position_contains_matched_item()
        {
            var hashTable = new HashTable<string>(4, 1, (x, y) => false);
            hashTable.TryAdd(2, "2");
            hashTable.TryAdd(6, "6");
            hashTable.TryAdd(3, "3");
            string value;
            bool found = hashTable.TryGetValue(3, out value);
            Assert.AreEqual("3", value);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_false_if_matched_item_was_already_replaced()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => true);
            hashTable.TryAdd(2, "2");
            hashTable.TryAdd(6, "6");
            string value;
            bool found = hashTable.TryGetValue(2, out value);
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_does_not_fill_value_if_matched_item_was_already_replaced()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => true);
            hashTable.TryAdd(2, "2");
            hashTable.TryAdd(6, "6");
            string value;
            bool found = hashTable.TryGetValue(2, out value);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_returns_false_for_default_key_and_empty_table()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            string value;
            bool found = hashTable.TryGetValue(0, out value);
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void Hashtable_trygetvalue_does_not_fill_value_for_default_key_and_empty_table()
        {
            var hashTable = new HashTable<string>(4, 0, (x, y) => false);
            string value;
            bool found = hashTable.TryGetValue(0, out value);
            Assert.IsNull(value);
        }
    }
}

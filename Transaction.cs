using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Wmts.Generated;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{


    public class Transaction
    {
        private readonly Lists _lists;
        private readonly TransactionDictionaries _transactionDictionaries;
        private readonly DecoratorTransactionLists _decoratorTransactionLists;
        private string[] _orderArray = [];

        public Transaction(Lists lists, DecoratorUpdateFlightGUIListConverter decoratorUpdateFlightGUIListConverter)
        {
            _lists = lists;
            _decoratorTransactionLists = lists.MakeDecoratorTransaction(decoratorUpdateFlightGUIListConverter, null, this);
            _transactionDictionaries = new(_decoratorTransactionLists, NewDictionaryTransactions(), decoratorUpdateFlightGUIListConverter);
            _decoratorTransactionLists.AddTransactionDictionaries(_transactionDictionaries);
        }
        public bool CorrectOrder(string key) => _transactionDictionaries.ContainsKeyTransactions(key);

        private Dictionary<string, Action> NewDictionaryTransactions()
        {
            return new()
            {
                { "display", () => Display() },
                { "update", () => Update()},
                { "delete", () => Delete()},
                { "add", () => Add()}
            };
        }

        public void LoadAndExecuteTransaction(string[] orderArray)
        {
            LoadNewTransaction(orderArray);
            ExecuteTransaction();
        }
        private void LoadNewTransaction(string[] orderArray) => _orderArray = orderArray;
        private void ExecuteTransaction()
        {
            if (_transactionDictionaries.ContainsKeyTransactions(_orderArray[0]))
                _transactionDictionaries.transactions[_orderArray[0]].Invoke();
            else
                Console.WriteLine("Incorrect Transaction Type");
        }

        private void Display()
        {
            if (_transactionDictionaries.ContainsKeyDisplays(_orderArray[3]))
                _transactionDictionaries.displays[_orderArray[3]].Invoke(_orderArray);
            else
                Console.WriteLine("Incorrect Data Type");
        }
        private void Update()
        {
            if (_transactionDictionaries.ContainsKeyUpdates(_orderArray[1]))
                _transactionDictionaries.updates[_orderArray[1]].Invoke(_orderArray);
            else
                Console.WriteLine("Incorrect Data Type");
        }
        private void Delete()
        {
            if (_transactionDictionaries.ContainsKeyDeletes(_orderArray[1]))
                _transactionDictionaries.deletes[_orderArray[1]].Invoke(_orderArray);
            else
                Console.WriteLine("Incorrect Data Type");
        }
        private void Add()
        {
            if (_transactionDictionaries.ContainsKeyAdds(_orderArray[1]))
                _transactionDictionaries.adds[_orderArray[1]].Invoke(_orderArray);
            else
                Console.WriteLine("Incorrect Data Type");
        }

        public bool CorrectConditionSyntax(string[] orderArray, TransactionType transaction)
        {
            int index = Convert.ToInt32(transaction);

            if (_transactionDictionaries == null)
                return false;

            //bez warunkow
            if (orderArray.Length == index)
                return true;

            //trust me 
            if ((orderArray.Length - index) % 4 != 0)
            {
                Console.WriteLine("Incorrect Length of Query");
                return false;
            }

            if (orderArray[index] != "where")
            {
                Console.WriteLine("Query should contain 'where'");
                return false;
            }

            if (!_transactionDictionaries.precicateFloats.ContainsKey(orderArray[index + 2]))
            {
                Console.WriteLine("Incorrect Operation Character(should be '>', '<', '=', '!=', '<=', '>=')");
                return false;
            }

            for (int i = index + 5; i < orderArray.Length; i += 4)
            {
                if (orderArray[i - 1] != "and" && orderArray[i - 1] != "or")
                {
                    Console.WriteLine("Incorrect Quantificator (should be and/or)");
                    return false;
                }
                if (!_transactionDictionaries.precicateFloats.ContainsKey(orderArray[i + 1]))
                {
                    Console.WriteLine("Incorrect Operation Character (should be '>', '<', '=', '!=', '<=', '>=')");
                    return false;
                }
            }

            return true;
        }

        public bool CorrectConditionVariables<T>(string[] orderArray, TransactionType transaction, Dictionary<string, Func<T, string[], bool>> objectDictionaryPredicates)
            where T : class
        {

            for (int i = Convert.ToInt32(transaction) + 1; i < orderArray.Length; i += 4)
            {
                var value = orderArray[i].Split('.');

                Func<string, bool>? func = null;
                if (!objectDictionaryPredicates.ContainsKey(value[0])
                    || (value.Length > 1 && !_transactionDictionaries._dictionaryStructCorrectness.TryGetValue(value[0], out func))
                    || (func != null && !func.Invoke(value[1])))
                {
                    Console.WriteLine("Incorrect Variable Name (in Conditions)");
                    return false;
                }
            }
            return true;
        }


        public static List<string> BoolDisplay<T>(Dictionary<string, Func<T, string>> listObjectDisplay, string conditions)
        {
            if (conditions == "*")
                return listObjectDisplay.ToList().Select(pair => pair.Key).ToList();
            
            IEnumerable<string> enumerable = conditions.Split(',');
            foreach (var variable in enumerable)
                if(!listObjectDisplay.ContainsKey(variable))
                {
                    Console.WriteLine("Incorrect Variable Name (To Display)");
                    return [];
                }

            return enumerable.ToList();
        }


        public bool BoolUpdate(string order, out (string property, string value, string structField, string structStructField)[] updateArray)
        {
            string[] updatesArrayTemp = order.Trim('(', ')').Split(',');
            updateArray = new (string, string, string, string)[updatesArrayTemp.Length];

            for (int i = 0; i < updatesArrayTemp.Length; i++)
            {
                var temp = updatesArrayTemp[i].Split('=');

                if (temp.Length != 2)
                {
                    Console.WriteLine("Incorrect key/value set syntax");
                    return false;
                }

                var temp2 = temp[0].Split('.');


                if (temp2.Length != 1 && !_transactionDictionaries._dictionaryStructCorrectness.ContainsKey(temp2[0]))
                {
                    Console.WriteLine("Incorrect key in value setting");
                    return false;
                }


                updateArray[i] = temp2.Length == 1 ? (temp[0], temp[1], "", "") : (temp2.Length == 2 ? (temp2[0], temp[1], temp2[1], "") : (temp2[0], temp[1], temp2[1], temp2[2]));

            }
            return true;
        }

        public bool IfCorrectNewID((string property, string value, string structField, string structStructFiled)[] updateArray)
        {
            for (int i = 0; i < updateArray.Length; i++)
                if (updateArray[i].property == "ID" && _lists.ContainsID(ulong.Parse(updateArray[i].value)))
                {
                    Console.WriteLine("This ID (" + updateArray[i].value + ") is already assigned to a different object");
                    return false;
                }
            return true;
        }

        public static bool Condition<T>(T fl, string[] orderArray, TransactionType transaction, Dictionary<string, Func<T, string[], bool>> dictionaryPredicates)
        {
            int index = Convert.ToInt32(transaction);

            //bez warunkow
            if (orderArray.Length == index)
                return true;

            string[] structures = orderArray[index + 1].Split('.');
            bool result = dictionaryPredicates[structures[0]].Invoke(
                fl, [orderArray[index + 2], orderArray[index + 3], structures.Length == 1 ? "" : structures[1], structures.Length <= 2 ? "" : structures[2]]);

            bool tempResult;
            for (int i = index + 5; i < orderArray.Length; i += 4)
            {
                //STRUKTURY
                structures = orderArray[i].Split('.');
                tempResult = dictionaryPredicates[structures[0]].Invoke(
                    fl, [orderArray[i + 1], orderArray[i + 2], structures.Length == 1 ? "" : structures[1], structures.Length <= 2 ? "" : structures[2]]);

                if (orderArray[i - 1] == "and" && (!result || !tempResult))
                    result = false;
                else if (orderArray[i - 1] == "or")
                {
                    if (result == true) return true;
                    result = tempResult;
                }
            }

            return result;
        }

    }
}


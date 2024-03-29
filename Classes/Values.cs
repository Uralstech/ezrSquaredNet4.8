﻿using ezrSquared.Errors;
using ezrSquared.General;
using ezrSquared.Nodes;
using ezrSquared.Helpers;
using static ezrSquared.Constants.constants;
using static ezrSquared.Main.ezr;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.IO;
using System;

namespace ezrSquared.Values
{
    public class ItemDictionary : LinkedList<KeyValuePair<item, item>>
    {
        public int Count => _values.Length;

        private LinkedList<KeyValuePair<item, item>>[] _values;
        private error? firstError;
        private int capacity;
        private int raise;

        public ItemDictionary()
        {
            raise = 4;
            _values = new LinkedList<KeyValuePair<item, item>>[16];
        }

        public void Add(item key, item val, out error? error)
        {
            var hash = GetKeyHashCode(key, out error);
            if (error != null) return;

            if (_values[hash] == null)
                _values[hash] = new LinkedList<KeyValuePair<item, item>>();

            var keyPresent = _values[hash].Any(p => CheckEqual(p, key));
            if (firstError != null)
            {
                error = firstError;
                firstError = null;
                return;
            }

            var newValue = new KeyValuePair<item, item>(key, val);

            if (keyPresent)
            {
                _values[hash] = new LinkedList<KeyValuePair<item, item>>();
                _values[hash].AddLast(newValue);
            }
            else
            {
                _values[hash].AddLast(newValue);

                capacity++;
                if (Count <= capacity)
                    ResizeCollection();
            }
        }

        private bool CheckEqual(KeyValuePair<item, item> pair, item key)
        {
            bool isEqual = pair.Key.ItemEquals(key, out error? error);
            if (error != null && firstError == null)
                firstError = error;

            return isEqual;
        }

        private void ResizeCollection()
        {
            raise++;
            LinkedList<KeyValuePair<item, item>>[] newArray = new LinkedList<KeyValuePair<item, item>>[(int)Math.Pow(2, raise)];
            Array.Copy(_values, newArray, _values.Length);

            _values = newArray;
        }

        public void Remove(item key, out error? error)
        {
            var hash = GetKeyHashCode(key, out error);
            if (error != null) return;

            var keyPresent = _values[hash].Any(p => CheckEqual(p, key));
            if (firstError != null)
            {
                error = firstError;
                firstError = null;
                return;
            }

            if (keyPresent)
                _values[hash] = new LinkedList<KeyValuePair<item, item>>();
        }

        public bool ContainsKey(item key, out error? error)
        {
            var hash = GetKeyHashCode(key, out error);
            if (error != null) return false;

            bool containsKey = _values[hash] == null ? false : _values[hash].Any(p => CheckEqual(p, key));
            if (firstError != null)
            {
                error = firstError;
                firstError = null;
                return false;
            }

            return containsKey;
        }

        public item? GetValue(item key, out error? error)
        {
            var hash = GetKeyHashCode(key, out error);
            if (error != null) return null;

            item? value = _values[hash] == null ? null : _values[hash].First(m => CheckEqual(m, key)).Value;
            if (firstError != null)
            {
                error = firstError;
                firstError = null;
                return null;
            }

            return value;
        }

        public IEnumerator<KeyValuePair<item, item>> GetEnumerator()
        {
            return (from collections in _values
                    where collections != null
                    from item in collections
                    select item).GetEnumerator();
        }

        public KeyValuePair<item, item>[] GetArray()
        {
            return (from collections in _values
                    where collections != null
                    from item in collections
                    select item).ToArray();
        }

        public int GetKeyHashCode(item key, out error? error)
        {
            error = null;

            var hash = key.GetItemHashCode(out error);
            if (error != null) return 0;

            return (Math.Abs(hash)) % _values.Length;
        }
    }

    public abstract class item
    {
        public position? startPos;
        public position? endPos;
        public context? context;

        public item() { }

        public item setPosition(position? startPos, position? endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            return this;
        }

        public item setContext(context? context)
        {
            this.context = context;
            return this;
        }

        public virtual item? compareEqual(item other, out error? error)
        {
            error = null;
            return new boolean(ItemEquals(other, out error)).setContext(context);
        }

        public virtual item? compareNotEqual(item other, out error? error)
        {
            error = null;
            return new boolean(!ItemEquals(other, out error)).setContext(context);
        }

        public virtual item? compareAnd(item other, out error? error)
        {
            error = null;

            bool boolValue = isTrue(out error);
            if (error != null) return null;

            bool otherBoolValue = other.isTrue(out error);
            if (error != null) return null;

            return new boolean(boolValue && otherBoolValue);
        }

        public virtual item? compareOr(item other, out error? error)
        {
            error = null;

            bool boolValue = isTrue(out error);
            if (error != null) return null;

            bool otherBoolValue = other.isTrue(out error);
            if (error != null) return null;

            return new boolean(boolValue || otherBoolValue);
        }

        public virtual item? checkIn(item other, out error? error)
        {
            error = null;
            if (other is list)
                return new boolean(((list)other).hasElement(this, out error)).setContext(context);
            else if (other is array)
                return new boolean(((array)other).hasElement(this, out error)).setContext(context);

            error = illegalOperation();
            return null;
        }
        public virtual bool isTrue(out error? error) { error = null; return false; }

        public virtual item? bitwiseOrdTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? bitwiseXOrdTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? bitwiseAndedTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? bitwiseLeftShiftedTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? bitwiseRightShiftedTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? bitwiseNotted(out error? error)
        {
            error = illegalOperation();
            return null;
        }

        public virtual item? addedTo(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? subbedBy(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? multedBy(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? divedBy(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? modedBy(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? powedBy(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? compareLessThan(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? compareGreaterThan(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? compareLessThanOrEqual(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? compareGreaterThanOrEqual(item other, out error? error)
        {
            error = illegalOperation(other);
            return null;
        }

        public virtual item? invert(out error? error)
        {
            error = illegalOperation();
            return null;
        }
        public virtual async Task<runtimeResult> execute(item[] args) { return new runtimeResult().failure(new runtimeError(startPos, endPos, RT_ILLEGALOP, "\"execute\" method called on unsupported object", context)); }
        public virtual async Task<runtimeResult> get(node node) { return new runtimeResult().failure(new runtimeError(node.startPos, node.endPos, RT_ILLEGALOP, "\"get\" method called on unsupported object", context)); }
        public virtual runtimeResult set(string name, item variable) { return new runtimeResult().failure(new runtimeError(startPos, endPos, RT_ILLEGALOP, "\"set\" method called on unsupported object", context)); }
        
        public virtual item copy() { throw new Exception($"No copy method defined for \"{GetType().Name}\"!"); }

        public error illegalOperation(item? other = null)
        {
            if (other != null)
                return new runtimeError(this.startPos, other.endPos, RT_ILLEGALOP, $"Illegal operation for types \"{this.GetType().Name}\" and \"{other.GetType().Name}\"", this.context);
            return new runtimeError(this.startPos, this.endPos, RT_ILLEGALOP, $"Illegal operation for type \"{this.GetType().Name}\"", this.context);
        }

        public override string ToString() { throw new Exception($"No ToString method defined for \"{GetType().Name}\"!"); }
        public virtual bool ItemEquals(item obj, out error? error) { throw new Exception($"No Equals method defined for \"{GetType().Name}\"!"); }
        public virtual int GetItemHashCode(out error? error) { throw new Exception($"No GetHashCode method defined for \"{GetType().Name}\"!"); }
    }

    public abstract class value : item
    {
        public object storedValue;
        public context? internalContext;
        private interpreter interpreter;

        public value(object storedValue)
        {
            this.storedValue = storedValue;
            interpreter = new interpreter();
        }

        public context generateContext()
        {
            context newContext = new context($"<<{GetType().Name}> internal>", context, startPos, false);
            newContext.symbolTable = new symbolTable(newContext.parent.symbolTable);
            return newContext;
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            internalContext = generateContext();
            return new runtimeResult().success(this);
        }

        public override async Task<runtimeResult> get(node node)
        {
            runtimeResult result = new runtimeResult();
            if (internalContext != null)
            {
                item value = result.register(await interpreter.visit(node, internalContext));
                if (result.shouldReturn()) return result;
                return result.success(value);
            }

            return await base.get(node);
        }

        public override string ToString() { return storedValue.ToString(); }
    }

    public class boolean : value
    {
        public boolean(bool value) : base(value) { }

        public override item? invert(out error? error)
        {
            error = null;
            return new boolean(!(bool)storedValue).setContext(context);
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("as_string", new predefined_function("boolean_as_string", asString, new string[0]));
            internalContext.symbolTable.set("as_character_list", new predefined_function("boolean_as_character_list", asCharList, new string[0]));
            internalContext.symbolTable.set("as_integer", new predefined_function("boolean_as_integer", asInteger, new string[0]));
            return new runtimeResult().success(this);
        }

        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }
        private runtimeResult asInteger(context context, position[] positions) { return new runtimeResult().success(new integer((bool)storedValue ? 1 : 0)); }

        public override bool isTrue(out error? error) { error = null; return (bool)storedValue; }
        public override item copy() { return new boolean((bool)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return base.ToString().ToLower(); }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (GetType() == obj.GetType()) return (bool)storedValue == (bool)((value)obj).storedValue; return false; }
        public override int GetItemHashCode(out error? error) { error = null; return ((bool)storedValue).GetHashCode(); }
    }

    public class nothing : value
    {
        public nothing() : base(null) { }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("as_string", new predefined_function("nothing_as_string", asString, new string[0]));
            internalContext.symbolTable.set("as_character_list", new predefined_function("nothing_as_character_list", asCharList, new string[0]));
            internalContext.symbolTable.set("as_integer", new predefined_function("nothing_as_integer", asInteger, new string[0]));
            internalContext.symbolTable.set("as_boolean", new predefined_function("nothing_as_boolean", asBoolean, new string[0]));
            return new runtimeResult().success(this);
        }

        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }
        private runtimeResult asInteger(context context, position[] positions) { return new runtimeResult().success(new integer(0)); }
        private runtimeResult asBoolean(context context, position[] positions) { return new runtimeResult().success(new boolean(false)); }

        public override item copy() { return new nothing().setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return "nothing"; }
        public override bool ItemEquals(item obj, out error? error) { error = null; return GetType() == obj.GetType(); }
        public override int GetItemHashCode(out error? error) { error = null; return 0; }
    }

    public class integer : value
    {
        public integer(int value) : base(value) { }

        public override item? bitwiseOrdTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue | (int)((integer)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? bitwiseXOrdTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue ^ (int)((integer)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? bitwiseAndedTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue & (int)((integer)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? bitwiseLeftShiftedTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue << (int)((integer)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? bitwiseRightShiftedTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue >> (int)((integer)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? bitwiseNotted(out error? error)
        {
            error = null;
            return new integer(~(int)storedValue).setContext(context);
        }

        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue + (int)((integer)other).storedValue).setContext(context);
            else if (other is @float)
                return new @float((int)storedValue + (float)((@float)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? subbedBy(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue - (int)((integer)other).storedValue).setContext(context);
            else if (other is @float)
                return new @float((int)storedValue - (float)((@float)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)storedValue * (int)((integer)other).storedValue).setContext(context);
            else if (other is @float)
                return new @float((int)storedValue * (float)((@float)other).storedValue).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }

                if (other is @float)
                    return new @float((int)storedValue / (float)otherValue.storedValue).setContext(context);
                return new integer((int)storedValue / (int)otherValue.storedValue).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? modedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Modulo by zero", context);
                    return null;
                }

                if (other is @float)
                    return new @float((int)storedValue % (float)otherValue.storedValue).setContext(context);
                return new integer((int)storedValue % (int)otherValue.storedValue).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? powedBy(item other, out error? error)
        {
            error = null;
            if (other is integer)
                return new integer((int)Math.Pow((int)storedValue, (int)((integer)other).storedValue)).setContext(context);
            else if (other is @float)
                return new @float((float)Math.Pow((int)storedValue, (float)((@float)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThan(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((int)storedValue < ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareGreaterThan(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((int)storedValue > ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((int)storedValue <= ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareGreaterThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((int)storedValue >= ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? invert(out error? error)
        {
            error = null;
            return new integer(((int)storedValue == 0) ? 1 : 0).setContext(context);
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("abs", new predefined_function("integer_abs", abs, new string[0]));
            internalContext.symbolTable.set("as_string", new predefined_function("integer_as_string", asString, new string[0]));
            internalContext.symbolTable.set("as_character_list", new predefined_function("integer_as_character_list", asCharList, new string[0]));
            internalContext.symbolTable.set("as_float", new predefined_function("integer_as_float", asFloat, new string[0]));
            internalContext.symbolTable.set("as_boolean", new predefined_function("integer_as_boolean", asBoolean, new string[0]));
            return new runtimeResult().success(this);
        }


        private runtimeResult abs(context context, position[] positions) { return new runtimeResult().success(new integer((int)Math.Abs((int)storedValue))); }
        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }
        private runtimeResult asFloat(context context, position[] positions) { return new runtimeResult().success(new @float((float)storedValue)); }
        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        public override bool isTrue(out error? error) { error = null; return (int)storedValue != 0; }
        public override item copy() { return new integer((int)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override bool ItemEquals(item obj, out error? error) { error = null; if (GetType() == obj.GetType()) return (int)storedValue == (int)((value)obj).storedValue; return false; }
        public override int GetItemHashCode(out error? error) { error = null; return ((int)storedValue).GetHashCode(); }
    }

    public class @float : value
    {
        public @float(float value) : base(value) { }
        public @float(int value) : base((float)value) { }

        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new @float((float)storedValue + ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? subbedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new @float((float)storedValue - ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new @float((float)storedValue * ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }

                return new @float((float)storedValue / ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? modedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Modulo by zero", context);
                    return null;
                }

                return new @float((float)storedValue % ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? powedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new @float((float)Math.Pow((float)storedValue, ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue))).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThan(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((float)storedValue < ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareGreaterThan(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((float)storedValue > ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((float)storedValue <= ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? compareGreaterThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
                return new boolean((float)storedValue >= ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue)).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? invert(out error? error)
        {
            error = null;
            return new @float(((float)storedValue == 0f) ? 1f : 0f).setContext(context);
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("abs", new predefined_function("float_abs", abs, new string[0]));
            internalContext.symbolTable.set("as_string", new predefined_function("float_as_string", asString, new string[0]));
            internalContext.symbolTable.set("as_character_list", new predefined_function("float_as_character_list", asCharList, new string[0]));
            internalContext.symbolTable.set("as_integer", new predefined_function("float_as_integer", asInteger, new string[0]));
            internalContext.symbolTable.set("as_boolean", new predefined_function("float_as_boolean", asBoolean, new string[0]));
            internalContext.symbolTable.set("round_to", new predefined_function("float_round_to", roundTo, new string[1] { "digit" }));
            return new runtimeResult().success(this);
        }

        private runtimeResult abs(context context, position[] positions) { return new runtimeResult().success(new @float((float)Math.Abs((float)storedValue))); }
        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }
        private runtimeResult asInteger(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if ((float)storedValue < int.MinValue || (float)storedValue > int.MaxValue)
                return result.failure(new runtimeError(startPos, endPos, RT_OVERFLOW, "Value either too large or too small to be converted to an integer", context));
            return result.success(new integer((int)storedValue));
        }
        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        private runtimeResult roundTo(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item digit = context.symbolTable.get("digit");
            if (digit is not integer)
                return result.failure(new runtimeError(startPos, endPos, RT_TYPE, "Digit must be an integer", context));

            return new runtimeResult().success(new @float((float)Math.Round((float)storedValue, (int)((integer)digit).storedValue)));
        }

        public override bool isTrue(out error? error) { error = null; return (float)storedValue != 0f; }
        public override item copy() { return new @float((float)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override bool ItemEquals(item obj, out error? error) { error = null; if (GetType() == obj.GetType()) return (float)storedValue == (float)((value)obj).storedValue; return false; }
        public override int GetItemHashCode(out error? error) { error = null; return ((float)storedValue).GetHashCode(); }
    }

    public class @string : value
    {
        public @string(string value) : base(value) { }
        public @string(char value) : base(value.ToString()) { }

        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is @string)
                return new @string((string)storedValue + ((@string)other).storedValue).setContext(context);
            else if (other is character_list)
                return new @string(storedValue + string.Join("", ((List<char>)((character_list)other).storedValue))).setContext(context);

            error = illegalOperation(other);
            return null;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                string result = "";
                for (int i = 0; i < ((other is @float) ? (float)((value)other).storedValue : (int)((value)other).storedValue); i++)
                    result += storedValue;
                return new @string(result).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "String division by negative value", context);
                    return null;
                }

                string result = ((string)storedValue).Substring(0, (int)(((string)storedValue).Length / ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue)));
                return new @string(result).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((string)storedValue).Length)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of string", context);
                    return null;
                }
                else if (otherValue is @float && (float)otherValue.storedValue > int.MaxValue)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_OVERFLOW, "Value too large to be converted to an integer", context);
                    return null;
                }

                return new @string(((string)storedValue)[(int)otherValue.storedValue]).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? checkIn(item other, out error? error)
        {
            error = null;
            if (other is @string)
                return new boolean(((string)((@string)other).storedValue).Contains((string)storedValue)).setContext(context);
            else if (other is character_list)
                return new boolean(string.Join("", ((List<char>)((character_list)other).storedValue)).Contains((string)storedValue)).setContext(context);
            return base.checkIn(other, out error);
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("length", new integer(storedValue.ToString().Length));
            internalContext.symbolTable.set("slice", new predefined_function("string_slice", stringSlice, new string[2] { "start", "end" }));
            internalContext.symbolTable.set("insert", new predefined_function("string_insert", stringInsert, new string[2] { "index", "substring" }));
            internalContext.symbolTable.set("replace", new predefined_function("string_replace", stringReplace, new string[2] { "old", "new" }));
            internalContext.symbolTable.set("split", new predefined_function("string_split", stringSplit, new string[1] { "substring" }));
            internalContext.symbolTable.set("join", new predefined_function("string_join", stringJoin, new string[1] { "array" }));
            internalContext.symbolTable.set("as_integer", new predefined_function("string_as_integer", asInteger, new string[0] { }));
            internalContext.symbolTable.set("as_float", new predefined_function("string_as_float", asFloat, new string[0] { }));
            internalContext.symbolTable.set("try_as_integer", new predefined_function("string_try_as_integer", tryAsInteger, new string[0] { }));
            internalContext.symbolTable.set("try_as_float", new predefined_function("string_try_as_float", tryAsFloat, new string[0] { }));
            internalContext.symbolTable.set("as_boolean", new predefined_function("string_as_boolean", asBoolean, new string[0] { }));
            internalContext.symbolTable.set("try_as_boolean", new predefined_function("string_try_as_boolean", tryAsBoolean, new string[0] { }));
            internalContext.symbolTable.set("as_boolean_value", new predefined_function("string_as_boolean_value", asBooleanValue, new string[0] { }));
            internalContext.symbolTable.set("as_character_list", new predefined_function("string_character_list", asCharList, new string[0] { }));
            internalContext.symbolTable.set("is_null_or_empty", new predefined_function("string_is_null_or_empty", isNullOrEmpty, new string[0] { }));
            internalContext.symbolTable.set("is_null_or_spaces", new predefined_function("string_is_null_or_spaces", isNullOrSpaces, new string[0] { }));

            return new runtimeResult().success(this);
        }

        private runtimeResult stringSlice(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item start = context.symbolTable.get("start");
            item end = context.symbolTable.get("end");

            if (start is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Start must be an integer", context));
            else if (end is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "End must be an integer", context));

            int startAsInt = (int)((integer)start).storedValue;
            int endAsInt = (int)((integer)end).storedValue;

            if (startAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be less than zero", context));
            else if (endAsInt > storedValue.ToString().Length)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "End cannot be greater than length of string", context));
            else if (startAsInt > endAsInt)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be greater than end", context));

            return result.success(new @string(storedValue.ToString().Substring(startAsInt, endAsInt)));
        }

        private runtimeResult stringInsert(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item start = context.symbolTable.get("index");
            item substring = context.symbolTable.get("substring");

            if (start is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Index must be an integer", context));
            else if (substring is not @string)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Substring must be a string", context));

            int startAsInt = (int)((integer)start).storedValue;

            if (startAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be less than zero", context));
            else if (startAsInt > storedValue.ToString().Length)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be greater than length of string", context));

            return result.success(new @string(storedValue.ToString().Insert(startAsInt, ((@string)substring).storedValue.ToString())));
        }

        private runtimeResult stringReplace(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item old = context.symbolTable.get("old");
            item new_ = context.symbolTable.get("new");

            if (old is not @string)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Old must be a string", context));
            if (new_ is not @string)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "New must be a string", context));
            return result.success(new @string(storedValue.ToString().Replace(((@string)old).storedValue.ToString(), ((@string)new_).storedValue.ToString())));
        }

        private runtimeResult stringSplit(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item substring = context.symbolTable.get("substring");

            if (substring is not @string)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Substring must be a string", context));
            string[] split = storedValue.ToString().Split(((string)((@string)substring).storedValue).ToCharArray());

            item[] elements = new item[split.Length];
            for (int i = 0; i < split.Length; i++)
                elements[i] = new @string(split[i]).setPosition(positions[0], positions[1]).setContext(context);
            return result.success(new array(elements));
        }

        private runtimeResult stringJoin(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item array = context.symbolTable.get("array");

            item[] items;
            if (array is array)
                items = (item[])((array)array).storedValue;
            else if (array is list)
                items = ((List<item>)((list)array).storedValue).ToArray();
            else
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Array must be an array or list", context));

            string[] arrayAsString = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
                arrayAsString[i] = items[i].ToString();

            return result.success(new @string(string.Join(storedValue.ToString(), arrayAsString)));
        }

        private runtimeResult asInteger(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (int.TryParse(storedValue.ToString(), out int integer))
                return result.success(new integer(integer));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to integer", context));
        }

        private runtimeResult asFloat(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (float.TryParse(storedValue.ToString(), out float float_))
                return result.success(new @float(float_));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to float", context));
        }

        private runtimeResult tryAsInteger(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (int.TryParse(storedValue.ToString(), out int integer))
                return result.success(new integer(integer));
            return result.success(new nothing());
        }

        private runtimeResult tryAsFloat(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (float.TryParse(storedValue.ToString(), out float float_))
                return result.success(new @float(float_));
            return result.success(new nothing());
        }

        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (bool.TryParse(storedValue.ToString(), out bool bool_))
                return result.success(new boolean(bool_));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to boolean", context));
        }

        private runtimeResult tryAsBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (bool.TryParse(storedValue.ToString(), out bool bool_))
                return result.success(new boolean(bool_));
            return result.success(new nothing());
        }

        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list((string)storedValue)); }
        private runtimeResult asBooleanValue(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        private runtimeResult isNullOrEmpty(context context, position[] positions) { return new runtimeResult().success(new boolean(string.IsNullOrEmpty((string)storedValue))); }
        private runtimeResult isNullOrSpaces(context context, position[] positions) { return new runtimeResult().success(new boolean(string.IsNullOrWhiteSpace((string)storedValue))); }

        public override bool isTrue(out error? error) { error = null; return ((string)storedValue).Length > 0; }
        public override item copy() { return new @string((string)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"\"{storedValue}\""; }
        public string ToPureString() { return (string)storedValue; }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (GetType() == obj.GetType()) return (string)storedValue == (string)((value)obj).storedValue; return false; }
        public override int GetItemHashCode(out error? error) { error = null; return ((string)storedValue).GetHashCode(); }
    }

    public class character_list : value
    {
        public character_list(string value) : base(value.ToCharArray().ToList()) { }
        public character_list(List<char> value) : base(value) { }
    
        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is @string)
            {
                ((List<char>)storedValue).AddRange(((@string)other).storedValue.ToString().ToCharArray());
                return new nothing().setContext(context);
            }
            else if (other is character_list)
            {
                ((List<char>)storedValue).AddRange(((List<char>)((character_list)other).storedValue));
                return new nothing().setContext(context);
            }
    
            error = illegalOperation(other);
            return null;
        }

        public override item? subbedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((List<char>)storedValue).Count)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of character_list", context);
                    return null;
                }

                string removed = ((List<char>)storedValue)[(int)otherValue.storedValue].ToString();
                ((List<char>)storedValue).RemoveAt(((otherValue is @float) ? (int)((float)otherValue.storedValue) : (int)otherValue.storedValue));
                return new @string(removed).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "CharacterList multiplication by negative value", context);
                    return null;
                }
    
                char[] multedValues = new char[(int)(((List<char>)storedValue).Count * ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];
    
                for (int i = 0; i < ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue); i++)
                    ((List<char>)storedValue).CopyTo(multedValues, ((List<char>)storedValue).Count * i);
                return new character_list(multedValues.ToList()).setContext(context);
            }
    
            error = illegalOperation(other);
            return null;
        }
    
        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "CharacterList division by negative value", context);
                    return null;
                }
    
                char[] divedValues = new char[(int)(((List<char>)storedValue).Count / ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];
                for (int i = 0; i < divedValues.Length; i++)
                    divedValues[i] = ((List<char>)storedValue)[i];
    
                return new character_list(divedValues.ToList()).setContext(context);
            }
    
            error = illegalOperation(other);
            return null;
        }
    
        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((List<char>)storedValue).Count)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of character_list", context);
                    return null;
                }

                return new @string(((List<char>)storedValue)[((otherValue is @float) ? (int)((float)otherValue.storedValue) : (int)otherValue.storedValue)].ToString()).setContext(context);
            }
    
            error = illegalOperation(other);
            return null;
        }
    
        public override item? checkIn(item other, out error? error)
        {
            error = null;
            if (other is @string)
                return new boolean(((string)((@string)other).storedValue).Contains(string.Join("", (List<char>)storedValue))).setContext(context);
            else if (other is character_list)
                return new boolean(string.Join("", ((List<char>)((character_list)other).storedValue)).Contains(string.Join("", (List<char>)storedValue))).setContext(context);
            return base.checkIn(other, out error);
        }
    
        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);
    
            internalContext.symbolTable.set("length", new integer(((List<char>)storedValue).Count));
            internalContext.symbolTable.set("slice", new predefined_function("character_list_slice", charListSlice, new string[2] { "start", "end" }));
            internalContext.symbolTable.set("insert", new predefined_function("character_list_insert", charListInsert, new string[2] { "index", "value" }));
            internalContext.symbolTable.set("set", new predefined_function("character_list_set", charListSet, new string[2] { "index", "value" }));
            internalContext.symbolTable.set("remove", new predefined_function("character_list_remove", charListRemove, new string[1] { "value" }));
            internalContext.symbolTable.set("as_integer", new predefined_function("character_list_as_integer", asInteger, new string[0] { }));
            internalContext.symbolTable.set("as_float", new predefined_function("character_list_as_float", asFloat, new string[0] { }));
            internalContext.symbolTable.set("try_as_integer", new predefined_function("character_list_try_as_integer", tryAsInteger, new string[0] { }));
            internalContext.symbolTable.set("try_as_float", new predefined_function("character_list_try_as_float", tryAsFloat, new string[0] { }));
            internalContext.symbolTable.set("as_boolean", new predefined_function("character_list_as_boolean", asBoolean, new string[0] { }));
            internalContext.symbolTable.set("try_as_boolean", new predefined_function("character_list_try_as_boolean", tryAsBoolean, new string[0] { }));
            internalContext.symbolTable.set("as_boolean_value", new predefined_function("character_list_as_boolean_value", asBooleanValue, new string[0] { }));
            internalContext.symbolTable.set("as_string", new predefined_function("character_list_as_string", asString, new string[0] { }));
            internalContext.symbolTable.set("is_null_or_empty", new predefined_function("character_list_is_null_or_empty", isNullOrEmpty, new string[0] { }));
            internalContext.symbolTable.set("is_null_or_spaces", new predefined_function("character_list_is_null_or_spaces", isNullOrSpaces, new string[0] { }));
            return new runtimeResult().success(this);
        }
    
        private runtimeResult charListSlice(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item start = context.symbolTable.get("start");
            item end = context.symbolTable.get("end");
    
            if (start is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Start must be an integer", context));
            else if (end is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "End must be an integer", context));
    
            int startAsInt = (int)((integer)start).storedValue;
            int endAsInt = (int)((integer)end).storedValue + 1;
    
            if (startAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be less than zero", context));
            else if (endAsInt > ((List<char>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "End cannot be greater than length of character_list", context));
            else if (startAsInt >= endAsInt - 1)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be greater than or equal to end", context));
    
            return result.success(new character_list(((List<char>)storedValue).GetRange(startAsInt, endAsInt - startAsInt)));
        }
    
        private runtimeResult charListInsert(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item index = context.symbolTable.get("index");
            item value = context.symbolTable.get("value");
    
            if (index is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Index must be an integer", context));
    
            int indexAsInt = (int)((integer)index).storedValue;
    
            if (indexAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be less than zero", context));
            else if (indexAsInt > ((List<char>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be greater than length of character_list", context));
    
            if (value is not @string && value is not character_list)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Value must be a string or character_list", context));

            char[] value_ = (value is @string) ? ((string)((@string)value).storedValue).ToCharArray() : ((List<char>)((character_list)value).storedValue).ToArray();
            ((List<char>)storedValue).InsertRange(indexAsInt, value_);
            return result.success(new nothing());
        }
    
        private runtimeResult charListSet(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item index = context.symbolTable.get("index");
            item value = context.symbolTable.get("value");
    
            if (index is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Index must be an integer", context));
    
            int indexAsInt = (int)((integer)index).storedValue;
    
            if (indexAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be less than zero", context));
            else if (indexAsInt >= ((List<char>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be greater than length of character_list", context));
    
            if (value is not @string && value is not character_list)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Value must be a string or character_list", context));
    
            string value_ = (value is @string) ? (string)((@string)value).storedValue : string.Join("", ((List<char>)((character_list)value).storedValue));
            if (value_.Length > 1 || value_.Length < 1)
                return result.failure(new runtimeError(positions[0], positions[1], RT_LEN, "Value must be of length 1", context));
    
            ((List<char>)storedValue)[indexAsInt] = value_[0];
            return result.success(new nothing());
        }
    
        private runtimeResult charListRemove(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item value = context.symbolTable.get("value");
    
            if (value is not @string && value is not character_list)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Value must be a string or character_list", context));
    
            string value_ = (value is @string) ? (string)((@string)value).storedValue : string.Join("", ((List<char>)((character_list)value).storedValue));
            char[] chars = string.Join("", ((List<char>)storedValue)).Replace(value_, string.Empty).ToCharArray();
            
            ((List<char>)storedValue).Clear();
            ((List<char>)storedValue).AddRange(chars);
            return result.success(new nothing());
        }
    
        private runtimeResult asInteger(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (int.TryParse(string.Join("", (List<char>)storedValue), out int integer))
                return result.success(new integer(integer));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to integer", context));
        }
    
        private runtimeResult asFloat(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (float.TryParse(string.Join("", (List<char>)storedValue), out float float_))
                return result.success(new @float(float_));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to float", context));
        }
    
        private runtimeResult tryAsInteger(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (int.TryParse(string.Join("", (List<char>)storedValue), out int integer))
                return result.success(new integer(integer));
            return result.success(new nothing());
        }
    
        private runtimeResult tryAsFloat(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (float.TryParse(string.Join("", (List<char>)storedValue), out float float_))
                return result.success(new @float(float_));
            return result.success(new nothing());
        }
    
        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (bool.TryParse(string.Join("", (List<char>)storedValue), out bool bool_))
                return result.success(new boolean(bool_));
            return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Could not convert string to boolean", context));
        }
    
        private runtimeResult tryAsBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            if (bool.TryParse(string.Join("", (List<char>)storedValue), out bool bool_))
                return result.success(new boolean(bool_));
            return result.success(new nothing());
        }
    
        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(string.Join("", (List<char>)storedValue))); }
        private runtimeResult asBooleanValue(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
    
            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }
    
        private runtimeResult isNullOrEmpty(context context, position[] positions) { return new runtimeResult().success(new boolean(string.IsNullOrEmpty(string.Join("", (List<char>)storedValue)))); }
        private runtimeResult isNullOrSpaces(context context, position[] positions) { return new runtimeResult().success(new boolean(string.IsNullOrWhiteSpace(string.Join("", (List<char>)storedValue)))); }
    
        public override bool isTrue(out error? error) { error = null; return ((List<char>)storedValue).Count > 0; }
        public override item copy() { return new character_list((List<char>)storedValue).setPosition(startPos, endPos).setContext(context); }
    
        public override string ToString() { return $"'{string.Join("", (List<char>)storedValue)}'"; }
        public string ToPureString() { return string.Join("", (List<char>)storedValue); }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (GetType() == obj.GetType()) return ToPureString() == ((character_list)obj).ToPureString(); return false; }
        public override int GetItemHashCode(out error? error) { error = null; return ToPureString().GetHashCode(); }
    }

    public class array : value
    {
        public array(item[] elements) : base(elements) { }

        public bool hasElement(item other, out error? error)
        {
            error = null;
            for (int i = 0; i < ((item[])storedValue).Length; i++)
            {
                if (((item[])storedValue)[i].ItemEquals(other, out error)) return true;
                if (error != null) return false;
            }
            return false;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Array multiplication by negative value", context);
                    return null;
                }

                item[] multedValues = new item[(int)(((item[])storedValue).Length * ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];

                for (int i = 0; i < (int)otherValue.storedValue; i++)
                    ((item[])storedValue).CopyTo(multedValues, ((item[])storedValue).Length * i);
                return new array(multedValues).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Array division by negative value", context);
                    return null;
                }

                item[] divedValues = new item[(int)(((item[])storedValue).Length / ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];
                for (int i = 0; i < divedValues.Length; i++)
                    divedValues[i] = ((item[])storedValue)[i];

                return new array(divedValues).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((item[])storedValue).Length)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of array", context);
                    return null;
                }

                return ((item[])storedValue)[((otherValue is @float) ? (int)((float)otherValue.storedValue) : (int)otherValue.storedValue)].setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("length", new integer(((item[])storedValue).Length));
            internalContext.symbolTable.set("slice", new predefined_function("array_slice", arraySlice, new string[2] { "start", "end" }));
            internalContext.symbolTable.set("as_boolean", new predefined_function("array_as_boolean", asBoolean, new string[0] { }));
            internalContext.symbolTable.set("as_string", new predefined_function("array_as_string", asString, new string[0] { }));
            internalContext.symbolTable.set("as_character_list", new predefined_function("array_as_character_list", asCharList, new string[0] { }));
            return new runtimeResult().success(this);
        }

        private runtimeResult arraySlice(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item start = context.symbolTable.get("start");
            item end = context.symbolTable.get("end");

            if (start is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Start must be an integer", context));
            else if (end is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "End must be an integer", context));

            int startAsInt = (int)((integer)start).storedValue;
            int endAsInt = (int)((integer)end).storedValue + 1;

            if (startAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be less than zero", context));
            else if (endAsInt > ((item[])storedValue).Length)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "End cannot be greater than length of array", context));
            else if (startAsInt > endAsInt)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be greater than end", context));

            return result.success(new array(((item[])storedValue).Skip(startAsInt).Take(endAsInt).ToArray()));
        }

        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }

        public override bool isTrue(out error? error) { error = null; return ((item[])storedValue).Length > 0; }
        public override item copy() { return new array((item[])storedValue).setPosition(startPos, endPos).setContext(context); }

        public override string ToString()
        {
            string[] elementStrings = new string[((item[])storedValue).Length];
            for (int i = 0; i < ((item[])storedValue).Length; i++)
                elementStrings[i] = ((item[])storedValue)[i].ToString();
            return $"({string.Join(", ", elementStrings)})";
        }

        public override int GetItemHashCode(out error? error)
        {
            error = null;
            int hashCode = 0;
            for (int i = 0; i < ((item[])storedValue).Length; i++)
            {
                hashCode = (((hashCode << 5) + hashCode) ^ ((item[])storedValue)[i].GetItemHashCode(out error));
                if (error != null) return 0;
            }

            return hashCode;
        }

        public override bool ItemEquals(item obj, out error? error)
        {
            error = null;
            if (obj is array)
            {
                int hash = GetItemHashCode(out error);
                if (error != null) return false;

                int otherHash = ((array)obj).GetItemHashCode(out error);
                if (error != null) return false;

                return hash == otherHash;
            }
            return false;
        }
    }

    public class list : value
    {
        public list(item[] elements) : base(elements.ToList()) { }
        public list(List<item> elements) : base(elements) { }

        public bool hasElement(item other, out error? error)
        {
            error = null;
            for (int i = 0; i < ((List<item>)storedValue).Count; i++)
            {
                if (((List<item>)storedValue)[i].ItemEquals(other, out error)) return true;
                if (error != null) return false;
            }
            return false;
        }

        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is list)
            {
                list otherAsArrayLike = (list)other;
                ((List<item>)storedValue).AddRange((List<item>)otherAsArrayLike.storedValue);
                return new nothing().setContext(context);
            }

            ((List<item>)storedValue).Add(other);
            return new nothing().setContext(context);
        }

        public override item? subbedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((List<item>)storedValue).Count)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of list", context);
                    return null;
                }

                item removedValue = ((List<item>)storedValue)[(int)((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue)];
                ((List<item>)storedValue).RemoveAt(((otherValue is @float) ? (int)((float)otherValue.storedValue) : (int)otherValue.storedValue));
                return removedValue.setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? multedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "List multiplication by negative value", context);
                    return null;
                }

                item[] multedValues = new item[(int)(((List<item>)storedValue).Count * ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];

                for (int i = 0; i < (int)otherValue.storedValue; i++)
                    ((List<item>)storedValue).CopyTo(multedValues, ((List<item>)storedValue).Count * i);
                return new list(multedValues).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "List division by negative value", context);
                    return null;
                }

                item[] divedValues = new item[(int)(((List<item>)storedValue).Count / ((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue))];
                for (int i = 0; i < divedValues.Length; i++)
                    divedValues[i] = ((List<item>)storedValue)[i];

                return new list(divedValues).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be negative value", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) >= ((List<item>)storedValue).Count)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_INDEX, "Index cannot be greater than or equal to length of list", context);
                    return null;
                }

                return ((List<item>)storedValue)[((otherValue is @float) ? (int)((float)otherValue.storedValue) : (int)otherValue.storedValue)].setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            internalContext.symbolTable.set("length", new integer(((List<item>)storedValue).Count));
            internalContext.symbolTable.set("slice", new predefined_function("list_slice", listSlice, new string[2] { "start", "end" }));
            internalContext.symbolTable.set("insert", new predefined_function("list_insert", listInsert, new string[2] { "index", "value" }));
            internalContext.symbolTable.set("set", new predefined_function("list_set", listSet, new string[2] { "index", "value" }));
            internalContext.symbolTable.set("remove", new predefined_function("list_remove", listRemove, new string[1] { "value" }));
            internalContext.symbolTable.set("as_boolean", new predefined_function("list_as_boolean", asBoolean, new string[0] { }));
            internalContext.symbolTable.set("as_string", new predefined_function("list_as_string", asString, new string[0] { }));
            internalContext.symbolTable.set("as_character_list", new predefined_function("list_as_character_list", asCharList, new string[0] { }));
            return new runtimeResult().success(this);
        }

        private runtimeResult listSlice(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item start = context.symbolTable.get("start");
            item end = context.symbolTable.get("end");

            if (start is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Start must be an integer", context));
            else if (end is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "End must be an integer", context));

            int startAsInt = (int)((integer)start).storedValue;
            int endAsInt = (int)((integer)end).storedValue + 1;

            if (startAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be less than zero", context));
            else if (endAsInt > ((List<item>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "End cannot be greater than length of list", context));
            else if (startAsInt >= endAsInt - 1)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Start cannot be greater than or equal to end", context));

            return result.success(new list(((List<item>)storedValue).GetRange(startAsInt, endAsInt - startAsInt)));
        }

        private runtimeResult listInsert(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item index = context.symbolTable.get("index");
            item value = context.symbolTable.get("value");

            if (index is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Index must be an integer", context));

            int indexAsInt = (int)((integer)index).storedValue;

            if (indexAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be less than zero", context));
            else if (indexAsInt > ((List<item>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be greater than length of list", context));

            ((List<item>)storedValue).Insert(indexAsInt, value);
            return result.success(new nothing());
        }

        private runtimeResult listSet(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item index = context.symbolTable.get("index");
            item value = context.symbolTable.get("value");

            if (index is not integer)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Index must be an integer", context));

            int indexAsInt = (int)((integer)index).storedValue;

            if (indexAsInt < 0)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be less than zero", context));
            else if (indexAsInt >= ((List<item>)storedValue).Count)
                return result.failure(new runtimeError(positions[0], positions[1], RT_INDEX, "Index cannot be greater than length of list", context));

            ((List<item>)storedValue)[indexAsInt] = value;
            return result.success(new nothing());
        }

        private runtimeResult listRemove(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item value = context.symbolTable.get("value");

            if (!((List<item>)storedValue).Contains(value))
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "List does not contain value", context));

            ((List<item>)storedValue).Remove(value);
            return result.success(new nothing());
        }

        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }

        public override bool isTrue(out error? error) { error = null; return ((List<item>)storedValue).Count > 0; }
        public override item copy() { return new list((List<item>)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override string ToString()
        {
            string[] elementStrings = new string[((List<item>)storedValue).Count];
            for (int i = 0; i < ((List<item>)storedValue).Count; i++)
                elementStrings[i] = ((List<item>)storedValue)[i].ToString();
            return $"[{string.Join(", ", elementStrings)}]";
        }
        public override int GetItemHashCode(out error? error)
        {
            error = null;
            int hashCode = 0;
            for (int i = 0; i < ((List<item>)storedValue).Count; i++)
            {
                hashCode = (((hashCode << 5) + hashCode) ^ ((List<item>)storedValue)[i].GetItemHashCode(out error));
                if (error != null) return 0;
            }

            return hashCode;
        }
        public override bool ItemEquals(item obj, out error? error)
        {
            error = null;
            if (obj is list)
            {
                int hash = GetItemHashCode(out error);
                if (error != null) return false;

                int otherHash = ((list)obj).GetItemHashCode(out error);
                if (error != null) return false;

                return hash == otherHash;
            }
            return false;
        }
    }

    public class dictionary : value
    {
        public dictionary(ItemDictionary dictionary) : base(dictionary) { }

        public override item? addedTo(item other, out error? error)
        {
            error = null;
            if (other is dictionary)
            {
                KeyValuePair<item, item>[] otherValue = ((ItemDictionary)((dictionary)other).storedValue).GetArray();
                for (int i = 0; i < otherValue.Length; i++)
                {
                    ((ItemDictionary)storedValue).Add(otherValue[i].Key, otherValue[i].Value, out error);
                    if (error != null) return null;
                }

                return new nothing().setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? subbedBy(item other, out error? error)
        {
            error = null;

            bool containsKey = ((ItemDictionary)storedValue).ContainsKey(other, out error);
            if (error != null) return null;

            if (containsKey)
            {
                ((ItemDictionary)storedValue).Remove(other, out error);
                return new nothing().setContext(context);
            }

            error = new runtimeError(other.startPos, other.endPos, RT_KEY, "Key does not correspond to any value in dictionary", context);
            return null;
        }

        public override item? divedBy(item other, out error? error)
        {
            error = null;
            if (other is integer || other is @float)
            {
                value otherValue = (value)other;
                if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) == 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Division by zero", context);
                    return null;
                }
                else if (((otherValue is @float) ? (float)otherValue.storedValue : (int)otherValue.storedValue) < 0)
                {
                    error = new runtimeError(other.startPos, other.endPos, RT_MATH, "Dictionary division by negative value", context);
                    return null;
                }

                KeyValuePair<item, item>[] pairs = ((ItemDictionary)storedValue).GetArray();
                ItemDictionary newDict = new ItemDictionary();

                for (int i = 0; i < pairs.Length / (int)otherValue.storedValue; i++)
                {
                    newDict.Add(pairs[i].Key, pairs[i].Value, out error);
                    if (error != null) return null;
                }

                return new dictionary(newDict).setContext(context);
            }

            error = illegalOperation(other);
            return null;
        }

        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            error = null;

            item? value = ((ItemDictionary)storedValue).GetValue(other, out error);
            if (error != null) return null;

            if (value != null)
                return value.setContext(context);
            else
            {
                error = new runtimeError(other.startPos, other.endPos, RT_KEY, "Key does not correspond to any value in dictionary", context);
                return null;
            }
        }
        public override async Task<runtimeResult> execute(item[] args)
        {
            await base.execute(args);

            KeyValuePair<item, item>[] pairs = ((ItemDictionary)storedValue).GetArray();
            item[] keys = new item[pairs.Length];
            item[] values = new item[pairs.Length];
            item[] keyValuePairs = new item[pairs.Length];

            for (int i = 0; i < pairs.Length; i++)
            {
                keys[i] = pairs[i].Key;
                values[i] = pairs[i].Value;
                keyValuePairs[i] = new array(new item[2] { pairs[i].Key, pairs[i].Value }).setPosition(startPos, endPos).setContext(context);
            }

            internalContext.symbolTable.set("length", new integer(pairs.Length));
            internalContext.symbolTable.set("keys", new array(keys));
            internalContext.symbolTable.set("values", new array(values));
            internalContext.symbolTable.set("pairs", new array(keyValuePairs));
            internalContext.symbolTable.set("as_boolean", new predefined_function("dictionary_as_boolean", asBoolean, new string[0] { }));
            internalContext.symbolTable.set("as_string", new predefined_function("dictionary_as_string", asString, new string[0] { }));
            internalContext.symbolTable.set("as_character_list", new predefined_function("dictionary_as_character_list", asCharList, new string[0] { }));
            return new runtimeResult().success(this);
        }

        private runtimeResult asBoolean(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();

            bool boolValue = isTrue(out error? error);
            if (error != null) return result.failure(error);
            return result.success(new boolean(boolValue));
        }

        private runtimeResult asString(context context, position[] positions) { return new runtimeResult().success(new @string(ToString())); }
        private runtimeResult asCharList(context context, position[] positions) { return new runtimeResult().success(new character_list(ToString())); }

        public override bool isTrue(out error? error) { error = null; return ((ItemDictionary)storedValue).Count > 0; }
        public override item copy() { return new dictionary((ItemDictionary)storedValue).setPosition(startPos, endPos).setContext(context); }

        public override string ToString()
        {
            KeyValuePair<item, item>[] values = ((ItemDictionary)storedValue).GetArray();
            string[] elementStrings = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
                elementStrings[i] = $"{values[i].Key} : {values[i].Value}";
            return '{' + string.Join(", ", elementStrings) + '}';
        }

        public override int GetItemHashCode(out error? error)
        {
            error = null;
            int hashCode = 0;
            KeyValuePair<item, item>[] pairs = ((ItemDictionary)storedValue).GetArray();
            for (int i = 0; i < pairs.Length; i++)
            {
                int keyHash = pairs[i].Key.GetItemHashCode(out error);
                if (error != null) return 0;

                int valueHash = pairs[i].Value.GetItemHashCode(out error);
                if (error != null) return 0;

                int hash1 = ((keyHash << 5) + keyHash) ^ valueHash;
                hashCode = ((hashCode << 5) + hashCode) ^ hash1;
            }
            return hashCode;
        }
        public override bool ItemEquals(item obj, out error? error)
        {
            error = null;
            if (obj is dictionary)
            {
                int hash = GetItemHashCode(out error);
                if (error != null) return false;

                int otherHash = ((dictionary)obj).GetItemHashCode(out error);
                if (error != null) return false;

                return hash == otherHash;
            }
            return false;
        }
    }

    public abstract class baseFunction : item
    {
        public string name;
        public baseFunction(string name) : base()
        { this.name = name; }

        public context generateContext()
        {
            context newContext = new context(name, context, startPos, false);
            newContext.symbolTable = new symbolTable(newContext.parent.symbolTable);
            return newContext;
        }

        public runtimeResult checkArgs(string[] argNames, item[] args)
        {
            runtimeResult result = new runtimeResult();
            if (args.Length > argNames.Length)
                return result.failure(new runtimeError(startPos, endPos, RT_ARGS, $"{args.Length - argNames.Length} too many arguments passed into \"{name}\"", context));
            else if (args.Length < argNames.Length)
                return result.failure(new runtimeError(startPos, endPos, RT_ARGS, $"{argNames.Length - args.Length} too few arguments passed into \"{name}\"", context));

            return result.success(new nothing());
        }

        public void populateArgs(string[] argNames, item[] args, context context)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string argName = argNames[i];
                item argValue = args[i];

                argValue.setContext(context);
                context.symbolTable.set(argName, argValue);
            }
        }

        public runtimeResult checkAndPopulateArgs(string[] argNames, item[] args, context context)
        {
            runtimeResult result = new runtimeResult();
            item returnValue = result.register(checkArgs(argNames, args));
            if (result.shouldReturn()) return result;
            populateArgs(argNames, args, context);
            return result.success(returnValue);
        }

        public override bool isTrue(out error? error) { error = null; return true; }

        public override int GetItemHashCode(out error? error) { error = null; return ToString().GetHashCode(); }
    }

    public class predefined_function : baseFunction
    {
        private string[] argNames;
        private Func<context, position[], runtimeResult>? function;
        private Func<context, position[], Task<runtimeResult>>? asyncFunction;

        public predefined_function(string name, Func<context, position[], runtimeResult> function, string[] argNames) : base(name)
        {
            this.function = function;
            this.argNames = argNames;
        }

        public predefined_function(string name, Func<context, position[], Task<runtimeResult>> asyncFunction, string[] argNames) : base(name)
        {
            this.asyncFunction = asyncFunction;
            this.argNames = argNames;
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            runtimeResult result = new runtimeResult();

            result.register(checkAndPopulateArgs(argNames, args, context));
            if (result.shouldReturn()) return result;

            item returnValue;
            if (function != null)
                returnValue = result.register(function.Invoke(context, new position[2] { startPos, endPos }));
            else
                returnValue = result.register(await asyncFunction.Invoke(context, new position[2] { startPos, endPos }));

            if (result.shouldReturn()) return result;

            return result.success(returnValue.setPosition(startPos, endPos).setContext(context));
        }

        public override item copy()
        {
            if (function != null)
                return new predefined_function(name, function, argNames).setPosition(startPos, endPos).setContext(context);
            return new predefined_function(name, asyncFunction, argNames).setPosition(startPos, endPos).setContext(context);
        }

        public override bool ItemEquals(item obj, out error? error) { error = null; if (obj is predefined_function) return ToString() == obj.ToString(); return false; }
        public override string ToString() { return $"<predefined function <{name}>>"; }
    }

    public class builtin_function : baseFunction
    {
        private string[] argNames;
        public builtin_function(string name, string[] argNames) : base(name)
        { this.argNames = argNames; }

        public override async Task<runtimeResult> execute(item[] args)
        {
            runtimeResult result = new runtimeResult();
            context newContext = generateContext();

            string methodName = $"_{name}";
            MethodInfo? info = GetType().GetMethod(methodName, (BindingFlags.NonPublic | BindingFlags.Instance));
            if (info != null)
            {
                result.register(checkAndPopulateArgs(argNames, args, newContext));
                if (result.shouldReturn()) return result;

                object output = info.Invoke(this, new object[] { newContext });

                item returnValue;
                if (output is runtimeResult)
                    returnValue = result.register((runtimeResult)output);
                else
                    returnValue = result.register(await (Task<runtimeResult>)output);

                if (result.shouldReturn()) return result;
                return result.success(returnValue.setPosition(startPos, endPos).setContext(newContext));
            }
            throw new Exception($"No {methodName} method defined!");
        }

        private runtimeResult _show(context context)
        {
            item value = context.symbolTable.get("message");
            if (value is @string)
                Console.WriteLine(((@string)value).ToPureString());
            else if (value is character_list)
                Console.WriteLine(((character_list)value).ToPureString());
            else
                Console.WriteLine(value.ToString());

            return new runtimeResult().success(new nothing());
        }

        private runtimeResult _show_error(context context)
        {
            runtimeResult result = new runtimeResult();

            item tag = context.symbolTable.get("tag");
            item message = context.symbolTable.get("message");

            if (tag is not @string && tag is not character_list)
                return result.failure(new runtimeError(startPos, endPos, RT_TYPE, "Tag must be a string or character_list", context));
            if (message is not @string && message is not character_list)
                return result.failure(new runtimeError(startPos, endPos, RT_TYPE, "Message must be a string or character_list", context));

            string msg = (message is @string) ? ((@string)message).ToPureString() : ((character_list)message).ToPureString();
            string tg = (tag is @string) ? ((@string)tag).ToPureString() : ((character_list)tag).ToPureString();

            return new runtimeResult().failure(new runtimeError(startPos, endPos, tg, msg, context));
        }

        private runtimeResult _get(context context)
        {
            item message = context.symbolTable.get("message");
            if (message is not nothing)
            {
                if (message is @string)
                    Console.Write(((@string)message).ToPureString());
                else if (message is character_list)
                    Console.Write(((character_list)message).ToPureString());
                else
                    Console.Write(message.ToString());
            }

            string? input = Console.ReadLine();
            return new runtimeResult().success(new @string(input == null ? "" : input));
        }

        private runtimeResult _clear(context context)
        {
            Console.Clear();
            return new runtimeResult().success(new nothing());
        }

        private runtimeResult _hash(context context)
        {
            runtimeResult result = new runtimeResult();

            int hash = context.symbolTable.get("value").GetItemHashCode(out error? error);
            if (error != null) return result.failure(error);

            return result.success(new integer(hash));
        }

        private runtimeResult _type_of(context context)
        {
            item value = context.symbolTable.get("value");

            string type;
            if (value is @object)
                type = ((@object)value).name;
            else
                type = value.GetType().Name;

            return new runtimeResult().success(new @string(type));
        }

        private async Task<runtimeResult> _run(context context)
        {
            runtimeResult result = new runtimeResult();
            item file = context.symbolTable.get("file");
            if (file is not @string && file is not character_list)
                return result.failure(new runtimeError(startPos, endPos, RT_TYPE, "File must be a string or character_list", context));

            string path = (file is @string) ? (string)((@string)file).storedValue : string.Join("", ((List<char>)((character_list)file).storedValue));
            if (!File.Exists(path))
                return result.failure(new runtimeError(startPos, endPos, RT_IO, $"Script \"{path}\" does not exist", context));

            string script;
            try
            {
                script = string.Join("\n", File.ReadAllLines(path));
            }
            catch (IOException exception)
            {
                return result.failure(new runtimeError(startPos, endPos, RT_IO, $"Failed to load script \"{path}\"\n{exception.Message}", context));
            }

            context runtimeContext = new context("<main>", globalPredefinedContext, new position(0, 0, 0, "<main>", ""), false);
            runtimeContext.symbolTable = new symbolTable(globalPredefinedContext.symbolTable);

            (error? error, item? _) = await run(Path.GetFileName(path), path, script, runtimeContext);
            if (error != null)
                return result.failure(new runtimeRunError(startPos, endPos, $"Failed to execute script \"{path}\"", error.asString(), context));
            return result.success(new nothing());
        }

        public override item copy() { return new builtin_function(name, argNames).setPosition(startPos, endPos).setContext(context); }

        public override bool ItemEquals(item obj, out error? error) { error = null; if (obj is builtin_function) return ToString() == obj.ToString(); return false; }
        public override string ToString() { return $"<builtin function <{name}>>"; }
    }

    public class function : baseFunction
    {
        private node bodyNode;
        private string[] argNames;
        private bool shouldReturnNull;
        private interpreter interpreter;

        public function(string? name, node bodyNode, string[] argNames, bool shouldReturnNull) : base((name != null) ? name : "<anonymous>")
        {
            this.bodyNode = bodyNode;
            this.argNames = argNames;
            this.shouldReturnNull = shouldReturnNull;
            this.interpreter = new interpreter();
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            runtimeResult result = new runtimeResult();
            context newContext = generateContext();

            result.register(checkAndPopulateArgs(argNames, args, newContext));
            if (result.shouldReturn()) return result;

            item? value = result.register(await interpreter.visit(bodyNode, newContext));
            if (result.shouldReturn() && result.functionReturnValue == null) return result;

            if (!shouldReturnNull && value != null)
                return result.success(value);
            else if (result.functionReturnValue != null)
                return result.success(result.functionReturnValue);
            return result.success(new nothing());
        }

        public override item copy() { return new function(name, bodyNode, argNames, shouldReturnNull).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"<function {name}>"; }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (obj is function) return ToString() == obj.ToString(); return false; }
    }

    public class special : baseFunction
    {
        private node bodyNode;
        private string[] argNames;
        private bool shouldReturnNull;
        private interpreter interpreter;

        public special(string name, node bodyNode, string[] argNames, bool shouldReturnNull) : base(name)
        {
            this.bodyNode = bodyNode;
            this.argNames = argNames;
            this.shouldReturnNull = shouldReturnNull;
            this.interpreter = new interpreter();
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            runtimeResult result = new runtimeResult();
            context newContext = generateContext();

            result.register(checkAndPopulateArgs(argNames, args, newContext));
            if (result.shouldReturn()) return result;

            item? value = result.register(await interpreter.visit(bodyNode, newContext));
            if (result.shouldReturn() && result.functionReturnValue == null) return result;

            if (!shouldReturnNull && value != null)
                return result.success(value);
            else if (result.functionReturnValue != null)
                return result.success(result.functionReturnValue);
            return result.success(new nothing());
        }

        public override item copy() { return new special(name, bodyNode, argNames, shouldReturnNull).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"<special function {name}>"; }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (obj is special) return ToString() == obj.ToString(); return false; }
    }

    public class @class : baseFunction
    {
        public string[] argNames { get; private set; }
        private node bodyNode;
        private @class? parent;

        public @class(string name, @class? inherit, node bodyNode, string[] argNames) : base(name)
        {
            this.bodyNode = bodyNode;
            this.argNames = argNames;
            this.parent = inherit;
        }

        public override async Task<runtimeResult> execute(item[] args)
        {
            runtimeResult result = new runtimeResult();
            context internalContext = generateContext();

            result.register(checkAndPopulateArgs(argNames, args, internalContext));
            if (result.shouldReturn()) return result;

            if (parent != null)
            {
                item[] parentArgs = new item[parent.argNames.Length];
                for (int i = 0; i < parentArgs.Length; i++)
                    parentArgs[i] = args[Array.IndexOf(argNames, parent.argNames[i])];

                item parentObject = result.register(await parent.execute(parentArgs));
                if (result.shouldReturn()) return result;

                internalContext.symbolTable.set("parent", parentObject);
            }

            @object object_ = (@object)new @object(name, internalContext).setPosition(startPos, endPos).setContext(context);
            result.register(await object_.initialize(bodyNode));
            if (result.shouldReturn()) return result;

            return result.success(object_);
        }

        public override item copy() { return new @class(name, parent, bodyNode, argNames).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"<class {name}>"; }
        public override bool ItemEquals(item obj, out error? error) { error = null; if (obj is @class) return ToString() == obj.ToString(); return false; }
    }

    public class @object : baseFunction
    {
        public context internalContext { get; private set; }
        private interpreter interpreter;

        public @object(string name, context internalContext) : base(name)
        {
            this.internalContext = internalContext;
            this.interpreter = new interpreter();
        }

        public async Task<runtimeResult> initialize(node body)
        {
            runtimeResult result = new runtimeResult();
            internalContext.symbolTable.set("this", this);

            result.register(await interpreter.visit(body, internalContext));
            if (result.shouldReturn()) return result;

            return result.success(new nothing());
        }

        private async Task<(item?, error?)> getOutput(item func, item[] args)
        {
            runtimeResult result = new runtimeResult();
            item output = result.register(await func.execute(args));
            if (result.shouldReturn() && result.error == null) return (new nothing().setContext(context), null);

            if (result.error != null)
                return (null, result.error);
            return (output, null);
        }

        public override item? compareEqual(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_equal");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }
        
            return base.compareEqual(other, out error);
        }
        
        public override item? compareNotEqual(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_not_equal");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareNotEqual(other, out error);
        }
        
        public override item? compareAnd(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_and");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareAnd(other, out error);
        }
        
        public override item? compareOr(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_or");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareOr(other, out error);
        }
        
        public override item? checkIn(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("check_in");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.checkIn(other, out error);
        }
        
        public override item? bitwiseOrdTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_or");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseOrdTo(other, out error);
        }
        
        public override item? bitwiseXOrdTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_xor");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseXOrdTo(other, out error);
        }
        
        public override item? bitwiseAndedTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_and");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseAndedTo(other, out error);
        }
        
        public override item? bitwiseLeftShiftedTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_left_shift");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseLeftShiftedTo(other, out error);
        }
        
        public override item? bitwiseRightShiftedTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_right_shift");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseRightShiftedTo(other, out error);
        }
        
        public override item? bitwiseNotted(out error? error)
        {
            item? func = internalContext.symbolTable.get("bitwise_not");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[0]);
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.bitwiseNotted(out error);
        }
        
        public override item? addedTo(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("addition");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.addedTo(other, out error);
        }
        
        public override item? subbedBy(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("subtraction");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.subbedBy(other, out error);
        }
        
        public override item? multedBy(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("multiplication");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.multedBy(other, out error);
        }
        
        public override item? divedBy(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("division");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.divedBy(other, out error);
        }
        
        public override item? modedBy(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("modulo");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.modedBy(other, out error);
        }
        
        public override item? powedBy(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("power");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.powedBy(other, out error);
        }
        
        public override item? compareLessThan(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_less_than");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareLessThan(other, out error);
        }
        
        public override item? compareGreaterThan(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_greater_than");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareGreaterThan(other, out error);
        }
        
        public override item? compareLessThanOrEqual(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_less_than_or_equal");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareLessThanOrEqual(other, out error);
        }
        
        public override item? compareGreaterThanOrEqual(item other, out error? error)
        {
            item? func = internalContext.symbolTable.get("compare_greater_than_or_equal");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[] { other });
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.compareGreaterThanOrEqual(other, out error);
        }
        
        public override item? invert(out error? error)
        {
            item? func = internalContext.symbolTable.get("invert");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[0]);
                task.Wait();

                (item? result, error? error_) = task.Result;
                error = error_;
                return result;
            }

            return base.invert(out error);
        }
        
        public override bool isTrue(out error? error)
        {
            item? func = internalContext.symbolTable.get("is_true");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[0]);
                task.Wait();

                (item? output, error? error_) = task.Result;
                error = error_;

                if (error != null) return false;
                return output.isTrue(out error);
            }
        
            return base.isTrue(out error);
        }
        
        public override int GetItemHashCode(out error? error)
        {
            error = null;
            item? func = internalContext.symbolTable.get("hash");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[0]);
                task.Wait();

                (item? output, error? error_) = task.Result;
                error = error_;

                if (error != null) return 0;
                if (output is not integer)
                {
                    error = new runtimeError(startPos, endPos, RT_TYPE, "Return type of special function \"hash\" must be an integer", context);
                    return 0;
                }
        
                return (int)((integer)output).storedValue;
            }
        
            return base.GetItemHashCode(out error);
        }

        public override async Task<runtimeResult> get(node node)
        {
            runtimeResult result = new runtimeResult();

            item value = result.register(await interpreter.visit(node, internalContext));
            if (result.shouldReturn()) return result;
            return result.success(value);
        }

        public override runtimeResult set(string name, item variable)
        {
            internalContext.symbolTable.set(name, variable.copy());
            return new runtimeResult().success(variable);
        }

        public override item copy() { return new @object(name, internalContext).setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"<object {name}>"; }
        public override bool ItemEquals(item obj, out error? error)
        {
            error = null;
        
            item? func = internalContext.symbolTable.get("equals");
            if (func != null && func is special)
            {
                Task<(item?, error?)> task = getOutput(func, new item[0]);
                task.Wait();

                (item? output, error? error_) = task.Result;
                error = error_;

                if (error != null) return false;
                return output.isTrue(out error);
            }
            else if (obj is @object)
            {
                int hash = GetItemHashCode(out error);
                if (error != null) return false;
        
                int otherHash = ((@object)obj).GetItemHashCode(out error);
                if (error != null) return false;
        
                return hash == otherHash;
            }
            return false;
        }
    }
}
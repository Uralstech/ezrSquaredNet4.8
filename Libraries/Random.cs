using ezrSquared.Values;
using ezrSquared.Helpers;
using ezrSquared.Errors;
using ezrSquared.General;
using static ezrSquared.Constants.constants;
using System.Threading.Tasks;

namespace ezrSquared.Libraries.Random
{
    public class random : baseFunction
    {
        public override bool UPDATEONACCESS => true;
        private System.Random random_;
        public random() : base("<random <random>>")
        { random_ = new System.Random(); }

        public override async Task<runtimeResult> execute(item[] args)
        {
            context internalContext = base.generateContext();
            internalContext.symbolTable.set("get", new predefined_function("random_get", randomNumber, new string[0]));
            internalContext.symbolTable.set("get_limited", new predefined_function("random_get_limited", randomNumberLimited, new string[2] { "minimum", "maximum" }));
            internalContext.symbolTable.set("get_float", new predefined_function("random_get_float", randomNumberFloat, new string[0]));

            return new runtimeResult().success(new @object(name, internalContext).setPosition(startPos, endPos).setContext(context));
        }

        private runtimeResult randomNumber(context context, position[] positions) { return new runtimeResult().success(new integer(random_.Next())); }

        private runtimeResult randomNumberLimited(context context, position[] positions)
        {
            runtimeResult result = new runtimeResult();
            item minimum = context.symbolTable.get("minimum");
            item maximum = context.symbolTable.get("maximum");

            if (minimum is not integer && minimum is not @float)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Minimum must be an integer or float", context));
            if (maximum is not integer && maximum is not @float)
                return result.failure(new runtimeError(positions[0], positions[1], RT_TYPE, "Maximum must be an integer or float", context));
            
            if (minimum is @float && ((float)((value)minimum).storedValue > int.MaxValue || (float)((value)minimum).storedValue < int.MinValue))
                return result.failure(new runtimeError(minimum.startPos, minimum.endPos, RT_OVERFLOW, "Value either too large or too small to be converted to an integer", context));
            if (maximum is @float && ((float)((value)maximum).storedValue > int.MaxValue || (float)((value)maximum).storedValue < int.MinValue))
                return result.failure(new runtimeError(maximum.startPos, maximum.endPos, RT_OVERFLOW, "Value either too large or too small to be converted to an integer", context));

            int min = (minimum is integer) ? (int)((integer)minimum).storedValue : (int)((@float)minimum).storedValue;
            int max = (maximum is integer) ? (int)((integer)maximum).storedValue : (int)((@float)maximum).storedValue;
            return result.success(new integer(random_.Next(min, max)));
        }

        private runtimeResult randomNumberFloat(context context, position[] positions) { return new runtimeResult().success(new @float((float)random_.NextDouble())); }

        public override item copy() { return new random().setPosition(startPos, endPos).setContext(context); }

        public override string ToString() { return $"<builtin library {name}>"; }
        public override bool ItemEquals(item obj, out error? error) { error = null; return obj is random; }
    }
}

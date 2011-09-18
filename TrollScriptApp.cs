using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace tsCompiler
{
    public class TrollScriptApp : IDisposable
    {
        public string Name { get; set; }

        private readonly AssemblyBuilder _asBuilder;
        private readonly ModuleBuilder _modBuilder;
        private readonly TypeBuilder _typeBuilder;
        private readonly MethodBuilder _methBuilder;
        private readonly ILGenerator _il;

        public TrollScriptApp(string name)
        {
            Name = name;
            _asBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName { Name = "Trollscript app" }, AssemblyBuilderAccess.Save);
            _modBuilder = _asBuilder.DefineDynamicModule("ts", name + ".exe");
            _typeBuilder = _modBuilder.DefineType("Program", TypeAttributes.Public);
            _methBuilder = _typeBuilder.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static);
            _asBuilder.SetEntryPoint(_methBuilder);
            _il = _methBuilder.GetILGenerator();

            _il.DeclareLocal(typeof(int));
            _il.DeclareLocal(typeof(int[]));

            // Initialize idx to 0
            _il.Emit(OpCodes.Ldc_I4_0);
            _il.Emit(OpCodes.Stloc_0);

            // Initialize cell with 30000 fields of int
            _il.Emit(OpCodes.Ldc_I4, 30000);
            _il.Emit(OpCodes.Newarr, typeof(int));
            _il.Emit(OpCodes.Stloc_1);
        }

        public void BuildApp(string data)
        {
            //TODO disabled
           // data = new string(data.Where(c => TokenBase.FromOp(c) != null).ToArray());

            // Make a list of tuples, containing the token and
            // amount of repetitions

            var ops = new List<Tuple<string, int>>();

            while (!string.IsNullOrEmpty(data))
            {
                string matchThis = data.Substring(0,3);

                //i need to match the pattern.

                var chars = Take3While(data, matchThis);

                data = data.Substring(chars.Length);

                ops.Add(new Tuple<string, int>(matchThis, chars.Length/3));
            }

            foreach (var pair in from t in ops where TokenBase.FromOp(t.Item1) != null select new { Token = TokenBase.FromOp(t.Item1), Count = t.Item2 })
            {
                pair.Token.Emit(_il, pair.Count);
            }

            _il.Emit(OpCodes.Ret);
        }

        private char[] Take3While(string data, string pattern)
        {
            string result ="";
            if (data.Length < pattern.Length)
                return new char[0];
            if (data.Length % 3 != 0)
                return new char[0];
            for (var i=0; i<data.Length; i+=3 )
            {
                var trigraph = data.Substring(i, 3);
                if (trigraph == pattern)
                    result += pattern;
                else
                    break;
            }
            return result.ToArray();
        }


        public void Dispose()
        {
            _typeBuilder.CreateType();
            _asBuilder.Save(Name + ".exe");
        }
    }
}

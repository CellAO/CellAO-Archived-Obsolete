#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...

#endregion

namespace ZoneEngine.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class FunctionPrototype
    {
        /// <summary>
        /// Locks function targets and executes the function
        /// </summary>
        /// <param name="self">Dynel (Character or NPC)</param>
        /// <param name="caller">Caller of the function</param>
        /// <param name="target">Target of the Function (Dynel or Statel)</param>
        /// <param name="arguments">Function Arguments</param>
        /// <returns></returns>
        public abstract bool Execute(Dynel self, Dynel caller, Object target, object[] arguments);

        private int functionNumber = -1;

        private string functionName = "";

        public abstract int ReturnNumber();

        public abstract string ReturnName();

        public int FunctionNumber
        {
            get
            {
                return functionNumber;
            }
            set
            {
                functionNumber = value;
            }
        }

        public string FunctionName
        {
            get
            {
                return functionName;
            }
            set
            {
                functionName = value;
            }
        }
    }

    public class FunctionCollection
    {
        private readonly Dictionary<int, Type> functions = new Dictionary<int, Type>();

        private Assembly assembly;

        public bool ReadFunctions()
        {
            try
            {
                this.assembly = Assembly.GetExecutingAssembly();

                foreach (Type t in this.assembly.GetTypes())
                {
                    if (t.IsClass)
                    {
                        if (t.Namespace == "ZoneEngine.Functions")
                        {
                            if ((t.Name != "FunctionPrototype") && (t.Name != "FunctionCollection"))
                            {
                                this.functions.Add(
                                    ((FunctionPrototype)this.assembly.CreateInstance(t.Namespace + "." + t.Name)).
                                        ReturnNumber(),
                                    t);
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls a function by its number
        /// </summary>
        /// <param name="functionNumber">
        /// The number of the function
        /// </param>
        /// <param name="self">
        /// The self.
        /// </param>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// </returns>
        public bool CallFunction(int functionNumber, Dynel self, Dynel caller, object target, object[] arguments)
        {
            FunctionPrototype func = this.GetFunctionByNumber(functionNumber);
            return func.Execute(self, caller, target, arguments);
        }

        public FunctionPrototype GetFunctionByNumber(int functionnumber)
        {
            if (this.functions.Keys.Contains(functionnumber))
            {
                return
                    (FunctionPrototype)
                    this.assembly.CreateInstance(
                        this.functions[functionnumber].Namespace + "." + this.functions[functionnumber].Name);
            }
            return null;
        }

        public int NumberofRegisteredFunctions()
        {
            return this.functions.Keys.Count;
        }
    }
}
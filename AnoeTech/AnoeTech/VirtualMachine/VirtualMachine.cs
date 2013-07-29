using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*  __    ^    __ 
 * |  \  / \  /  |
 * |   \/   \/   |
 * |      ^      |
 * |_____/ \_____|oulDWerk
 * 
 * 
 *  The Virtual Machine is used to Register all functions needed accesible at runtime. The functions are stored in a Dictionary by a
 *  MetaName. The Virtual Machine also has a VMConsole which provides a simple text based GUI for creating or loading and executing scripts or
 *  commands. All functions and accessors registered with the dictionary may be utilized from the command console or from any class that has
 *  access to the VM. There are two functions for executing a command, Execute() and ExecuteFromConsole(). Execute will execute the command
 *  and return the result as an object. ExecuteFromConsole will check to make sure the command is valid before executing and display
 *  debug info out to the console.
 *  
 */


namespace AnoeTech
{
    public class VirtualMachine
    {

        #region Structs

        private struct FunctionHeader
        {
            public Func<object[], object> Function;
            public                    int NumberOfParameters;
            public                 Type[] ParameterType;
        }

        #endregion

        #region Variables

        VMConsole _console;

        Dictionary<string, FunctionHeader> FunctionDictionary = new Dictionary<string, FunctionHeader>();
        
        #endregion

        #region Accessors

        public VMConsole Console{ get{ return _console;} }

        #endregion

        #region Functions

        public void Initiate()
        {
             _console = new VMConsole();    // Create a new console
             _console.Initiate(this);       // and Initiate it
        }

        public void RegisterFunction<T>(string command, int numberOfArguments, Type[] parameterTypes, Func<object[], T> function)
        {
            FunctionHeader functionHeader = new FunctionHeader();       // Create a new Function Header
            functionHeader.Function = args => function(args);           // Pass the Function into the Header
            functionHeader.NumberOfParameters = numberOfArguments;      // Set the number of parameters the function takes
            functionHeader.ParameterType = parameterTypes;              // Set the array of parameter types
            if (!FunctionDictionary.ContainsKey(command))
                FunctionDictionary.Add(command, functionHeader);            // Add the Meta Tag and Function Header to the Dictionary
            else
                Console.Output("=Function " + command + " already registered with VM=");

            Console.Output("Function " + command + " has been registered with the VM.");
        }

        public void RegisterFunction(string command, int numberOfArguments, Type[] parameterTypes, Action<object[]> function)
        {
            FunctionHeader functionHeader = new FunctionHeader();                           // This code does the same as above
            functionHeader.Function = args => { function.Invoke(args); return null; };      // but is responsible for handling
            functionHeader.NumberOfParameters = numberOfArguments;                          // functions that have no return value.
            functionHeader.ParameterType = parameterTypes;
            if (!FunctionDictionary.ContainsKey(command))
                FunctionDictionary.Add(command, functionHeader);
            else
                Console.Output("=Function " + command + " already registered with VM=");

            Console.Output("Function " + command + " has been registered with the VM.");
        }

        public object Execute(string command, object[] args)
        {
            return FunctionDictionary[command].Function.Invoke(args);       // Execute a command and pass the result to the caller
        }

        public void ExecuteFromConsole(string command, object[] args)
        {
            if (FunctionDictionary.ContainsKey(command)) // If the Command is registered
            {
                if (args.Length == FunctionDictionary[command].NumberOfParameters) // And the correct number of parameters were used
                {
                    bool allIsGood = true;
                    for (int counter = 0; counter < FunctionDictionary[command].NumberOfParameters; counter++)
                        if (args[counter].GetType() == FunctionDictionary[command].ParameterType[counter]) // And the parameters are all the correct type
                            continue;
                        else
                            allIsGood = false;
                    if (allIsGood)
                    {
                        FunctionDictionary[command].Function.Invoke(args); //Then execute the command
                        Console.Output("Executed: " + Console.Command);
                    }
                    else
                        Console.Output("=Command " + command + " takes different type for parameter=");
                }
                else
                    Console.Output("=Command " + command + " takes " + FunctionDictionary[command].NumberOfParameters + " parameters=");
            }
            else
                Console.Output("=Command " + command + " not recognized=");
        }

        public void Update()
        {
        }

        #endregion
    }
}

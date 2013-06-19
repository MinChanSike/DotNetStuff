﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace More
{
    public class NpcReflector : NpcExecutor
    {
        /// <summary> List of execution objects </summary>
        readonly NpcExecutionObject[] npcExecutionObjects;

        public override IEnumerable<NpcExecutionObject> ExecutionObjects
        {
            get
            {
                return ((IEnumerable<NpcExecutionObject>)npcExecutionObjects);
            }
        }
        public override IDictionary<String,Type> EnumAndObjectTypes
        {
            get
            {
                return enumAndObjectTypesDictionary;
            }
        }

        readonly List<NpcMethodOverloadable> methodList;
        /// <summary>Mapping from method names (either lowercase or normal case) to remote methods </summary>
        readonly Dictionary<String,NpcMethodOverloadable> methodDictionary;

        // An ObjectType is a collection of other types
        // A type is either a primitive, array, enum, or object type
        readonly Dictionary<String, Type> enumAndObjectTypesDictionary;

        public NpcReflector(params Object [] executionObjects)
        {
            if (executionObjects == null) throw new ArgumentNullException("executionObjects");
            if (executionObjects.Length <= 0) throw new ArgumentException("exeuctionObjects must have at least one object", "executionObjects");

            this.npcExecutionObjects = new NpcExecutionObject[executionObjects.Length];

            this.methodList                   = new List<NpcMethodOverloadable>();
            this.methodDictionary             = new Dictionary<String, NpcMethodOverloadable>();

            this.enumAndObjectTypesDictionary = new Dictionary<String, Type>();

            //
            // Find all methods that are apart of an [NpcInterface]
            //
            SosTypeSerializationVerifier verifier = new SosTypeSerializationVerifier();
            HashSet<Type> hashSetToCheckInterfaces = new HashSet<Type>();

            for (int i = 0; i < executionObjects.Length; i++)
            {
                Object executionObject = executionObjects[i];
                NpcExecutionObject npcExecutionObject = executionObject as NpcExecutionObject;
                if (npcExecutionObject == null)
                {
                    npcExecutionObject = new NpcExecutionObject(executionObject);
                }

                npcExecutionObjects[i] = npcExecutionObject;

                List<MethodInfo> npcMethodInfos = new List<MethodInfo>();
                //Console.WriteLine("Getting Npc methods for '{0}'", executionObject.type.FullName);
                npcExecutionObject.type.GetNpcMethods(npcMethodInfos, hashSetToCheckInterfaces);

                if (npcMethodInfos.Count <= 0)
                    throw new InvalidOperationException(String.Format(
                        "Type '{0}' did not implement have any npc methods.  An npc method is a method inside an interface with the [NpcInterface] attribute.",
                        npcExecutionObject.type.Name));

                for (int methodIndex = 0; methodIndex < npcMethodInfos.Count; methodIndex++)
                {
                    MethodInfo methodInfo = npcMethodInfos[methodIndex];
                    NpcMethodInfo npcMethodInfo = new NpcMethodInfo(npcExecutionObject, methodInfo);
                    //Console.WriteLine("   Registering types for method '{0}'", npcMethodInfo.npcMethodName);

                    //
                    // Check that all parameter types can be parsed
                    //
                    for (UInt16 k = 0; k < npcMethodInfo.parametersLength; k++)
                    {
                        RegisterType(verifier, npcMethodInfo.parameters[k].ParameterType);
                    }

                    //
                    // Find the appropriate ToString method for the return type
                    //
                    RegisterType(verifier, methodInfo.ReturnType);
 
                    //
                    // Add method info to dictionary
                    //
                    NpcMethodOverloadable overloadableMethod;
                    if(methodDictionary.TryGetValue(npcMethodInfo.npcMethodName, out overloadableMethod))
                    {
                        overloadableMethod.AddOverload(npcMethodInfo);
                    }
                    else
                    {
                        NpcMethodOverloadable newMethod = new NpcMethodOverloadable(npcExecutionObject, npcMethodInfo);

                        methodList.Add(newMethod);

                        methodDictionary.Add(npcMethodInfo.npcMethodName, newMethod);
                        methodDictionary.Add(npcMethodInfo.npcMethodNameLowerInvariant, newMethod);
                    }
                    npcExecutionObject.npcMethods.Add(npcMethodInfo);
                }
            }
        }

        //
        // Every function parameter type and return type must be registered.
        // Registration checks that the type can be serialized and also saves the type information to send
        // to the client.
        //
        void RegisterType(SosTypeSerializationVerifier verifier, Type type)
        {
            if (type == typeof(void) || type.IsSosPrimitive()) return;
            if (type.IsArray)
            {
                RegisterType(verifier, type.GetElementType());
                return;
            }

            Type alreadyRegisteredType;
            if (enumAndObjectTypesDictionary.TryGetValue(type.FullName, out alreadyRegisteredType))
            {
                if(alreadyRegisteredType != type)
                    throw new InvalidOperationException(String.Format("Error: there are 2 different types with the same name '{0}'", type.FullName));
                return;
            }

            String because = verifier.CannotBeSerializedBecause(type);
            if (because != null) throw new InvalidOperationException(String.Format(
                 "The type '{0}' cannot be serialized because {1}", type.FullName, because));

            enumAndObjectTypesDictionary.Add(type.FullName, type);
            if(!type.IsEnum)
            {
                //
                // Register the user defined types fields
                //
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfos == null || fieldInfos.Length <= 0) return;
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    RegisterType(verifier, fieldInfos[i].FieldType);
                }
            }
        }
        public override NpcMethodInfo GetNpcMethodInfo(String methodName, UInt16 parameterCount, out NpcExecutionObject executionObject)
        {
            //
            // Find method with matchine name
            //
            NpcMethodOverloadable overloadableMethod;
            if (!methodDictionary.TryGetValue(methodName, out overloadableMethod) &&
                !methodDictionary.TryGetValue(methodName.ToLowerInvariant(), out overloadableMethod))
            {
                throw new InvalidOperationException(String.Format("Method '{0}' was not found", methodName));
            }

            //
            // Find method with matching name that has the correct number of parameters
            //
            NpcMethodInfo npcMethodInfo = overloadableMethod.GetMethod(parameterCount);
            if (npcMethodInfo == null)
                throw new InvalidOperationException(String.Format("Method '{0}' was found but it does not have {1} parameters", methodName, parameterCount));
            executionObject = overloadableMethod.executionObject;
            return npcMethodInfo;
        }
        public override NpcReturnObjectOrException ExecuteWithObjects(NpcExecutionObject executionObject, NpcMethodInfo npcMethodInfo, params Object[] args)
        {
            UInt16 argsLength = (args == null) ? (UInt16)0 : (UInt16)args.Length;

            //
            // Setup Parameter array of Objects to invoke the method
            //
            MethodInfo methodInfo = npcMethodInfo.methodInfo;
            ParameterInfo[] parameters = npcMethodInfo.parameters;

            //
            // Invoke
            //
            try
            {
                //
                // Call pre call if specified
                //
                if(executionObject.preAndPostCall != null)
                {
                    executionObject.preAndPostCall.PreCall(executionObject, npcMethodInfo, args);
                }

                Object returnObject;
                //
                // Make sure execution lock is respected if it was provided
                //
                try
                {
                    if (executionObject.executionLock == null)
                    {
                        returnObject = methodInfo.Invoke(executionObject.invokeObject, args);
                    }
                    else
                    {
                        lock (executionObject.executionLock)
                        {
                            returnObject = methodInfo.Invoke(executionObject.invokeObject, args);
                        }
                    }
                }
                finally
                {
                    //
                    // Call post call if specified
                    //
                    if (executionObject.preAndPostCall != null)
                    {
                        executionObject.preAndPostCall.PostCall(executionObject, npcMethodInfo, args);
                    }
                }

                return new NpcReturnObjectOrException(methodInfo.ReturnType, returnObject, returnObject.SerializeObject());
            }
            catch (TargetInvocationException e)
            {
                return new NpcReturnObjectOrException(e.InnerException, e.SerializeObject());
            }
        }
        public void PrintInformation(TextWriter writer)
        {
            writer.WriteLine("Methods:");
            foreach (NpcMethodOverloadable overloadableMethod in methodList)
            {
                foreach (NpcMethodInfo npcMethodInfo in overloadableMethod)
                {
                    Console.WriteLine("   Name='{0}' {1}", npcMethodInfo.npcMethodName, npcMethodInfo.methodInfo);
                }
            }

            writer.WriteLine("Enum and Object types:");
            foreach (KeyValuePair<String,Type> pair in enumAndObjectTypesDictionary)
            {
                Console.WriteLine("   " + pair.Key);
            }
        }
    }
}
using Modulify.Documents.Internals;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulify.Documents
{
    public static class DocumentExtensions
    {
        private const string MODULE_HINT = ":module_hint";

        /// <summary>
        /// Add the document module to the <see cref="IModuleCollection"/>.
        /// if ForceBinarySupports set true, this adds the JSON as binary module also.
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Module"></param>
        /// <returns></returns>
        public static IModuleCollection AddDocument(this IModuleCollection Collection, IDocumentTextModule Module, bool ForceBinarySupports = false)
        {
            // --> registers the document module type.
            Collection.BaseTypes.Add(typeof(IDocumentModule));
            if (ForceBinarySupports)
            {
                Collection.AddDocument(new ForcedBinarySupport(Module));
            }

            // --> then, add the document itself.
            return Collection.Add(Module);
        }

        /// <summary>
        /// Add the document module to the <see cref="IModuleCollection"/>.
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Module"></param>
        /// <returns></returns>
        public static IModuleCollection AddDocument(this IModuleCollection Collection, IDocumentBinaryModule Module)
        {
            // --> registers the document module type.
            Collection.BaseTypes.Add(typeof(IDocumentModule));

            // --> then, add the document itself.
            return Collection.Add(Module);
        }

        /// <summary>
        /// Get the document type hint if possible.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static string GetHint(this JObject Input)
        {
            if (Input != null)
            {
                var Value = Input.Value<string>(MODULE_HINT);

                if (string.IsNullOrWhiteSpace(Value))
                    return null;

                return Value;
            }

            return null;
        }

        /// <summary>
        /// Set the document type hint to the output.
        /// </summary>
        /// <param name="Output"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        private static JObject SetHint(this JObject Output, string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
                return Output;

            if (Output is null)
                Output = new JObject();

            Output[MODULE_HINT] = Value;
            return Output;
        }

        /// <summary>
        /// Serialize the <see cref="IDocument"/> into the input.
        /// this throws <see cref="ArgumentNullException"/>, <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static JObject SerializeDocument(this IModuleProvider Provider, IDocument Input)
        {
            if (Input != null)
            {
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentTextModule)
                    .Where(X => X != null && X.CanSupport(Input));

                foreach (var Each in Modules)
                {
                    var Output = Each.Serialize(Input);
                    if (Output != null)
                        return Output.SetHint(Each.Name);
                }

                throw new NotSupportedException("the input is not supported.");
            }

            throw new ArgumentNullException("the input is null.");
        }

        /// <summary>
        /// Serialize the <see cref="IDocument"/> into the input in text mode.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool TrySerializeDocument(this IModuleProvider Provider, IDocument Input, out JObject Output)
        {
            if (Input != null)
            {
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentTextModule)
                    .Where(X => X != null && X.CanSupport(Input));

                foreach (var Each in Modules)
                {
                    if ((Output = Each.Serialize(Input)) != null)
                        return true;
                }
            }

            Output = null;
            return false;
        }

        /// <summary>
        /// Deserialize the <see cref="IDocument"/> from the input in text mode.
        /// this throws <see cref="ArgumentNullException"/>, <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static IDocument DeserializeDocument(this IModuleProvider Provider, JObject Input)
        {
            if (Input != null)
            {
                var Hint = Input.GetHint() ?? string.Empty;
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentTextModule)
                    .Where(X => X != null && X.CanSupport(Input))
                    .OrderBy(X => X.MakeOrder(Hint));

                foreach (var Each in Modules)
                {
                    var Document = Each.Deserialize(Input);
                    if (Document != null)
                    {
                        return Document;
                    }
                }

                throw new NotSupportedException("the input is not supported or not document.");
            }

            throw new ArgumentNullException("the input is null.");
        }

        /// <summary>
        /// Deserialize the <see cref="IDocument"/> from the input.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool TryDeserializeDocument(this IModuleProvider Provider, JObject Input, out IDocument Output)
        {
            if (Input != null)
            {
                var Hint = Input.GetHint() ?? string.Empty;
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentTextModule)
                    .Where(X => X != null && X.CanSupport(Input))
                    .OrderBy(X => X.MakeOrder(Hint));

                foreach (var Each in Modules)
                {
                    if ((Output = Each.Deserialize(Input)) != null)
                        return true;
                }
            }

            Output = null;
            return false;
        }

        /// <summary>
        /// Serialize the <see cref="IDocument"/> into the input in binary mode.
        /// this throws <see cref="ArgumentNullException"/>, <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static void SerializeDocument(this IModuleProvider Provider, Stream Output, IDocument Input)
        {
            if (Input != null)
            {
                var Module = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentBinaryModule)
                    .FirstOrDefault(X => X != null && X.CanSupport(Input));

                if (Module != null)
                {
                    Module.Serialize(Output, Input);
                    return;
                }

                throw new NotSupportedException("the input is not supported.");
            }

            throw new ArgumentNullException("the input is null.");
        }

        /// <summary>
        /// Serialize the <see cref="IDocument"/> into the input in binary mode.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool TrySerializeDocument(this IModuleProvider Provider, Stream Output, IDocument Input)
        {
            if (Input != null)
            {
                var Module = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentBinaryModule)
                    .FirstOrDefault(X => X != null && X.CanSupport(Input));

                var Rewind = Output.CanSeek ? Output.Position : -1;
                if (Module != null)
                {
                    try
                    {
                        Module.Serialize(Output, Input);
                        return true;
                    }

                    catch { }
                }

                if (Rewind >= 0)
                    Output.Position = Rewind;
            }

            return false;
        }

        /// <summary>
        /// Deserialize the <see cref="IDocument"/> from the input in binary mode.
        /// this throws <see cref="ArgumentNullException"/>, <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static IDocument DeserializeDocument(this IModuleProvider Provider, Stream Input)
        {
            if (Input != null)
            {
                if (!Input.CanRead || !Input.CanSeek)
                    throw new InvalidOperationException("the input should support `Read` and `Seek` access.");

                var Rewind = Input.Position;
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentBinaryModule)
                    .Where(X =>
                    {
                        try { return X != null && X.CanSupport(Input); }
                        catch { return false; }
                        finally
                        {
                            Input.Position = Rewind;
                        }
                    });

                var Exceptions = new List<Exception>();
                foreach(var Each in Modules)
                {
                    var Succeed = false;
                    try
                    {
                        var Output = Each.Deserialize(Input);
                        if (Output != null)
                        {
                            Succeed = true;
                            return Output;
                        }
                    }

                    catch(Exception Error) { Exceptions.Add(Error); }
                    finally
                    {
                        if (!Succeed)
                            Input.Position = Rewind;
                    }
                }

                if (Exceptions.Count > 0)
                    throw new AggregateException("multiple exceptions are thrown.", Exceptions);

                throw new NotSupportedException("the input is not supported.");
            }

            throw new ArgumentNullException("the input is null.");
        }

        /// <summary>
        /// Deserialize the <see cref="IDocument"/> from the input in binary mode.
        /// this throws <see cref="ArgumentNullException"/>, <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="Provider"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool TryDeserializeDocument(this IModuleProvider Provider, Stream Input, out IDocument Output)
        {
            if (Input != null)
            {
                if (!Input.CanRead || !Input.CanSeek)
                {
                    Output = null;
                    return false;
                }

                var Rewind = Input.Position;
                var Modules = Provider
                    .FindAll(typeof(IDocumentModule))
                    .Select(X => X as IDocumentBinaryModule)
                    .Where(X =>
                    {
                        try { return X != null && X.CanSupport(Input); }
                        catch { return false; }
                        finally
                        {
                            Input.Position = Rewind;
                        }
                    });

                foreach (var Each in Modules)
                {
                    var Succeed = false;
                    try
                    {
                        if ((Output = Each.Deserialize(Input)) != null)
                        {
                            Succeed = true;
                            return true;
                        }
                    }

                    catch { }
                    finally
                    {
                        if (!Succeed)
                            Input.Position = Rewind;
                    }
                }
            }

            Output = null;
            return false;
        }

        /// <summary>
        /// Make the order key for the hint and module.
        /// </summary>
        /// <param name="Module"></param>
        /// <param name="Hint"></param>
        /// <returns></returns>
        private static int MakeOrder(this IDocumentModule Module, string Hint)
        {
            return string.Equals(Module.Name ?? string.Empty, Hint, StringComparison.OrdinalIgnoreCase) ? 0 : 1;
        }

    }
}

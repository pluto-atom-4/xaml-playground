using System;
using System.Collections.Generic;
using Python.Runtime;

namespace ScientificApp.Models;

/// <summary>
/// Thread-safe wrapper for Python.NET runtime.
/// Manages Python initialization, module loading, and function calls.
/// Provides graceful fallback if Python is unavailable.
/// </summary>
public sealed class PythonBackend : IDisposable
{
    private static readonly Lazy<PythonBackend> _instance = new(() => new PythonBackend());
    private static readonly object _lock = new();
    
    private bool _initialized = false;
    private bool _available = false;
    private string? _initError = null;
    private dynamic? _analysisModule = null;

    public static PythonBackend Instance => _instance.Value;
    public bool IsAvailable => _available;
    public string? InitializationError => _initError;

    private PythonBackend()
    {
        InitializePython();
    }

    /// <summary>
    /// Initialize Python runtime on first access
    /// </summary>
    private void InitializePython()
    {
        lock (_lock)
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                // Check if Python is already initialized
                if (!PythonEngine.IsInitialized)
                {
                    PythonEngine.Initialize();
                }

                // Get reference to Python builtins
                var sys = Py.Import("sys");
                
                // Add python_backend to path so we can import our module
                var appDir = AppDomain.CurrentDomain.BaseDirectory;
                var pythonBackendPath = System.IO.Path.Combine(appDir, "python_backend");
                
                dynamic path = sys.GetAttr("path");
                if (!path.Contains(pythonBackendPath))
                {
                    path.append(pythonBackendPath);
                }

                // Import our analysis module
                _analysisModule = Py.Import("analysis");
                _available = true;
            }
            catch (Exception ex)
            {
                _initError = $"Failed to initialize Python: {ex.Message}";
                _available = false;
            }
        }
    }

    /// <summary>
    /// Call a Python function with error handling
    /// </summary>
    public Dictionary<string, object>? CallFunction(string functionName, params object[] args)
    {
        if (!_available || _analysisModule == null)
        {
            return new Dictionary<string, object>
            {
                { "success", false },
                { "error", _initError ?? "Python backend not available" }
            };
        }

        lock (_lock)
        {
            try
            {
                if (_analysisModule == null)
                {
                    return new Dictionary<string, object>
                    {
                        { "success", false },
                        { "error", "Python analysis module not loaded" }
                    };
                }

                var pyFunc = _analysisModule.GetAttr(functionName);
                var pyArgs = args.Select(arg => arg.ToPython()).ToArray();
                var result = pyFunc.Invoke(pyArgs);

                // Convert Python dict to C# Dictionary
                return ConvertPythonDictToCS(result);
            }
            catch (PythonException pex)
            {
                return new Dictionary<string, object>
                {
                    { "success", false },
                    { "error", $"Python error in {functionName}: {pex.Message}" }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "success", false },
                    { "error", $"Error calling Python function {functionName}: {ex.Message}" }
                };
            }
        }
    }

    /// <summary>
    /// Convert Python dictionary to C# Dictionary
    /// </summary>
    private static Dictionary<string, object> ConvertPythonDictToCS(dynamic pyDict)
    {
        var result = new Dictionary<string, object>();
        
        try
        {
            var items = pyDict.items();
            foreach (var item in items)
            {
                var key = item[0].ToString();
                var value = item[1];

                // Convert Python types to C# equivalents
                object? csValue = value switch
                {
                    null => null,
                    _ when value is PyObject => ConvertPythonValue(value),
                    _ => value
                };

                result[key] = csValue ?? "null";
            }
        }
        catch
        {
            // If conversion fails, just pass through the dict
            result["_raw"] = pyDict;
        }

        return result;
    }

    /// <summary>
    /// Convert Python value to C# value
    /// </summary>
    private static object ConvertPythonValue(dynamic pyValue)
    {
        try
        {
            // Try to get the Python type
            var typeStr = pyValue.GetType().Name;

            // Check if it's a list/array
            if (pyValue is PyList || typeStr.Contains("List"))
            {
                var list = new List<object>();
                foreach (var item in pyValue)
                {
                    list.Add(ConvertPythonValue(item));
                }
                return list;
            }

            // Check if it's a float
            if (pyValue is PyFloat || typeStr.Contains("float"))
            {
                return (double)pyValue;
            }

            // Check if it's an int
            if (pyValue is PyInt || typeStr.Contains("int"))
            {
                return (int)pyValue;
            }

            // Check if it's a bool
            if (typeStr.Contains("bool"))
            {
                return (bool)pyValue;
            }

            // Check if it's a string
            if (pyValue is PyString || typeStr.Contains("str"))
            {
                return (string)pyValue;
            }

            // Default: convert to string
            return pyValue.ToString();
        }
        catch
        {
            return pyValue.ToString();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _analysisModule?.Dispose();
            _analysisModule = null;
        }
    }
}

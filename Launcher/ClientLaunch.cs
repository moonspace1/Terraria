using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Terraria;

namespace Launcher;

public static class ClientLaunch
{
    public static Dictionary<string, Assembly> LoadedAssemblies;

    public static Dictionary<string, nint> LoadedSystemAssemblies;

    public static string CommonPath;

    public static string FNAPath;

    public static string MonoPath;

    public static string SystemPath;

    public static string OS
    {
        get
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Linux";
            }
            return "Windows";
        }
    }

    public static void Main(string[] args)
    {
        Init();
        Environment.SetEnvironmentVariable("FNA_WORKAROUND_WINDOW_RESIZABLE", "1");
        Program.LaunchGame(args, monoArgs: true);
    }

    public static void Init()
    {
        LoadedAssemblies = new Dictionary<string, Assembly>();
        LoadedSystemAssemblies = new Dictionary<string, nint>();
        CommonPath = Path.Combine(Environment.CurrentDirectory, "Libraries", "Common");
        FNAPath = Path.Combine(Environment.CurrentDirectory, "Libraries", "FNA");
        MonoPath = Path.Combine(Environment.CurrentDirectory, "Libraries", "Mono");
        SystemPath = Path.Combine(Environment.CurrentDirectory, "Libraries", "System", OS);
        Resolve();
    }

    public static void Resolve()
    {
        AssemblyLoadContext.Default.ResolvingUnmanagedDll += ResolveSystemBinary;
        AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            if (LoadedAssemblies.ContainsKey(name))
            {
                return LoadedAssemblies[name];
            }
            Assembly assembly = ResolveBinary(name, "dll");
            if (assembly != null)
            {
                LoadedAssemblies.Add(name, assembly);
            }
            return assembly;
        };
    }

    public static Assembly ResolveBinary(string fileName, string ext)
    {
        foreach (string item in new List<string> { CommonPath, FNAPath, MonoPath })
        {
            string[] array = Directory.EnumerateFiles(item, "*" + fileName + "." + ext).ToArray();
            if (array.Length != 0)
            {
                return Assembly.LoadFrom(array[0]);
            }
        }
        return null;
    }

    public static nint ResolveSystemBinary(Assembly binary, string name)
    {
        if (LoadedSystemAssemblies.TryGetValue(name, out var value))
        {
            return value;
        }
        string text = Directory.GetFiles(SystemPath, "*" + name + "*", SearchOption.AllDirectories).FirstOrDefault();
        if (text == null)
        {
            return IntPtr.Zero;
        }
        LoadedSystemAssemblies[name] = NativeLibrary.Load(text);
        return LoadedSystemAssemblies[name];
    }
}
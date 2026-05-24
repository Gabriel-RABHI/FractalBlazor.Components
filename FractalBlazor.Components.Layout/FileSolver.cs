using System.Reflection;

namespace  FractalBlazor.Components.Layout
{
    public class FileSolver
    {
        private static object _locker = new object();
        private static Dictionary<string, FileSolver> _cache = new Dictionary<string, FileSolver>();
        private static bool _assembliesLoaded = false;

        public Stream Content { get; private set; }

        public string ContentType { get; private set; }

        public static bool Load(string filename, Action<FileSolver> send)
        {
            lock (_locker)
            {
                LoadFbBlazorAssemblies();
                if (_cache.TryGetValue(filename, out var file))
                {
                    file.Content.Position = 0;
                    send(file);
                    return true;
                }
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("FbBlazor.")))
                {
                    var path = a.GetManifestResourceNames().Where(p => p.Contains(filename)).FirstOrDefault();
                    if (path != null)
                    {
                        var stream = a.GetManifestResourceStream(path);
                        var loaded = new FileSolver() { Content = stream };
                        if (filename.ToLower().EndsWith(".css"))
                            loaded.ContentType = "text/css";
                        if (filename.ToLower().EndsWith(".js"))
                            loaded.ContentType = "application/javascript";
                        _cache.Add(filename, loaded);
                        send(loaded);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void LoadFbBlazorAssemblies()
        {
            if (!_assembliesLoaded)
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var f in Directory.GetFiles(location, "*.dll", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f).Contains("FbBlazor.")))
                    Assembly.LoadFrom(f);
                _assembliesLoaded = true;
            }
        }
    }
}

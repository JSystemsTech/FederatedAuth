using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace FederatedIPWeb.App_Start
{
    public static class CustomWebOptimization
    {
        public static IHtmlString RenderStyleBundle(params string[] bundleNames) => Styles.Render(CustomBundlerExtensions.GetRelativeStyleBundlePaths(bundleNames));

        public static IHtmlString RenderScriptBundle(params string[] bundleNames) => Scripts.Render(CustomBundlerExtensions.GetRelativeScriptBundlePaths(bundleNames));

    }
    internal static class CustomBundlerExtensions
    {
        private static string BundleDir = "~/bundles/";
        public static string GetRelativeBundlePath(string bundle, string subDir = "") => $"{BundleDir}{subDir}{bundle}";
        public static string GetRelativeScriptBundlePath(string bundle) => GetRelativeBundlePath(bundle, "scripts");
        public static string[] GetRelativeScriptBundlePaths(IEnumerable<string> bundleNames) => bundleNames.Select(bundle => GetRelativeScriptBundlePath(bundle)).ToArray();
        public static string GetRelativeStyleBundlePath(string bundle) => GetRelativeBundlePath(bundle, "css");
        public static string[] GetRelativeStyleBundlePaths(IEnumerable<string> bundleNames) => bundleNames.Select(bundle => GetRelativeStyleBundlePath(bundle)).ToArray();


        private static T GetObject<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
        private delegate string RelativeBundlePathResolver(string bundle);
        private static TBundle AddBundle<TBundle>(BundleCollection bundles, string bundleName, RelativeBundlePathResolver resolver)
            where TBundle : Bundle
        {
            TBundle bundle = GetObject<TBundle>(resolver(bundleName));
            bundles.Add(bundle);
            return bundle;
        }
        public static ScriptBundle AddScriptBundle(this BundleCollection bundles, string bundleName) => AddBundle<ScriptBundle>(bundles, bundleName, GetRelativeScriptBundlePath);
        public static StyleBundle AddStyleBundle(this BundleCollection bundles, string bundleName) => AddBundle<StyleBundle>(bundles, bundleName, GetRelativeStyleBundlePath);

        private const string ScriptsDir = "~/Scripts/";
        public static string GetRelativeScriptsPath(string script, string scriptDir = ScriptsDir) => $"{scriptDir}{script}";
        public static ScriptBundle AddScript(this ScriptBundle bundle, string script, string scriptDir = ScriptsDir)
        {
            bundle.Include(GetRelativeScriptsPath(script, scriptDir));
            return bundle;
        }
        private const string StyleDir = "~/Content/";
        public static string GetRelativeStylesPath(string style, string styleDir = StyleDir) => $"{styleDir}{style}";
        public static StyleBundle AddStyle(this StyleBundle bundle, string style, string styleDir = StyleDir)
        {
            bundle.Include(GetRelativeStylesPath(style, styleDir));
            return bundle;
        }

    }
}